using MelloRin.CSd3d.Core;
using MelloRin.CSd3d.Lib;
using MelloRin.CSd3d.Lib.Notes;
using NAudio.Wave;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.XInput;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

using Timer = System.Timers.Timer;

namespace MelloRin.CSd3d.Scenes
{
	class Game : ITask, IControllable
	{
		static public bool gameRunning { get; private set; }

		private readonly int[] _LnoteColor = new int[5] { 0, 1, 1, 0, 2 };
		private readonly TimeSpan oneSec = TimeSpan.FromSeconds(1);
		private readonly int lineStart = 400;
		private readonly int judgeLine = 600;
		private readonly int noteSize = 70;

		private RenderTaskerHandler drawer;
		private BitmapBrush[] _LeffectSprite = new BitmapBrush[8];
		private BitmapBrush[] _LscoreSprite = new BitmapBrush[10];
		private BitmapBrush[] _LnoteSprite = new BitmapBrush[3];
		private BitmapBrush[] _LjudgeSprite = new BitmapBrush[3];

		private NoteQueue[] _LnoteQueue = new NoteQueue[5];

		private int judgeShowTime = 0;
		private Timer judgeTimer = new Timer(100d);


		private ConcurrentDictionary<string, NoteData> _LcreatedNotes = new ConcurrentDictionary<string, NoteData>();

		private Timer[] _LnoteEffectTimer = new Timer[4];
		private int[] _LnoteEffectRunningFrame = new int[4];

		private bool[] keyInputList = new bool[SettingManager.inputKeysKey.Length];

		private string musicName;

		private NoteManager noteManager;
		private AudioFileReader audioReader;
		private WaveOut wavePlayer;
		private Stopwatch noteTimer = new Stopwatch();

		private int score;
		private int fail = 0;
		private int noteCount = 0;
		private int totalNote { get; }
		private int currentCombo = 0;
		private int perfect = 0;
		private int maxCombo = 0;

		Controller controller = new Controller(UserIndex.One);
		bool[] keyFlag = new bool[9];

		public Game(RenderTaskerHandler drawer, string musicName)
		{
			D2DSprite.resetData();
			D2DSprite.resetData();
			if (drawer.targetForm.Created)
			{
				this.musicName = musicName;
				this.drawer = drawer;

				noteManager = new NoteManager(musicName);
				totalNote = noteManager.noteCount;

				initialize();
			}
			else
			{
				throw new Exception();
			}
		}

		public void run(TaskQueue taskQueue)
		{
			gameRunning = true;

			for (int i = 0; i < _LnoteQueue.Length; ++i)
			{
				_LnoteQueue[i] = new NoteQueue();
			}

			drawer.targetForm.KeyDown += _EkeyDown;
			drawer.targetForm.KeyUp += _EkeyUp;

			string musicPath = Program.musicFileDir + musicName + ".mp3";

			if (File.Exists(musicPath))
			{
				wavePlayer = new WaveOut();
				audioReader = new AudioFileReader(musicPath);
				wavePlayer.Init(audioReader);
			}
			else
			{
				throw new Exception();
			}

			Thread _Tgame = new Thread(() => _tGame());
			_Tgame.Start();

			Thread _TgamePad = new Thread(() => _tcontrollerEvent());
			_TgamePad.Start();

			Thread _Tmusic = new Thread(() => _tmusic());
			_Tmusic.Start();

			taskQueue.runNext();
		}

		private void _tGame()
		{
			Timer noteCreateTimer = new Timer(1000d);
			noteCreateTimer.Elapsed += _EnoteCreate;

			createNote(0, 0);
			createNote(0, 1);
			audioReader.Volume = 0.3f;
			noteCreateTimer.Start();
			wavePlayer.Play();
			noteTimer.Start();

			Stopwatch timer = new Stopwatch();
			timer.Start();

			while (gameRunning && drawer.targetForm.Created)
			{
				if (!PublicDataManager.deviceCreated)
				{
					noteCreateTimer.Stop();
					gameRunning = false;
				}
				moveNotes();

				Thread.Sleep(2);
			}
			noteCreateTimer.Stop();

			PublicDataManager.currentTaskQueue.addTask(new ResultScreen(drawer, musicName, new ScoreData(score, perfect, fail, maxCombo)));
		}

		private void _EnoteCreate(object sender, ElapsedEventArgs e)
		{
			TimeSpan temp = audioReader.CurrentTime + oneSec;
			int createTargetMin = temp.Minutes;
			int createTargetSec = temp.Seconds;

			createNote(createTargetMin, createTargetSec);
		}

		private void _tcontrollerEvent()
		{
			while (gameRunning)
			{
				if (controller.IsConnected)
				{
					keyProcss(controller.GetState().Gamepad);
				}
				Thread.Sleep(2);
			}
		}

		private void _tmusic()
		{
			while (wavePlayer.PlaybackState != PlaybackState.Playing)
			{
				Thread.Sleep(1);
			}
			while (wavePlayer.PlaybackState != PlaybackState.Stopped && gameRunning)
			{
				Thread.Sleep(100);
			}
			wavePlayer.Stop();
			gameRunning = false;
		}

		public void initialize()
		{
			BitmapBrush backGround = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "gameScreen.png");

			backGround.Opacity = 0.2f;
			drawer.sprite.modBackgroundImage("background", backGround);

			drawer.sprite.add("skin", new SpriteData(D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "skin.png"), 390, 0));

			for (int i = 0; i < 4; ++i)
			{
				drawer.sprite.add("effect" + i, new SpriteData(null, lineStart + (noteSize * i), judgeLine - (noteSize / 2)));
			}



			for (int i = 1; i <= 7; ++i)
			{
				_LeffectSprite[i - 1] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, String.Format("effect-{0}.png", i));
			}
			_LeffectSprite[_LeffectSprite.Length - 1] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, null, true);

			for (int i = 0; i < _LnoteEffectTimer.Length; ++i)
			{
				int cache = i;
				_LnoteEffectTimer[i] = new Timer(35d);
				_LnoteEffectTimer[i].Elapsed += (sender, e) =>
				{
					drawer.sprite.modImage("effect" + cache, _LeffectSprite[_LnoteEffectRunningFrame[cache]]);

					if (++_LnoteEffectRunningFrame[cache] == _LeffectSprite.Length)
					{
						_LnoteEffectRunningFrame[cache] = 0;
						_LnoteEffectTimer[cache].Stop();
					}
				};
			}

			_LjudgeSprite[0] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "perfect.png");
			_LjudgeSprite[1] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "fail.png");
			_LjudgeSprite[2] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, null, true);

			drawer.sprite.add("judge", new SpriteData(_LjudgeSprite[2], 435, 445));

			judgeTimer.Elapsed += (sender, e) =>
			{
				++judgeShowTime;

				if (judgeShowTime >= 6)
				{
					drawer.sprite.modImage("judge", _LjudgeSprite[2]);
					judgeShowTime = 0;
					judgeTimer.Stop();
				}
			};
			


			_LnoteSprite[0] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "note-r.png");
			_LnoteSprite[1] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "note-b.png");
			_LnoteSprite[2] = D2DSprite.makeBitmapBrush(drawer.sprite.renderTarget, "note-c.png");

			drawer.font.add("scoreTable", new FontData("0000000", drawer.font.renderTarget, Color.White, 1000, 0, 70, "applemint"));
		}

		private void createNote(int min, int sec)
		{
			SecData temp = noteManager.getSecData(min, sec);
			Console.WriteLine("{0} : {1}", min, sec);

			if (temp != null)
			{
				foreach (int ms in temp.msData.Keys)
				{
					++noteCount;
					int lineNum = temp.msData[ms].lineNum;

					_LcreatedNotes.TryAdd("note" + noteCount, temp.msData[ms]);
					drawer.sprite.add("note" + noteCount, new SpriteData(_LnoteSprite[_LnoteColor[lineNum]], lineStart + (noteSize * (lineNum % 4)), (judgeLine - 1000) - ms));
				}
			}
		}

		private void moveNotes()
		{
			long absuoluteMs = noteTimer.ElapsedMilliseconds;

			foreach (string currentData in _LcreatedNotes.Keys)
			{
				try
				{
					long def = ((_LcreatedNotes[currentData].sec * 1000) + _LcreatedNotes[currentData].ms) - absuoluteMs;
					int lineNum = _LcreatedNotes[currentData].lineNum;
					if (def < 80L && !_LnoteQueue[lineNum].search(currentData))
					{
						_LnoteQueue[lineNum].addQuque(currentData);
					}

					if (def < -40L)
					{
						Console.WriteLine("{0}/{1} deleted", currentData, _LnoteQueue[lineNum].pop());
						_LcreatedNotes.TryRemove(currentData, out NoteData outData);
						drawer.sprite.delete(currentData);
						currentCombo = 0;
						++fail;
						judgeChange(1);
						continue;
					}
					drawer.sprite.modPoint(currentData, lineStart + (noteSize * (_LcreatedNotes[currentData].lineNum % 4)), judgeLine - (int)def);
				}
				catch (Exception)
				{

				}

			}
		}

		public void noteProcess(int index)
		{
			if (_LnoteQueue[index].head != null)
			{
				string currentData = _LnoteQueue[index].pop();
				_LcreatedNotes.TryRemove(currentData, out NoteData outData);
				drawer.sprite.delete(currentData);

				++perfect;
				++currentCombo;
				judgeChange(0);

				if (maxCombo < currentCombo)
					maxCombo = currentCombo;

				scoreCalc();

				if (index == 4)
				{
					for (int i = 0; i < _LnoteEffectTimer.Length; ++i)
					{
						_LnoteEffectRunningFrame[i] = 0;
					}
					for (int i = 0; i < _LnoteEffectTimer.Length; ++i)
					{
						_LnoteEffectTimer[i].Start();
					}
				}
				else
				{
					_LnoteEffectTimer[index].Start();
				}
			}
		}

		private void judgeChange(int judge)
		{
			judgeTimer.Stop();

			judgeShowTime = 0;
			drawer.sprite.modImage("judge", _LjudgeSprite[judge]);

			judgeTimer.Start();
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

		private void _EkeyDown(object sender, KeyEventArgs e)
		{
			string input = e.KeyCode.ToString().ToLower();

			if (PublicDataManager.settings.inputKeySearch(input))
			{
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[0])) && !keyInputList[0])
				{
					Console.WriteLine("1");
					keyInputList[0] = true;
					noteProcess(0);
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[1])) && !keyInputList[1])
				{
					Console.WriteLine("2");
					keyInputList[1] = true;
					noteProcess(1);
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[2])) && !keyInputList[2])
				{
					Console.WriteLine("3");
					keyInputList[2] = true;
					noteProcess(2);
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[3])) && !keyInputList[3])
				{
					Console.WriteLine("4");
					keyInputList[3] = true;
					noteProcess(3);
				}
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

			if (PublicDataManager.settings.inputKeySearch(input))
			{
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[0])))
				{
					keyInputList[0] = false;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[1])))
				{
					keyInputList[1] = false;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[2])))
				{
					keyInputList[2] = false;
				}
				if (input.Equals(PublicDataManager.settings.getInputKeys(SettingManager.inputKeysKey[3])))
				{
					keyInputList[3] = false;
				}
			}
			if (e.KeyCode.Equals(Keys.Space))
			{
				keyInputList[4] = false;
			}
		}

		private void scoreCalc()
		{
			//100만×Perfect횟수, 60만×Good횟수, 10만×맥스콤보 이 3개를 모두 더한 뒤 총 노트수만큼 나누고 소수점을 버림
			double p = 1000000 * perfect;
			double max = 100000 * maxCombo;
			score = (int)((p + max) / totalNote);
			updateScore(score);
		}

		object scoreLocker = new object();
		private void updateScore(int score)
		{
			lock (scoreLocker)
			{
				drawer.font.modString("scoreTable", String.Format("{0,7}", score).Replace(" ", "0"));
			}
		}
	}
}