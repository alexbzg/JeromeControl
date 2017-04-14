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
using System.Reflection;

namespace JeromeControl
{
    public class JCChildFormState : StorableFormConfig
    {
        public bool active;
    }

    public class JCComponentConfig
    {
        public virtual JCChildFormState[] formStates { get { return _formStates; } set { _formStates = value; } }
        private JCChildFormState[] _formStates;
        public virtual void initFormStates( int formCount ) {
            formStates = new JCChildFormState[formCount];
            for (var c = 0; c < formCount; c++)
                formStates[c] = new JCChildFormState();
        }
        public JCComponentConfig( int formCount )
        {
            initFormStates(formCount);
        }

    }

    public class JCConfig
    {
        internal static readonly string[] ChildFormsTypes = new string[] { "AntennaeRotator.FRotator", "NetComm.FNetComm", "WX0B.FWX0B" };
        internal static readonly string[] ChildFormsTitles = new string[] { "AntennaRotator", "NetComm", "WX0B" };
        internal static readonly int[] ChildFormsCount = new int[] { 2, 1, 1 };

        public string esHost = null;
        public int esPort = 0;
        public JCComponentConfig[] components;

        public void write()
        {
            using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\config.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(JCConfig));
                ser.Serialize(sw, this);
            }
        }

        public int getTypeIdx( JCChildForm form )
        {
            return Array.IndexOf(JCConfig.ChildFormsTypes, form.GetType().ToString() );
        }

        public JCChildFormState getFormState( JCChildForm form )
        {
            return components[getTypeIdx(form)].formStates[form.idx];
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
            for (int c = 0; c < ChildFormsTypes.Count(); c++)
            {
                if ( result.components[c] == null )
                {
                    Type childFormType = Type.GetType(ChildFormsTypes[c]);
                    result.components[c] = ((JCChildForm)childFormType).createConfig(ChildFormsCount[c]);
                }
            }
            return result;
        }
    }
}
