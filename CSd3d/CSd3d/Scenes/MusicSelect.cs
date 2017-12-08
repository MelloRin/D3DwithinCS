using MelloRin.CSd3d.Core;
using MelloRin.CSd3d.Lib;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading;

namespace MelloRin.CSd3d.Scenes
{
	class MusicSelect : ITask
	{
		private RenderTaskerHandler drawer;
		private AudioFileReader audioReader;
		private WaveOut wavePlayer;
		private bool screenRunning = true;

		public MusicSelect(RenderTaskerHandler drawer)
		{
			D2DSprite.resetData();
			this.drawer = drawer;
			drawer.sprite.setBackground("background", new SpriteData(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "musicSelect.png"), 0, 0));
			drawer.sprite.addButton("2musiccard1", new ClickableSprite(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "music-1.png"), 70, 120,0));
			D2DSprite._LClickableSprite["2musiccard1"].OnMouseClick += _EmusicCard1Selected;


			/*((ClickableSprite)data).OnMouseClick += (Object sender, EventArgs e) =>
			{
				Console.WriteLine("{0} 클릭됨.", tag);
			};*/
		}

		private void _EmusicCard1Selected(object sender, EventArgs e)
		{
			Thread _Tmusic = new Thread(() => _tmusic());
			_Tmusic.Start();
			drawer.sprite.addButton("musicstart1", new ClickableSprite(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "playBtn.png"), 420, 340, 0));
			D2DSprite._LClickableSprite["musicstart1"].OnMouseClick += music1Start;
		}

		private void _tmusic()
		{
			if(wavePlayer == null || wavePlayer.PlaybackState != PlaybackState.Stopped)
			{
				string musicPath = Program.musicFileDir + "music1prev" + ".mp3";

				if (File.Exists(musicPath))
				{
					wavePlayer = new WaveOut();
					audioReader = new AudioFileReader(musicPath);
					wavePlayer.Init(audioReader);

					wavePlayer.Play();
				}
				else
				{
					throw new Exception();
				}

				while (wavePlayer.PlaybackState != PlaybackState.Stopped && screenRunning)
				{
					Thread.Sleep(1);
				}
				wavePlayer.Stop();
			}
		}

		private void music1Start(object sender, EventArgs e)
		{
			screenRunning = false;
			wavePlayer.Stop();
			PublicDataManager.currentTaskQueue.addTask(new Game(drawer,"music1"));
		}

		public void run(TaskQueue taskQueue)
		{
			
		}
	}
}
