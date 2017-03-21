using System;
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
        JCAppContext appContext;
        WX0BConfig config;
        Color defForeColor;

        JeromeController terminalJController;

        public FWX0B(JCAppContext _appContext)
        {
            appContext = _appContext;
            config = _appContext.config.WX0BConfig;
            InitializeComponent();
            defForeColor = cbConnectTerminal.ForeColor;
            updateTerminalConnectionParamsCaption();
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
                terminalJController = JeromeController.create(config.terminalConnectionParams);
                if (terminalJController.connect())
                {
                    terminalJController.onConnected += TerminalJControllerConnected;
                    terminalJController.onDisconnected += TerminalJControllerDisconnected;
                    cbConnectTerminal.Checked = true;
                }
                else
                    appContext.showNotification("WX0B", "Cоединение с терминалом " + terminalJController.connectionParams.host + " не удалось!", ToolTipIcon.Error);
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
                    appContext.showNotification("WX0B", "Cоединение с терминалом " + terminalJController.connectionParams.host + "потеряно!", ToolTipIcon.Error);
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

        private void writeConfig()
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
                if (terminalJController != null)
                    terminalJController.disconnect();
                cbConnectTerminal.ForeColor = defForeColor;
            }
        }

        private void bAddController_Click(object sender, EventArgs e)
        {
            WX0BControllerPanel cp = new WX0BControllerPanel(this, new WX0BController());
            cp.Dock = DockStyle.Top;
            gbControllers.Controls.Add(cp);
        }
    }

    public class WX0BController
    {

    }

    public class WX0BTerminal
    {

    }

    public class WX0BCluster
    {
        string title;

    }

    public class WX0BControllerConfigEntry
    {
        public JeromeConnectionParams connectionParams;
        public int esMHz;
    }

    public class WX0BConfig
    {
        public JeromeConnectionParams terminalConnectionParams;
        public List<WX0BControllerConfigEntry> controllers;
        public int activeController;
        public bool terminalActive;
    }





}
