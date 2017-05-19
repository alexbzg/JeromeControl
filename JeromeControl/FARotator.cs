using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using AntennaeRotator.Properties;
using System.Threading.Tasks;
using System.Globalization;
using ExpertSync;
using System.Net;
using Jerome;
using AsyncConnectionNS;
using System.Diagnostics;
using JeromeControl;
using JeromeControl.Properties;
using JeromeModuleSettings;
using StorableFormState;

namespace AntennaeRotator
{

    public partial class FRotator : JCChildForm
    {
        
        static DeviceTemplate[] templates = {
                    new DeviceTemplate { engineLines = new Dictionary<int, int[]>{ { 1, new int[] { 16, 20 } }, {-1, new int[] { 15, 19} } },
                                            ledLine = 22, uartTRLine = 12, uartEncoder = true,
                                            limitsLines = new Dictionary<int, int> {  { 1, 14 }, { -1, 13 } }
                                        }, //0 NetRotator5
                    new DeviceTemplate { engineLines = new Dictionary<int,int[]>{ {1, new int[] { 12 } }, {-1, new int[] { 11 } } },
                                            gearLines = new int[] { 10, 7, 6, 3 },
                                            rotateButtonsLines = new Dictionary<int,int>{ {2, -1}, { 4, 1 } },
                                            adc = 1 } //1 Yaesu v6.4

                    };
        static Regex rEVT = new Regex(@"#EVT,IN,\d+,(\d+),(\d)");

        private AntennaeRotatorConfig config { get { return (AntennaeRotatorConfig)componentConfig; } }
        private AntennaeRotatorFormState formState {  get { return (AntennaeRotatorFormState)config.formStates[idx]; } }

        DeviceTemplate currentTemplate;
        JeromeController controller;
        int currentAngle = -1;
        int targetAngle = -1;
        int engineStatus = 0;
        int currentGear = -1;
        int mapAngle = -1;
        int startAngle = -1;
        volatile int limitReached = 0;
        int encGrayVal = -1;
        bool angleChanged = false;
        bool mvtBlink = false;
        List<Bitmap> maps = new List<Bitmap>();
        ToolStripMenuItem[] connectionsDropdown;
        System.Threading.Timer timeoutTimer;
        System.Threading.Timer adcTimer;
        volatile Task engineTask;
        CancellationTokenSource engineTaskCTS = new CancellationTokenSource();
        volatile bool engineTaskActive;
        volatile bool controllerTimeout;
        const int adcDataLength = 10;
        int[] adcData = new int[adcDataLength];
        int adcDataCount = 0;
        bool calibration = false;
        int curADCVal;
        int calCount = 0;



        internal void clearLimits()
        {
            currentConnection.limits = new Dictionary<int, int> { { 1, -1 }, { -1, -1 } };
            writeConfig();
        }

        int prevHeight;
        double mapRatio = 0;
        AntennaeRotatorConnectionSettings currentConnection;
        bool closingFl = false;
        private int secOnGear0;

        public int getCurrentAngle()
        {
            return currentAngle;
        }

        public FRotator(JCAppContext _appContext, int __idx) : base( _appContext, __idx)
        {
            InitializeComponent();
            appContext = _appContext;
            updateConnectionsMenu();
           /* Trace.Listeners.Add(new TextWriterTraceListener(Application.StartupPath + "\\rotator.log"));
            Trace.AutoFlush = true;
            Trace.Indent();
            Trace.WriteLine("Initialization");*/

            string currentMapStr = "";
            if (config.currentMap != -1 && config.currentMap < config.maps.Count)
                currentMapStr = config.maps[config.currentMap];
            config.maps.RemoveAll(item => !File.Exists(item));
            if (!currentMapStr.Equals(string.Empty))
                config.currentMap = config.maps.IndexOf(currentMapStr);
            else
                config.currentMap = -1;
            config.maps.ForEach(item => loadMap(item));
            if (config.currentMap != -1)
                setCurrentMap(config.currentMap);
            else
            {
                if (config.maps.Count > 0)
                    setCurrentMap(0);
            }
            prevHeight = Height;
            System.Diagnostics.Debug.WriteLine("Rotator constructor finished");
            if (formState.currentConnection != -1)
                loadConnection(formState.currentConnection);
        }

        private void scheduleTimeoutTimer()
        {
            timeoutTimer = new System.Threading.Timer(
                obj =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (controller != null && controller.connected)
                            controller.disconnect( true );
                    });
                },
                null, 50000, Timeout.Infinite);
        }

        private void setCurrentMap(int val)
        {
            config.currentMap = val;
            pMap.BackgroundImage = maps[val];
            writeConfig();
            mapRatio = (double)maps[val].Width / (double)maps[val].Height;
            adjustToMap();
        }

        private void updateConnectionsMenu()
        {
            while (miConnections.DropDownItems.Count > 2)
            {
                miConnections.DropDownItems.RemoveAt(0);

            }
            for (int co = 0; co < config.connections.Count; co++)
                createConnectionMenuItem(co);
        }


        public void loadConnection(int index)
        {
            if (currentConnection == config.connections[index])
                return;
            if (controller != null && controller.connected)
                disconnect();

            currentConnection = config.connections[index];
            formState.currentConnection = index;

            currentTemplate = getTemplate(currentConnection.deviceType);
            if (currentConnection.limitsSerialize != null)
            {
                currentConnection.limits = new Dictionary<int, int> { { 1, currentConnection.limitsSerialize[0] },
                    { -1, currentConnection.limitsSerialize[1] }
                };
            }

            writeConfig();

            System.Diagnostics.Debug.WriteLine("Rotator connnection loaded");
            connect();
            pMap.Refresh();
        }

        private DeviceTemplate getTemplate(int deviceType)
        {
            return templates[deviceType];
        }

        public DialogResult showMessage(string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return (DialogResult)this.Invoke((Func<DialogResult>)delegate
            {
                return MessageBox.Show(FRotator.ActiveForm, text, currentConnection == null ? "AntennaNetRotator" : currentConnection.name, buttons, icon);
            });
        }


        private void miModuleSettings_Click(object sender, EventArgs e)
        {
            (new fModuleSettings()).ShowDialog();
        }

        private void setGear(int val)
        {
            if (currentGear != val)
            {
                currentGear = val;
                if (val == -1)
                    currentTemplate.gearLines.ToList().ForEach(x => toggleLine(x, 0));
                else
                {
                    if (val == 0)
                        secOnGear0 = 0;
                    if (currentConnection.deviceType == 1)
                        for (int co = 0; co < currentTemplate.gearLines.Count(); co++)
                            toggleLine(currentTemplate.gearLines[co], co < val ? 1 : 0);

                }
            }
        }


        public void engine(int val)
        {
            if (val != engineStatus && (limitReached == 0 || limitReached != val))
            {
                this.UseWaitCursor = true;
                if (val == 0 || engineStatus != 0)
                {
                    int prevDir = engineStatus;
                    if (currentConnection.deviceType == 0)
                    {
                        toggleLine(currentTemplate.engineLines[prevDir][1], 0);
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (slCalibration.Text != "Концевик" || !slCalibration.Visible)
                            {
                                slCalibration.Text = "Остановка";
                                slCalibration.Visible = true;
                            }

                        });
                        if (engineTaskActive)
                        {
                            engineTaskCTS.Cancel();
                            try
                            {
                                engineTask.Wait();
                            }
                            catch (AggregateException) { }
                        }
                        engineTaskActive = true;
                        pMap.Enabled = false;
                        engineTask = TaskEx.Run(
                            async () =>
                            {
                                await TaskEx.Delay(currentConnection.switchIntervals[1] * 1000);
                                toggleLine(currentTemplate.engineLines[prevDir][0], 0);
                                clearEngineTask();
                            });
                    } else if ( currentConnection.deviceType == 1 )
                    {
                        toggleLine(currentTemplate.engineLines[prevDir][0], 0);
                    }
                }
                if (val != 0 && !engineTaskActive)
                {
                    if (currentConnection.deviceType == 0)
                    {
                        toggleLine(currentTemplate.engineLines[val][0], 1);
                        pMap.Enabled = false;
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (slCalibration.Text != "Концевик" || !slCalibration.Visible)
                            {
                                slCalibration.Text = "Пуск";
                                slCalibration.Visible = true;
                            }

                        });
                        engineTaskActive = true;
                        CancellationToken ct = engineTaskCTS.Token;
                        engineTask = TaskEx.Run(
                            async () =>
                            {
                                await TaskEx.Delay(currentConnection.switchIntervals[0] * 1000, ct);
                                if (!ct.IsCancellationRequested)
                                {
                                    toggleLine(currentTemplate.engineLines[val][1], 1);
                                }
                                clearEngineTask();
                            }, ct);
                        if (limitReached != 0 && !currentConnection.hwLimits)
                            offLimit();
                    } else if ( currentConnection.deviceType == 1 )
                    {
                        toggleLine(currentTemplate.engineLines[val][0], 1);
                    }
                }
                engineStatus = val;
                if (!engineTaskActive)
                    UseWaitCursor = false;
            }
        }

        private void clearEngineTask()
        {
            engineTaskActive = false;
            engineTaskCTS.Dispose();
            engineTaskCTS = new CancellationTokenSource();
            this.Invoke((MethodInvoker)delegate
            {
                if (slCalibration.Text != "Концевик")
                    slCalibration.Visible = false;
                pMap.Enabled = true;
                this.UseWaitCursor = false;
            });

        }

        private void processEVT(string msg)
        {
            Match match = rEVT.Match(msg);
            int line = Convert.ToInt16( match.Groups[1].Value );
            int lineState = match.Groups[2].Value == "0" ? 1 : 0;
            if (currentTemplate.rotateButtonsLines != null && currentTemplate.rotateButtonsLines.Keys.Contains(line))
                Task.Factory.StartNew(() => buttonCmd(line, lineState));
        }

        private void buttonCmd(int line, int state)
        {
            if (state == 0)
            {
                engine(0);
                setGear(0);
            }
            else
            {
                engine(currentTemplate.rotateButtonsLines[line]);
                setGear(1);
            }
        }


        private async void disconnect()
        {
            if (timeoutTimer != null)
            {
                timeoutTimer.Dispose();
                timeoutTimer = null;
            }
            if (adcTimer != null)
            {
                adcTimer.Dispose();
                adcTimer = null;
            }
            if (controller != null )
            {
                if (engineStatus != 0)
                {
                    engine(0);
                }
                if (engineTaskActive)
                    clearEngineTask();
                if (engineTask != null)
                {
                    await engineTask;
                    engineTask.Dispose();
                    engineTask = null;
                }
                if (currentTemplate.ledLine != 0)
                    toggleLine(currentTemplate.ledLine, 0);
                if (currentTemplate.uartEncoder)
                    controller.usartBytesReceived -= usartBytesReceived;
                if ( closingFl )
                    controller.onDisconnected -= onDisconnect;
                controller.onConnected -= onConnect;
                if (currentConnection.hwLimits)
                    controller.lineStateChanged -= lineStateChanged;
                controller.disconnect();
                controller = null;
            }
        }

        private void onDisconnect(object obj, DisconnectEventArgs e)
        {
            timer.Enabled = false;
            if (adcTimer != null)
                adcTimer.Change(Timeout.Infinite, Timeout.Infinite);
            targetAngle = -1;
            pMap.Invalidate();
            offLimit();
            if (e.requested && !controller.active)
            {
                currentConnection = null;
                if (!closingFl)
                {
                    formState.currentConnection = -1;
                    writeConfig();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Text = "Нет соединения";
                        lCaption.Text = "Нет соединения";
                        Icon = (Icon)Resources.ResourceManager.GetObject(CommonInf.icons[0]);
                        miConnections.Text = "Соединения";
                        if (connectionsDropdown != null)
                            miConnections.DropDownItems.AddRange(connectionsDropdown);
                        miSetNorth.Visible = false;
                        miCalibrate.Visible = false;
                        miIngnoreEngineOffMovement.Visible = false;
                    });
                }
            } else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    Text += " - нет соединения";
                    lCaption.Text += " - нет соединения";
                    pMap.Enabled = false;
                    miSetNorth.Enabled = false;
                    appContext.showNotification("AntennaNetRotator", currentConnection.name + ": соединение потеряно!", ToolTipIcon.Error);
                });
            }
        }

        public override void writeConfig()
        {
            if (loaded && currentConnection != null)
            {
                System.Drawing.Rectangle bounds = this.WindowState != FormWindowState.Normal ? this.RestoreBounds : this.DesktopBounds;
                if (currentConnection.limits != null)
                {
                    currentConnection.limitsSerialize = new int[] { -1, -1 };
                    if (currentConnection.limits.ContainsKey(1))
                        currentConnection.limitsSerialize[0] = currentConnection.limits[1];
                    if (currentConnection.limits.ContainsKey(-1))
                        currentConnection.limitsSerialize[1] = currentConnection.limits[-1];
                }
            }
            appContext.writeConfig();
        }


        private void setLine(int line, int mode)
        {
            if (controller != null && controller.connected)
                controller.setLineMode(line, mode);
        }

        private void toggleLine(int line, int state)
        {
            if (controller != null && controller.connected)
                controller.switchLine(line, state);
        }

        private void offLimit()
        {
            this.Invoke((MethodInvoker)delegate {
                if (slCalibration.Visible && slCalibration.Text == "Концевик")
                    slCalibration.Visible = false;
            });
            limitReached = 0;
        }

        private void onLimit(int dir)
        {
            if (limitReached == dir)
                return;
            if (engineStatus == dir)
                engine(0);
            if (currentConnection.hwLimits)
                currentConnection.limits[dir] = currentAngle;
            writeConfig();
            this.Invoke((MethodInvoker)delegate
            {
                if (!slCalibration.Visible || slCalibration.Text != "Концевик")
                {
                    slCalibration.Text = "Концевик";
                    slCalibration.Visible = true;
                    string sDir = dir == 1 ? "по часовой стрелке" : "против часовой стрелки";
                    showMessage("Достигнут концевик. Дальнейшее движение антенны " + sDir + " невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });
        }


        private void lineStateChanged(object sender, LineStateChangedEventArgs e)
        {
            if (currentTemplate.limitsLines.Values.Contains(e.line))
            {
                if (e.state == 0)
                {
                    int dir = currentTemplate.limitsLines.SingleOrDefault(x => x.Value == e.line).Key;
                    onLimit(dir);
                }
                else if (slCalibration.Text == "Концевик")
                    this.Invoke((MethodInvoker)delegate
                    {
                        slCalibration.Visible = false;
                    });
            }
        }

        private void usartBytesReceived(object sender, BytesReceivedEventArgs e)
        {
            if (timeoutTimer != null)
                timeoutTimer.Change(1000, Timeout.Infinite);
            //System.Diagnostics.Debug.WriteLine("---");
            int lo = -1;
            int hi = -1;
            for (int co = 0; co < e.count; co++)
                if (e.bytes[co] >= 128)
                    hi = (e.bytes[co] - 128) << 5;
                else
                    lo = e.bytes[co] - 64;
            if (lo != -1 && hi != -1 && encGrayVal != lo + hi)
            {
                encGrayVal = lo + hi;
                int val = encGrayVal;
                for (int mask = val >> 1; mask != 0; mask = mask >> 1)
                {
                    val = val ^ mask;
                }
                this.Invoke((MethodInvoker)delegate { setCurrentAngle(val); });
            }
        }

        private void connect()
        {
            if (controller == null)
            {
                controller = JeromeController.create(currentConnection.jeromeParams);
                if (currentTemplate.uartEncoder)
                    controller.usartBytesReceived += usartBytesReceived;
                controller.onDisconnected += onDisconnect;
                controller.onConnected += onConnect;
                controller.usartBinaryMode = true;
                if (currentConnection.hwLimits)
                    controller.lineStateChanged += lineStateChanged;

            }
            controller.asyncConnect();
            System.Diagnostics.Debug.WriteLine("Rotator connnection started");
            updateGUI(
            delegate ()
              {
                  connectionsDropdown = new ToolStripMenuItem[miConnections.DropDownItems.Count];
                  miConnections.Text = "Отключиться";
                  miConnections.DropDownItems.CopyTo(connectionsDropdown, 0);
                  miConnections.DropDownItems.Clear();
                  Text = currentConnection.name + " идет соединение";
                  lCaption.Text = Text;
                  Icon = (Icon)Resources.ResourceManager.GetObject(CommonInf.icons[currentConnection.icon]);
                  System.Diagnostics.Debug.WriteLine("Rotator interface updated");
              });


        }

        private void updateGUI( Action a )
        {
            if (InvokeRequired)
                Invoke(a);
            else
                a();
        }

        private void readADC()
        {
            int adcVal = controller.readADC(currentTemplate.adc);
            if (adcVal == -1)
                return;
            if (adcDataCount < adcDataLength - 1)
            {
                adcData[adcDataCount++] = adcVal;
            }
            else
            {
                Array.Copy(adcData, 1, adcData, 0, adcDataLength - 1);
                adcData[adcDataLength - 1] = adcVal;
            }
            int newADCVal = 0;
            for (int co = 0; co < adcDataCount; co++)
            {
                newADCVal += adcData[co];
            }
            newADCVal = Convert.ToInt16(newADCVal / adcDataCount);
            if (currentConnection.calibrated)
            {
                int a = Convert.ToInt16(((double)(adcVal - currentConnection.limits[-1]) / (double)(currentConnection.limits[1] - currentConnection.limits[-1])) * 450);
                setCurrentAngle(a);
            }
            else
            {
                showAngleLabel(newADCVal, -1);
                if (calibration)
                {
                    if (newADCVal == 1023 )
                    {
                        calibration = false;
                        calibrationStop(false);
                        currentConnection.calibrated = false;
                        writeConfig();
                        Invoke((MethodInvoker)delegate ()
                      {
                          showMessage("Достигнут предел значений АЦП. Калибровка невозможна. Дождитесь остановки антенны, отрегулируйте АЦП и повторите калибровку.",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                      });
                    }
                    else if (curADCVal < newADCVal - 5 || curADCVal > newADCVal + 5)
                    {
                        curADCVal = newADCVal;
                        calCount = 0;
                    }
                    else
                    {
                        if (calCount++ > 50)
                        {
                            calibration = false;
                            calCount = 0;
                            if (engineStatus == 1)
                            {
                                calibrationStop(true);
                                currentConnection.limits[1] = newADCVal;
                                currentConnection.calibrated = true;
                                writeConfig();
                                Invoke((MethodInvoker)delegate ()
                               {
                                   miSetNorth.Enabled = true;
                                   showMessage("Калибровка завершена.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               });
                            }
                            else
                            {
                                currentConnection.limits[-1] = newADCVal;
                                engine(1);
                                calibrationStart(1);
                            }
                        }
                    }
                }
            }



        }

        private void calibrationStart(int dir)
        {
            currentConnection.calibrated = false;
            currentConnection.northAngle = -1;
            writeConfig();
            calibration = true;
            miSetNorth.Enabled = false;
            Invoke((MethodInvoker)delegate ()
          {
              miCalibrate.Text = "Остановить калибровку";
              slCalibration.Text = "Калибровка";
              slCalibration.Visible = true;
          });
            engine(dir);
            setGear(1);
        }

        private void calibrationStop(bool stopEngine)
        {
            calibration = false;
            if (stopEngine)
                engine(0);
            setGear(0);
            Invoke((MethodInvoker)delegate ()
          {
              miCalibrate.Text = "Калибровать";
              slCalibration.Visible = false;
          });
        }


        private void onConnect( object sender, EventArgs e)
        {
            controllerTimeout = false;
            
            if (currentTemplate.adc != 0)
                adcTimer = new System.Threading.Timer(obj => readADC(), null, 100, 100);
            
            if (currentTemplate.ledLine != 0)
                setLine(currentTemplate.ledLine, 0);
            foreach (int[] dir in currentTemplate.engineLines.Values)
                foreach (int line in dir)
                {
                    setLine(line, 0);
                    toggleLine(line, 0);
                }
            if (currentTemplate.uartTRLine != 0)
                setLine(currentTemplate.uartTRLine, 0);
            if (currentTemplate.limitsLines != null)
                foreach (int line in currentTemplate.limitsLines.Values)
                    setLine(line, 1);

            

            this.Invoke((MethodInvoker)delegate ()
          {
              timer.Enabled = true;
              miSetNorth.Visible = true;
              miSetNorth.Enabled = true;
              pMap.Enabled = true;
              if (currentTemplate.adc != 0)
                  miCalibrate.Visible = true;

              Text = currentConnection.name;
              lCaption.Text = currentConnection.name;
              Icon = (Icon)Resources.ResourceManager.GetObject(CommonInf.icons[currentConnection.icon]);
          });
            if (currentTemplate.uartEncoder)
            {
                if (currentConnection.hwLimits)
                {
                    string lines = controller.readlines();
                    foreach (KeyValuePair<int, int> kv in currentTemplate.limitsLines)
                        if (lines[kv.Value - 1] == '0')
                            onLimit(kv.Key);
                }
                else if (currentConnection.northAngle != -1)
                    currentConnection.limits = new Dictionary<int, int> { { 1, currentConnection.northAngle + 180 }, { -1, currentConnection.northAngle + 180 } };
                scheduleTimeoutTimer();
            }
        }


        private int aD(int a, int b)
        {
            int r = a - b;
            if (r > 180)
                r -= 360;
            else if (r < -180)
                r += 360;
            return r;
        }

        private void rotateToAngle(int angle)
        {
            if (controller != null && controller.connected)
            {
                targetAngle = currentConnection.northAngle + angle - (currentConnection.northAngle + angle > 360 ? 360 : 0);
                startAngle = currentAngle;
                pMap.Invalidate();
                System.Diagnostics.Debug.WriteLine("start " + currentAngle.ToString() + " - " + angle.ToString());
                if (targetAngle != currentAngle)
                {
                    int d = aD(targetAngle, currentAngle);
                    int dir = Math.Sign(d);
                    if (currentTemplate.adc == 0)
                    {
                        int limit = currentConnection.limits[dir];
                        if (limit != -1)
                        {
                            int dS = aD(limit, currentAngle);
                            if (Math.Sign(dS) == dir && Math.Abs(dS) < Math.Abs(d))
                                dir = -dir;
                        }
                    } else
                    {
                        int rt = currentAngle + d;
                        if (rt < 0 || rt > 450)
                            dir = -dir;
                    }
                    engine(dir);
                }
            }
        }

        private int getNearestLimit(int dir)
        {
            int nLimit = dir;
            if (currentConnection.limits[nLimit] == -1)
                nLimit = -nLimit;
            if (currentTemplate.adc == 0)
                return currentConnection.limits[nLimit];
            else return nLimit == -1 ? 0 : 450;
        }


        private void setCurrentAngle(int num)
        {
            if (currentConnection == null)
                return;
            int newAngle = currentTemplate.uartEncoder ? (int)(((double)num) * 0.3515625) : num;
            if (newAngle != currentAngle)
            {
                currentAngle = newAngle;
                angleChanged = true;
                if (currentConnection.northAngle != -1 && engineStatus != 0 && targetAngle != -1)
                {
                    int tD = aD(targetAngle, currentAngle);



                    if (Math.Abs(tD) < 3)
                    {
                        engine(0);
                        targetAngle = -1;
                        pMap.Invalidate();
                    }
                    if (Math.Sign(tD) == engineStatus && currentTemplate.gearLines != null && currentGear > 0
                        && Math.Abs(tD) <= (currentGear + 1) * currentConnection.switchIntervals[0])
                    {
                        System.Diagnostics.Debug.WriteLine("gear- current: " + currentAngle.ToString() + " target: " + targetAngle.ToString());
                        setGear(currentGear - 1);
                    }

                }
                if (currentTemplate.adc == 0 && engineStatus != 0 && !currentConnection.hwLimits)
                {
                    int limit = currentConnection.limits[engineStatus];
                    if (limit != -1)
                    {
                        int ld = aD(limit, currentAngle);
                        if (Math.Sign(ld) == engineStatus && Math.Abs(ld) < 3)
                            onLimit(engineStatus);
                    }
                }

                int displayAngle = currentAngle;
                if (currentConnection.northAngle != -1)
                    displayAngle += (displayAngle < currentConnection.northAngle ? 360 : 0) - currentConnection.northAngle;
                if (currentTemplate.adc != 0)
                    Invoke((MethodInvoker)delegate ()
                   {
                       lOverlap.Visible = currentAngle > 360;
                   });


                showAngleLabel(displayAngle, -1);
            }
        }

        private void loadMap(string fMap)
        {
            if (File.Exists(fMap))
            {
                maps.Add(new Bitmap(fMap));
                if (config.maps.IndexOf(fMap) == -1)
                    config.maps.Add(fMap);
            }
        }

        private void adjustToMap()
        {
            if (WindowState == FormWindowState.Maximized)
            {
                int tmpHeight = Height;
                WindowState = FormWindowState.Normal;
                Height = tmpHeight;
                Top = 0;
                Left = 0;
            }
            if (Math.Abs(mapRatio - (double)pMap.Width / (double)pMap.Height) > 0.01)
            {
                Width = (int)(mapRatio * pMap.Height) + Width - pMap.Width;
                //pMap.Refresh();
            }
        }

        private void pMap_Paint(object sender, PaintEventArgs e)
        {
            if (config.currentMap != -1 && currentConnection != null && currentConnection.northAngle != -1)
            {
                Action<int, Color> drawAngle = (int angle, Color color) =>
                {
                    if (angle == -1)
                        return;
                    double rAngle = (((double)(angle - currentConnection.northAngle)) / 180) * Math.PI;
                    e.Graphics.DrawLine(new Pen(color, 2), pMap.Width / 2, pMap.Height / 2,
                        pMap.Width / 2 + (int)(Math.Sin(rAngle) * (pMap.Height / 2)),
                        pMap.Height / 2 - (int)(Math.Cos(rAngle) * (pMap.Height / 2)));

                };
                drawAngle(currentAngle, Color.Red);
                drawAngle(targetAngle, Color.Green);
                if (currentTemplate.uartEncoder)
                    currentConnection.limits.Values.ToList().ForEach(item => drawAngle(item, Color.Gray));
                else
                {
                    drawAngle(0, Color.Gray);
                    drawAngle(450, Color.Gray);
                }
                //e.Graphics.DrawImage(bmpMap, new Rectangle( 0, 0, pMap.Width, pMap.Height) );
                mapAngle = currentAngle;
            }
        }


        private int mouse2Angle(int mx, int my)
        {
            int angle;
            if (mx == pMap.Width / 2)
            {
                if (my < pMap.Height / 2)
                {
                    angle = 90;
                }
                else
                {
                    angle = 270;
                }
            }
            else
            {
                double y = pMap.Height / 2 - my;
                double x = mx - pMap.Width / 2;
                angle = (int)((Math.Atan(y / x) / Math.PI) * 180);
                if (x < 0)
                {
                    if (y > 0)
                    {
                        angle = 180 + angle;
                    }
                    else
                    {
                        angle = angle - 180;
                    }
                }
            }
            angle = 90 - angle;
            if (angle < 0) { angle += 360; }
            return angle;
        }

        private void pMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (maps.Count > 1)
                    if (config.currentMap < maps.Count - 1)
                        setCurrentMap(++config.currentMap);
                    else
                        setCurrentMap(0);
            }
            else if (currentConnection != null && currentConnection.northAngle != -1 && currentAngle != -1 && !engineTaskActive)
                rotateToAngle(mouse2Angle(e.X, e.Y));
        }

        private void miSetNorth_Click(object sender, EventArgs e)
        {
            FSetNorth fSNorth = new FSetNorth(currentConnection);
            if (fSNorth.ShowDialog(this) == DialogResult.OK)
            {
                currentConnection.northAngle = fSNorth.northAngle;
                if (currentTemplate.uartEncoder && !currentConnection.hwLimits)
                    currentConnection.limits = new Dictionary<int, int> { { 1, currentConnection.northAngle + 180 }, { -1, currentConnection.northAngle + 180 } };
                writeConfig();
                pMap.Invalidate();
            }
        }


        private void showAngleLabel(int cur, int mouse)
        {
            string[] p = lAngle.Text.Split('/');
            lAngle.Text = (cur == -1 ? p[0] : cur.ToString()) + '/' + (mouse == -1 ? p[1] : mouse.ToString());
        }


        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            closingFl = true;
            disconnect();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (angleChanged)
            {
                pMap.Invalidate();
                angleChanged = false;
                mvtBlink = !mvtBlink;
                slMvt.DisplayStyle = mvtBlink ? ToolStripItemDisplayStyle.Image : ToolStripItemDisplayStyle.Text;
            }
            else
            {
                slMvt.DisplayStyle = ToolStripItemDisplayStyle.Text;
            }
            if (engineStatus != 0 && targetAngle != -1 && ( (currentConnection.deviceType == 1 && currentGear < currentTemplate.gearLines.Count() - 1)
                && (currentGear != 0 || ++secOnGear0 > 3)))
            {
                int tD = aD(targetAngle, currentAngle);
                if (Math.Sign(tD) == engineStatus && Math.Abs(tD) > (currentGear + 2) * currentConnection.switchIntervals[0])
                {
                    System.Diagnostics.Debug.WriteLine("gear+ current: " + currentAngle.ToString() + " target: " + targetAngle.ToString());
                    setGear(currentGear + 1);
                }

            }

        }

        private void miMapAdd_Click(object sender, EventArgs e)
        {
            if (ofdMap.ShowDialog() == DialogResult.OK)
            {
                loadMap(ofdMap.FileName);
                setCurrentMap(maps.Count - 1);
                writeConfig();
            }
        }

        private void pMap_Resize(object sender, EventArgs e)
        {
            //pMap.Refresh();
        }


        private void miConnections_Click(object sender, EventArgs e)
        {
            if (controller != null && controller.active)
            {
                disconnect();
            }

        }

        private void miNewConnection_Click(object sender, EventArgs e)
        {
            AntennaeRotatorConnectionSettings nConn = new AntennaeRotatorConnectionSettings();
            if (editConnection(nConn))
            {
                config.connections.Add(nConn);
                createConnectionMenuItem(config.connections.IndexOf(nConn));
                writeConfig();
            }
        }

        private void createConnectionMenuItem(int index)
        {
            ToolStripMenuItem miConn = new ToolStripMenuItem();
            miConn.Text = config.connections[index].name;
            miConn.Click += delegate (object sender, EventArgs e)
            {
                loadConnection(index);
            };
            miConnections.DropDownItems.Insert(index, miConn);
        }

        public bool editConnection(AntennaeRotatorConnectionSettings conn)
        {
            fConnectionParams fParams = new fConnectionParams(conn);
            fParams.ShowDialog(this);
            bool result = fParams.DialogResult == DialogResult.OK;
            if (result)
            {
                conn.deviceType = fParams.cbDeviceType.SelectedIndex;
                conn.jeromeParams.host = fParams.tbHost.Text.Trim();
                conn.jeromeParams.port = Convert.ToInt16(fParams.tbPort.Text.Trim());
                conn.name = fParams.tbName.Text.Trim();
                conn.jeromeParams.usartPort = Convert.ToInt16(fParams.tbUSARTPort.Text.Trim());
                conn.icon = fParams.icon;
                if (conn.deviceType == 0)
                    conn.hwLimits = fParams.chbHwLimits.Checked;
                conn.switchIntervals[0] = Convert.ToInt32(fParams.nudIntervalOn.Value);
                conn.switchIntervals[1] = Convert.ToInt32(fParams.nudIntervalOff.Value);
                conn.esMhz = Convert.ToInt32(fParams.tbESMHz.Text.Trim());
                writeConfig();
                if (conn.Equals(currentConnection))
                {
                    Icon = (Icon)Resources.ResourceManager.GetObject(CommonInf.icons[conn.icon]);
                }
            }
            return result;
        }

        private void miEditConnections_Click(object sender, EventArgs e)
        {
            new FConnectionsList(config).ShowDialog(this);
            updateConnectionsMenu();
        }


        private void fMain_Resize(object sender, EventArgs e)
        {
            if (mapRatio != 0)
                adjustToMap();
        }


        private void lSizeM_Click(object sender, EventArgs e)
        {
            Height = 300;
        }

        private void lSizeP_Click(object sender, EventArgs e)
        {
            Height = 800;
        }

        private void bStop_Click(object sender, EventArgs e)
        {
            engine(0);
            if (targetAngle != -1)
            {
                targetAngle = -1;
                pMap.Invalidate();
            }
        }


        private void miMapRemove_Click(object sender, EventArgs e)
        {
            maps.RemoveAt(config.currentMap);
            config.maps.RemoveAt(config.currentMap);
            if (maps.Count > 0)
            {
                if (config.currentMap > 0)
                    setCurrentMap(--config.currentMap);
                else
                    setCurrentMap(1);
            }
            else
            {
                config.currentMap = -1;
                pMap.BackgroundImage = null;
                pMap.Refresh();
                writeConfig();
            }
        }

        private void miAbout_Click(object sender, EventArgs e)
        {
        }


        private void pMap_MouseMove(object sender, MouseEventArgs e)
        {
            showAngleLabel(-1, mouse2Angle(e.X, e.Y));
        }

        private void miIngnoreEngineOffMovement_CheckStateChanged(object sender, EventArgs e)
        {
            currentConnection.ignoreEngineOffMovement = miIngnoreEngineOffMovement.Checked;
            writeConfig();
        }


/*
        public void esMessage(int mhz, bool trx)
        {
            return;
            //int mhz = ((int)e.vfoa) / 1000000;
            if ( config.connections.Exists( x => x.esMhz == mhz) ) 
                   this.Invoke((MethodInvoker)delegate {
                       int dst = config.connections.FindIndex(x => x.esMhz == mhz);
                       if ( config.connections[dst] != currentConnection )
                       {
                           if (controller != null && controller.connected)
                               disconnect();
                           loadConnection(dst);
                       }
                   });
        }
        */

        private void miCalibrate_Click(object sender, EventArgs e)
        {
            if (calibration)
                calibrationStop(true);
            else
            {
                calibrationStart(-1);
            }

        }
    }

    class DeviceTemplate
    {
        internal Dictionary<int, int[]> engineLines;
        internal Dictionary<int, int> limitsLines;
        internal int[] gearLines;
        internal Dictionary<int, int> rotateButtonsLines;
        internal int adc;
        internal bool uartEncoder;
        internal int uartTRLine;
        internal int ledLine;
    }




    public static class CommonInf
    {
        public static string[] icons = { "icon_ant1", "icon_10", "icon_40", "icon_left", "icon_right", "icon_up" };
    }

}
