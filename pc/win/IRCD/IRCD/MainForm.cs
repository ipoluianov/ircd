using System;
using System.Windows.Forms;

namespace IRCD
{
    public partial class MainForm : Form
    {
        SerialConnector connector_;
        SerialConnector.FrameReceivedArgs lastEvent;

        public MainForm(SerialConnector connector)
        {
            connector_ = connector;
            connector_.FrameReceivedEvent += ConnectorFrameReceivedEvent;
            InitializeComponent();
        }

        private void ConnectorFrameReceivedEvent(object sender, SerialConnector.FrameReceivedArgs e)
        {
            lastEvent = e;
        }

        private void LoadItems()
        {
            Storage.Instance().Load();
            lvActions.Items.Clear();
            foreach (var item in Storage.Instance().Settings().Items)
            {
                var lvItem = lvActions.Items.Add(item.ButtonName);
                lvItem.SubItems.Add(item.Action);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lastEvent == null)
                return;
            Storage.Instance().Load();
            SettingsItem item = new SettingsItem();
            item.Action = DateTime.Now.ToString("HH:mm:ss.fff");
            item.Signature = lastEvent.Signature;
            Storage.Instance().Settings().Items.Add(item);
            Storage.Instance().Save();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadItems();
        }
    }
}
