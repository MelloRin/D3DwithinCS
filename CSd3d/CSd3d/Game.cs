using MelloRin.CSd3d.Lib;
using SharpDX;
using System;
using System.Threading;
using System.Timers;

using Timer = System.Timers.Timer;

namespace MelloRin.CSd3d
{
	class Game : Itask
	{
		static public bool gameRunning { get; private set; }
		private D3Dhandler drawer;
		private Timer timer;
		private int sec;

		public Game(D3Dhandler drawer)
		{
			this.drawer = drawer;

			drawer.font.add("gamestate", new FontData("running", drawer.font.renderTarget, Color.YellowGreen,100,100,30));

			timer = new Timer
			{
				Interval = 1000
			};
			timer.Elapsed += _timerEvent;
		}

		private void _timerEvent(object sender, ElapsedEventArgs e)
		{
			Console.WriteLine("{0}sec elapsed...", ++sec);

			if (sec >= 5)
			{
				gameRunning = false;
				Console.WriteLine("Timer Stoped");

				drawer.font.delete("gamestate");
				timer.Stop();
			}
		}

		public void run()
		{
			sec = 0;
			gameRunning = true;
			timer.Start();
			Thread _Tgame = new Thread(() =>
			{
				while (gameRunning)
				{
					if (!PublicData_manager.device_created)
					{
						gameRunning = false;
					}
					Thread.Sleep(1);
				}
			});

			_Tgame.Start();
			PublicData_manager.currentTaskQueue.runNext();
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
