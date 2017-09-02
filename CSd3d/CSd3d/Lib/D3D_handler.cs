using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;

using Font = Microsoft.DirectX.Direct3D.Font;

namespace CSd3d.Lib
{
	class D3D_handler : IDisposable
	{
		private MainForm mainForm;
		private Device device;

		private Font scoreBar;
		private Font gameState;

		private bool b_up = true;
		private int B = 1;

		public bool InitallizeApplication(MainForm mainForm)
		{
			this.mainForm = mainForm;

			createDevice(mainForm);

			try { createFont(); } catch (ArgumentNullException) { Console.WriteLine("스코어바 생성중 오류"); }

			return true;
		}

		private void createDevice(MainForm mainForm)
		{
			PresentParameters _pp = new PresentParameters();
			_pp.Windowed = Boolean.Parse(PublicData_manager.settings.get_setting("windowded"));
			_pp.SwapEffect = SwapEffect.Discard;

			try //hw렌더링
			{
				device = new Device(0, DeviceType.Hardware, mainForm.Handle,
					CreateFlags.HardwareVertexProcessing, _pp);
				PublicData_manager.device_created = true;
				Console.WriteLine("HW렌더링");

			}
			catch (DirectXException)
			{
				try
				{
					device = new Device(0, DeviceType.Reference, mainForm.Handle,
					CreateFlags.SoftwareVertexProcessing, _pp);
					PublicData_manager.device_created = true;
					Console.WriteLine("SW렌더링");
				}
				catch (DirectXException) { }
			}
		}

		private void createFont()
		{
			try
			{
				FontDescription fd = new FontDescription();
				fd.Height = 24;
				fd.FaceName = "MS고딕";

				scoreBar = new Font(device, fd);
			}
			catch (DirectXException)
			{
				scoreBar = null;
			}

			try
			{
				FontDescription fd = new FontDescription();
				fd.Height = 10;
				fd.FaceName = "MS고딕";

				gameState = new Font(device, fd);
			}
			catch (DirectXException)
			{
				gameState = null;
			}
		}

		public void loop()
		{
			if (PublicData_manager.device_created)
			{
				try
				{
					background_Render();
					//Console.WriteLine("{0},{1},{2},{3}", color.A, color.R, color.G, color.B);
					device.BeginScene();

					draw_Text();

					device.EndScene();
					if (device != null)
					{
						device.Present();
					}
				}
				catch (DirectXException e)
				{
					Console.WriteLine("D3D 에러" + e.ToString());
					return;
				}
			}
		}

		private void background_Render()
		{
			Color color = Color.FromArgb(0, 0, 0, B);

			//device.Clear(ClearFlags.Target, Color.White , 1.0f, 0);
			if (b_up)
			{
				device.Clear(ClearFlags.Target, color, 1.0f, 0);

				if (B < 255)
					B += 2;
				else
				{
					B = 255;
					b_up = false;
				}
			}
			else
			{
				device.Clear(ClearFlags.Target, color, 1.0f, 0);

				if (B >= 2)
					B -= 2;
				else
				{
					B = 1;
					b_up = true;
				}
			}
		}

		private void draw_Text()
		{
			scoreBar.DrawText(null, "gameState = " + PublicData_manager.game_started, 0, 0, Color.White);
		}

		public void Dispose()
		{
			if (device != null)
				device.Dispose();
			if (scoreBar != null)
				scoreBar.Dispose();
			if (gameState != null)
				gameState.Dispose();
		}
	}
}