using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AsyncConnectionNS;
using System.IO;

namespace WX0B
{
    public partial class FWX0B : Form
    {
        public string host
        {
            get
            {
                return tbAddress.Text;
            }
        }

        public int port
        {
            get
            {
                return Convert.ToInt32(tbPort.Text);
            }
        }

        public FWX0B()
        {
            InitializeComponent();
        }

        public FWX0B( string host, int port)
        {
            InitializeComponent();
            tbAddress.Text = host;
            tbPort.Text = port.ToString();
        }

    
    }

    public class WX0BController
    {

    }

    public class WX0BTerminal
    {

    }

    public class WX0BCluster
    {
        string title;

    }

    public class WX0BData
    {
        public WX0BData( string host, int port)
        {
            WebClient webClient = new WebClient();
            try
            {
               /* dynamic result = JsonValue.Parse(webClient.DownloadString("http://" + host + ":" + port.ToString() + "/data");
                Console.WriteLine(result.response.user.firstName);
                HttpWebResponse resp = (HttpWebResponse)rq.GetResponse();
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        xr = XElement.Parse(stream.ReadToEnd());
                    }
                    JeromeControllerState result = new JeromeControllerState();
                    string linesModes = xr.XPathSelectElement("iotable").Value;
                    string linesStates = xr.XPathSelectElement("iovalue").Value;
                    for (int co = 0; co < 22; co++)
                    {
                        result.linesModes[co] = linesModes[co] == '1';
                        result.linesStates[co] = linesStates[co] == '1';
                    }
                    for (int co = 0; co < 4; co++)
                        result.adcsValues[co] = (int)xr.XPathSelectElement("adc" + (co + 1).ToString());
                    return result;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Query to " + host + ":" + httpPort.ToString() +
                        " returned status code" + resp.StatusCode.ToString());
                }*/
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Query to " + host + ":" + port.ToString() +
                    " error: " + e.ToString());
            }
        }
    }




}
