using MelloRin.CSd3d.Lib;
using SharpDX.XInput;
using System.Threading;
using System.Timers;

using Timer = System.Timers.Timer;

namespace MelloRin.CSd3d.Scenes
{
	public class StartPage : ITask
	{
		private D3Dhandler drawer;
		private Timer animationTimer;

		private bool startPageRunning = true;
		private bool[] keyFlag = new bool[1];

		public StartPage(D3Dhandler drawer)
		{
			this.drawer = drawer;
			drawer.sprite.setBackground("background", new SpriteData(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "mainScreen.png"),0,-720));
		}

		public void run(TaskQueue taskQueue)
		{
			Thread _TstartPage = new Thread(() =>
			{
				Controller controller = new Controller(UserIndex.One);

				animationTimer  = new Timer(1);
				animationTimer.Elapsed += _EanimationStart;

				while (startPageRunning)
				{
					//Thread.Sleep(5000);
					animationTimer.Start();
					if (controller.IsConnected)
					{
						keyProcss(controller.GetState().Gamepad);
					}
					Thread.Sleep(1);
				}

				PublicDataManager.currentTaskQueue.addTask(new Game(drawer));
			});
			_TstartPage.Start();

			taskQueue.runNext();
		}

		private void _EanimationStart(object sender, ElapsedEventArgs e)
		{
			int currentBackgroundY = D2DSprite._LBackgroundSprite["background"].y;

			if (currentBackgroundY >= -400)
				currentBackgroundY += 7;
			else
				currentBackgroundY += 10;

			drawer.sprite.modBackgroundPoint("background", 0, currentBackgroundY);

			if(currentBackgroundY  >= 0)
			{
				D2DSprite._LBackgroundSprite["background"].y = 0;
				animationTimer.Stop();
				startPageRunning = false;
			}
		}

		private void keyProcss(Gamepad pad)
		{
			if (pad.Buttons.HasFlag(GamepadButtonFlags.Start) && !keyFlag[1])
			{
				keyFlag[1] = true;
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.Start) && keyFlag[1])
			{
				keyFlag[1] = false;
			}
		}
	}
}