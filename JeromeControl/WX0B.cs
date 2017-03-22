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

namespace WX0B
{
    public partial class FWX0B : Form, IJCChildForm
    {
        internal JCAppContext appContext;
        internal WX0BConfig config;
        internal Color defForeColor;

        JeromeController terminalJConnection;
        internal List<WX0BController> controllers = new List<WX0BController>();
        internal List<WX0BControllerPanel> controllerPanels = new List<WX0BControllerPanel>();

        public FWX0B(JCAppContext _appContext)
        {
            appContext = _appContext;
            config = _appContext.config.WX0BConfig;
            InitializeComponent();
            defForeColor = cbConnectTerminal.ForeColor;
            updateTerminalConnectionParamsCaption();
            foreach (WX0BControllerConfigEntry cConfig in config.controllers)
                createController(cConfig);
            if (config.terminalConnectionParams != null)
            {
                cbConnectTerminal.Enabled = true;
                if (config.terminalActive)
                    connectTerminal();
            }
            else
            {
                cbConnectTerminal.Enabled = false;
            }

        }

        public void connectTerminal()
        {
            UseWaitCursor = true;
            try
            {
                terminalJConnection = JeromeController.create(config.terminalConnectionParams);
                if (terminalJConnection.connect())
                {
                    terminalJConnection.onConnected += TerminalJControllerConnected;
                    terminalJConnection.onDisconnected += TerminalJControllerDisconnected;
                    cbConnectTerminal.Checked = true;
                }
                else
                    appContext.showNotification("WX0B", "Cоединение с терминалом " + terminalJConnection.connectionParams.host + " не удалось!", ToolTipIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void TerminalJControllerConnected(object sender, EventArgs e)
        {
            cbConnectTerminal.ForeColor = Color.Green;
        }

        private void TerminalJControllerDisconnected(object sender, DisconnectEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (e.requested)
                {
                    cbConnectTerminal.Checked = false;
                    cbConnectTerminal.ForeColor = defForeColor;
                    config.terminalActive = false;
                    writeConfig();
                }
                else
                {
                    appContext.showNotification("WX0B", "Cоединение с терминалом " + terminalJConnection.connectionParams.host + "потеряно!", ToolTipIcon.Error);
                    cbConnectTerminal.ForeColor = Color.Red;
                }
            });

        }

        private void updateTerminalConnectionParamsCaption()
        {
            if (config.terminalConnectionParams == null)
                bTerminalConnectionParams.Text = "Настроить соединиение";
            else
                bTerminalConnectionParams.Text = config.terminalConnectionParams.host;
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

        internal void writeConfig()
        {
            appContext.config.WX0BConfig = config;
            appContext.writeConfig();
        }

        public void esMessage(int mhz, bool trx)
        {
            throw new NotImplementedException();
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
        }

        public void deleteController( WX0BControllerPanel cp )
        {
            if (MessageBox.Show("Вы действительно хотите удалить контроллер?", "WX0B", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                gbControllers.Controls.Remove(cp);
                controllers.Remove(cp.controller);
                controllerPanels.Remove(cp);
                foreach (WX0BControllerPanel _cp in controllerPanels)
                    _cp.updateIndex();
                config.controllers.Remove(cp.controller.config);
                cp.Dispose();
                writeConfig();
            }
        }
    }

    public class WX0BController
    {
        public WX0BControllerConfigEntry config;
        internal JeromeController jConnection;

        public WX0BController( WX0BControllerConfigEntry _config)
        {
            config = _config;
        }




    }

    public class WX0BControllerConfigEntry
    {
        public JeromeConnectionParams connectionParams;
        public int esMHz;
    }

    public class WX0BConfig
    {
        public JeromeConnectionParams terminalConnectionParams;
        public List<WX0BControllerConfigEntry> controllers = new List<WX0BControllerConfigEntry>();
        public int activeController = -1;
        public bool terminalActive;
    }





}
