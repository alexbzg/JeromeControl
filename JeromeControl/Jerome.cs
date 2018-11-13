//#define DEBUG_JEROME_COMM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using AsyncConnectionNS;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Jerome
{

    public class LineStateChangedEventArgs : EventArgs
    {
        public int line;
        public int state;
    }

    public class JeromeController
    {

        class CmdEntry
        {
            public string cmd;
            public Action<string> cb;

            public CmdEntry(string cmdE, Action<string> cbE)
            {
                cmd = cmdE;
                cb = cbE;
            }
        }

        private volatile CmdEntry currentCmd;
        private ConcurrentQueue<CmdEntry> cmdQueue = new ConcurrentQueue<CmdEntry>();


        private static int timeout = 10000;
        private static Regex rEVT = new Regex(@"#EVT,IN,\d+,(\d+),(\d)");

        private System.Threading.Timer replyTimer;
        private System.Threading.Timer pingTimer;
        private volatile bool disconnecting;
        private volatile bool _active;

        public bool active {  get { return _active; } }

        // ManualResetEvent instances signal completion.

        private string password;
        private AsyncConnection connection = new AsyncConnection();
        private AsyncConnection usartConnection;

        public event EventHandler<DisconnectEventArgs> onDisconnected;
        public event EventHandler<EventArgs> onConnected;
        public event EventHandler<LineStateChangedEventArgs> lineStateChanged;
        public event EventHandler<LineReceivedEventArgs> usartLineReceived
        {
            add { usartConnection.lineReceived += value; }
            remove { usartConnection.lineReceived -= value; }
        }
        public event EventHandler<BytesReceivedEventArgs> usartBytesReceived
        {
            add { usartConnection.bytesReceived += value; }
            remove { usartConnection.bytesReceived -= value; }
        }

        public JeromeConnectionParams connectionParams;

        public bool connected
        {
            get
            {
                return connection != null && connection.connected;
            }
        }


        public bool usartConnected {
            get
            {
                return usartConnection != null && usartConnection.connected;
            }
        }

        public bool usartBinaryMode
        {
            get { return usartConnection.binaryMode; }
            set { usartConnection.binaryMode = value; }
        }

        
        public static JeromeController create( JeromeConnectionParams p )
        {
            JeromeController jc = new JeromeController();
            jc.password = p.password;
            jc.connectionParams = p;
            jc.connection.onConnected += jc._onConnected;
            jc.connection.onDisconnected += jc._onDisconnected;
            jc.connection.lineReceived += jc.processReply;
            jc.connection.reconnect = true;
            if (p.usartPort != 0)
                jc.usartConnection = new AsyncConnection();
            return jc;
        }

        private void newCmd(string cmd, Action<string> cb)
        {
            CmdEntry ce = new CmdEntry(cmd, cb);
            cmdQueue.Enqueue(ce);
            if (currentCmd == null)
                processQueue();
        }

        private void newCmd(string cmd)
        {
            newCmd(cmd, null);
        }

        private void _onDisconnected(object sender, DisconnectEventArgs e)
        {
            if (disconnecting)
                disconnecting = false;
            else
            {
                onDisconnected?.Invoke(this, e);
                clearQueue();
            }
        }

        private void clearQueue()
        {
            currentCmd = null;
            CmdEntry ignored;
            while (cmdQueue.TryDequeue(out ignored)) ;
            if (pingTimer != null)
                pingTimer.Change(Timeout.Infinite, Timeout.Infinite);
            if (replyTimer != null)
                replyTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void _onConnected(object sender, EventArgs e)
        {
            newCmd("PSW,SET," + connectionParams.password);
            newCmd("EVT,ON");
            pingTimer = new System.Threading.Timer(obj =>  newCmd(""), null, timeout, timeout);
            if (usartConnection != null)
                usartConnection.connect(connectionParams.host,connectionParams.usartPort,true);
            onConnected?.Invoke(this, e);
        }

        private void processQueue()
        {
            if (!connected || disconnecting)
                return;
            CmdEntry bufCmd;
            if (currentCmd == null && cmdQueue.TryDequeue(out bufCmd)  ){
                currentCmd = bufCmd;
#if DEBUG
#if DEBUG_JEROME_COMM
                System.Diagnostics.Debug.WriteLine(bufCmd.cmd);
#endif
#endif
                connection.sendCommand("$KE" + ( bufCmd.cmd.Equals(String.Empty) ? "" : "," + bufCmd.cmd ));
                replyTimer = new System.Threading.Timer(obj => replyTimeout(), null, timeout, Timeout.Infinite);
            }
        }

        public string sendCommand(string cmd)
        {
            if (!connected || disconnecting)
                return "";
            string result = "";
            ManualResetEvent reDone = new ManualResetEvent(false);
            newCmd(cmd, delegate (string r)
            {
                result = r;
                reDone.Set();
            });
            reDone.WaitOne(timeout);
            return result;
        }




        public bool connect()
        {
            _active = true;
            connection.connect(connectionParams.host, connectionParams.port);
            return connected;
        }

        public void asyncConnect()
        {
            _active = true;
            connection.connect(connectionParams.host, connectionParams.port, true);
        }

        private void UsartConnection_lineReceived(object sender, LineReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void disconnect()
        {
            _active = false;
            disconnect(false);
        }

        public void disconnect( bool reconnect )
        {
            System.Diagnostics.Debug.WriteLine("disconnect");
            clearQueue();
            connection.disconnect();
            if (usartConnection != null )
                usartConnection.disconnect();
            if ( reconnect )
            {
                connection.reconnect = true;
                connection.asyncConnect();
            }
        }

        


        private void processReply(object sender, LineReceivedEventArgs e )
        {
            string reply = e.line;
#if DEBUG
#if DEBUG_JEROME_COMM
                System.Diagnostics.Debug.WriteLine(reply);
#endif
#endif

            if ( pingTimer != null)
                pingTimer.Change(timeout, timeout);
            Match match = rEVT.Match(reply);
            if (match.Success)
            {
                int line = Convert.ToInt16( match.Groups[1].Value );
                int lineState = match.Groups[2].Value == "0" ? 0 : 1;
                Task.Factory.StartNew( () =>
                    lineStateChanged?.Invoke(this, new LineStateChangedEventArgs { line = line, state = lineState } ) );
            }
            else if ( !reply.StartsWith( "#SLINF" ) && !reply.Contains( "FLAGS" ) && !reply.Contains( "JConfig" ) )
            {
                replyTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                if (currentCmd != null && currentCmd.cb != null)
                {
                    Action<string> cb = currentCmd.cb;
                    Task.Factory.StartNew( () => cb.Invoke(reply) );
                }
                currentCmd = null;
                processQueue();

            }
        }

        public void setLineMode(int line, int mode)
        {
            newCmd("IO,SET," + line.ToString() + "," + mode.ToString());
        }

        public void switchLine(int line, int state)
        {
            newCmd("WR," + line.ToString() + "," + state.ToString());
        }

        public string readlines()
        {
            string reply = sendCommand("RID,ALL");
            int split = reply.LastIndexOf( ',' );
            return reply.Substring(split + 1).TrimEnd('\r', '\n');
        }

        public int readADC( int adc)
        {
            string reply = sendCommand("ADC," + adc.ToString());
            return (reply.Length > 11 ? Convert.ToInt16(reply.Substring(7, 4)) : -1);
        }


        private void replyTimeout()
        {
            System.Diagnostics.Debug.WriteLine("Reply timeout");
            disconnecting = true;
            disconnect(true);
            onDisconnected?.Invoke(this, new DisconnectEventArgs() { requested = false });
        }

        public void usartSendBytes( byte[] data)
        {
            if ( usartConnected )
            {
                usartConnection.sendBytes(data);
            }
        }



    }

    public class JeromeConnectionParams
    {
        public string name;
        public string host;
        public int port = 2424;
        public int usartPort = 2525;
        public string password = "Jerome";
        public int httpPort = 80;

        public bool edit()
        {
            FConnectionParams fcp = new FConnectionParams(this);
            if (fcp.ShowDialog() == DialogResult.OK)
            {
                name = fcp.data.name;
                host = fcp.data.host;
                port = fcp.data.port;
                httpPort = fcp.data.httpPort;
                password = fcp.data.password;
                return true;
            }
            else
                return false;
        }


        public JeromeControllerState getState()
        {
            HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("http://" + host + ":" + httpPort + "/state.xml");
            try
            {
                HttpWebResponse resp = (HttpWebResponse)rq.GetResponse();
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    XElement xr;
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
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Query to " + host + ":" + httpPort.ToString() +
                    " error: " + e.ToString());
            }
            return null;
        }
    }

    public class JeromeControllerState {
        public bool[] linesModes = new bool[22];
        public bool[] linesStates = new bool[22];
        public int[] adcsValues = new int[4];
    }

}
