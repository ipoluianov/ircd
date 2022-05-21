using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRCD
{
    public partial class DebugForm : Form
    {
        SerialConnector connector_;

        public DebugForm(SerialConnector connector)
        {
            connector_ = connector;
            connector_.LogEvent += ConnectorLogEvent;
            connector_.FrameReceivedEvent += ConnectorFrameReceivedEvent;
            InitializeComponent();
        }

        private void ConnectorFrameReceivedEvent(object sender, SerialConnector.FrameReceivedArgs e)
        {
            setChart(e.Signature);
            LoadKeys(true);
        }

        private void ConnectorLogEvent(object sender, SerialConnector.LogArgs e)
        {
            Log(e.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadKeys(false);
        }

        private void setChart(List<double> signature)
        {
            lastSign = signature;
            lvPeaks.Items.Clear();
            chart1.Series[0].Points.Clear();
            double offset = 0;
            double level = 0;
            chart1.Series[0].Points.AddXY(-10000, 0);
            for (int i = 0; i < signature.Count; i++)
            {
                double time = signature[i];
                offset += time;
                chart1.Series[0].Points.AddXY(offset, level);
                if (level > 0)
                    level = 0;
                else
                    level = 0.9;
                chart1.Series[0].Points.AddXY(offset, level);
                string c = string.Format("{0:N0}", lastSign[i]);
                string c1 = string.Format("{0:N2}", lastSign[i]);
                var lvItem = lvPeaks.Items.Add(c);
                lvItem.SubItems.Add(c1);

            }
            chart1.Series[0].Points.AddXY(offset + 10000, 0);
        }

        private string hex(byte[] buffer)
        {
            string hex = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                if (hex.Length > 0)
                    hex += " ";
                var n = Convert.ToString(buffer[i], 16);
                if (n.Length < 2)
                    n = "0" + n;
                hex += n.ToUpper();

            }
            return hex;
        }

        List<double> lastSign = new List<double>();
        //List<string> lastSignCodes = new List<string>();

        private void Log(string text)
        {
            var item = lvItems.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            item.SubItems.Add(text);
            lvItems.EnsureVisible(lvItems.Items.Count - 1);
        }

        private void timerCheckPort_Tick(object sender, EventArgs e)
        {

        }

        private void LoadKeys(bool detect)
        {
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvItems.Items.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadKeys(false);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
        }

    }
}
