using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRCD
{
    public class SerialConnector : IDisposable
    {
        static SerialConnector instance = null;
        private static object locker = new object();

        private bool started = false;
        public void Start() {
            started = true;
            timer_.Interval = 100;
            timer_.Tick += TimerTick;
            timer_.Start();
        }

        protected SerialConnector()
        {
        }

        public static SerialConnector Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new SerialConnector();
                    }
                }
            }
            return instance;
        }

        private SerialPort port;
        private Timer timer_ = new Timer();
        private Queue<byte[]> logTraffic = new Queue<byte[]>();

        public class FrameReceivedArgs
        {
            public FrameReceivedArgs(byte[] data, List<double> signature, string action)
            {
                Data = data;
                Signature = signature;
                Action = action;
            }
            public byte[] Data { get; }
            public List<double> Signature { get; }
            public string Action { get; }
        }
        public delegate void FrameReceivedHandler(object sender, FrameReceivedArgs e);
        public event FrameReceivedHandler FrameReceivedEvent;

        public class LogArgs
        {
            public LogArgs(string text) { Text = text; }
            public string Text { get; }
        }
        public delegate void LogHandler(object sender, LogArgs e);
        public event LogHandler LogEvent;

        private void TimerTick(object sender, EventArgs e)
        {
            checkPort();

            lock (logTraffic)
            {
                while (logTraffic.Count > 0)
                {
                    byte[] buffer = logTraffic.Dequeue();

                    if (buffer.Length >= 4)
                    {
                        ushort signatureBegin = BitConverter.ToUInt16(buffer, 0);
                        ushort signatureEnd = BitConverter.ToUInt16(buffer, buffer.Length - 2);
                    }

                    List<double> times = new List<double>();

                    if (buffer.Length > 6)
                    {
                        ushort count = BitConverter.ToUInt16(buffer, 2);
                        if (buffer.Length >= 6 + count * 2)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                times.Add(BitConverter.ToUInt16(buffer, 4 + i * 2));
                            }
                        }

                    }

                    string detectedAction = "";

                    foreach (var item in Storage.Instance().Settings().Items)
                    {
                        foreach (var signature in item.Signatures)
                        {
                            bool same = areSame(signature.Items, times);
                            if (same)
                            {
                                detectedAction = item.Action;
                                Log("EVENT:" + item.Action);
                                break;
                            }
                        }
                    }

                    FrameReceivedEvent?.Invoke(this, new FrameReceivedArgs(buffer, times, detectedAction));
                }
            }
        }

        private void checkPort()
        {
            if (!started)
                return;

            if (port != null)
                if (port.IsOpen)
                    return;

            if (port != null)
                port.Dispose();
            port = null;

            if (port == null)
            {
                try
                {
                    port = new SerialPort("COM7", 9600, Parity.None, 8, StopBits.One);
                    port.Open();
                    port.DataReceived += Port_DataReceived; ;
                    port.ErrorReceived += Port_ErrorReceived; ;
                    Log("Opened");
                }
                catch (Exception ex)
                {
                    Log("error: " + ex.ToString());
                    if (port != null)
                        port.Dispose();
                    port = null;
                }
            }
        }

        private void Log(string text)
        {
            LogEvent?.Invoke(this, new LogArgs(text));
        }

        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Log("Error Received");
            if (port != null)
            {
                port.Dispose();
                port = null;
            }
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (logTraffic)
            {
                int bufSize = port.BytesToRead;
                byte[] buffer = new byte[bufSize];
                port.Read(buffer, 0, bufSize);
                logTraffic.Enqueue(buffer);
            }
        }

        public void Dispose()
        {
        }

        private bool areSame(List<double> a1, List<double> a2)
        {
            bool result = true;

            if (a1.Count == a2.Count)
            {
                for (int i = 0; i < a1.Count; i++)
                {
                    if (a1[i] > 10)
                    {
                        double maxDiff = a1[i] * 0.3;
                        double diff = Math.Abs(a1[i] - a2[i]);
                        if (diff > maxDiff)
                        {
                            result = false;
                        }
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

    }
}
