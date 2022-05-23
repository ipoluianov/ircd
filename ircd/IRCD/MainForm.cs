using System;
using System.Windows.Forms;

namespace IRCD
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Tools.CheckWindowSize(this);
        }

        private void LoadItems()
        {
            Storage.Instance().Load();
            lvActions.Items.Clear();
            foreach (var item in Storage.Instance().Settings().Items)
            {
                var lvItem = lvActions.Items.Add(item.ButtonName);
                lvItem.Tag = item.Id;
                lvItem.SubItems.Add(item.Action);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Storage.Instance().Load();
            SettingsItem item = new SettingsItem();
            item.Id = Storage.Instance().Settings().generateId();
            item.Action = DateTime.Now.ToString("HH:mm:ss.fff");
            item.ButtonName = "ButtonName";
            Storage.Instance().Settings().Items.Add(item);
            Storage.Instance().Save();
            LoadItems();
        }

        private void EditItem()
        {
            ActionEditDialog dialog = new ActionEditDialog();
            if (lvActions.SelectedItems.Count != 1)
                return;

            int itemId = (int)lvActions.SelectedItems[0].Tag;
            dialog.ItemId = itemId;
            dialog.ShowDialog();

            LoadItems();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditItem();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvActions.SelectedItems.Count != 1)
                return;

            int itemId = (int)lvActions.SelectedItems[0].Tag;

            Storage.Instance().Load();
            SettingsItem item = new SettingsItem();
            for (int i = 0; i < Storage.Instance().Settings().Items.Count; i++)
            {
                if (Storage.Instance().Settings().Items[i].Id == itemId)
                {
                    Storage.Instance().Settings().Items.RemoveAt(i);
                    break;
                }
            }

            Storage.Instance().Save();
            LoadItems();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SerialConnector.Instance().FrameReceivedEvent += MainFormFrameReceivedEvent; ;
            LoadItems();
        }

        private void MainFormFrameReceivedEvent(object sender, SerialConnector.FrameReceivedArgs e)
        {
            if (e.DetectedItem != null)
            {
                var lvItem = lvActionLog.Items.Add(e.DetectedItem.ButtonName);
                lvItem.SubItems.Add(e.DetectedItem.Action);
            }
        }

        private void UpdateButtons()
        {
            if (lvActions.SelectedItems.Count != 1)
            {
            }
            else
            {
            }
        }

        private void lvActions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvActionLog.Items.Clear();
        }

        private void lvActions_DoubleClick(object sender, EventArgs e)
        {
            EditItem();
        }
    }
}
