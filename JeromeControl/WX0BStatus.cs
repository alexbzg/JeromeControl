using StorableFormState;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jerome;

namespace WX0B
{
    public partial class FWX0BStatus : FormWStorableState
    {
        internal FWX0B fWX0B;
        public override StorableFormConfig storableConfig => fWX0B.config.statusConfig;
        public FWX0BStatus( FWX0B _fWX0B )
        {
            fWX0B = _fWX0B;
            InitializeComponent();
        }

        public override void writeConfig()
        {
            fWX0B.writeConfig();
        }

        private void FWX0BStatus_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                fWX0B.Show();
                fWX0B.Activate();
                WindowState = FormWindowState.Normal;
                Hide();
                fWX0B.config.statusOnly = false;
                writeConfig();
            }
        }

        private void FWX0BStatus_FormClosed(object sender, FormClosedEventArgs e)
        {
            fWX0B.Close();
        }

        internal void updateForm()
        {
            JeromeConnectionParams tParams = fWX0B.config.terminalConnectionParams;
            lController.Text = ( tParams == null || tParams.host == "" ) ? "Терминал" : tParams.name + " " + tParams.host;

        }

    }
}
