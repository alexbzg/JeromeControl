using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jerome;
using AsyncConnectionNS;

namespace WX0B
{
    public partial class WX0BControllerPanel : UserControl
    {
        public WX0BController controller;
        public FWX0B fWX0B;
        internal int index;

        public WX0BControllerPanel( FWX0B _fWX0B, WX0BController _controller)
        {
            controller = _controller;
            fWX0B = _fWX0B;
            fWX0B.controllers.Add(controller);
            fWX0B.controllerPanels.Add(this);
            InitializeComponent();
            updateIndex();
            updateConnectionParamsCaption();
            controller.jConnection.onConnected += сontrollerConnected;
            controller.jConnection.onDisconnected += controllerDisconnected;
            if (controller.config.esMHz != 0)
                tbESMHz.Text = controller.config.esMHz.ToString();
        }

        public void updateIndex()
        {
            index = fWX0B.controllers.IndexOf(controller);
            gbController.Text = ( index + 1).ToString();
        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            fWX0B.deleteController(this);
        }

        public void updateConnectionParamsCaption()
        {
            if ( controller.config.connectionParams == null || controller.config.connectionParams.host == null)
                bConnectionParams.Text = "Настроить соединиение";
            else
                bConnectionParams.Text = controller.config.connectionParams.host;
        }

        private void bConnectionParams_Click(object sender, EventArgs e)
        {
            if (controller.config.connectionParams == null)
            {
                JeromeConnectionParams nc = new JeromeConnectionParams();
                if (nc.edit())
                {
                    controller.config.connectionParams = nc;
                    cbConnect.Enabled = true;
                    fWX0B.writeConfig();
                }
            }
            else
            {
                if (controller.config.connectionParams.edit())
                    fWX0B.writeConfig();
            }
            updateConnectionParamsCaption();
        }

        public void connect()
        {
            fWX0B.UseWaitCursor = true;
            if ( !controller.jConnection.connect() )
                fWX0B.appContext.showNotification("WX0B", "Cоединение с контроллером " + controller.jConnection.connectionParams.host + " не удалось!", ToolTipIcon.Error);
            UseWaitCursor = false;
        }

        private void сontrollerConnected(object sender, EventArgs e)
        {
            cbConnect.ForeColor = Color.Green;
        }

        private void controllerDisconnected(object sender, DisconnectEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (e.requested)
                {
                    cbConnect.Checked = false;
                    cbConnect.ForeColor = fWX0B.defForeColor;
                    fWX0B.writeConfig();
                }
                else
                {
                    fWX0B.appContext.showNotification("WX0B", "Cоединение с контроллером " + controller.jConnection.connectionParams.host + "потеряно!", ToolTipIcon.Error);
                    cbConnect.ForeColor = Color.Red;
                }
            });

        }



        private void cbConnect_CheckedChanged(object sender, EventArgs e)
        {
            if (cbConnect.Checked)
            {
                connect();
                fWX0B.config.activeController = index;
                fWX0B.writeConfig();
            }
            else
            {
                if (controller.jConnection != null)
                    controller.jConnection.disconnect();
                cbConnect.ForeColor = fWX0B.defForeColor;
                if (fWX0B.config.activeController == index)
                    fWX0B.config.activeController = -1;
            }

        }

        private void tbESMHz_Validated(object sender, EventArgs e)
        {
            controller.config.esMHz = Convert.ToInt32(tbESMHz.Text);
            fWX0B.writeConfig();
        }
    }
}
