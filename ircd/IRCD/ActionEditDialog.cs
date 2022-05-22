using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRCD
{
    public partial class ActionEditDialog : Form
    {
        public ActionEditDialog()
        {
            InitializeComponent();
            ItemId = 0;
        }

        public int ItemId { get; set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveData();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ActionEditDialog_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private SettingsItem currentItem = null;

        private void LoadData()
        {
            Storage.Instance().Load();
            foreach (var item in Storage.Instance().Settings().Items)
            {
                if (item.Id == ItemId)
                {
                    currentItem = item.Clone();
                    break;
                }
            }

            if (currentItem == null)
                return;

            txtKeyName.Text = currentItem.ButtonName;
            txtAction.Text = currentItem.Action;
        }

        private void SaveData()
        {
            Storage.Instance().Load();
            foreach (var item in Storage.Instance().Settings().Items)
            {
                if (item.Id == ItemId)
                {
                    Storage.Instance().Settings().Items.Remove(item);
                    Storage.Instance().Settings().Items.Add(currentItem);
                    break;
                }
            }

            Storage.Instance().Settings().Items.Sort(delegate (SettingsItem x, SettingsItem y)
            {
                if (x.Id < y.Id)
                    return -1;
                if (x.Id > y.Id)
                    return 1;
                return 0;
            });
            Storage.Instance().Save();
        }

        private void txtKeyName_TextChanged(object sender, EventArgs e)
        {
            if (currentItem == null)
                return;
            currentItem.ButtonName = txtKeyName.Text;
        }

        private void txtAction_TextChanged(object sender, EventArgs e)
        {
            if (currentItem == null)
                return;
            currentItem.Action = txtAction.Text;
        }
    }
}
