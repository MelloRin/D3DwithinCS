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
		private bool[] keyInputList = new bool[SettingManager.inputKeysKey.Length];
		private bool escInputState = false;

		public MainForm()
		{
			AutoScaleDimensions = new SizeF(7F, 12F);
			AutoScaleMode = AutoScaleMode.Font;
			FormBorderStyle = FormBorderStyle.FixedSingle;

			if (!UInt32.TryParse(PublicDataManager.settings.getSetting("width"), out uint width)
			|| !UInt32.TryParse(PublicDataManager.settings.getSetting("height"), out uint height))
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
				Console.WriteLine("{0},{1}", e.X, e.Y);
				foreach (string i in D2DSprite._LClickableSprite.Keys)
				{
					ClickableSprite seekTarget = D2DSprite._LClickableSprite[i];
					seekTarget.pointCheck(e.X, e.Y);
				}
			});

			MaximizeBox = false;
			Icon = null;
			Text = "BEATmaker";
		}

		public void run(TaskQueue taskQueue)
		{
			Thread _TmainThread = new Thread(() =>
			{
				Show();

				Application.DoEvents();
				//Cursor = new Cursor(Cursor.Current.Handle);
				/*Cursor.Hide();
				Point center = new Point(DesktopLocation.X + Width / 2, (DesktopLocation.Y + 32) + Height / 2);*/

				while (Created)
				{
					Application.DoEvents();

					/*if (Cursor.Position != center && PublicDataManager.mouseCaptureState && Focused)
					{
						Cursor.Position = center;
					}*/

					Thread.Sleep(2);
				}
			});

			_TmainThread.Start();
			taskQueue.runNext();
		}

		private void _EkeyDown(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (e.KeyCode == Keys.Escape && !escInputState)
			{
				escInputState = true;

				if (!PublicDataManager.mouseCaptureState)
					Cursor.Hide();
				else
					Cursor.Show();

				PublicDataManager.mouseCaptureState = !PublicDataManager.mouseCaptureState;
			}


			if (PublicDataManager.settings.inputKeySearch(input))
			{
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[0])) && !keyInputList[0])
				{
					Console.WriteLine("(UP)key DOWN");
					keyInputList[0] = true;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[1])) && !keyInputList[1])
				{
					Console.WriteLine("(DOWN)key DOWN");
					keyInputList[1] = true;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[2])) && !keyInputList[2])
				{
					Console.WriteLine("(LEFT)key DOWN");
					keyInputList[2] = true;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[3])) && !keyInputList[3])
				{
					Console.WriteLine("(RIGHT)key DOWN");
					keyInputList[3] = true;
				}
			}
		}

		private void _EkeyUp(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (e.KeyCode == Keys.Escape && escInputState)
			{
				escInputState = false;
			}

			if (PublicDataManager.settings.inputKeySearch(input))
			{
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[0])))
				{
					Console.WriteLine("(UP)key UP");
					keyInputList[0] = false;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[1])))
				{
					Console.WriteLine("(DOWN)key UP");
					keyInputList[1] = false;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[2])))
				{
					Console.WriteLine("(LEFT)key UP");
					keyInputList[2] = false;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[3])))
				{
					Console.WriteLine("(RIGHT)key UP");
					keyInputList[3] = false;
				}
			}
		}

		private void _EformClosed(object sender, FormClosedEventArgs e)
		{
			SaveFileDataSet dataSet = new SaveFileDataSet();

			dataSet.addData("Display", PublicDataManager.settings.getDisplaytable());
			dataSet.addData("Input", PublicDataManager.settings.getKeytable());
			dataSet.addData("Score", PublicDataManager.score.getScoretable());

			FileManager.FileManager.saveData(dataSet);
			PublicDataManager.deviceCreated = false;

			Dispose(true);
		}
	}
}