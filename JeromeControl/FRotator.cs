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
using JeromeControl.Properties;

namespace AntennaeRotator
{

    public partial class fMain : Form
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
        FormState formState = new FormState();
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
        ConnectionSettings currentConnection;
        ConnectionGroup currentConnectionGroup;
        bool closingFl = false;
        bool loaded = false;
        bool formSPmodified = false;
        int connectionFromArgs = -1;
        IPEndPoint esEndPoint;
        ExpertSyncConnector esConnector;

        public int getCurrentAngle() {
            return currentAngle;
        }

        public fMain( string[] args )
        {
            InitializeComponent();
            if (File.Exists( Application.StartupPath + "\\config.xml"))
            {
                loadConfig();
                updateConnectionsMenu();
                updateConnectionGroupsMenu();
                Trace.Listeners.Add(new TextWriterTraceListener(Application.StartupPath + "\\rotator.log"));
                Trace.AutoFlush = true;
                Trace.Indent();
                Trace.WriteLine("Initialization");

                if (args.Count() > 0)
                {
                    Trace.WriteLine("Command line argument " + args[0]);
                    connectionFromArgs = Convert.ToInt16(args[0]);
                }
                string currentMapStr = "";
                if ( formState.currentMap != -1 && formState.currentMap < formState.maps.Count )
                    currentMapStr = formState.maps[ formState.currentMap ];
                formState.maps.RemoveAll( item => !File.Exists(item) );
                if (!currentMapStr.Equals(string.Empty))
                    formState.currentMap = formState.maps.IndexOf(currentMapStr);
                else
                    formState.currentMap = -1;
                formState.maps.ForEach( item => loadMap( item ) );
                if (formState.currentMap != -1)
                    setCurrentMap(formState.currentMap);
                else if (formState.maps.Count > 0 )
                    setCurrentMap(0);
                prevHeight = Height;
            }
        }

        private void scheduleTimeoutTimer()
        {
            timeoutTimer = new System.Threading.Timer(
                obj =>
                {
                    this.Invoke((MethodInvoker)delegate
                   {
                        if ( currentConnection != null )
                            showMessage("Потеряна связь с устройством!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (controller != null && controller.connected)
                            disconnect();
                   });
                },
                null, 1000, Timeout.Infinite);
        }

        private void setCurrentMap(int val)
        {
            formState.currentMap = val;
            pMap.BackgroundImage = maps[val];
            writeConfig();
            mapRatio = (double)maps[val].Width / (double)maps[val].Height;
            adjustToMap();
        }

        private void updateConnectionsMenu() {
            while ( miConnections.DropDownItems.Count > 2 ) {
                miConnections.DropDownItems.RemoveAt(0);

            }
            for (int co = 0; co < formState.connections.Count; co++)
                createConnectionMenuItem(co);
        }

        private void updateConnectionGroupsMenu()
        {
            foreach (ConnectionGroup cg in formState.connectionGroups)
            {
                int idx = 0;
                ToolStripMenuItem miCG = new ToolStripMenuItem();
                miCG.Text = cg.name;
                miCG.Click += delegate (object sender, EventArgs e)
                {
                    connectGroup(cg);
                };
                miConnections.DropDownItems.Insert(idx++, miCG);
            }

        }

        private void formSPfromConnection(int ci)
        {
            ConnectionSettings c = formState.connections[ci];
            if (!c.formSize.IsEmpty)
            {
                this.DesktopBounds =
                    new Rectangle(c.formLocation, c.formSize);
                formSPmodified = false;
            }
        }

        private void loadConnection(int index)
        {
            currentConnection = formState.connections[index];
            formState.currentConnection = index;
            if ( !formSPmodified )
                formSPfromConnection(index);

            currentTemplate = getTemplate( currentConnection.deviceType );
            if ( currentConnection.limitsSerialize != null )
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
                return MessageBox.Show(fMain.ActiveForm, text, currentConnection == null ? "AntennaNetRotator" : currentConnection.name, buttons, icon);
            });
        }


        private void miModuleSettings_Click(object sender, EventArgs e)
        {
            (new fModuleSettings()).ShowDialog();
        }

        private void connectGroup(ConnectionGroup cg)
        {
            if (cg.items.Count == 0)
                showMessage("Выбранная группа " + cg.name + " не содержит подключений.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                currentConnectionGroup = cg;
                loadConnection(cg.items[0].connectionId);
            }
        }


        public void engine(int val)
        {
            if ( val != engineStatus && ( limitReached == 0 || limitReached != val )) {
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
                      if ( slCalibration.Text != "Концевик" || !slCalibration.Visible )
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
                            await TaskEx.Delay( currentConnection.switchIntervals[1] * 1000 );
                            System.Diagnostics.Debug.WriteLine("Delayed switch off");
                            toggleLine(currentTemplate.engineLines[prevDir][0], 0);
                            clearEngineTask();
                        });
                }
                if ( val != 0 && !engineTaskActive) {
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

        private void onDisconnect(object obj, DisconnectEventArgs e) {
            currentConnection = null;
            if ( !closingFl )
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
                if ( currentConnection.limits != null )
                {
                    currentConnection.limitsSerialize = new int[] { -1, -1 };
                    if (currentConnection.limits.ContainsKey(1))
                        currentConnection.limitsSerialize[0] = currentConnection.limits[1];
                    if (currentConnection.limits.ContainsKey(-1))
                        currentConnection.limitsSerialize[1] = currentConnection.limits[-1];
                }
            }
            using (StreamWriter sw = new StreamWriter( Application.StartupPath + "\\config.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(FormState));
                ser.Serialize(sw, formState);
            }
        }


        private void setLine(int line, int mode)
        {
            if (controller != null && controller.connected)
                controller.setLineMode(line, mode);
        }

        private void toggleLine(int line, int state )
        {
            if (controller != null && controller.connected)
                controller.switchLine(line, state);
        }

        private void loadConfig()
        {
            XmlSerializer ser = new XmlSerializer(typeof(FormState));
            using (FileStream fs = File.OpenRead( Application.StartupPath + "\\config.xml"))
            {
                try
                {
                    formState = (FormState)ser.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading config: " + ex.ToString());
                }
            }
        }

        private void offLimit()
        {
            this.Invoke((MethodInvoker)delegate {
                if (slCalibration.Visible && slCalibration.Text == "Концевик")
                    slCalibration.Visible = false;
            });
            limitReached = 0;
        }

        private void onLimit( int dir )
        {
            if (limitReached == dir)
                return;
            if (engineStatus == dir)
                engine(0);
            if ( currentConnection.hwLimits )
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


        private void lineStateChanged( object sender, LineStateChangedEventArgs e )
        {
            if ( currentTemplate.limitsLines.Values.Contains( e.line ) )
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

        private void usartBytesReceived( object sender, BytesReceivedEventArgs e )
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
            if ( lo != -1 && hi != -1 && encGrayVal != lo + hi )
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
            if ( controller.connect() ) {                           
                miConnections.Text = "Отключиться";
                controller.usartBinaryMode = true;
                if ( currentConnection.hwLimits )
                    controller.lineStateChanged += lineStateChanged;
                controller.usartBytesReceived += usartBytesReceived;
                controller.disconnected += onDisconnect;
                connectionsDropdown = new ToolStripMenuItem[miConnections.DropDownItems.Count];
                miConnections.DropDownItems.CopyTo(connectionsDropdown,0);
                miConnections.DropDownItems.Clear();

                miConnectionGroups.Visible = false;
                //miConnectionParams.Enabled = false;


                setLine(currentTemplate.ledLine, 0);
                foreach (int[] dir in currentTemplate.engineLines.Values)
                    foreach ( int line in dir) { 
                        setLine( line, 0 );
                        toggleLine( line, 0 );
                    }
                setLine(currentTemplate.uartTRLine, 0);
                foreach (int line in currentTemplate.limitsLines.Values)
                    setLine(line, 1);

                timer.Enabled = true;

                miSetNorth.Visible = true;
                miSetNorth.Enabled = true;

                Text = currentConnection.name;
                lCaption.Text = currentConnection.name;
                if (currentConnectionGroup != null)
                    Text += " (" + currentConnectionGroup.name + ")";
                Icon = (Icon) Resources.ResourceManager.GetObject(CommonInf.icons[currentConnection.icon]);
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
            if ( controller != null && controller.connected ) {
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



                    if (Math.Abs(tD) < 3 )
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
                    displayAngle += ( displayAngle < currentConnection.northAngle ? 360 : 0 ) - currentConnection.northAngle;

                showAngleLabel(displayAngle, -1);
            }
        }

        private void loadMap(string fMap)
        {
            if (File.Exists(fMap))
            {
                maps.Add( new Bitmap(fMap) );
                if (formState.maps.IndexOf(fMap) == -1)
                    formState.maps.Add(fMap);
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
            if (formState.currentMap != -1 && currentConnection != null && currentConnection.northAngle != -1 )
            {
                Action<int,Color> drawAngle = (int angle, Color color) =>
                    {
                        if (angle == -1)
                            return;
                        double rAngle = (((double)(angle - currentConnection.northAngle)) / 180) * Math.PI;
                        e.Graphics.DrawLine(new Pen(color, 2), pMap.Width / 2, pMap.Height / 2,
                            pMap.Width / 2 + (int)(Math.Sin(rAngle) * (pMap.Height / 2)), 
                            pMap.Height / 2 - (int)(Math.Cos(rAngle) * ( pMap.Height / 2 ) ) );

                    };
                drawAngle(currentAngle, Color.Red );
                drawAngle(targetAngle, Color.Green );
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
                    if (formState.currentMap < maps.Count - 1)
                        setCurrentMap(++formState.currentMap);
                    else
                        setCurrentMap(0);
            }
            else if ( currentConnection != null && currentConnection.northAngle != -1 && currentAngle != -1 && !engineTaskActive)            
                rotateToAngle(mouse2Angle( e.X, e.Y ));            
        }

        private void miSetNorth_Click(object sender, EventArgs e)
        {
            FSetNorth fSNorth = new FSetNorth(currentConnection);
            if (fSNorth.ShowDialog(this) == DialogResult.OK)
            {
                currentConnection.northAngle = fSNorth.northAngle;
                if ( !currentConnection.hwLimits )
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
                currentConnectionGroup = null;
            }

        }

        private void miNewConnection_Click(object sender, EventArgs e)
        {
            ConnectionSettings nConn = new ConnectionSettings();
            if (editConnection(nConn))
            {
                formState.connections.Add(nConn);
                createConnectionMenuItem(formState.connections.IndexOf(nConn));
                writeConfig();
            }
        }

        private void createConnectionMenuItem(int index)
        {
            ToolStripMenuItem miConn = new ToolStripMenuItem();
            miConn.Text = formState.connections[ index ].name;
            miConn.Click += delegate(object sender, EventArgs e)
            {
                loadConnection(index);
            };
            miConnections.DropDownItems.Insert(index, miConn);
        }

        public bool editConnection(ConnectionSettings conn)
        {
            fConnectionParams fParams = new fConnectionParams(conn);
            fParams.ShowDialog(this);
            bool result = fParams.DialogResult == DialogResult.OK;
            if (result)
            {
                conn.jeromeParams.host = fParams.tbHost.Text.Trim();
                conn.jeromeParams.port = Convert.ToInt16( fParams.tbPort.Text.Trim() );
                conn.name = fParams.tbName.Text.Trim();
                conn.jeromeParams.usartPort = Convert.ToInt16( fParams.tbUSARTPort.Text.Trim() );
                conn.icon = fParams.icon;
                conn.hwLimits = fParams.chbHwLimits.Checked;
                conn.switchIntervals[0] = Convert.ToInt32( fParams.nudIntervalOn.Value );
                conn.switchIntervals[1] = Convert.ToInt32(fParams.nudIntervalOff.Value);
                writeConfig();
                if ( conn.Equals( currentConnection ) )
                {
                    Icon = (Icon) Resources.ResourceManager.GetObject(CommonInf.icons[conn.icon]);
                }
            }
            return result;
        }

        private void miEditConnections_Click(object sender, EventArgs e)
        {
            new FConnectionsList(formState).ShowDialog(this);
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
            maps.RemoveAt(formState.currentMap);
            formState.maps.RemoveAt(formState.currentMap);
            if (maps.Count > 0)
            {
                if (formState.currentMap > 0)
                    setCurrentMap(--formState.currentMap);
                else
                    setCurrentMap(1);
            }
            else
            {
                formState.currentMap = -1;
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
            if (connectionFromArgs == -1 )
            {
                if (formState.currentConnection != -1 && formState.connections.Count > formState.currentConnection)
                    formSPfromConnection(formState.currentConnection);
            } 
            else if (formState.connections.Count > connectionFromArgs)
                loadConnection(connectionFromArgs);
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
            if (miExpertSync.Checked)
            {
                FESConnection fes;
                if (esEndPoint != null)
                    fes = new FESConnection(esEndPoint.Address.ToString(), esEndPoint.Port);
                else
                    fes = new FESConnection();
                fes.ShowDialog();
                if (fes.DialogResult == DialogResult.OK)
                {
                    esConnector = ExpertSyncConnector.create(fes.host, fes.port);
                    esEndPoint = new IPEndPoint(IPAddress.Parse(fes.host), fes.port);
                    esConnector.disconnected += esDisconnected;
                    esConnector.onMessage += esMessage;
                    writeConfig();
                    miExpertSync.Checked = esConnector.connect();
                }
            }
            else
            {
                esConnector.disconnect();
                miExpertSync.Checked = false;
            }

        }

        private void esDisconnected(object sender, DisconnectEventArgs e)
        {
            if (!e.requested)
                MessageBox.Show("Соединение с ExpertSync потеряно!");
            miExpertSync.Checked = false;
        }

        private void esMessage(object sender, MessageEventArgs e)
        {
            int mhz = ((int)e.vfoa) / 1000000;
            if (currentConnectionGroup != null && currentConnectionGroup.items.Exists( x => x.esMhz == mhz))
                this.Invoke((MethodInvoker)delegate {
                    int dst = currentConnectionGroup.items.First(x => x.esMhz == mhz).connectionId;
                    if ( formState.connections[dst] != currentConnection )
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

        public string name = "";
        public JeromeConnectionParams jeromeParams = new JeromeConnectionParams();
        public int northAngle = -1;
        public int[] switchIntervals = new int[] { 5, 5 };
        [XmlIgnoreAttribute]
        public Dictionary<int, int> limits = new Dictionary<int, int> { { 1, -1 }, { -1, -1 } };
        public int deviceType = 0;
        public int icon = 0;
        public System.Drawing.Point formLocation;
        public System.Drawing.Size formSize;
        internal bool ignoreEngineOffMovement;
        public bool hwLimits;
        public int[] limitsSerialize;

        public override string ToString()
        {
            return name;
        }

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

        public bool contains( int id )
        {
            return items.Exists(x => x.connectionId == id);
        }

        public string mhzStr( int id )
        {
            if (contains(id))
                return string.Join(";", items.Where(x => x.connectionId == id).Select(x => x.esMhz.ToString()));
            else
                return "";
        }

        public void removeConnection( int id )
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
        public int currentConnection = -1;
        public List<string> maps = new List<string>();
        public int currentMap = -1;
        public List<ConnectionSettings> connections = new List<ConnectionSettings>();
        public List<ConnectionGroup> connectionGroups = new List<ConnectionGroup>();
    }

    public static class CommonInf
    {
        public static string[] icons = { "icon_ant1", "icon_10", "icon_40", "icon_left", "icon_right", "icon_up" };
    }

}
