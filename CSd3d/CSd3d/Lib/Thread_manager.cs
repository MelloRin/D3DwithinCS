﻿using System;
using System.Threading;
using System.Windows.Forms;

using FileManager;

namespace CSd3d.Lib
{
    class Thread_manager
    {
        public Thread_manager()
        {
            MainForm mainForm = null;// = new MainForm();
            D3D_handler d3dHandler = null;//new D3D_handler();

            Thread _Td3d = new Thread(() =>
            {
                while (mainForm.Created)
                {
                    d3dHandler.loop();
                    Thread.Sleep(2);
                }
            });

            /*Thread _Tsetdata = new Thread(() =>
            {
                if (File_manager.load_data(out PublicData_manager.dataSet))
                {
                    new Thread(() =>
                    {
                        PublicData_manager.settings.setSetting(PublicData_manager.dataSet);
                    }).Start();

                    new Thread(() =>
                    {
                        PublicData_manager.score.setScore(PublicData_manager.dataSet);
                    }).Start();
                }
            });*/

            Thread _TmainThread = new Thread(() =>
            {

                if (File_manager.load_data(out PublicData_manager.dataSet))
                {
                    //_Tsetdata.Start();
                    
                    PublicData_manager.settings.setSetting(PublicData_manager.dataSet);
                    PublicData_manager.score.setScore(PublicData_manager.dataSet);

                    mainForm = new MainForm();
                    d3dHandler = new D3D_handler();
                }
                if (d3dHandler.InitallizeApplication(mainForm))
                {
                    mainForm.windowsize_adjust();
                    mainForm.Show();

                    Console.WriteLine(PublicData_manager.settings.get_setting("width") + "*" + PublicData_manager.settings.get_setting("height"));
                    Console.WriteLine(PublicData_manager.settings.get_input_keys("up"));
                    Console.WriteLine(PublicData_manager.settings.get_input_keys("down"));
                    Console.WriteLine(PublicData_manager.settings.get_input_keys("left"));
                    Console.WriteLine(PublicData_manager.settings.get_input_keys("right"));
                    _Td3d.Start();
                }
                else
                    MessageBox.Show("초기화 에러");

                while (mainForm.Created)
                {
                    Application.DoEvents();
                    Thread.Sleep(2);
                }
            });

            /*if (File_manager.load_data(out PublicData_manager.dataSet))
            {
                _Tsetdata.Start();
                
            }*/
            _TmainThread.Start();
        }
    }
}