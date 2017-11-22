using MelloRin.CSd3d.Lib;
using NAudio.Wave;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.XInput;
using System;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace MelloRin.CSd3d.Scenes
{
	class Game : ITask
	{
		static public bool gameRunning { get; private set; }

		private RenderTaskerHandler drawer;
		private BitmapBrush[] _LEffectSprite = new BitmapBrush[8];
		private BitmapBrush[] _LScoreSprite = new BitmapBrush[10];

		private Timer timer;

		private Timer noteEffectTimer;
		private int noteEffectRunningFrame = 0;

		private int sec;

		private int noteCount = 0;

		private int totalNote = 30;
		private int perfect = 0;
		private int good = 0;
		private int maxCombo = 0;
		private int noteY;

		Random r = new Random();

		Controller controller = new Controller(UserIndex.One);
		bool[] keyFlag = new bool[9];

		public Game(RenderTaskerHandler drawer)
		{
			this.drawer = drawer;

			drawer.font.add("gamestate", new FontData("running", drawer.font.renderTarget, Color.YellowGreen, 100, 100, 30));

			timer = new Timer(1000d);
			timer.Elapsed += _Etimer;


			noteEffectTimer = new Timer(30d);

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

		private void _Eeffect(object sender, ElapsedEventArgs e)
		{
			drawer.sprite.modImage("line1", _LEffectSprite[noteEffectRunningFrame]);

			if (++noteEffectRunningFrame == _LEffectSprite.Length)
			{
				noteEffectRunningFrame = 0;
				noteEffectTimer.Stop();
			}
		}

		public void run(TaskQueue taskQueue)
		{
			sec = 0;
			gameRunning = true;

			loadImage();
			timer.Start();

			drawer.targetForm.KeyDown += _EkeyDown;
			drawer.targetForm.KeyUp += _EkeyUp;

			Thread _Tgame = new Thread(() =>
			{
				while (gameRunning)
				{
					if (controller.IsConnected)
					{
						keyProcss(controller.GetState().Gamepad);
					}

					if(noteCount < totalNote)
					{
						noteY = D2DSprite._LSprite["note"].y;

						if (noteY >= 550)
						{
							noteY = 0;
							maxCombo = 0;
							++noteCount;
							Console.WriteLine("fail {0}",noteCount);
							//noteEffectTimer.Start();
						}

						drawer.sprite.modPoint("note", 400, noteY + 1);
					}
					else
					{
						drawer.sprite.modPoint("note", 1280, 720);
					}


					if (!PublicDataManager.deviceCreated)
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

				string musicPath = Program.musicFileDir + musicName;

				if (File.Exists(musicPath))
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

		private void loadImage()
		{
			drawer.font.add("nowTime", new FontData("Current Time " + DateTime.Now.ToString(), drawer.font.renderTarget, Color.Red, 0, 32));

			drawer.sprite.add("note", new SpriteData(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "note-b.png"), 400, 0));

			drawer.sprite.add("line1", new SpriteData(null, 400, 480));

			//effect point = 400,480

			for (int i = 1; i <= 7; ++i)
			{
				_LEffectSprite[i - 1] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, String.Format("effect-{0}.png", i));
			}
			_LEffectSprite[_LEffectSprite.Length - 1] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, null, true);

			for (int i = 0; i <= 9; ++i)
			{
				_LScoreSprite[i] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, String.Format("score-{0}.png", i));
			}

			drawer.font.add("scoreTable", new FontData("0000000", drawer.font.renderTarget, Color.White, 1000, 0, 70,"applemint"));
			/*for (int i = 1; i <= 7; ++i)
			{
				drawer.sprite.add(String.Format("score{0}", i), new SpriteData(_LScoreSprite[0], 1280 - (i * 55), 0));
			}*/
		}

		private void keyProcss(Gamepad pad)
		{
			if (pad.Buttons.HasFlag(GamepadButtonFlags.A) && !keyFlag[5])
			{
				keyFlag[5] = true;
				Console.WriteLine("A down");

				if (noteY <= 525 && noteY >= 450)
				{
					Console.WriteLine("Perfect {0}", noteCount);
					drawer.sprite.modPoint("note", 400, 1);
					++noteCount;
					++perfect;
					++maxCombo;

					scoreCalc();
					noteEffectTimer.Start();
				}
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.A) && keyFlag[5])
			{
				keyFlag[5] = false;
				Console.WriteLine("A up");
			}

			if (pad.LeftTrigger >= 128 && !keyFlag[8])
			{
				keyFlag[8] = true;
				Console.WriteLine("UP Tilt");

				if (noteY <= 525 && noteY >= 450)
				{
					Console.WriteLine("Perfect {0}", noteCount);
					drawer.sprite.modPoint("note", 400, 1);
					++noteCount;
					++perfect;
					++maxCombo;

					scoreCalc();
					noteEffectTimer.Start();
				}
			}
			if (!(pad.LeftTrigger >= 128) && keyFlag[8])
			{
				keyFlag[8] = false;
			}
		}

		private bool[] keyInputList = new bool[SettingManager.inputKeysKey.Length];
		private void _EkeyDown(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (PublicDataManager.settings.inputKeySearch(input))
			{
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[0])) && !keyInputList[0])
				{
					keyInputList[0] = true;
					if (noteY <= 525 && noteY >= 450)
					{
						Console.WriteLine("Perfect {0}", noteCount);
						drawer.sprite.modPoint("note", 400, 1);
						++noteCount;
						++perfect;
						++maxCombo;

						scoreCalc();
						noteEffectTimer.Start();
					}
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[1])) && !keyInputList[1])
				{
					keyInputList[1] = true;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[2])) && !keyInputList[2])
				{
					keyInputList[2] = true;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[3])) && !keyInputList[3])
				{
					keyInputList[3] = true;
				}
			}
		}

		private void _EkeyUp(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (PublicDataManager.settings.inputKeySearch(input))
			{
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[0])))
				{
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

		private void reset()
		{
			perfect = 0;
			good = 0;
			maxCombo = 0;
			noteCount = 0;
			scoreCalc();
		}

		//100만×Perfect횟수, 60만×Good횟수, 10만×맥스콤보 이 3개를 모두 더한 뒤 총 노트수만큼 나누고 소수점을 버림
		private void scoreCalc()
		{
			double p = 1000000 * perfect;
			double g = 600000 * good;
			double max = 100000 * maxCombo;

			updateScore((uint)((p + g + max) / totalNote));
		}

		object scoreLocker = new object();
		private void updateScore(uint score)
		{
			lock(scoreLocker)
			{
				drawer.font.modString("scoreTable", String.Format("{0,7}", score).Replace(" ", "0")); 
				/*string Sscore = String.Format("{0,7}", score);
				Sscore = Sscore.Replace(" ", "0");

				for (int i = 0; i < Sscore.Length; ++i)
				{
					drawer.sprite.modImage(String.Format("score{0}", (Sscore.Length) - i), _LScoreSprite[Int32.Parse(Sscore[i].ToString())]);
				}*/
			}
		}
	}
}