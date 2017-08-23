using System;
using System.Windows.Forms;

using CSd3d.Lib;

namespace CSd3d
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler((object sender, KeyEventArgs e) =>
            {
                string input = e.KeyCode.ToString().ToLower();

                if (input.Equals(PublicData_manager.settings[PublicData_manager.settings_key[3]]))
                    Console.WriteLine("(UP)key DOWN");
                else if (input.Equals(PublicData_manager.settings[PublicData_manager.settings_key[4]]))
                    Console.WriteLine("(DOWN)key DOWN");
                else if (input.Equals(PublicData_manager.settings[PublicData_manager.settings_key[5]]))
                    Console.WriteLine("(LEFT)key DOWN");
                else if (input.Equals(PublicData_manager.settings[PublicData_manager.settings_key[6]]))
                    Console.WriteLine("(RIGHT)key DOWN");
            });
            this.KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) =>
            {
                string input = e.KeyCode.ToString().ToLower();

                if (input.Equals(PublicData_manager.settings[PublicData_manager.settings_key[3]]))
                    Console.WriteLine("(UP)key UP");
                else if (input.Equals(PublicData_manager.settings[PublicData_manager.settings_key[4]]))
                    Console.WriteLine("(DOWN)key UP");
                else if (input.Equals(PublicData_manager.settings[PublicData_manager.settings_key[5]]))
                    Console.WriteLine("(LEFT)key UP");
                else if (input.Equals(PublicData_manager.settings[PublicData_manager.settings_key[6]]))
                    Console.WriteLine("(RIGHT)key UP");
            });

            this.FormClosed += new FormClosedEventHandler((object sender, FormClosedEventArgs e) =>
            {
                File_manager file_Manager = new File_manager();
                file_Manager.save_settigs();
            });

            this.디스플레이ToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Display_setting modal = new Display_setting(this);
                modal.ShowDialog();
            });
        }
        public void windowsize_adjust()
        {
            uint width, height;
            if (!UInt32.TryParse(PublicData_manager.settings.get_setting("width"), out width)
            || !UInt32.TryParse(PublicData_manager.settings.get_setting("height"), out height))
            {
                width = 640;
                height = 480;
            }
            SetBounds(0, 0, (int)width, (int)height);
        }
    }
}
