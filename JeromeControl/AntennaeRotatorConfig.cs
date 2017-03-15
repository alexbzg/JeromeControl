using Jerome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
        public System.Drawing.Point formLocation;
        public System.Drawing.Size formSize;
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

    public class AntennaeRotatorConfig
    {
        public int currentConnection = -1;
        public List<string> maps = new List<string>();
        public int currentMap = -1;
        public List<AntennaeRotatorConnectionSettings> connections = new List<AntennaeRotatorConnectionSettings>();
    }


}
