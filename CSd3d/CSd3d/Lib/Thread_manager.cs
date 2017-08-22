using System;
using System.Threading;
using System.Windows.Forms;

namespace CSd3d
{
    class Thread_manager
    {
        public Thread_manager()
        {
            MainForm mainForm = new MainForm();
            D3D_handler d3dHandler = new D3D_handler();

            Thread _Td3d = new Thread(new ThreadStart(() =>
            {
                while (mainForm.Created)
                {
                    d3dHandler.loop();
                    Thread.Sleep(PublicData_manager.render_Delay);
                }
            }));

            Thread _TmainThread = new Thread(new ThreadStart(() =>
            {
                File_manager filemanager = new File_manager();
                filemanager.read_settings();
                if (d3dHandler.InitallizeApplication(mainForm))
                {
                    mainForm.windowsize_adjust();
                    mainForm.Show();

                    Console.WriteLine(PublicData_manager.settings[PublicData_manager.settings_key[3]]);
                    Console.WriteLine(PublicData_manager.settings[PublicData_manager.settings_key[4]]);
                    Console.WriteLine(PublicData_manager.settings[PublicData_manager.settings_key[5]]);
                    Console.WriteLine(PublicData_manager.settings[PublicData_manager.settings_key[6]]);
                    Console.WriteLine(PublicData_manager.settings[PublicData_manager.settings_key[0]]);
                    Console.WriteLine(PublicData_manager.settings[PublicData_manager.settings_key[1]]);
                    
                    _Td3d.Start();
                }
                else
                    MessageBox.Show("초기화 에러");

                while (mainForm.Created)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }));
            _TmainThread.Start();
        }
    }
}
