using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace JeromeControl
{
    public class JCConfig
    {
        public string esHost = null;
        public int esPort = 0;

        public void write()
        {
            using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\config.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(JCConfig));
                ser.Serialize(sw, this);
            }
        }

        public static JCConfig read()
        {
            JCConfig result = null;
            if (File.Exists(Application.StartupPath + "\\config.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(JCConfig));
                using (FileStream fs = File.OpenRead(Application.StartupPath + "\\config.xml"))
                {
                    try
                    {
                        result = (JCConfig)ser.Deserialize(fs);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }
            if (result == null)
                result = new JCConfig();
            return result;
        }
    }
}
