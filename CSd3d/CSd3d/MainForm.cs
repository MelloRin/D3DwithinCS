using MelloRin.CSd3d.Lib;
using MelloRin.FileManager;
using SharpDX.Windows;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace MelloRin.CSd3d
{
	class MainForm : RenderForm, ITask
	{
		private bool[] keyInputList = new bool[Setting_manager.input_keys_key.Length];
		public MainForm()
		{
			AutoScaleDimensions = new SizeF(7F, 12F);
			AutoScaleMode = AutoScaleMode.Font;
			FormBorderStyle = FormBorderStyle.FixedSingle;

			if (!UInt32.TryParse(PublicData_manager.settings.get_setting("width"), out uint width)
			|| !UInt32.TryParse(PublicData_manager.settings.get_setting("height"), out uint height))
			{
				width = 1280;
				height = 720;
			}
			ClientSize = new Size((int)width, (int)height);

			FormClosed += new FormClosedEventHandler(_EformClosed);
			KeyDown += new KeyEventHandler(_EkeyDown);
			KeyUp += new KeyEventHandler(_EkeyUp);

			MouseClick += new MouseEventHandler((object sender, MouseEventArgs e) =>
			{
				foreach (string i in D2DSprite._LSprite.Keys)
				{
					SpriteData seekTarget = D2DSprite._LSprite[i];
					seekTarget.collisionCheck(e.X, e.Y);
				}
			});

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

		private void _EkeyDown(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (PublicData_manager.settings.input_key_search(input))
			{
				if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[0])) && !keyInputList[0])
				{
					Console.WriteLine("(UP)key DOWN");
					keyInputList[0] = true;
				}
				if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[1])) && !keyInputList[1])
				{
					Console.WriteLine("(DOWN)key DOWN");
					keyInputList[1] = true;
				}
				if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[2])) && !keyInputList[2])
				{
					Console.WriteLine("(LEFT)key DOWN");
					keyInputList[2] = true;
				}
				if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[3])) && !keyInputList[3])
				{
					Console.WriteLine("(RIGHT)key DOWN");
					keyInputList[3] = true;
				}
			}
		}

		private void _EkeyUp(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (PublicData_manager.settings.input_key_search(input))
			{
				if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[0])))
				{
					Console.WriteLine("(UP)key UP");
					keyInputList[0] = false;
				}
				if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[1])))
				{
					Console.WriteLine("(DOWN)key UP");
					keyInputList[1] = false;
				}
				if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[2])))
				{
					Console.WriteLine("(LEFT)key UP");
					keyInputList[2] = false;
				}
				if (input.Equals(PublicData_manager.settings.get_input_keys(Setting_manager.input_keys_key[3])))
				{
					Console.WriteLine("(RIGHT)key UP");
					keyInputList[3] = false;
				}
			}
		}

		private void _EformClosed(object sender, FormClosedEventArgs e)
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