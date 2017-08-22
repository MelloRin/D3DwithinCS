using System;
using System.Windows.Forms;

namespace CSd3d
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.KeyDown += new System.Windows.Forms.KeyEventHandler((object sender, KeyEventArgs e) =>
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
            this.KeyUp += new System.Windows.Forms.KeyEventHandler((object sender, KeyEventArgs e) =>
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
        }
        /*
        private void pause_btn(object sender, EventArgs e)
        {
            //Console.WriteLine(sender.GetType().ToString());
            if (!Flagmanager.render_paused)
            {
                Flagmanager.render_paused = true;
                ((Button)sender).Text = "시작";
            }
            else
            {
                Flagmanager.render_paused = false;
                ((Button)sender).Text = "정지";
            }
        }*/
    }
}
