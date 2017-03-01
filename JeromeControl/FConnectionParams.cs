using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AntennaeRotator.Properties;
using JeromeControl.Properties;

namespace AntennaeRotator
{
    public partial class fConnectionParams : Form
    {
        private Bitmap bmIcon;
        public int icon = 1;

        public fConnectionParams(AntennaeRotatorConnectionSettings cSettings)
        {
            InitializeComponent();

            tbName.Text = cSettings.name;
            tbHost.Text = cSettings.jeromeParams.host;
            tbPort.Text = cSettings.jeromeParams.port.ToString();
            tbUSARTPort.Text = cSettings.jeromeParams.usartPort.ToString();
            tbPassword.Text = cSettings.jeromeParams.password;
            nudIntervalOn.Value = cSettings.switchIntervals[0];
            nudIntervalOff.Value = cSettings.switchIntervals[1];
            cbDeviceType.SelectedIndex = cSettings.deviceType;
            chbHwLimits.Checked = cSettings.hwLimits;
            icon = cSettings.icon;


            displayIcon();
        }

        private void displayIcon()
        {
            bmIcon = Bitmap.FromHicon(((Icon)Resources.ResourceManager.GetObject(CommonInf.icons[icon])).Handle);
            pIcon.BackgroundImage = bmIcon;
        }


        private void nudInterval_Validating(object sender, CancelEventArgs e)
        {
            if (nudIntervalOff.Value < 0)
                e.Cancel = true;
        }

        private void cbDeviceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!chbHwLimits.Visible)
                chbHwLimits.Checked = false;
        }

        private void bIconPrev_Click(object sender, EventArgs e)
        {
            if (icon == 0)
                icon = CommonInf.icons.Count() - 1;
            else
                icon--;
            displayIcon();
        }

        private void bIconNext_Click(object sender, EventArgs e)
        {
            if (icon == CommonInf.icons.Count() - 1)
                icon = 0;
            else
                icon++;
            displayIcon();
        }

    }




}
