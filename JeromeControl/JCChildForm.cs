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
        public virtual void InitializeComponent() { }
        public JCComponentConfig componentConfig { get {
                return appContext?.config?.components[JCConfig.getTypeIdx(this)];
            }
            set {
                appContext.config.components[JCConfig.getTypeIdx(this)] = value;
            }
        }

        public override StorableFormConfig storableConfig => componentConfig?.formStates[idx];

        public virtual void esMessage(int mhz, bool trx) { }

        public JCChildForm(JCAppContext _appContext, int _idx) : base() {
            appContext = _appContext;
            idx = _idx;
            componentConfig.forms[idx] = this;
            componentConfig.formStates[idx].active = true;
            appContext.writeConfig();
            this.FormClosed += _formClosed;
        }

        private void _formClosed(object sender, EventArgs e)
        {
            if (!appContext.exiting)
            {
                appContext.config.getFormState(this).active = false;
                componentConfig.forms[idx] = null;
                appContext.writeConfig();
            }
        }

        public JCChildForm() : base() {
            InitializeComponent();
        }
        }   

    }
