using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IRCD
{
    public class Storage
    {
        static Storage instance = null;
        private static object locker = new object();

        Settings settings_ = new Settings();

        protected Storage()
        {
        }

        public static Storage Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new Storage();
                    }
                }
            }
            return instance;
        }

        public void Load()
        {
            string xml = "";
            try
            {
                xml = File.ReadAllText("d:\\ir.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (var sww = new StringReader(xml))
                {
                    using (XmlReader reader = XmlReader.Create(sww))
                    {
                        settings_ = serializer.Deserialize(reader) as Settings;
                    }
                }
            }
            catch
            {
            }
        }

        public void Save()
        {
            try
            {
                string xml = "";
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww, new XmlWriterSettings { Indent = true }))
                    {
                        serializer.Serialize(writer, settings_);
                        xml = sww.ToString();
                        File.WriteAllText("d:\\ir.xml", xml);
                    }
                }
            }
            catch
            {
            }
        }

        public Settings Settings()
        {
            return settings_;
        }
        
    }
}
