using MelloRin.CSd3d.Lib;
using SharpDX.XInput;
using System.Threading;
using System.Timers;

using Timer = System.Timers.Timer;

namespace MelloRin.CSd3d.Scenes
{
	public class StartPage : ITask , IControllable
	{
		private D3Dhandler drawer;
		private Timer animationTimer;

		private bool startPageRunning = true;
		private bool[] keyFlag = new bool[1];

		public StartPage(D3Dhandler drawer)
		{
			this.drawer = drawer;
			drawer.sprite.setBackground("background", new SpriteData(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "mainScreen.png"),0,0));
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
					if (controller.IsConnected)
					{
						keyProcss(controller.GetState().Gamepad);
					}
					Thread.Sleep(1);
					startPageRunning = false;
				}

				PublicDataManager.currentTaskQueue.addTask(new Game(drawer,"aac"));
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

		public void keyProcss(Gamepad pad)
		{
			if (pad.Buttons.HasFlag(GamepadButtonFlags.Start) && !keyFlag[0])
			{
				keyFlag[0] = true;
				animationTimer.Start();
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.Start) && keyFlag[0])
			{
				keyFlag[0] = false;
			}
		}
	}
}