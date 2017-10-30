using MelloRin.CSd3d.Lib;
using NAudio.Wave;
using SharpDX;
using SharpDX.XInput;
using System;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace MelloRin.CSd3d
{
	class Game : ITask
	{
		static public bool gameRunning { get; private set; }
		private RenderTaskerHandler drawer;
		private Timer timer;

		private Timer noteEffectTimer;
		private int noteEffectRunningFrame = 0;

		private int sec;

		Controller controller = new Controller(UserIndex.One);
		bool[] keyFlag = new bool[9];

		public Game(RenderTaskerHandler drawer)
		{
			this.drawer = drawer;

			drawer.font.add("gamestate", new FontData("running", drawer.font.renderTarget, Color.YellowGreen, 100, 100, 30));

			timer = new Timer(1000);
			timer.Elapsed += _Etimer;

			noteEffectTimer = new Timer
			{
				Interval = 20
			};
			noteEffectTimer.Elapsed += _Eeffect;
		}

		private void _Etimer(object sender, ElapsedEventArgs e)
		{
			Console.WriteLine("{0}sec elapsed...", ++sec);

			if (sec >= 2000)
			{
				gameRunning = false;
				Console.WriteLine("Timer Stoped");

				drawer.font.delete("gamestate");
				timer.Stop();
			}
		}

		private void _Eeffect(object sender,ElapsedEventArgs e)
		{
			switch(noteEffectRunningFrame)
			{
				case 0:
					drawer.sprite.modPoint("effect1", 400, 480);
					break;
				case 1:
					drawer.sprite.modPoint("effect2", 400, 480);
					drawer.sprite.modPoint("effect1", 1280,	720);
					break;
				case 2:
					drawer.sprite.modPoint("effect3", 400, 480);
					drawer.sprite.modPoint("effect2", 1280, 720);
					break;
				case 3:
					drawer.sprite.modPoint("effect4", 400, 480);
					drawer.sprite.modPoint("effect3", 1280, 720);
					break;
				case 4:
					drawer.sprite.modPoint("effect5", 400, 480);
					drawer.sprite.modPoint("effect4", 1280, 720);
					break;
				case 5:
					drawer.sprite.modPoint("effect6", 400, 480);
					drawer.sprite.modPoint("effect5", 1280, 720);
					break;
				case 6:
					drawer.sprite.modPoint("effect7", 400, 480);
					drawer.sprite.modPoint("effect6", 1280, 720);
					break;
				case 7:
					drawer.sprite.modPoint("effect7", 1280, 720);
					noteEffectRunningFrame = -1;
					noteEffectTimer.Stop();
					break;
			}
			++noteEffectRunningFrame;
		}

		public void run(TaskQueue taskQueue)
		{
			sec = 0;
			gameRunning = true;

			drawer.font.add("nowTime", new FontData("Current Time " + DateTime.Now.ToString(), drawer.font.renderTarget, Color.Red, 0, 32));

			drawer.sprite.add("note", new SpriteData(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "n3.png"), 400, 0, 1));
			//effect point = 400,480

			for(int i = 1; i <=7; ++i)
			{
				drawer.sprite.add("effect" + i, new SpriteData(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, String.Format("effect{0}.png",i)), 1280, 720, 2));
			}
		
			timer.Start();


			Thread _Tgame = new Thread(() =>
			{
				while (gameRunning)
				{
					if (controller.IsConnected)
					{
						keyProcss(controller.GetState().Gamepad);
					}

					int noteY = D2DSprite._LSprite["note"].y;

					if (noteY >= 530)
					{
						noteY = 0;
						//noteEffectTimer.Start();
					}

					drawer.sprite.modPoint("note", 400, noteY + 1);

					if (!PublicData_manager.device_created)
					{
						gameRunning = false;
					}
					Thread.Sleep(1);
				}
			});
			_Tgame.Start();

			Thread _Tmusic = new Thread(() =>
			{
				string musicName = "aac.aac";

				string musicPath = String.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), musicName);
				FileInfo fileInfo = new FileInfo(musicPath);

				if (fileInfo.Exists)
				{
					WaveOut wavePlayer = new WaveOut();
					AudioFileReader reader = new AudioFileReader(musicPath);

					Console.WriteLine("Total Play Time): " + reader.TotalTime);
					wavePlayer.Init(reader);
					reader.Volume = 0.3f;
					reader.CurrentTime = new TimeSpan(0, 0, 25);
					wavePlayer.Play();

					//TimeSpan timeLimit = new TimeSpan(0, 0,10);

					while (gameRunning)
					{
						Thread.Sleep(1000);
						Console.WriteLine(reader.CurrentTime + " / " + reader.TotalTime);
					}
					wavePlayer.Stop();
				}
				else
				{
					Console.WriteLine("{0}  missing", musicPath);
				}
			});
			//_Tmusic.Start();

			taskQueue.runNext();
		}

		private void keyProcss(Gamepad pad)
		{
			if (pad.Buttons.HasFlag(GamepadButtonFlags.A) && !keyFlag[5])
			{
				keyFlag[5] = true;
				Console.WriteLine("A down");

				int noteY = D2DSprite._LSprite["note"].y;

				if (noteY <= 520 && noteY >= 450)
				{
					noteY = 0;
					noteEffectTimer.Start();
				}
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.A) && keyFlag[5])
			{
				keyFlag[5] = false;
				Console.WriteLine("A up");
			}



			/*if (pad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) && !keyFlag[8])
			{
				if (!PublicData_manager.mouseCaptureState)
					Cursor.Hide();
				else
					Cursor.Show();

				PublicData_manager.mouseCaptureState = !PublicData_manager.mouseCaptureState;

				Console.WriteLine("L 범퍼");
				keyFlag[8] = true;
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) && keyFlag[8])
			{
				keyFlag[8] = false;
			}*/
		}

		public void temp()
		{
			Random r = new Random();
			//100만×Perfect횟수, 60만×Good횟수, 10만×맥스콤보 이 3개를 모두 더한 뒤 총 노트수만큼 나누고 소수점을 버림
			for (int i = 0; i < 100; i++)
			{
				int max_note = 335;
				int good_count = r.Next(1, 100);

				double p = 1000000 * (max_note - good_count);
				double g = 600000 * good_count;
				double max = 100000 * max_note;

				uint total_score = (uint)((p + g + max) / max_note);
			}
		}
	}
}