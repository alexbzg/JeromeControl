using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StorableFormState
{
    public class FormWStorableState : Form
    {
        public virtual StorableFormConfig storableConfig { get; }
        public virtual void writeConfig() { }
        public bool loaded = false;

        public void storeFormState()
        {
            Rectangle bounds = this.WindowState != FormWindowState.Normal ? this.RestoreBounds : this.DesktopBounds;
            storableConfig.formLocation = bounds.Location;
            storableConfig.formSize = bounds.Size;
        }

        public virtual void restoreFormState()
        {
              if (storableConfig?.formLocation != null && !storableConfig.formLocation.IsEmpty)
                  this.DesktopBounds =
                          new Rectangle(storableConfig.formLocation, storableConfig.formSize);
        }

        public FormWStorableState()
        {
            Load += FormWStorableState_Load;
            ResizeEnd += FormWStorableState_MoveResize;
            Move += FormWStorableState_MoveResize;
        }

        private void FormWStorableState_MoveResize(object sender, EventArgs e)
        {
            if (loaded)
            {
                storeFormState();
                writeConfig();
            }
        }


        private void FormWStorableState_Load(object sender, EventArgs e)
        {
            restoreFormState();
            loaded = true;

        }
    }



    public class StorableFormConfig
    {
        public System.Drawing.Point formLocation;
        public System.Drawing.Size formSize;
    }

}
