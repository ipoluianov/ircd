using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCD
{
    public class SettingsItem
    {
        public SettingsItem()
        {
            Signature = new List<double>();
        }

        public string ButtonName { get; set; }
        public string Action { get; set; }
        public List<double> Signature { get; set; }
    }

    public class Settings
    {
        public Settings()
        {
            Items = new List<SettingsItem>();
        }
        public List<SettingsItem> Items { get; set; }
    }
}
