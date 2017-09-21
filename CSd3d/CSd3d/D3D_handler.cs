using MelloRin.CSd3d.Lib;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using System;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

using Device = SharpDX.Direct3D11.Device;
using Timer = System.Timers.Timer;

namespace MelloRin.CSd3d
{
	class D3D_handler : IDisposable, Itask
	{
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

		private SwapChainDescription desc;

		//vsync
		private readonly int targetFPS = 60;
		private int lastFrameTime;

		private int msPerFPS;
		private double leftMsPerFPS;
		private double leftMs = 0d;

		private int renderTime;
		private int sleepTime;

		private int currentFrameTime;
		private int frame = 0;

		//background
		private bool b_up = false;
		private float B = 0f;

		public D3D_handler(RenderForm mainForm)
		{
			targetForm = mainForm;
			leftMsPerFPS = 1000f / targetFPS;
			msPerFPS = (Int32)leftMsPerFPS;
			leftMsPerFPS -= msPerFPS;

			Timer timer = new Timer();
			timer.Interval = 1000;
			timer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) =>
			{
				Console.WriteLine("{0}fps", frame);
				frame = 0;
			});
			timer.Start();
		}

		public void run()
		{
			createDevice();

			Thread _Td3d = new Thread(() =>
			{
				while (targetForm.Created)
				{
					loop();
					vSync();
				}
				Dispose();
			});


			_Td3d.Start();

			PublicData_manager.currentTaskQueue.runNext();
		}

		private void createDevice()
		{
			targetForm.Invoke(new MethodInvoker(delegate ()
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

			setDefault();

			PublicData_manager.device_created = true;
		}

		private void setDefault()
		{
			_backBufferTexture = _swapChain.GetBackBuffer<Texture2D>(0);
			font = new D2DFont(_backBufferTexture);
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

			setDefaultTargets();
		}

		private void loop()
		{
			if (PublicData_manager.device_created)
			{
				++frame;
				try
				{
					lastFrameTime = DateTime.Now.Millisecond;
					background_Render();

					font.Begin();
					font.DrawString("Hello SharpDX", 0, 0);
					font.DrawString("Current Time " + DateTime.Now.ToString(), 0, 32);
					font.End();

					Present();
				}
				catch (SharpDXException e)
				{
					Console.WriteLine("D3D 에러" + e.ToString());
					return;
				}
			}
		}

		private void vSync()
		{
			currentFrameTime = DateTime.Now.Millisecond;
			if (lastFrameTime > currentFrameTime)
			{
				renderTime = (currentFrameTime + 1000) - lastFrameTime;
			}
			else
			{
				renderTime = currentFrameTime - lastFrameTime;
			}
			leftMs += leftMsPerFPS;

			if (leftMs >= 1d)
			{
				int errorCorrect = (Int32)leftMs;
				sleepTime = (msPerFPS - renderTime) + errorCorrect;
				leftMs -= errorCorrect;
			}
			else
				sleepTime = msPerFPS - renderTime;
			if (sleepTime > 0)
			{
				Thread.Sleep(sleepTime);
			}
		}

		private void background_Render()
		{
			if (b_up)
			{
				if (B < 1f)
					B += 0.01f;
				else
				{
					b_up = false;
				}
			}
			else
			{
				if (B >= 0f)
					B -= 0.01f;
				else
				{
					b_up = true;
				}
			}

			Clear(new RawColor4(0, 0, B, 1));
		}


		private void Clear(RawColor4 color)
		{
			_deviceContext.ClearRenderTargetView(_backbufferView, color);
			_deviceContext.ClearDepthStencilView(_zbufferView, DepthStencilClearFlags.Depth, 1.0F, 0);
		}

		private void Present()
		{
			_swapChain.Present(0, PresentFlags.None);
		}

		private void setDefaultTargets()
		{
			_deviceContext.Rasterizer.SetViewport(0, 0, targetForm.ClientSize.Width, targetForm.ClientSize.Height);
			_deviceContext.OutputMerger.SetTargets(_zbufferView, _backbufferView);
		}

		public void Dispose()
		{
			if (_device != null)
				_device.Dispose();
		}
	}
}