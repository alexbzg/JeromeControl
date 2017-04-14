using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StorableFormState;

namespace JeromeControl
{
    public class JCChildForm : FormWStorableState
    {
        public JCAppContext appContext;
        public int idx;

        public virtual void esMessage(int mhz, bool trx) { }

        public JCChildForm(JCAppContext _appContext, int _idx) : base() {
            appContext = _appContext;
            idx = _idx;
        }

    }
}
