using IRCD.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRCD
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyCustomApplicationContext());
            //Application.Run(new DebugForm());
        }

        public class MyCustomApplicationContext : ApplicationContext
        {
            private NotifyIcon trayIcon;
            private MainForm mainForm;
            private DebugForm debugForm;
            private SerialConnector connector = new SerialConnector();

            public MyCustomApplicationContext()
            {
                List<MenuItem> menuItems = new List<MenuItem>();
                menuItems.Add(new MenuItem("Settings", OpenSettings));
                menuItems.Add(new MenuItem("Debug", OpenDebug));
                menuItems.Add(new MenuItem("Exit", Exit));
                trayIcon = new NotifyIcon();
                trayIcon.Icon = Resources.icon;
                trayIcon.ContextMenu = new ContextMenu(menuItems.ToArray());
                trayIcon.Visible = true;
            }

            void OpenSettings(object sender, EventArgs e)
            {
                if (mainForm != null)
                {
                    mainForm.Activate();
                    return;
                }
                mainForm = new MainForm(connector);
                mainForm.FormClosed += Form_FormClosed;
                mainForm.ShowDialog();
            }

            void OpenDebug(object sender, EventArgs e)
            {
                if (debugForm != null)
                {
                    debugForm.Activate();
                    return;
                }
                debugForm = new DebugForm(connector);
                debugForm.FormClosed += DebugForm_FormClosed;
                debugForm.ShowDialog();
            }

            private void DebugForm_FormClosed(object sender, FormClosedEventArgs e)
            {
                debugForm = null;
            }

            private void Form_FormClosed(object sender, FormClosedEventArgs e)
            {
                mainForm = null;
            }

            void Exit(object sender, EventArgs e)
            {
                trayIcon.Visible = false;
                Application.Exit();
            }
        }
    }
}
