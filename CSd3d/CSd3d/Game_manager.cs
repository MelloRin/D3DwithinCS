using MelloRin.CSd3d.Lib;
using System;
using System.Threading;

namespace MelloRin.CSd3d
{
	class Game_manager : Itask
	{
		static public bool gameRunning { get; private set; }

		public void run()
		{
			gameRunning = true;
			Thread _Tgame = new Thread(() =>
			{
				while(gameRunning)
				{

				}
				gameRunning = false;
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
