using System;
using System.Collections.Generic;
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
            //setChart(e.Signature);
            AddToReceivedFrames(e.Signature);
        }

        private void ConnectorLogEvent(object sender, SerialConnector.LogArgs e)
        {
            Log(e.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

        private void AddToReceivedFrames(List<double> signature)
        {
            var item = lvItems.Items.Add(string.Format("Frame #{0}", nextId));
            item.Tag = signature;
            nextId++;

            int maxTimesCount = 0;
            for (int i = 0; i < lvItems.Items.Count; i++)
            {
                var selectedItem = lvItems.Items[i];
                var sign = selectedItem.Tag as List<double>;
                if (sign != null)
                {
                    if (sign.Count > maxTimesCount)
                        maxTimesCount = sign.Count;
                }
            }
            if (maxTimesCount + 1 < lvItems.Columns.Count)
            {
                lvItems.Columns.Clear();
            }
            while (lvItems.Columns.Count < maxTimesCount + 1)
            {
                var col = lvItems.Columns.Add(((lvItems.Columns.Count + 1) % 2).ToString());
                col.Width = 40;
                if (lvItems.Columns.Count == 1)
                {
                    col.Width = 70;
                }
            }

            lvItems.Columns[0].Text = "Frame #";

            for (int i = 0; i < signature.Count; i++)
                item.SubItems.Add(signature[i].ToString());

            lvItems.EnsureVisible(lvItems.Items.Count - 1);
            item.Selected = true;
        }

        private void Log(string text)
        {
            /*var item = lvItems.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            item.SubItems.Add(text);
            lvItems.EnsureVisible(lvItems.Items.Count - 1);*/
        }

        private void timerCheckPort_Tick(object sender, EventArgs e)
        {
        }

        int nextId = 0;

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvItems.Items.Clear();
            nextId = 0;
            UpdateSelected();
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

        private void UpdateSelected()
        {
            ClearCurrentItems();

            for (int i = 0; i < lvItems.SelectedItems.Count; i++)
            {
                var selectedItem = lvItems.SelectedItems[i];
                var sign = selectedItem.Tag as List<double>;
                AddCurrentItem(selectedItem.Text, sign);
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

        private void lvItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelected();
        }
    }
}
