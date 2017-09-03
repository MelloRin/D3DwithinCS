using MelloRin.FileManager;
using SharpDX.Windows;
using SharpDX.XInput;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataSet = MelloRin.FileManager.DataSet;

namespace MelloRin.CSd3d.Lib
{
	class Thread_manager
	{
		public Thread_manager()
		{
			D3D_handler device = null;
			RenderForm mainForm = null;

			Stopwatch sw = new Stopwatch();

			sw.Start();

			Thread _Td3d = new Thread(() =>
			{
				while (mainForm.Created)
				{
					device.loop();
					Thread.Sleep(1000);
				}

				device.Dispose();
			});

			Thread _TmainThread = new Thread(() =>
			{
				mainForm = new MainForm();
				device = new D3D_handler(mainForm);

				sw.Stop();
				Console.WriteLine(sw.ElapsedMilliseconds);

				Console.WriteLine(PublicData_manager.settings.get_setting("width") + "*" + PublicData_manager.settings.get_setting("height"));
				Console.WriteLine(PublicData_manager.settings.get_input_keys("up"));
				Console.WriteLine(PublicData_manager.settings.get_input_keys("down"));
				Console.WriteLine(PublicData_manager.settings.get_input_keys("left"));
				Console.WriteLine(PublicData_manager.settings.get_input_keys("right"));
				_Td3d.Start();

				Controller con = new Controller(UserIndex.One);

				while (mainForm.Created)
				{
					Application.DoEvents();
					Thread.Sleep(2);

					if (con.IsConnected)
					{
						Gamepad pad = con.GetState().Gamepad;

						if (pad.Buttons.Equals(GamepadButtonFlags.A))
						{
							Console.WriteLine("(A)");
						}
						if (pad.Buttons.Equals(GamepadButtonFlags.B))
						{
							Console.WriteLine("(B)");
						}
					}
				}
			});

			Thread _Tloaddata = new Thread(() =>
			{
				File_manager.load_data(out DataSet dataSet);

				Parallel.Invoke
				(
					() => { PublicData_manager.settings = new Setting_manager(dataSet); },
					() => { PublicData_manager.score = new Savedata_manager(dataSet); }
				);
				_TmainThread.Start();
			});
			_Tloaddata.Start();
		}
	}
}