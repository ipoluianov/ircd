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
            framesTable.StartMonitoring();
            Storage.Instance().Load();
            foreach (var item in Storage.Instance().Settings().Items)
            {
                if (item.Id == ItemId)
                {
                    currentItem = item.Clone();
                    break;
                }
            }
            LoadData();
        }

        private SettingsItem currentItem = null;

        private void LoadData()
        {
            if (currentItem == null)
                return;

            txtKeyName.Text = currentItem.ButtonName;
            txtAction.Text = currentItem.Action;

            framesTableSettings.Clear();
            foreach(var sig in currentItem.Signatures)
                framesTableSettings.AddToReceivedFrames(sig.Items);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int count = 0;
            foreach (var item in framesTable.SelectedItems)
            {
                if (count == 0)
                    count = item.Signature.Count;
                else
                {
                    if (count != item.Signature.Count)
                    {
                        MessageBox.Show("Wrong pulses count");
                        return;
                    }
                }
            }
            List<double> result = new List<double>();
            for (int i = 0; i < count; i++)
                result.Add(0);
            foreach (var item in framesTable.SelectedItems)
            {
                for (int i = 0; i < count; i++)
                {
                    result[i] += item.Signature[i];
                }
            }

            for (int i = 0; i < count; i++)
                result[i] = result[i] / framesTable.SelectedItems.Count;

            Signature sig = new Signature();
            sig.Items = result;
            currentItem.Signatures.Add(sig);
            LoadData();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (framesTableSettings.SelectedItems.Count != 1)
                return;

            int indexToRemove = framesTableSettings.SelectedItems[0].Index;
            currentItem.Signatures.RemoveAt(indexToRemove);
            LoadData();
        }
    }
}
