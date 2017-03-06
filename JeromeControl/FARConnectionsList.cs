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
    public partial class FConnectionsList : Form
    {
        AntennaeRotatorConfig config;
        public FConnectionsList(AntennaeRotatorConfig fs)
        {
            InitializeComponent();
            config = fs;
            fillList();
            if ( lbConnections.Items.Count > 0 )
                lbConnections.SelectedIndex = 0;
        }

        private void fillList()
        {
            lbConnections.Items.Clear();
            foreach (AntennaeRotatorConnectionSettings cs in config.connections)
                lbConnections.Items.Add(cs.name);

        }

        private void bEdit_Click(object sender, EventArgs e)
        {
            if (((FRotator)this.Owner).editConnection(config.connections[lbConnections.SelectedIndex]))
            {
                int sel = lbConnections.SelectedIndex;
                fillList();
                lbConnections.SelectedIndex = sel;
            }

        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить соединение " + lbConnections.Items[lbConnections.SelectedIndex] + "?",
                "Удаление соединения", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int sel = lbConnections.SelectedIndex;
                config.connections.RemoveAt(lbConnections.SelectedIndex);
                lbConnections.Items.RemoveAt(lbConnections.SelectedIndex);
                ((FRotator)this.Owner).writeConfig();
                if (lbConnections.Items.Count > 0)
                {
                    if (sel < lbConnections.Items.Count)
                        lbConnections.SelectedIndex = sel;
                    else
                        lbConnections.SelectedIndex = lbConnections.Items.Count - 1;
                }

            }
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            Close();
        }


    }
}
