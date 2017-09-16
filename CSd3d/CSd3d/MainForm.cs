using MelloRin.CSd3d.Lib;
using MelloRin.FileManager;
using SharpDX.Windows;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MelloRin.CSd3d
{
	class MainForm : RenderForm, Itask
	{
		public MainForm()
		{
			windowsize_adjust();

			FormClosed += new FormClosedEventHandler(_formClosed);
			KeyDown += new KeyEventHandler(_keyDown);
			MaximizeBox = false;
			Icon = null;
			Text = "Test";
		}

		public void run()
		{
			Thread _TmainThread = new Thread(() =>
			{
				Show();
				while (Created)
				{
					Application.DoEvents();
					Thread.Sleep(2);
				}
			});

			_TmainThread.Start();

			PublicData_manager.currentTaskQueue.runNext();
		}

		public void windowsize_adjust()
		{
			AutoScaleDimensions = new SizeF(7F, 12F);
			AutoScaleMode = AutoScaleMode.Font;
			FormBorderStyle = FormBorderStyle.FixedSingle;

			uint width, height;
			if (!UInt32.TryParse(PublicData_manager.settings.get_setting("width"), out width)
			|| !UInt32.TryParse(PublicData_manager.settings.get_setting("height"), out height))
			{
				width = 1280;
				height = 720;
			}
			ClientSize = new Size((int)width, (int)height);
		}


		private void _keyDown(object sender, KeyEventArgs e)
		{
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
		}

		private void _formClosed(object sender, FormClosedEventArgs e)
		{
			DataSet dataSet = new DataSet();

			dataSet.adddata("Display", PublicData_manager.settings.getDisplaytable());
			dataSet.adddata("Input", PublicData_manager.settings.getKeytable());
			dataSet.adddata("Score", PublicData_manager.score.getScoretable());

			File_manager.save_data(dataSet);
			PublicData_manager.device_created = false;

			Dispose(true);
		}
	}
}