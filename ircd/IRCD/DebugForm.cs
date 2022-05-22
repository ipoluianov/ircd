using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IRCD
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            SerialConnector.Instance().LogEvent += ConnectorLogEvent;
            SerialConnector.Instance().FrameReceivedEvent += ConnectorFrameReceivedEvent;
            InitializeComponent();
        }

        private void ConnectorFrameReceivedEvent(object sender, SerialConnector.FrameReceivedArgs e)
        {
            //setChart(e.Signature);
            //AddToReceivedFrames(e.Signature);
            //framesTable.AddToReceivedFrames(e.Signature);
        }

        private void ConnectorLogEvent(object sender, SerialConnector.LogArgs e)
        {
            Log(e.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            framesTable.StartMonitoring();
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
            /*var item = lvItems.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            item.SubItems.Add(text);
            lvItems.EnsureVisible(lvItems.Items.Count - 1);*/
        }

        private void timerCheckPort_Tick(object sender, EventArgs e)
        {
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            framesTable.Clear();
        }

        private void ClearCurrentItems()
        {
            chartFrames.ChartAreas.Clear();
            chartFrames.Series.Clear();
            //chartFrames.Series.Clear();
            //lvTimes.Items.Clear();
            //lvTimes.Columns.Clear();
            maxTime = 0;
        }

        private void AddCurrentItem(string name, List<double> signature)
        {
            var area = chartFrames.ChartAreas.Add("Area " + name);
            area.AxisX.Title = name;
            var ser = chartFrames.Series.Add(name);
            ser.ChartArea = area.Name;
            ser.BorderWidth = 3;
            ser.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;

            double offset = 0;
            double level = 0;
            for (int i = 0; i < signature.Count; i++)
            {
                double time = signature[i];
                offset += time;
                ser.Points.AddXY(offset, level);
                if (level > 0)
                    level = 0;
                else
                    level = 0.9;
                ser.Points.AddXY(offset, level);
            }

            if (offset > maxTime)
                maxTime = offset;
        }

        double maxTime = 0;

        private void UpdateSelected(FramesTable.SelectionChangedArgs e)
        {
            ClearCurrentItems();

            // TODO:
            for (int i = 0; i < e.Items.Count; i++)
            {
                var selectedItem = e.Items[i];
                var sign = selectedItem.Signature;
                AddCurrentItem(selectedItem.Name, sign);
            }

            maxTime = maxTime - (maxTime % 10000) + 10000;

            foreach (var area in chartFrames.ChartAreas)
            {
                area.AxisX.Minimum = 0;
                area.AxisX.Maximum = maxTime;
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisX.MinorGrid.Enabled = false;

                area.AxisY.Title = "Level";
                area.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
                area.AxisY.MajorGrid.Enabled = false;
                area.AxisY.MinorGrid.Enabled = false;
            }
        }

        private void framesTable_SelectionChangedEvent(object sender, FramesTable.SelectionChangedArgs e)
        {
            UpdateSelected(e);
        }
    }
}
