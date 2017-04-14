using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using AntennaeRotator;
using NetComm;
using WX0B;
using StorableFormState;

namespace JeromeControl
{
    public class JCChildFormState : StorableFormConfig
    {
        public bool active;
    }

    public class JCConfig
    {
        internal static readonly string[] ChildFormsTypes = new string[] { "AntennaeRotator.FRotator", "NetComm.FNetComm", "WX0B.FWX0B" };
        internal static readonly string[] ChildFormsTitles = new string[] { "AntennaRotator", "NetComm", "WX0B" };
        internal static readonly int[] ChildFormsCount = new int[] { 2, 1, 1 };

        public string esHost = null;
        public int esPort = 0;
        public AntennaeRotatorConfig antennaeRotatorConfig = new AntennaeRotatorConfig();
        public NetCommConfig netCommConfig = new NetCommConfig();
        public WX0BConfig WX0BConfig = new WX0BConfig();
        public JCChildFormState[][] childForms = new JCChildFormState[ChildFormsTypes.Count()][];

        public void write()
        {
            using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\config.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(JCConfig));
                ser.Serialize(sw, this);
            }
        }

        public int getTypeIdx( IJCChildForm form )
        {
            return Array.IndexOf(JCConfig.ChildFormsTypes, form.GetType().ToString() );
        }

        public JCChildFormState getChildForm( IJCChildForm form )
        {
            return childForms[getTypeIdx(form)][form.idx];
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
            if (result.antennaeRotatorConfig == null)
                result.antennaeRotatorConfig = new AntennaeRotatorConfig();
            if (result.netCommConfig == null)
                result.netCommConfig = new NetCommConfig();
            if (result.WX0BConfig == null)
                result.WX0BConfig = new WX0BConfig();
            for (int c = 0; c < ChildFormsTypes.Count(); c++)
            {
                if ( result.childForms[c] == null )
                    result.childForms[c] = new JCChildFormState[ChildFormsCount[c]];
                for (int c0 = 0; c0 < ChildFormsCount[c]; c0++)
                    if (result.childForms[c][c0] == null)
                        result.childForms[c][c0] = new JCChildFormState();
            }
            return result;
        }
    }
}
