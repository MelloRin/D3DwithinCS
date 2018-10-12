using MelloRin.CSd3d.Core;
using MelloRin.CSd3d.Lib;
using SharpDX;
using System;
using System.Windows.Forms;

namespace MelloRin.CSd3d.Scenes
{
	public class ResultScreen : ITask
	{
		private RenderTaskerHandler drawer;
		private ScoreData data;
		private string musicName;

		public ResultScreen(RenderTaskerHandler drawer, string musicName, ScoreData data)
		{
			if(PublicDataManager.deviceCreated)
			{
				drawer.targetForm.KeyDown += _EkeyDown;

				D2DFont.resetData();
				D2DSprite.resetData();
				this.drawer = drawer;
				this.data = data;
				this.musicName = musicName;

				initialize();
			}

		}


		private void _EkeyDown(object sender, KeyEventArgs e)
		{
			drawer.targetForm.KeyDown -= _EkeyDown;
			PublicDataManager.currentTaskQueue.addTask(new MusicSelect(drawer));
		}

		public void initialize()
		{
			drawer.sprite.modBackgroundImage("background", D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "resultScreen.png"));

			drawer.sprite.add("music", new SpriteData(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, musicName + ".png"), 45, 140));

			drawer.font.add("score", new FontData(String.Format("{0,7}",data.score).Replace(' ','0'), drawer.font.renderTarget, Color4.White, 780, 460, 80));
			drawer.font.add("perfect", new FontData(data.perfect.ToString(), drawer.font.renderTarget, Color4.White, 990, 230, 50));
			drawer.font.add("fail", new FontData(data.fail.ToString(), drawer.font.renderTarget, Color4.White, 990, 310, 50));
		}

		public void run(TaskQueue taskQueue)
		{
			taskQueue.runNext();
		}
	}
}
