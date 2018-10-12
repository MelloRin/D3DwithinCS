using NAudio.Wave;
using SharpDX.XInput;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MelloRin.FileManager;

using Timer = System.Timers.Timer;


namespace NoteMaker
{
	public partial class Form1 : Form
	{
		bool[] keyFlag = new bool[9];
		private bool[] keyInputList = new bool[5];
		Controller controller = new Controller(UserIndex.One);
		StreamWriter writer = new StreamWriter("noteInfo.txt");
		AudioFileReader reader;
		bool musicRunning = false;

		StringBuilder builder = new StringBuilder();
		Stopwatch sw = new Stopwatch();

		Timer timer = new Timer();
		public Form1()
		{
			InitializeComponent();
			KeyDown += _EkeyDown;
			KeyUp += _EkeyUp;

			FormClosed += Form1_FormClosed;

			Thread _Tmusic = new Thread(() =>
			{
				string musicName = "music1.mp3";

				if (File.Exists(musicName))
				{
					WaveOut wavePlayer = new WaveOut();
					reader = new AudioFileReader(musicName);

					Console.WriteLine("Total Play Time): " + reader.TotalTime);
					wavePlayer.Init(reader);
					reader.Volume = 0.3f;
					//reader.CurrentTime = new TimeSpan(0, 0, 25);
					wavePlayer.Play();
					sw.Start();

					musicRunning = true;

					
					//TimeSpan timeLimit = new TimeSpan(0, 0,10);

					while (Created) ;
					wavePlayer.Stop();
				}
			});
			_Tmusic.Start();


			Thread _TgamePad = new Thread(() =>
			{
				while (!musicRunning) ;
				while (Created)
				{
					if (controller.IsConnected)
					{
						keyProcss(controller.GetState().Gamepad);
					}
				}
			});
			//_TgamePad.Start();
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Guid guid = Guid.NewGuid();
			string[] temp = guid.ToString().Split('-');
			string uuid = "";

			for (int i = 0; i < temp.Length; i++)
			{
				uuid += temp[i];
			}

			string rawData = "0/0/3/000\n1 / 0 / 4 / 000\n2 / 0 / 4 / 370\n3 / 0 / 4 / 730\n3 / 0 / 5 / 450\n2 / 0 / 5 / 600\n0 / 0 / 5 / 860\n1 / 0 / 6 / 100\n2 / 0 / 6 / 250\n0 / 0 / 6 / 550\n0 / 0 / 6 / 800\n2 / 0 / 7 / 270\n1 / 0 / 7 / 750\n3 / 0 / 9 / 130\n0 / 0 / 9 / 720\n2 / 0 / 10 / 140\n1 / 0 / 10 / 205\n4 / 0 / 10 / 500\n0 / 0 / 11 / 020\n4 / 0 / 11 / 500\n3 / 0 / 12 / 000\n4 / 0 / 12 / 200";
			string output = uuid + AES256_manager.encrypt(rawData, uuid);
			writer.Write(output);
			writer.Close();
		}
		

		public void noteProcess(int index)
		{
			TimeSpan temp = TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds);
			builder.AppendFormat("{0}/{1}\n", index, temp.ToString(@"m\/ss\/fff"));
		}

		private void _EkeyDown(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (input.Equals("d") && !keyInputList[0])
			{
				Console.WriteLine("1");
				keyInputList[0] = true;
				noteProcess(0);
			}
			if (input.Equals("f") && !keyInputList[1])
			{
				Console.WriteLine("2");
				keyInputList[1] = true;
				noteProcess(1);
			}
			if (input.Equals("j") && !keyInputList[2])
			{
				Console.WriteLine("3");
				keyInputList[2] = true;
				noteProcess(2);
			}
			if (input.Equals("k") && !keyInputList[3])
			{
				Console.WriteLine("4");
				keyInputList[3] = true;
				noteProcess(3);
				
			}
			if (e.KeyCode.Equals(Keys.Space) && !keyInputList[4])
			{
				Console.WriteLine("5");
				keyInputList[4] = true;
				noteProcess(4);
			}
		}

		private void _EkeyUp(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (input.Equals("d"))
			{
				keyInputList[0] = false;
			}
			if (input.Equals("f"))
			{
				keyInputList[1] = false;
			}
			if (input.Equals("j"))
			{
				keyInputList[2] = false;
			}
			if (input.Equals("k"))
			{
				keyInputList[3] = false;
			}
			
			if (e.KeyCode.Equals(Keys.Space))
			{
				keyInputList[4] = false;
			}
		}

		public void keyProcss(Gamepad pad)
		{
			if (pad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) && !keyFlag[0])
			{
				Console.WriteLine("1");
				keyFlag[0] = true;
				noteProcess(0);
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) && keyFlag[0])
			{
				keyFlag[0] = false;
			}

			if (pad.Buttons.HasFlag(GamepadButtonFlags.DPadUp) && !keyFlag[1])
			{
				Console.WriteLine("2");
				keyFlag[1] = true;
				noteProcess(1);
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.DPadUp) && keyFlag[1])
			{
				keyFlag[1] = false;
			}

			if (pad.Buttons.HasFlag(GamepadButtonFlags.Y) && !keyFlag[2])
			{
				Console.WriteLine("3");
				keyFlag[2] = true;
				noteProcess(2);
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.Y) && keyFlag[2])
			{
				keyFlag[2] = false;
			}

			if (pad.Buttons.HasFlag(GamepadButtonFlags.B) && !keyFlag[3])
			{
				Console.WriteLine("4");
				keyFlag[3] = true;
				noteProcess(3);
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.B) && keyFlag[3])
			{
				keyFlag[3] = false;
			}


			if (pad.LeftTrigger >= 128 && !keyFlag[4])
			{
				Console.WriteLine("5");
				keyFlag[4] = true;
				noteProcess(4);
			}
			if (!(pad.LeftTrigger >= 128) && keyFlag[4])
			{
				keyFlag[4] = false;
			}
		}
	}
}
