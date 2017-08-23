using System.Windows.Forms;

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
            int selected_index = this.comboBox1.SelectedIndex;
            string selected_setting = this.comboBox1.Items[selected_index].ToString();

            string[] temp = selected_

            PublicData_manager.settings.change_setting("width", selected_setting.Split('*')[0]);
            PublicData_manager.settings.change_setting("height", selected_setting.Split('*')[1]);

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
