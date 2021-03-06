﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jerome;
using System.IO;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Net;
using AsyncConnectionNS;
using JeromeModuleSettings;
using JeromeControl;
using System.Threading;
using StorableFormState;

namespace NetComm
{
    public partial class FNetComm : JCChildForm
    {
        public static readonly int[] lines = { 4, 21, 20, 19, 18, 5, 6 };
        public static readonly int enLine = 22;
        public static readonly int clkLine = 7;

        public override StorableFormConfig storableConfig
        {
            get
            {
                return appContext.config.getFormState(this);
            }
        }

        protected Dictionary<JeromeConnectionParams, JeromeConnectionState> connections = new Dictionary<JeromeConnectionParams,JeromeConnectionState>();
        protected List<CheckBox> buttons = new List<CheckBox>();
        protected List<string> buttonLabels = new List<string>();
        protected Dictionary<JeromeConnectionParams, ToolStripMenuItem> menuControl = new Dictionary<JeromeConnectionParams, ToolStripMenuItem>();
        protected Dictionary<JeromeConnectionParams, ToolStripMenuItem> menuWatch = new Dictionary<JeromeConnectionParams, ToolStripMenuItem>();
        protected System.Threading.Timer watchTimer;
        protected Color buttonsColor;
        protected Dictionary<int, int> esBindings = new Dictionary< int, int> ();
        protected NetCommConfig config { get { return (NetCommConfig)componentConfig; } }
        protected bool trx = false;
        protected volatile bool closing = false;
        protected volatile bool watchPending = false;

        private bool connected
        {
            get
            {
                return connections.Values.ToList().Exists( x => x.active && x.connected );
            }
        }

        protected void updateButtonLabel(int no)
        {
            if (buttonLabels[no].Equals(string.Empty))
                buttons[no].Text = (no + 1).ToString();
            else
                buttons[no].Text = buttonLabels[no];
        }

        public FNetComm( JCAppContext _appContext, int __idx ) : base( _appContext, __idx)
        {
            InitializeComponent();
            Width = 200;

            appContext = _appContext;
            initConfig();
            for (int co = buttonLabels.Count(); co < lines.Count(); co++)
                buttonLabels.Add("");
            miRelaySettings.Enabled = connections.Count > 0;
            foreach ( JeromeConnectionParams c in connections.Keys )
                createConnectionMI( c );
            Parallel.ForEach(connections.Where(x => x.Value.active),
                x => connect(x.Key));

        }

        protected void onWatchTimer()
        {
            if (watchPending)
                return;
            watchPending = true;
            Parallel.ForEach( connections.Where(x => x.Value.watch), x =>
            {
                JeromeControllerState state = x.Key.getState();
                if (state != null )
                    for (int co = 0; co < lines.Count(); co++)
                        x.Value.linesStates[co] = state.linesStates[lines[co] - 1];
            });
            try
            {
                if (!closing)
                    this.Invoke((MethodInvoker)delegate
                        { updateButtonsMode(); });
            } 
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine( e.ToString());
            }
            watchPending = false;
        }


        protected void createConnectionMI(JeromeConnectionParams c)
        {
            createConnectionMI(c, true);
            createConnectionMI(c, false);
        }


        protected void createConnectionMI(JeromeConnectionParams c, bool watch)
        {
            ToolStripMenuItem mi = new ToolStripMenuItem();
            mi.Text = c.name;
            if (watch)
            {
                mi.Visible = true;
                mi.CheckOnClick = true;
                mi.Checked = connections[c].watch;
                mi.Click += delegate(object sender, EventArgs e)
                {
                    connections[c].watch = !connections[c].watch;
                    writeConfig();
                };
                miWatch.DropDownItems.Add(mi);
                menuWatch[c] = mi;
            }
            else
            {
                mi.Visible = true;
                mi.Click += delegate(object sender, EventArgs e)
                {
                    if (connections[c].active)
                    {
                        connections[c].controller.disconnect();
                        connections[c].active = false;
                        writeConfig();
                    }
                    else
                        connect(c);
                };
                miControl.DropDownItems.Add(mi);
                menuControl[c] = mi;
            }
        }

        protected void connect(JeromeConnectionParams cp)
        {
            connections[cp].active = true;
            connections[cp].controller = JeromeController.create(cp);
            connections[cp].controller.onDisconnected += controllerDisconnected;
            connections[cp].controller.onConnected += controllerConnected;
            writeConfig();
            connections[cp].controller.asyncConnect();
        }

        protected void controllerConnected( object sender, EventArgs e )
        {
            bool senderFound = false;
            KeyValuePair<JeromeConnectionParams, JeromeConnectionState> senderEntry =
                connections.FirstOrDefault(x => x.Value.controller == sender && (senderFound = true));
            if (!senderFound)
                return;
            JeromeConnectionParams cp = senderEntry.Key;
            this.Invoke((MethodInvoker)delegate
          {
              connections[cp].watch = false;
              menuWatch[cp].Visible = false;
              menuWatch[cp].Checked = false;
              menuControl[cp].Checked = true;
              this.Text = cp.name;
              updateButtonsMode();
              string linesState = connections[cp].controller.readlines();
              for (var c = 0; c < lines.Count(); c++)
              {
                  connections[cp].controller.setLineMode(lines[c], 0);
                  buttons[c].Checked = linesState[lines[c] - 1] == '1';
              }
              connections[cp].controller.setLineMode(enLine, 0);
              connections[cp].controller.switchLine(enLine, 1);
              connections[cp].controller.setLineMode(clkLine, 0);
              connections[cp].controller.switchLine(clkLine, 0);
          });
        }

        protected void updateButtonsMode()
        {
            for (int co = 0; co < buttons.Count(); co++)
            {
                bool busy = connections.Values.ToList().Exists(x => x.watch && x.linesStates[co]);
                buttons[co].Enabled = !trx && connected && !busy;
                if (esBindings.ContainsValue(co) )
                {
                    int band = esBindings.First(x => x.Value == co).Key;
                    if (busy)
                    {
                        if (!appContext.busyBands.Contains(band))
                            appContext.busyBands.Add(band);
                    } else 
                        if (appContext.busyBands.Contains(band))
                            appContext.busyBands.Remove(band);
                }
            }
        }

        protected void controllerDisconnected(object obj, DisconnectEventArgs e)
        {
            JeromeConnectionParams c = ((JeromeController)obj).connectionParams;
            this.Invoke((MethodInvoker)delegate
            {
                if (e.requested)
                {
                    menuWatch[c].Visible = true;
                    menuControl[c].Checked = false;
                    this.Text = "Ant Comm";
                }
                else
                {
                    this.Text = "Disconnected!";
                    appContext.showNotification("NetComm", c.name + ": соединение потеряно!", ToolTipIcon.Error);
                }
                updateButtonsMode();
            });
        }

        protected void FMain_Load(object sender, EventArgs e)
        {
            Width = 100;
            for (int co = 0; co < lines.Count(); co++)
            {
                CheckBox b = new CheckBox();
                b.Height = 25;
                b.Width = Width - 7;
                b.Top = 25 + (b.Height + 2) * co;
                b.Left = 1;
                b.TextAlign = ContentAlignment.MiddleCenter;
                buttons.Add(b);
                int no = co;
                updateButtonLabel(no);
                b.BackColor = SystemColors.Control;
                b.Appearance = Appearance.Button;
                b.Enabled = false;
                b.Anchor = AnchorStyles.Right | AnchorStyles.Left;
                b.CheckedChanged += new EventHandler(delegate(object obj, EventArgs ea)
                {
                    if (b.Checked)
                    {
                        buttons.Where(x => x != b).ToList().ForEach(x => x.Checked = false);
                        b.ForeColor = Color.Red;
                    }
                    else
                    {
                        b.ForeColor = buttonsColor;
                    }
                    Parallel.ForEach(connections.Where(x => x.Value.active && x.Value.connected), x =>
                    { switchLine(x.Value.controller, lines[x.Value.lines[no] - 1], b.Checked ? 1 : 0); });
                });
                b.MouseDown += form_MouseClick;
                Controls.Add(b);
            }
            buttonsColor = buttons[0].ForeColor;
            watchTimer = new System.Threading.Timer(obj => onWatchTimer(), null, 1000, 1000);
/*            if (connections.Count > 0 )
            {
                if (connectionFromArgs != null)
                    connect(connectionFromArgs);
                else if (config.lastConnection > -1 && connections.ContainsKey( config.connections[config.lastConnection] ) )
                    connect(config.connections[config.lastConnection]);

            }*/
        }

        public void switchLine(JeromeController controller, int line, int state)
        {
            controller.switchLine(enLine, 0);
            controller.switchLine(line, state);
            //Thread.Sleep(1);
            controller.switchLine(clkLine, 1);
            //Thread.Sleep(1);
            controller.switchLine(clkLine, 0);
            controller.switchLine(enLine, 1);
        }



        public override void writeConfig()
        {
            if (!loaded)
                return;
            config.lastConnection = -1;

            config.connections = new JeromeConnectionParams[connections.Count];
            config.states = new JeromeConnectionState[connections.Count];
            int co = 0;
            foreach (KeyValuePair<JeromeConnectionParams, JeromeConnectionState> x in connections)
            {
                if (x.Value.active)
                    config.lastConnection = co;
                config.connections[co] = x.Key;
                config.states[co] = x.Value;
                co++;
            }

            config.buttonLabels = buttonLabels.ToArray();

            config.esMhzValues = new int[ esBindings.Count ];
            config.esButtons = new int[esBindings.Count];
            co = 0;
            foreach ( KeyValuePair<int,int> x in esBindings ) {
                config.esMhzValues[co] = x.Key;
                config.esButtons[co] = x.Value;
                co++;
            }

            appContext.writeConfig();

        }

        protected void initConfig()
        {
            if (config.connections != null)
            {
                if (config.states == null || config.states.Count() == 0)
                    config.states = new JeromeConnectionState[config.connections.Count()];
                for (int co = 0; co < config.connections.Count(); co++)
                {
                    if (config.states[co] == null)
                        config.states[co] = new JeromeConnectionState();
                    //config.states[co].active = false;
                    connections[config.connections[co]] = config.states[co];
                }
            }
            else
                connections = new Dictionary<JeromeConnectionParams, JeromeConnectionState>();
            if (config.buttonLabels != null) 
                buttonLabels = config.buttonLabels.ToList();
            if ( config.esMhzValues != null )
                for (int co = 0; co < config.esButtons.Count(); co++)
                    esBindings[config.esMhzValues[co]] = config.esButtons[co];
        }

        protected void miModuleSettings_Click(object sender, EventArgs e)
        {
            (new fModuleSettings()).ShowDialog();
        }

        protected void miConnectionsList_Click(object sender, EventArgs e)
        {
            FConnectionsList flist = new FConnectionsList(connections.Keys.ToList());
            flist.connectionCreated += connectionCreated;
            flist.connectionEdited += connectionEdited;
            flist.connectionDeleted += connectionDeleted;
            flist.ShowDialog(this);
        }

        protected void connectionCreated(object obj, EventArgs e)
        {
            JeromeConnectionParams c = (JeromeConnectionParams)obj;
            connections[c] = new JeromeConnectionState();
            createConnectionMI(c);
            miRelaySettings.Enabled = true;
            writeConfig();
        }

        protected void connectionEdited(object obj, EventArgs e)
        {
            JeromeConnectionParams c = (JeromeConnectionParams)obj;
            menuControl[c].Text = connections[c].active ? "Отключиться от " + c.name : c.name;
            menuWatch[c].Text = c.name;
            writeConfig();
        }

        protected void connectionDeleted(object obj, EventArgs e)
        {
            JeromeConnectionParams c = (JeromeConnectionParams)obj;
            if (connections[c].active)
                MessageBox.Show("Нельзя удалить активное соединение! Изменения не будут сохранены");
            else
            {
                connections.Remove(c);
                miControl.DropDownItems.Remove(menuControl[c]);
                menuControl.Remove(c);
                miWatch.DropDownItems.Remove(menuWatch[c]);
                menuWatch.Remove(c);
                miRelaySettings.Enabled = connections.Count > 0;
                writeConfig();
            }
        }


        protected void form_MouseClick(object sender, MouseEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Mouse click");
            if (e.Button == MouseButtons.Right)
            {
                CheckBox b = null;
                if (sender.GetType() == typeof(CheckBox))
                    b = (CheckBox)sender;
                else
                {
                    Control i = GetChildAtPoint(new Point(e.X, e.Y));
                    if (i != null && i.GetType() == typeof(CheckBox))
                        b = (CheckBox)i;
                }
                if ( b != null && buttons.Contains(b))
                {
                    int no = buttons.IndexOf(b);
                    string bindStr = string.Join("; ", esBindings.Where(x => x.Value == no).Select(x => x.Key).ToArray());
                    FButtonProps ib = new FButtonProps(buttonLabels[no], bindStr);
                    ib.StartPosition = FormStartPosition.CenterParent;
                    ib.ShowDialog(this);
                    if (ib.DialogResult == DialogResult.OK)
                    {
                        buttonLabels[no] = ib.name;
                        b.Text = ib.name;
                        foreach (KeyValuePair<int, int> x in esBindings.Where(x => x.Value == no).ToList())
                            esBindings.Remove(x.Key);
                        foreach (int mhz in ib.esMHzValues)
                            esBindings[mhz] = no;
                        writeConfig();
                        updateButtonLabel(no);
                    }
                }
            }
        }

        protected void miRelaySettings_Click(object sender, EventArgs e)
        {
            FRelaySettings frs = new FRelaySettings(connections, buttonLabels);
            frs.ShowDialog();
            if (frs.DialogResult == DialogResult.OK)
            {
                for (int co = 0; co < lines.Count(); co++)
                    connections[frs.connection].lines[co] = frs.cbLines[co].SelectedIndex + 1;
                writeConfig();
            }
        }


        public override void esMessage(int mhz, bool _trx)
        {
            if (trx != _trx)
            {
                trx = _trx;
                this.Invoke((MethodInvoker)delegate {
                    updateButtonsMode();
                });
            }
            if ( esBindings.ContainsKey( mhz ) ) 
                this.Invoke( (MethodInvoker) delegate {
                    buttons[esBindings[mhz]].Checked = true;
                });
        }


        protected void FMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 && e.KeyCode - Keys.NumPad0 - 1 < buttons.Count)
            {
                CheckBox b = buttons[e.KeyCode - Keys.NumPad0 - 1];
                if (b.Enabled)
                    b.Checked = !b.Checked;
               /* else
                    MessageBox.Show((e.KeyCode - Keys.NumPad0 - 1).ToString());*/
            }
        }

        protected void FMain_ResizeEnd(object sender, EventArgs e)
        {
            int bHeight = ( this.ClientSize.Height - 50 ) / lines.Count() - 2;
            for (int co = 0; co < buttons.Count(); co++)
            {
                CheckBox b = buttons[co];
                b.Height = bHeight;
                b.Top = 25 + (bHeight + 2) * co;
            }
        }

        private async Task disconnectTask( JeromeConnectionState st)
        {
            st.controller.disconnect();
        }

        protected async void FNetComm_FormClosed(object sender, FormClosedEventArgs e)
        {
            closing = true;
            watchTimer.Change(Timeout.Infinite, Timeout.Infinite);
            await TaskEx.WhenAll(connections.Where(x => x.Value.controller != null)
                .Select(x => disconnectTask( x.Value )));
        }
    }


}
