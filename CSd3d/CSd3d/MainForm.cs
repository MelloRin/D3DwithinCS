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

                if(PublicData_manager.settings.input_key_search(input))
                {
                    if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[0])))
                        Console.WriteLine("(UP)key DOWN");
                    else if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[1])))
                        Console.WriteLine("(DOWN)key DOWN");
                    else if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[2])))
                        Console.WriteLine("(LEFT)key DOWN");
                    else if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[3])))
                        Console.WriteLine("(RIGHT)key DOWN");
                }
            });
            this.KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) =>
            {
                string input = e.KeyCode.ToString().ToLower();

                if (PublicData_manager.settings.input_key_search(input))
                {
                    if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[0])))
                        Console.WriteLine("(UP)key UP");
                    else if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[1])))
                        Console.WriteLine("(DOWN)key UP");
                    else if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[2])))
                        Console.WriteLine("(LEFT)key UP");
                    else if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[3])))
                        Console.WriteLine("(RIGHT)key UP");
                }
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
