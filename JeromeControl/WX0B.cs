﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AsyncConnectionNS;
using System.IO;
using JeromeControl;
using Jerome;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StorableFormState;


namespace WX0B
{
    public partial class FWX0B : JCChildForm
    {
        internal static WX0BTerminalTemplate TerminalTemplate = new WX0BTerminalTemplate() {
            switches = new WX0BTerminalSwitchTemplate[] {
                new WX0BTerminalSwitchTemplate() {
                  combo = new int[] { 1 },
                  button = 5,
                  led = 1
                },
                new WX0BTerminalSwitchTemplate() {
                  combo = new int[] { 2 },
                  button = 6,
                  led = 3
                },
                new WX0BTerminalSwitchTemplate() {
                  combo = new int[] { 3 },
                  button = 7,
                  led = 22
                },
                new WX0BTerminalSwitchTemplate() {
                  combo = new int[] { 1,2 },
                  button = 8,
                  led = 21
                },
                new WX0BTerminalSwitchTemplate() {
                  combo = new int[] { 1,3 },
                  button = 9,
                  led = 20
                },
                new WX0BTerminalSwitchTemplate() {
                  combo = new int[] { 2,3 },
                  button = 10,
                  led = 18
                },
                new WX0BTerminalSwitchTemplate() {
                  combo = new int[] { 1,2,3 },
                  button = 11,
                  led = 16,
                  isDefault = true
                }
            },
            lockButton = 12,
            lockLED = 15,
            pttButton = 14,
            pttLED = 13
        };
        internal static int[] ControllerTemplate = new int[] { 1, 5, 10, 12 };

        internal Dictionary<WX0BTerminalSwitchTemplate, WX0BTerminalSwitch> switches = new Dictionary<WX0BTerminalSwitchTemplate, WX0BTerminalSwitch>();
        internal WX0BTerminalSwitch defaultSwitch;
        internal volatile WX0BTerminalSwitch activeSwitch = null;
        internal volatile WX0BTerminalSwitch lockSwitch = null;
        internal volatile int pttState = 1;
        internal volatile int lockButtonState = 1;
        internal volatile bool tx = false;
        internal volatile bool pttTX = false;
        internal volatile bool esTX = false;
        volatile bool closingFl = false;

        internal int _idx;
        internal WX0BConfig config { get { return (WX0BConfig)componentConfig; } }
        internal Color defForeColor; 

        internal JeromeController terminalJConnection;

        internal List<WX0BController> controllers = new List<WX0BController>();
        internal List<WX0BControllerPanel> controllerPanels = new List<WX0BControllerPanel>();

        internal FWX0BStatus fStatus; 

        public FWX0B(JCAppContext _appContext, int __idx) : base( _appContext, __idx)
        {
            appContext = _appContext;
            _idx = __idx;
            InitializeComponent();
            fStatus = new FWX0BStatus(this);
            foreach ( WX0BTerminalSwitchTemplate st in TerminalTemplate.switches)
            {
                switches[st] = new WX0BTerminalSwitch(st);
                if (st.isDefault)
                {
                    defaultSwitch = switches[st];
                    activeSwitch = switches[st];
                }
            }
            defForeColor = cbConnectTerminal.ForeColor;
            if (config.terminalConnectionParams == null )
                config.terminalConnectionParams = new JeromeConnectionParams();
            updateTerminalConnectionParamsCaption();
            terminalJConnection = JeromeController.create(config.terminalConnectionParams);
            terminalJConnection.onConnected += TerminalJControllerConnected;
            terminalJConnection.onDisconnected += TerminalJControllerDisconnected;
            terminalJConnection.lineStateChanged += TerminalJConnectionLineStateChanged;
            foreach (WX0BControllerConfigEntry cConfig in config.controllers)
                createController(cConfig);
            if (config.terminalConnectionParams != null)
            {
                cbConnectTerminal.Enabled = true;
                if (config.terminalActive)
                    //new System.Threading.Timer(new TimerCallback(x => connectTerminal()), null, 5000, Timeout.Infinite);*/
                    connectTerminal();
            }
            else
            {
                cbConnectTerminal.Enabled = false;
            }
            if (config.activeController != -1 && config.activeController < controllers.Count)
                controllers[config.activeController].jConnection.asyncConnect();
            else
                setActiveController(-1);
            
            if (config.statusOnly)
                WindowState = FormWindowState.Minimized;
        }

        private void delayedConnectTerminal()
        {
            new System.Threading.Timer(new TimerCallback(x => connectTerminal()), null, 3000, Timeout.Infinite);
        }

        public void connectTerminal()
        {
            if (!terminalJConnection.connected)
            {
                config.terminalActive = true;
                writeConfig();
                terminalJConnection.asyncConnect();
            }
        }

        private void updateTX()
        {
            bool val = pttState == 0 || esTX;
            if (val != tx)
            {
                tx = val;
                displayPTT();
                if (tx)
                {
                    if (lockSwitch != null)
                    {
                        switchLeds(activeSwitch, lockSwitch);
                        updateController(lockSwitch);
                    }
                }
                else
                {
                    switchLeds(lockSwitch, activeSwitch);
                    updateController(activeSwitch);
                }
            }

        }

        private void TerminalJConnectionLineStateChanged(object sender, LineStateChangedEventArgs e)
        {
            if (e.line == TerminalTemplate.pttButton)
            {
                if (e.state != pttState)
                {
                    pttState = e.state;
                    updateTX();
                }
            }
            else if (!tx && e.line == TerminalTemplate.lockButton)
            {
                if (e.state != lockButtonState)
                {
                    lockButtonState = e.state;
                    if (e.state == 0)
                    {
                        lockSwitch = lockSwitch == null ? activeSwitch : null;
                        displayLockSwitch();
                    }

                }
            }
            else if (!tx)
            {
                KeyValuePair<WX0BTerminalSwitchTemplate, WX0BTerminalSwitch> kv = switches.Where(x => x.Key.button == e.line).FirstOrDefault();
                if ( kv.Key != null && kv.Value.lineState != e.state)
                {
                    kv.Value.lineState = e.state;
                    if ( e.state == 0)
                    {
                        switchLeds(activeSwitch, kv.Value);
                        activeSwitch = kv.Value;
                        displayActiveSwitch();
                        updateController(activeSwitch);
                    }
                }
            }
        }

        private void updateController( WX0BTerminalSwitch sw)
        {
            if ( sw != null && config.activeController != -1 && config.activeController < controllers.Count)
                controllers[config.activeController].setLines(sw.controllerLinesState);
        }

        private void TerminalJControllerConnected(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
          {
              cbConnectTerminal.Checked = true;
              cbConnectTerminal.Image = JeromeControl.Properties.Resources.icon_connected;
              fStatus.pTerminalStatus.BackgroundImage = JeromeControl.Properties.Resources.icon_connected;
          });
            foreach ( WX0BTerminalSwitchTemplate st in TerminalTemplate.switches)
            {
                terminalJConnection.setLineMode(st.button, 1);
                terminalJConnection.setLineMode(st.led, 0);
            }
            terminalJConnection.setLineMode(TerminalTemplate.pttLED, 0);
            terminalJConnection.setLineMode(TerminalTemplate.lockLED, 0);
            terminalJConnection.setLineMode(TerminalTemplate.pttButton, 1);
            terminalJConnection.setLineMode(TerminalTemplate.lockButton, 1);
            displayActiveSwitch();
            displayLockSwitch();
            displayPTT();
            WX0BTerminalSwitch s = tx ? lockSwitch : activeSwitch;
            if (terminalJConnection.connected)
                foreach (WX0BTerminalSwitchTemplate st in TerminalTemplate.switches)
                    terminalJConnection.switchLine(st.led, switches[st] == s ? 1 : 0);
        }

        private void displayActiveSwitch()
        {
            if (terminalJConnection.connected)
                terminalJConnection.usartSendBytes(new byte[] { (byte)activeSwitch.usartSignal });
        }

        private void displayLockSwitch()
        {
            if (terminalJConnection.connected)
            {
                terminalJConnection.switchLine(TerminalTemplate.lockLED, lockSwitch == null ? 0 : 1);
                terminalJConnection.usartSendBytes(new byte[] { (byte)( (lockSwitch == null ? 0 : lockSwitch.usartSignal)  | 8) });
            }
        }

        private void displayPTT()
        {
            if (terminalJConnection.connected) {
                terminalJConnection.usartSendBytes(new byte[] { (byte)(tx ? 32 : 16) });
                terminalJConnection.switchLine(TerminalTemplate.pttLED, tx ? 1 : 0);
            }
        }

        private void switchLeds( WX0BTerminalSwitch oldSwitch, WX0BTerminalSwitch newSwitch )
        {
            if ( terminalJConnection.connected)
            {
                if (oldSwitch != null)
                    terminalJConnection.switchLine(oldSwitch.template.led, 0);
                if ( newSwitch != null)
                    terminalJConnection.switchLine(newSwitch.template.led, 1);
            }
        }


        private void TerminalJControllerDisconnected(object sender, DisconnectEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (e.requested)
                {
                    cbConnectTerminal.Checked = false;
                    cbConnectTerminal.Image = JeromeControl.Properties.Resources.icon_connect;
                    fStatus.pTerminalStatus.BackgroundImage = JeromeControl.Properties.Resources.icon_connect;
                    if (!closingFl)
                    {
                        config.terminalActive = false;
                        writeConfig();
                    }
                }
                else
                {
                    appContext.showNotification("WX0B", "Cоединение с терминалом " + terminalJConnection.connectionParams.host + "потеряно!", ToolTipIcon.Error);
                    cbConnectTerminal.Image = JeromeControl.Properties.Resources.icon_disconnected;
                    fStatus.pTerminalStatus.BackgroundImage = JeromeControl.Properties.Resources.icon_disconnected;
                }
            });

        }

        private void updateTerminalConnectionParamsCaption()
        {
            if (config.terminalConnectionParams.host == null || config.terminalConnectionParams.host == "")
            {
                bTerminalConnectionParams.Text = "Настроить";
                fStatus.lTerminal.Text = "Терминал";
            }
            else
            {
                bTerminalConnectionParams.Text = config.terminalConnectionParams.name + " " + config.terminalConnectionParams.host;
                fStatus.lTerminal.Text = bTerminalConnectionParams.Text;
            }

        }

        private void bTerminalConnectionParams_Click(object sender, EventArgs e)
        {
            if (config.terminalConnectionParams == null)
            {
                JeromeConnectionParams nc = new JeromeConnectionParams();
                if (nc.edit())
                {
                    config.terminalConnectionParams = nc;
                    cbConnectTerminal.Enabled = true;
                    writeConfig();
                }
            }
            else
            {
                if (config.terminalConnectionParams.edit())
                    writeConfig();
            }
            updateTerminalConnectionParamsCaption();
        }

        public override void writeConfig()
        {
            appContext.writeConfig();
        }

        public override void esMessage(int mhz, bool trx)
        {
            System.Diagnostics.Debug.WriteLine("MHz " + mhz.ToString());
            if ( trx != esTX)
            {
                esTX = trx;
                updateTX();
            }
            WX0BController c = controllers.FirstOrDefault(x => x.config.esMHz == mhz);
            setActiveController(c == null ? -1 : controllers.IndexOf(c));
        }

        private void cbConnectTerminal_CheckedChanged(object sender, EventArgs e)
        {
            if (cbConnectTerminal.Checked)
                connectTerminal();
            else
            {
                if (terminalJConnection != null)
                    terminalJConnection.disconnect();
                cbConnectTerminal.ForeColor = defForeColor;
            }
        }

        private void bAddController_Click(object sender, EventArgs e)
        {
            WX0BControllerConfigEntry cConfig = new WX0BControllerConfigEntry();
            config.controllers.Add(cConfig);
            writeConfig();
            createController(cConfig);

        }

        private void createController( WX0BControllerConfigEntry cConfig)
        {
            WX0BControllerPanel cp = new WX0BControllerPanel(this, new WX0BController(cConfig));
            cp.Dock = DockStyle.Bottom;
            gbControllers.Controls.Add(cp);
            cp.controller.jConnection.onConnected += controllerConnected;
            updateFormHeight();
        }

        private void controllerConnected( object sender, EventArgs e)
        {
            updateController(tx ? lockSwitch : activeSwitch);
        }

        public void deleteController( WX0BControllerPanel cp )
        {
            if (MessageBox.Show("Вы действительно хотите удалить контроллер?", "WX0B", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (cp.index == config.activeController)
                    setActiveController(-1);

                gbControllers.Controls.Remove(cp);
                controllers.Remove(cp.controller);
                controllerPanels.Remove(cp);
                foreach (WX0BControllerPanel _cp in controllerPanels)
                    _cp.updateIndex();
                config.controllers.Remove(cp.controller.config);
                cp.Dispose();
                updateFormHeight();
                writeConfig();
            }
        }

        internal void setActiveController( int idx)
        {
            if ( idx != config.activeController && idx < controllers.Count)
            {
                if (config.activeController != -1 && config.activeController < controllers.Count)
                    controllers[config.activeController].jConnection.disconnect();
                if (idx != -1)
                {
                    controllers[idx].jConnection.asyncConnect();
                    controllerPanels[idx].updateStatusPanelCaption();
                }
                if (!closingFl)
                {
                    config.activeController = idx;
                    writeConfig();
                }
            }
        }

        public override void restoreFormState()
        {
            if (storableConfig?.formLocation != null && !storableConfig.formLocation.IsEmpty)
                this.Location = storableConfig.formLocation;
        }

        private void FWX0B_FormClosing(object sender, FormClosingEventArgs e)
        {
            closingFl = true;
            terminalJConnection?.disconnect();
            setActiveController(-1);
        }

        private void updateFormHeight()
        {            
            Height = gbControllers.Bottom + 50;
        }

        private void FWX0B_Resize(object sender, EventArgs e)
        {
            if ( WindowState == FormWindowState.Minimized )
            {
                fStatus.Show();
                fStatus.Activate();
                WindowState = FormWindowState.Normal;
                Hide();
                config.statusOnly = true;
                writeConfig();
            }
        }
    }

    internal class WX0BTerminalSwitchTemplate
    {
        internal int[] combo;
        internal int button;
        internal int led;
        internal bool isDefault;
    }

    internal class WX0BTerminalTemplate
    {
        internal WX0BTerminalSwitchTemplate[] switches;
        internal int lockButton;
        internal int lockLED;
        internal int pttButton;
        internal int pttLED;
    }

    internal class WX0BTerminalSwitch
    {
        internal int[] controllerLinesState;
        internal int usartSignal;
        internal int lineState = 1;
        internal WX0BTerminalSwitchTemplate template;

        internal WX0BTerminalSwitch(WX0BTerminalSwitchTemplate _template)
        {
            template = _template;
            controllerLinesState = new int[] { 0, 0, 0, 0 };
            usartSignal = 0;
            foreach (int i in template.combo)
                usartSignal |= 1 << (i - 1);
            if (template.combo.Length == 1)
            {
                controllerLinesState[template.combo[0] - 1] = 1;
                controllerLinesState[3] = 1;
            }
            else if (template.combo.Length == 2)
                for (int i = 0; i < 3; i++)
                    if (!Array.Exists(template.combo, x => x == i + 1))
                        controllerLinesState[i] = 1;
        }

    }


    public class WX0BController
    {
        public WX0BControllerConfigEntry config;
        internal JeromeController jConnection;


        public WX0BController( WX0BControllerConfigEntry _config)
        {
            config = _config;
            jConnection = JeromeController.create(config.connectionParams);
        }

        public void setLines( int[] linesStates )
        {
            if (jConnection.connected)
                for (int c = 0; c < linesStates.Count(); c++)
                    jConnection.switchLine(FWX0B.ControllerTemplate[c], linesStates[c]);
        }


    }

    public class WX0BControllerConfigEntry
    {
        public JeromeConnectionParams connectionParams = new JeromeConnectionParams();
        public int esMHz;
    }

    public class WX0BConfig : JCComponentConfig
    {
        public JeromeConnectionParams terminalConnectionParams;
        public List<WX0BControllerConfigEntry> controllers = new List<WX0BControllerConfigEntry>();
        public int activeController = -1;
        public bool terminalActive;
        public StorableFormConfig statusConfig = new StorableFormConfig();
        public bool statusOnly = false;
    }





}
