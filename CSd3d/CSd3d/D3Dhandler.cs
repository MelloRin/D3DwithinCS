using MelloRin.CSd3d.Lib;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

using Device = SharpDX.Direct3D11.Device;
using Timer = System.Timers.Timer;

namespace MelloRin.CSd3d
{
	class D3Dhandler : IDisposable, Itask
	{
		#region field members
		//form
		private RenderForm targetForm;

		//D3D device
		private Device _device;
		private SwapChain _swapChain;
		private RenderTargetView _backbufferView;
		private DepthStencilView _zbufferView;
		private DeviceContext _deviceContext;

		private Texture2D _backBufferTexture;
		public D2DFont font { get; private set; }
		public D2DSprite sprite { get; private set; }

		private SwapChainDescription desc;

		//rendertask
		private ConcurrentDictionary<string, Action> _Ltask = new ConcurrentDictionary<string, Action>();
		private Action[] task;

		//vsync
		private readonly int targetFPS = 60;

		private Stopwatch renderTimer;
		private Timer timer;

		private int msPerFPS { get; }
		private long nextRenderStartTime;
		private int sleepTime;

		private int frame = 0;
		private StreamWriter writer = new StreamWriter("log.txt");

		//background
		private bool b_up = false;
		private float B = 0f;
		#endregion

		public D3Dhandler(RenderForm mainForm)
		{
			targetForm = mainForm;
			msPerFPS = 1000 / targetFPS;

			renderTimer = new Stopwatch();
			timer = new Timer
			{
				Interval = 1000
			};
			timer.Elapsed += new ElapsedEventHandler((sender, e) =>
			{
				font.add("nowTime", new FontData("Current Time " + DateTime.Now.ToString(), font.renderTarget, Color.Red, 0, 32));

				Console.WriteLine("{0}fps", frame);
				frame = 0;
			});
		}

		public void run()
		{
			createDevice();

			Thread _Td3d = new Thread(() =>
			{
				timer.Start();
				renderTimer.Start();
				//Clear(new RawColor4(0, 0, 0, 1));

				_Ltask.TryAdd("background", background_Render);
				_Ltask.TryAdd("font", font.draw );
				task = new Action[_Ltask.Count];
				_Ltask.Values.CopyTo(task, 0);

				while (targetForm.Created)
				{
					nextRenderStartTime = renderTimer.ElapsedMilliseconds + msPerFPS;
					
					try
					{
						background_Render();
						font.draw();
						sprite.draw();
						
						//Parallel.Invoke(task);
						
						Present();
						++frame;
					}
					catch (SharpDXException e)
					{
						Console.WriteLine("D3D 에러" + e.ToString());
						return;
					}

					sleepTime = (Int32)(nextRenderStartTime - renderTimer.ElapsedMilliseconds);
					//writer.WriteLine("{0}ms Sleep", sleepTime);
					if (sleepTime > 0)
					{
						Thread.Sleep(sleepTime);
					}
				}
				writer.Close();
				Dispose();
			});
			_Td3d.Start();

			PublicData_manager.currentTaskQueue.runNext();
		}

		private void constructing()
		{
			/*Dictionary<string,Action> list = new Dictionary<string, Action>();
					
			ConcurrentDictionary<string, int> test = new ConcurrentDictionary<string, int>();

			Thread[] thread = new Thread[10];
			Thread _Ttest = new Thread(() =>
			{


			});

			list.Add("asdf",temp);

			Action[] action = new Action[list.Count];
			list.Values.CopyTo(action, 0);
			Parallel.Invoke(action);*/
		}

		private void createDevice()
		{
			targetForm.Invoke(new MethodInvoker(() =>
			{
				desc = new SwapChainDescription()
				{
					BufferCount = 1,//buffer count
					ModeDescription = new ModeDescription(targetForm.ClientSize.Width, targetForm.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),//sview
					IsWindowed = Boolean.Parse(PublicData_manager.settings.get_setting("windowded")),
					OutputHandle = targetForm.Handle,
					SampleDescription = new SampleDescription(1, 0),
					SwapEffect = SwapEffect.Discard,
					Usage = Usage.RenderTargetOutput
				};
			}));

			FeatureLevel[] levels = new FeatureLevel[] { FeatureLevel.Level_11_0 };
			DeviceCreationFlags flag = DeviceCreationFlags.None | DeviceCreationFlags.BgraSupport;

			Device.CreateWithSwapChain(DriverType.Hardware, flag, levels, desc, out _device, out _swapChain);

			_deviceContext = _device.ImmediateContext;

			_backBufferTexture = _swapChain.GetBackBuffer<Texture2D>(0);
			font = new D2DFont(_backBufferTexture);
			font.add("tittle", new FontData("Hello SharpDX", font.renderTarget, Color.White));

			string imageSrc = String.Format("{0}\\res\\sprite\\{1}", Directory.GetCurrentDirectory(), "note_blue.png");
			sprite = new D2DSprite(_backBufferTexture);

			sprite.add("note", new SpriteData(D2DSprite.makeBitmap(sprite.renderTarget, imageSrc), 300, 300, 1));

			_backbufferView = new RenderTargetView(_device, _backBufferTexture);
			_backBufferTexture.Dispose();

			var _zbufferTexture = new Texture2D(_device, new Texture2DDescription()
			{
				Format = Format.D16_UNorm,
				ArraySize = 1,
				MipLevels = 1,
				Width = Int32.Parse(PublicData_manager.settings.get_setting("width")),
				Height = Int32.Parse(PublicData_manager.settings.get_setting("height")),
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				BindFlags = BindFlags.DepthStencil,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.None
			});

			// Create the depth buffer view
			_zbufferView = new DepthStencilView(_device, _zbufferTexture);
			_zbufferTexture.Dispose();

			_deviceContext.Rasterizer.SetViewport(0, 0, targetForm.ClientSize.Width, targetForm.ClientSize.Height);
			_deviceContext.OutputMerger.SetTargets(_zbufferView, _backbufferView);

			PublicData_manager.device_created = true;
		}

		private void background_Render()
		{
			if (b_up)
			{
				if (B < 1f)
					B += 0.01f;
				else
					b_up = false;
			}
			else
			{
				if (B >= 0f)
					B -= 0.01f;
				else
					b_up = true;
			}
			Clear(new RawColor4(0, 0, B, 1));
		}

		private void Clear(RawColor4 color)
		{
			_deviceContext.ClearRenderTargetView(_backbufferView, color);
			_deviceContext.ClearDepthStencilView(_zbufferView, DepthStencilClearFlags.Depth, 1.0F, 0);
		}

		private void Present() { _swapChain.Present(0, PresentFlags.None); }


		public void Dispose()
		{
			if (_device != null)
				_device.Dispose();
			if (font != null)
				font.Dispose();
			if (timer != null)
				timer.Dispose();
			if (renderTimer != null)
				renderTimer = null;
		}
	}
}