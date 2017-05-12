using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AsyncConnectionNS;

namespace ExpertSync
{
    public partial class FESConnection : Form
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

        public FESConnection( string host = "127.0.0.1", int port = 50040)
        {
            InitializeComponent();
            tbAddress.Text = host;
            tbPort.Text = port.ToString();
        }

    
    }

    public class StateObject
    {
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public Func<string> reCb;
    }

    public class MessageEventArgs : EventArgs
    {
        public byte modulation;
        public double vfoa;
        public double vfob;
        public bool trx;
    }

    public class ExpertSyncConnector
    {
        private static int _timeout = 1000;

        private string _host;
        private int _port;

        private AsyncConnection ac;

        public virtual bool reconnect {  get { return ac.reconnect; } set { ac.reconnect = value; } }
        public virtual int timeout { get { return ac.timeout; } set { ac.timeout = value; } }
        public virtual bool connected { get { return ac.connected; } }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Message
        {
            public byte sid;          //!< software ID
            public byte type;         //!< тип пакета
            public byte receiver;     //!< номер программного приёмника
            public double vfoa;                //!< частота приёмника A
            public double vfob;                //!< частота приёмника B
            public byte modulation;   //!< индекс модуляции eSyncModulation
            public byte trx;          // false - режим приёма, true - режим передачи
        }

        public event EventHandler<MessageEventArgs> onMessage;

        public ExpertSyncConnector( string host, int port )
        {
            _host = host;
            _port = port;
            ac = new AsyncConnection();
            ac.binaryMode = true;
            ac.timeout = _timeout;
            ac.bytesReceived += bytesReceived;
        }

        public bool connect()
        {
            return ac.connect(_host, _port);
        }

        public void asyncConnect()
        {
            ac.connect(_host, _port, true);
        }

        public void disconnect()
        {
            ac.disconnect();
        }

        private void bytesReceived( object Sender, BytesReceivedEventArgs e )
        {
            GCHandle pinnedBuffer = GCHandle.Alloc(e.bytes, GCHandleType.Pinned);
            Message msg = (Message)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(),
                    typeof(Message));
            onMessage?.Invoke(this, new MessageEventArgs
            {
                modulation = msg.modulation,
                vfoa = msg.vfoa,
                vfob = msg.vfob,
                trx = msg.trx == 1
            });
        }


    }




}
