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

namespace AntennaeRotator
{

    public partial class FRotator : Form
    {
        static DeviceTemplate[] templates = {
                    new DeviceTemplate { engineLines = new Dictionary<int, int[]>{ { 1, new int[] { 16, 20 } }, {-1, new int[] { 15, 19} } },
                                            ledLine = 22, uartTRLine = 12,
                                            limitsLines = new Dictionary<int, int> {  { 1, 14 }, { -1, 13 } }
                                        } //0 NetRotator5
                    };

        internal bool editConnectionGroup(ConnectionSettings connectionSettings)
        {
            throw new NotImplementedException();
        }

        DeviceTemplate currentTemplate;
        AntennaeRotatorConfig config;
        JeromeController controller;
        int currentAngle = -1;
        int targetAngle = -1;
        int engineStatus = 0;
        int mapAngle = -1;
        int startAngle = -1;
        volatile int limitReached = 0;
        int encGrayVal = -1;
        bool angleChanged = false;
        bool mvtBlink = false;
        List<Bitmap> maps = new List<Bitmap>();
        ToolStripMenuItem[] connectionsDropdown;
        System.Threading.Timer timeoutTimer;
        volatile Task engineTask;
        CancellationTokenSource engineTaskCTS = new CancellationTokenSource();
        volatile bool engineTaskActive;

        internal void clearLimits()
        {
            currentConnection.limits = new Dictionary<int, int> { { 1, -1 }, { -1, -1 } };
            writeConfig();
        }

        int prevHeight;
        double mapRatio = 0;
        AntennaeRotatorConnectionSettings currentConnection;
        bool closingFl = false;
        bool loaded = false;
        bool formSPmodified = false;
        int connectionFromArgs = -1;
        IPEndPoint esEndPoint;
        ExpertSyncConnector esConnector;
        JCAppContext appContext;

        public int getCurrentAngle()
        {
            return currentAngle;
        }

        public FRotator(JCAppContext _appContext)
        {
            InitializeComponent();
            appContext = _appContext;
            config = _appContext.config.antennaeRotatorConfig;
            updateConnectionsMenu();
            Trace.Listeners.Add(new TextWriterTraceListener(Application.StartupPath + "\\rotator.log"));
            Trace.AutoFlush = true;
            Trace.Indent();
            Trace.WriteLine("Initialization");

            string currentMapStr = "";
            if (this.config.currentMap != -1 && this.config.currentMap < this.config.maps.Count)
                currentMapStr = this.config.maps[this.config.currentMap];
            this.config.maps.RemoveAll(item => !File.Exists(item));
            if (!currentMapStr.Equals(string.Empty))
                this.config.currentMap = this.config.maps.IndexOf(currentMapStr);
            else
                this.config.currentMap = -1;
            this.config.maps.ForEach(item => loadMap(item));
            if (this.config.currentMap != -1)
                setCurrentMap(this.config.currentMap);
            else if (this.config.maps.Count > 0)
                setCurrentMap(0);
            prevHeight = Height;
        }

        private void scheduleTimeoutTimer()
        {
            timeoutTimer = new System.Threading.Timer(
                obj =>
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (currentConnection != null)
                            showMessage("Потеряна связь с устройством!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (controller != null && controller.connected)
                            disconnect();
                    });
                },
                null, 1000, Timeout.Infinite);
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


        private void formSPfromConnection(int ci)
        {
            AntennaeRotatorConnectionSettings c = config.connections[ci];
            if (!c.formSize.IsEmpty)
            {
                this.DesktopBounds =
                    new Rectangle(c.formLocation, c.formSize);
                formSPmodified = false;
            }
        }

        private void loadConnection(int index)
        {
            currentConnection = config.connections[index];
            config.currentConnection = index;
            if (!formSPmodified)
                formSPfromConnection(index);

            currentTemplate = getTemplate(currentConnection.deviceType);
            if (currentConnection.limitsSerialize != null)
            {
                currentConnection.limits = new Dictionary<int, int> { { 1, currentConnection.limitsSerialize[0] },
                    { -1, currentConnection.limitsSerialize[1] }
                };
            }

            writeConfig();

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

        public void engine(int val)
        {
            if (val != engineStatus && (limitReached == 0 || limitReached != val))
            {
                //System.Diagnostics.Debug.WriteLine("Engine switch begins");
                this.UseWaitCursor = true;
                /*Cursor tmpCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;*/
                if (val == 0 || engineStatus != 0)
                {
                    int prevDir = engineStatus;
                    toggleLine(currentTemplate.engineLines[prevDir][1], 0);
                    System.Diagnostics.Debug.WriteLine("Scheduling Delayed switch off");
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
                            System.Diagnostics.Debug.WriteLine("Delayed switch off");
                            toggleLine(currentTemplate.engineLines[prevDir][0], 0);
                            clearEngineTask();
                        });
                }
                if (val != 0 && !engineTaskActive)
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
                    System.Diagnostics.Debug.WriteLine("Scheduling Delayed switch on");
                    engineTask = TaskEx.Run(
                        async () =>
                        {
                            await TaskEx.Delay(currentConnection.switchIntervals[0] * 1000, ct);
                            if (!ct.IsCancellationRequested)
                            {
                                System.Diagnostics.Debug.WriteLine("Delayed switch on");
                                toggleLine(currentTemplate.engineLines[val][1], 1);
                            }
                            clearEngineTask();
                        }, ct);
                    if (limitReached != 0 && !currentConnection.hwLimits)
                        offLimit();
                }
                engineStatus = val;
                System.Diagnostics.Debug.WriteLine("engine " + val.ToString());
                //Cursor.Current = tmpCursor;
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

        private async void disconnect()
        {
            if (timeoutTimer != null)
            {
                timeoutTimer.Dispose();
                timeoutTimer = null;
            }
            if (controller != null && controller.connected)
            {
                if (engineStatus != 0)
                {
                    engine(0);
                }
                else
                    clearEngineTask();
                if (engineTask != null)
                {
                    await engineTask;
                    engineTask.Dispose();
                    engineTask = null;
                }
                toggleLine(currentTemplate.ledLine, 0);
                controller.disconnect();
            }
        }

        private void onDisconnect(object obj, DisconnectEventArgs e)
        {
            currentConnection = null;
            if (!closingFl)
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
                    miConnectionGroups.Visible = true;
                    miIngnoreEngineOffMovement.Visible = false;
                    timer.Enabled = false;
                    targetAngle = -1;
                    pMap.Invalidate();
                    offLimit();
                });
        }

        public void writeConfig()
        {
            if (loaded && currentConnection != null)
            {
                System.Drawing.Rectangle bounds = this.WindowState != FormWindowState.Normal ? this.RestoreBounds : this.DesktopBounds;
                currentConnection.formLocation = bounds.Location;
                currentConnection.formSize = bounds.Size;
                if (currentConnection.limits != null)
                {
                    currentConnection.limitsSerialize = new int[] { -1, -1 };
                    if (currentConnection.limits.ContainsKey(1))
                        currentConnection.limitsSerialize[0] = currentConnection.limits[1];
                    if (currentConnection.limits.ContainsKey(-1))
                        currentConnection.limitsSerialize[1] = currentConnection.limits[-1];
                }
            }
            appContext.config.antennaeRotatorConfig = config;
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
            miConnections.Enabled = false;
            if (controller == null)
                controller = JeromeController.create(currentConnection.jeromeParams);
            UseWaitCursor = true;
            if (controller.connect())
            {
                miConnections.Text = "Отключиться";
                controller.usartBinaryMode = true;
                if (currentConnection.hwLimits)
                    controller.lineStateChanged += lineStateChanged;
                controller.usartBytesReceived += usartBytesReceived;
                controller.disconnected += onDisconnect;
                connectionsDropdown = new ToolStripMenuItem[miConnections.DropDownItems.Count];
                miConnections.DropDownItems.CopyTo(connectionsDropdown, 0);
                miConnections.DropDownItems.Clear();

                miConnectionGroups.Visible = false;
                //miConnectionParams.Enabled = false;


                setLine(currentTemplate.ledLine, 0);
                foreach (int[] dir in currentTemplate.engineLines.Values)
                    foreach (int line in dir)
                    {
                        setLine(line, 0);
                        toggleLine(line, 0);
                    }
                setLine(currentTemplate.uartTRLine, 0);
                foreach (int line in currentTemplate.limitsLines.Values)
                    setLine(line, 1);

                timer.Enabled = true;

                miSetNorth.Visible = true;
                miSetNorth.Enabled = true;

                Text = currentConnection.name;
                lCaption.Text = currentConnection.name;
                Icon = (Icon)Resources.ResourceManager.GetObject(CommonInf.icons[currentConnection.icon]);
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
            else
            {
                showMessage("Подключение не удалось", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            miConnections.Enabled = true;
            UseWaitCursor = false;
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
                pMap.Invalidate();
                System.Diagnostics.Debug.WriteLine("start " + currentAngle.ToString() + " - " + angle.ToString());
                if (targetAngle != currentAngle)
                {
                    int d = aD(targetAngle, currentAngle);
                    int dir = Math.Sign(d);
                    int limit = currentConnection.limits[dir];
                    if (limit != -1)
                    {
                        int dS = aD(limit, currentAngle);
                        if (Math.Sign(dS) == dir && Math.Abs(dS) < Math.Abs(d))
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
            return currentConnection.limits[nLimit];
        }


        private void setCurrentAngle(int num)
        {
            if (currentConnection == null)
                return;
            int newAngle = (int)(((double)num) * 0.3515625);
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
                }
                if (engineStatus != 0 && !currentConnection.hwLimits)
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
                currentConnection.limits.Values.ToList().ForEach(item => drawAngle(item, Color.Gray));
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
                if (!currentConnection.hwLimits)
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
            if (controller != null && controller.connected)
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
            if (controller != null && controller.connected)
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
                conn.jeromeParams.host = fParams.tbHost.Text.Trim();
                conn.jeromeParams.port = Convert.ToInt16(fParams.tbPort.Text.Trim());
                conn.name = fParams.tbName.Text.Trim();
                conn.jeromeParams.usartPort = Convert.ToInt16(fParams.tbUSARTPort.Text.Trim());
                conn.icon = fParams.icon;
                conn.hwLimits = fParams.chbHwLimits.Checked;
                conn.switchIntervals[0] = Convert.ToInt32(fParams.nudIntervalOn.Value);
                conn.switchIntervals[1] = Convert.ToInt32(fParams.nudIntervalOff.Value);
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

        private void fMain_Load(object sender, EventArgs e)
        {
            if (config.currentConnection != -1 && config.connections.Count > config.currentConnection)
                formSPfromConnection(config.currentConnection);
            loaded = true;
        }

        private void fMain_ResizeEnd(object sender, EventArgs e)
        {
            if (loaded)
            {
                if (currentConnection != null)
                    writeConfig();
                else
                    formSPmodified = true;
            }
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

        private void miConnectionGroupsList_Click(object sender, EventArgs e)
        {
        }

        private void miExpertSync_Click(object sender, EventArgs e)
        {

        }

        private void esDisconnected(object sender, DisconnectEventArgs e)
        {
            if (!e.requested)
                MessageBox.Show("Соединение с ExpertSync потеряно!");
            miExpertSync.Checked = false;
        }

        public void esMessage(int mhz)
        {
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

    }

    class DeviceTemplate
    {
        internal Dictionary<int, int[]> engineLines;
        internal Dictionary<int, int> limitsLines;
        internal int uartTRLine;
        internal int ledLine;
    }

    public class ConnectionSettings
    {


    }


    public class ConnectionGroupEntry
    {
        public int connectionId;
        public int esMhz;
    }

    public class ConnectionGroup : ICloneable
    {
        public string name;
        public List<ConnectionGroupEntry> items = new List<ConnectionGroupEntry>();

        public bool contains(int id)
        {
            return items.Exists(x => x.connectionId == id);
        }

        public string mhzStr(int id)
        {
            if (contains(id))
                return string.Join(";", items.Where(x => x.connectionId == id).Select(x => x.esMhz.ToString()));
            else
                return "";
        }

        public void removeConnection(int id)
        {
            if (contains(id))
                items.RemoveAll(x => x.connectionId == id);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }


    }

    public class FormState
    {
    }

    public static class CommonInf
    {
        public static string[] icons = { "icon_ant1", "icon_10", "icon_40", "icon_left", "icon_right", "icon_up" };
    }

}
