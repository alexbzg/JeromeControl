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
    public partial class FWX0B : Form
    {
        JCAppContext appContext;
        WX0BConfig config;

        JeromeController terminalJController;

        public FWX0B(JCAppContext _appContext)
        {
            appContext = _appContext;
            config = _appContext.config.WX0BConfig;
            InitializeComponent();
            if (config.terminalConnectionParams != null)
            {
                bTerminalConnectionParams.Text = config.terminalConnectionParams.host;
                cbConnectTerminal.Enabled = true;
                if (config.terminalActive)
                    connectTerminal();
            }
            else
            {
                bTerminalConnectionParams.Text = "Настроить соединение";
                cbConnectTerminal.Enabled = false;
            }
        }

        public void connectTerminal()
        {
            cbConnectTerminal.Checked = true;
            terminalJController = JeromeController.create(config.terminalConnectionParams);
            terminalJController.onConnected += TerminalJControllerConnected;
            terminalJController.onDisconnected += TerminalJControllerDisconnected;
        }

        private void TerminalJControllerConnected(object sender, EventArgs e)
        {
            cbConnectTerminal.ForeColor = Color.Green;
        }

        private void TerminalJController_onDisconnected(object sender, DisconnectEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TerminalJControllerDisconnected(object sender, DisconnectEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (e.requested)
                {
                    cbConnectTerminal.Checked = false;
                }
                else
                {
                    appContext.showNotification("NetComm", c.name + ": соединение потеряно!", ToolTipIcon.Error);
                }
                updateButtonsMode();
            });

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
