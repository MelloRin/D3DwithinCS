using System;
using System.Threading;
using System.Windows.Forms;

namespace CSd3d.Lib
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
                if (d3dHandler.InitallizeApplication(mainForm))
                {
                    mainForm.windowsize_adjust();
                    mainForm.Show();

                    Console.WriteLine(PublicData_manager.settings.get_setting("width") + "*" + PublicData_manager.settings.get_setting("height"));
                    Console.WriteLine(PublicData_manager.settings.get_setting("up"));
                    Console.WriteLine(PublicData_manager.settings.get_setting("down"));
                    Console.WriteLine(PublicData_manager.settings.get_setting("left"));
                    Console.WriteLine(PublicData_manager.settings.get_setting("right"));
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