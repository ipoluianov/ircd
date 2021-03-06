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
            timer_.Interval = 50;
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

        private SettingsItem lastDetectedItem = null;
        private DateTime lastDetectedItemDT = DateTime.Now;

        public class FrameReceivedArgs
        {
            public FrameReceivedArgs(byte[] data, List<double> signature, SettingsItem detectedItem, bool repetition)
            {
                Data = data;
                Signature = signature;
                DetectedItem = detectedItem;
                Repetition = repetition;
            }
            public byte[] Data { get; }
            public List<double> Signature { get; }
            public SettingsItem DetectedItem { get; }
            public bool Repetition { get; }
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

                    SettingsItem detectedItem = null;
                    bool isRepetition = false;

                    double repetitionMaxDelay = 300;

                    // Determine repetitions
                    if (lastDetectedItem != null)
                    {
                        if (DateTime.Now.Subtract(lastDetectedItemDT).TotalMilliseconds < repetitionMaxDelay)
                        {
                            foreach (var signature in lastDetectedItem.SignaturesRepetitions)
                            {
                                bool same = AreSame(signature.Items, times);
                                if (same)
                                {
                                    detectedItem = lastDetectedItem;
                                    isRepetition = true;
                                    lastDetectedItemDT = DateTime.Now;
                                    Log("EVENT:" + lastDetectedItem.Action + " rep");
                                    break;
                                }
                            }
                        }

                        if (detectedItem == null)
                        {
                            lastDetectedItem = null;
                        }
                    }

                    if (detectedItem == null)
                    {
                        foreach (var item in Storage.Instance().Settings().Items)
                        {
                            foreach (var signature in item.Signatures)
                            {
                                bool same = AreSame(signature.Items, times);
                                if (same)
                                {
                                    detectedItem = item;
                                    lastDetectedItemDT = DateTime.Now;
                                    Log("EVENT:" + item.Action);
                                    break;
                                }
                            }
                        }
                    }

                    lastDetectedItem = detectedItem;

                    /*if (detectedItem != null)
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/C " + detectedItem.Action;
                        process.StartInfo = startInfo;
                        process.Start();
                    }*/

                    FrameReceivedEvent?.Invoke(this, new FrameReceivedArgs(buffer, times, detectedItem, isRepetition));
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
                    port = new SerialPort(Storage.SerialPortName, 9600, Parity.None, 8, StopBits.One);
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

        static public bool AreSame(List<double> a1, List<double> a2)
        {
            bool result = true;

            if (a1.Count == a2.Count)
            {
                for (int i = 0; i < a1.Count; i++)
                {
                    if (a1[i] > 10)
                    {
                        double maxDiff = a1[i] * 0.4; // TODO: settings
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
