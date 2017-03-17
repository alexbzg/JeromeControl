using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WX0B
{
    public partial class WX0BControllerPanel : UserControl
    {
        public WX0BController controller;
        public FWX0B fWX0B;

        public WX0BControllerPanel( FWX0B _fWX0B, WX0BController _controller)
        {
            controller = _controller;
            fWX0B = _fWX0B;
            InitializeComponent();
        }
    }
}
