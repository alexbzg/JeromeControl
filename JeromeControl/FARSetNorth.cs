using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AntennaeRotator
{
    public partial class FSetNorth : Form
    {
        public Dictionary<int, int> limits = new Dictionary<int, int> { { -1, -1 }, { 1, -1 } };
        public int northAngle = -1;
        private AntennaeRotatorConnectionSettings _cs;

        public FSetNorth(AntennaeRotatorConnectionSettings cs)
        {
            InitializeComponent();
            northAngle = cs.northAngle;
            if (cs.hwLimits)
            {
                bStop0.Enabled = false;
                bStop1.Enabled = false;
                bDeleteStops.Enabled = false;
            }
            else
                limits = new Dictionary<int, int>(cs.limits);
            _cs = cs;
            if (!cs.hwLimits)
                bDeleteStops.Visible = false;
        }

        private void bRotate0_MouseDown(object sender, MouseEventArgs e)
        {
            ((FRotator)this.Owner).engine(1);
        }

        private void bRotate_MouseUp(object sender, MouseEventArgs e)
        {
            ((FRotator)this.Owner).engine(0);
        }

        private void bRotate1_MouseDown(object sender, MouseEventArgs e)
        {
            ((FRotator)this.Owner).engine(-1);
        }

        private void bStop0_Click(object sender, EventArgs e)
        {
            limits[1] = ((FRotator)this.Owner).getCurrentAngle();
        }

        private void bNorth_Click(object sender, EventArgs e)
        {
            northAngle = ((FRotator)this.Owner).getCurrentAngle();
        }

        private void bStop1_Click(object sender, EventArgs e)
        {
            limits[-1] = ((FRotator)this.Owner).getCurrentAngle();
        }

        private void bDeleteStops_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите сбросить настройки концевиков?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ((FRotator)this.Owner).clearLimits();
                limits = new Dictionary<int, int>(_cs.limits);
            }
        }
    }
}
