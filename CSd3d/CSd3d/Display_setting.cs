﻿using System.Windows.Forms;

namespace CSd3d
{
    public partial class Display_setting : Form
    {
        private MainForm parent_form = null;
        public Display_setting(MainForm mainForm)
        {
            InitializeComponent();
            parent_form = mainForm;
            comboBox1.SelectedIndex = 0;
        }

        private void apply_btn_Click(object sender, System.EventArgs e)
        {
            /*int selected_index = this.comboBox1.SelectedIndex;
            string selected_setting = this.comboBox1.Items[selected_index].ToString();*/

            string[] temp = comboBox1.Items[comboBox1.SelectedIndex].ToString().Split('*');

            PublicData_manager.settings.config_setting("width", temp[0]);
            PublicData_manager.settings.config_setting("height", temp[1]);

            parent_form.Invoke(new MethodInvoker(delegate ()
            {
                parent_form.windowsize_adjust();
            }));
            Close();
        }

        private void cancel_btn_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
