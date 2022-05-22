using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IRCD
{
    public partial class FramesTable : UserControl
    {
        public FramesTable()
        {
            InitializeComponent();
        }

        public void StartMonitoring()
        {
            SerialConnector.Instance().FrameReceivedEvent += ConnectorFrameReceivedEvent;
        }

        public void Clear()
        {
            lvItems.Items.Clear();
            SelectionChangedEvent?.Invoke(this, new SelectionChangedArgs(new List<FrameTableItem>()));
        }

        private void ConnectorFrameReceivedEvent(object sender, SerialConnector.FrameReceivedArgs e)
        {
            AddToReceivedFrames(e.Signature);
        }

        int nextId = 0;

        public class FrameTableItem
        {
            public FrameTableItem(string name, List<double> signature)
            {
                Name = name;
                Signature = signature;
            }
            public string Name { get; set; }
            public List<double> Signature { get; set; }
        }

        public class SelectionChangedArgs
        {
            public SelectionChangedArgs(List<FrameTableItem> items)
            {
                Items = items;
            }
            public List<FrameTableItem> Items { get; }
        }
        public delegate void SelectionChangedHandler(object sender, SelectionChangedArgs e);
        public event SelectionChangedHandler SelectionChangedEvent;

        public void AddToReceivedFrames(List<double> signature)
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

        private void lvItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<FrameTableItem> items = new List<FrameTableItem>();
            foreach (var item in lvItems.SelectedItems)
            {
                ListViewItem lvItem = item as ListViewItem;
                items.Add(new FrameTableItem(lvItem.Text, lvItem.Tag as List<double>));
            }
            SelectionChangedEvent?.Invoke(this, new SelectionChangedArgs(items));
        }
    }
}
