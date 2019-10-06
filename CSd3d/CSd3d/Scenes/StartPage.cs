using MelloRin.CSd3d.Core;
using MelloRin.CSd3d.Lib;
using SharpDX;
using SharpDX.XInput;
using System.Threading;
using System.Windows.Forms;

namespace MelloRin.CSd3d.Scenes
{
	public class StartPage : ITask , IControllable
	{
		private RenderTaskerHandler drawer;

		private bool startPageRunning = true;
		private bool[] keyFlag = new bool[1];

		public StartPage(RenderTaskerHandler drawer)
		{
			this.drawer = drawer;
			drawer.targetForm.KeyDown += _EkeyDown;

			drawer.sprite.setBackground("background", new ClickableSprite(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "mainScreen.png"),0,0,0));

			drawer.font.add("anykey", new FontData("Press Anykey", drawer.font.renderTarget, Color4.White, 460, 500, 60));
		}

		private void _EkeyDown(object sender, KeyEventArgs e)
		{
			startPageRunning = false;

			drawer.targetForm.KeyDown -= _EkeyDown;
		}

		public void run(TaskQueue taskQueue)
		{
			Thread _TstartPage = new Thread(() =>
			{
				Controller controller = new Controller(UserIndex.One);

				while (startPageRunning && drawer.targetForm.Created)
				{
					if (controller.IsConnected)
					{
						keyProcss(controller.GetState().Gamepad);
					}
					Thread.Sleep(10);
				}

				PublicDataManager.currentTaskQueue.addTask(new MusicSelect(drawer));
			});
			_TstartPage.Start();

			taskQueue.runNext();
		}

		public void keyProcss(Gamepad pad)
		{
			if (pad.Buttons.HasFlag(GamepadButtonFlags.Start) && !keyFlag[0])
			{
				keyFlag[0] = true;
				startPageRunning = false;
			}
			if (!pad.Buttons.HasFlag(GamepadButtonFlags.Start) && keyFlag[0])
			{
				keyFlag[0] = false;
			}
		}

		public void initialize()
		{

		}
	}
}