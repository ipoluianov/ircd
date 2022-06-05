using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IRCD
{
    public class Signature
    {
        public Signature()
        {
            Items = new List<double>();
        }
        public List<double> Items { get; set; }
    }

    public class SettingsItem
    {
        public SettingsItem()
        {
            Signatures = new List<Signature>();
            SignaturesRepetitions = new List<Signature>();
        }
   
        public int Id { get; set; }
        public string ButtonName { get; set; }
        public string Action { get; set; }
        public List<Signature> Signatures { get; set; }
        public List<Signature> SignaturesRepetitions { get; set; }

        public SettingsItem Clone()
        {
            SettingsItem item = Tools.CreateDeepCopy<SettingsItem>(this);
            return item;
        }
    }

    public class Settings
    {
        public Settings()
        {
            Items = new List<SettingsItem>();
        }
        public List<SettingsItem> Items { get; set; }

        public int generateId()
        {
            int id = 1;
            foreach(var item in Items)
            {
                if (item.Id > id)
                {
                    id = item.Id;
                }
            }
            return id + 1;
        }
    }
}
