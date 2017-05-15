using Jerome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using JeromeControl;
using System.Windows.Forms;
using System.Threading;

namespace AntennaeRotator
{
    public class AntennaeRotatorConnectionSettings
    {
        public string name = "";
        public JeromeConnectionParams jeromeParams = new JeromeConnectionParams();
        public int northAngle = -1;
        public int[] switchIntervals = new int[] { 5, 5 };
        [XmlIgnoreAttribute]
        public Dictionary<int, int> limits = new Dictionary<int, int> { { 1, -1 }, { -1, -1 } };
        public int deviceType = 0;
        public int icon = 0;
        internal bool ignoreEngineOffMovement;
        public bool hwLimits;
        public int[] limitsSerialize;
        public int esMhz;
        public bool calibrated;



        public override string ToString()
        {
            return name;
        }

    }

    public class AntennaeRotatorFormState : JCChildFormState
    {
        public int currentConnection = -1;
    }

    public class AntennaeRotatorConfig : JCComponentConfig
    {
        public List<string> maps = new List<string>();
        public int currentMap = -1;
        public List<AntennaeRotatorConnectionSettings> connections = new List<AntennaeRotatorConnectionSettings>();

        public override JCChildFormState[] formStates {
            get {
                return _formStates;
            }
            set {
                if (value == null)
                    _formStates = null;
                else
                {
                    _formStates = new AntennaeRotatorFormState[value.Count()];
                    for (int c = 0; c < value.Count(); c++)
                        _formStates[c] = (AntennaeRotatorFormState)value[c];
                }
            }
        }
        [XmlIgnoreAttribute]
        private AntennaeRotatorFormState[] _formStates;

        public override void initFormStates( int formCount)
        {
            formStates = new AntennaeRotatorFormState[formCount];
            for (var c = 0; c < formCount; c++)
                formStates[c] = new AntennaeRotatorFormState();
        }

        public override void esMessage(int mhz, bool trx)
        {
            try
            {
                List<AntennaeRotatorConnectionSettings> bc = connections.Where(x => x.esMhz == mhz).ToList();
                JCAppContext appContext = JCAppContext.CurrentAppContext;
                for (int c = 0; c < forms.Count(); c++)
                {
                    if (c < bc.Count())
                    {
                        int cIdx = connections.IndexOf(bc[c]);
                        if (forms[c] == null)
                        {
                            _formStates[c].currentConnection = cIdx;
                            appContext.updateGUI(delegate ()
                           {
                               appContext.createForm("AntennaeRotator.FRotator", c);
                           });


                        }
                        else
                            appContext.updateGUI(delegate ()
                           {
                               ((FRotator)forms[c]).loadConnection(cIdx);
                           });
                    }
                    else
                    {
                        if (forms[c] != null)
                            forms[c].Invoke((MethodInvoker)delegate ()
                          {
                              forms[c].Close();
                          });
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }


        }
    }

}
