using CSd3d.Lib;
using FileManager;
using System;
using System.Diagnostics;
using System.Windows.Forms;


namespace CSd3d
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            KeyDown += new KeyEventHandler((object sender, KeyEventArgs e) =>
            {
                Stopwatch sw = new Stopwatch();

                sw.Start();
                string input = e.KeyCode.ToString().ToLower();

                if (PublicData_manager.settings.input_key_search(input))
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
                sw.Stop();

                Console.WriteLine("{0}ms ", sw.ElapsedMilliseconds);
            });
            KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) =>
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

            FormClosed += new FormClosedEventHandler((object sender, FormClosedEventArgs e) =>
            {
                DataSet dataSet = new DataSet();

                dataSet.adddata("Display", PublicData_manager.settings.getDisplaytable());
                dataSet.adddata("Input", PublicData_manager.settings.getKeytable());
                dataSet.adddata("Score", PublicData_manager.score.getScoretable());

                File_manager.save_data(dataSet);
            });

            디스플레이ToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
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

/*
try
                {
                    PublicData_manager.dataSet.getdata("Display") = PublicData_manager.settings.getDisplaytable();
                }
                catch (KeyNotFoundException)
                {
                    Data tempData = new Data();
                    tempData.hashtable = PublicData_manager.settings.getDisplaytable();

                    PublicData_manager.dataSet.adddata("Display", tempData);
                }

                try
                {
                    PublicData_manager.dataSet.getdata("Input").hashtable = PublicData_manager.settings.getKeytable();
                }
                catch (KeyNotFoundException)
                {
                    Data tempData = new Data();
                    tempData.hashtable = PublicData_manager.settings.getKeytable();

                    PublicData_manager.dataSet.adddata("Input", tempData);
                }
                try
                {
                    PublicData_manager.dataSet.getdata("Score").hashtable = PublicData_manager.score.getScoretable();
                }
                catch (KeyNotFoundException)
                {
                    Data tempData = new Data();
                    tempData.hashtable = PublicData_manager.score.getScoretable();

                    PublicData_manager.dataSet.adddata("Score", tempData);
                } 
*/
