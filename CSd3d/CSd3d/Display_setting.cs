using System.Windows.Forms;

namespace CSd3d
{
    public partial class Display_setting : Form
    {
        private MainForm parent_form = null;
        public Display_setting(MainForm mainForm)
        {
            this.parent_form = mainForm;
            InitializeComponent();
            this.comboBox1.SelectedIndex = 0;
        }

        private void apply_btn_Click(object sender, System.EventArgs e)
        {
            int selected_index = this.comboBox1.SelectedIndex;
            string selected_setting = this.comboBox1.Items[selected_index].ToString();

            PublicData_manager.settings[PublicData_manager.settings_key[0]] = selected_setting.Split('*')[0];
            PublicData_manager.settings[PublicData_manager.settings_key[1]] = selected_setting.Split('*')[1];

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
