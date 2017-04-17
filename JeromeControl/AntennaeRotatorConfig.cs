﻿using Jerome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using JeromeControl;

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
        public int currentConnection = -1;
        public List<string> maps = new List<string>();
        public int currentMap = -1;
        public List<AntennaeRotatorConnectionSettings> connections = new List<AntennaeRotatorConnectionSettings>();

        public override JCChildFormState[] formStates { get { return _formStates; } set { _formStates = (AntennaeRotatorFormState[])value; } }
        [XmlIgnoreAttribute]
        private AntennaeRotatorFormState[] _formStates;

        public override void initFormStates( int formCount)
        {
            formStates = new AntennaeRotatorFormState[formCount];
            for (var c = 0; c < formCount; c++)
                formStates[c] = new AntennaeRotatorFormState();
        }

        public AntennaeRotatorConfig(JCAppContext _appContext) : base(_appContext) { }

        public override void esMessage(int mhz, bool trx)
        {
            List<AntennaeRotatorConnectionSettings> bc = connections.Where(x => x.esMhz == mhz).ToList();
            for (int c = 0; c < forms.Count(); c++)
            {
                if (c < bc.Count())
                {
                    int cIdx = connections.IndexOf(bc[c]);
                    if (forms[c] == null)
                    {
                        FRotator f = new FRotator(appContext, c);
                    }
                    ((FRotator)forms[c]).loadConnection(cIdx);                    
                } else
                {
                    if (forms[c] != null)
                        forms[c].Close();
                }

            }
                
        }
    }

}
