using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace IRCD
{
    public class Tools
    {
        public static T CreateDeepCopy<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(ms);
            }
        }

        public static void CheckWindowSize(Form form)
        {
            var screenBounds = Screen.FromControl(form).Bounds;
            int maxWidth = Convert.ToInt32(screenBounds.Width * 0.95);
            int maxHeight = Convert.ToInt32(screenBounds.Height * 0.95);
            if (form.Width > maxWidth)
                form.Width = maxWidth;
            if (form.Height > maxHeight)
                form.Height = maxHeight;
        }
    }
}
