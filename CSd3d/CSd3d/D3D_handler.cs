using MelloRin.CSd3d.Lib;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using Device = SharpDX.Direct3D11.Device;

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
		private SharpDX.Direct3D11.DeviceContext _deviceContext;

		private Texture2D _backBufferTexture;
		D2DFont font;

		private SwapChainDescription desc;

		//vsync
		private readonly int targetFPS = 60;
		private int lastFrameTime;
		private int msPerFPS;

		//background
		private bool b_up = false;
		private float B = 0f;


		StreamWriter writer = new StreamWriter("log.txt");



		public D3D_handler(RenderForm mainForm)
		{
			targetForm = mainForm;
		}

		public void run()
		{
			msPerFPS = 1000 / targetFPS;

			createDevice();

			Thread _Td3d = new Thread(() =>
			{
				PublicData_manager.sw.Start();

				while (targetForm.Created)
				{
					lastFrameTime = DateTime.Now.Millisecond;
					loop();

					vSync();
				}
				Dispose();
				writer.Close();
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

			SharpDX.Direct3D.FeatureLevel[] levels = new SharpDX.Direct3D.FeatureLevel[] { SharpDX.Direct3D.FeatureLevel.Level_11_0 };
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

			SetDefaultTargets();
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

		private void SetDefaultTargets()
		{
			_deviceContext.Rasterizer.SetViewport(0, 0, targetForm.ClientSize.Width, targetForm.ClientSize.Height);
			_deviceContext.OutputMerger.SetTargets(_zbufferView, _backbufferView);
		}

		private void loop()
		{
			if (PublicData_manager.device_created)
			{
				++PublicData_manager.frame;
				try
				{
					background_Render();
					//Console.WriteLine("{0},{1},{2},{3}", color.A, color.R, color.G, color.B);

					if (PublicData_manager.sw.ElapsedMilliseconds >= 1000)
					{
						Console.WriteLine("{0}ms {1}fps", PublicData_manager.sw.ElapsedMilliseconds, PublicData_manager.frame);
						PublicData_manager.sw.Restart();
						PublicData_manager.frame = 0;
					}

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
			int renderTime;
			int sleepTime;

			int currentFrameTime = DateTime.Now.Millisecond;

			if (lastFrameTime > currentFrameTime)
			{
				renderTime = (currentFrameTime + 1000) - lastFrameTime;
			}
			else
			{
				renderTime = currentFrameTime - lastFrameTime;
			}

			sleepTime = msPerFPS - renderTime;

			writer.WriteLine("{0} ~ {1}  {2}", lastFrameTime, currentFrameTime, sleepTime);
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

		public void Dispose()
		{
			if (_device != null)
				_device.Dispose();
		}

		class D2DFont : IDisposable
		{
			private TextFormat _directWriteTextFormat;
			private SolidColorBrush _directWriteFontColor;
			private RenderTarget _direct2DRenderTarget;
			
			private string _fontName = "Calibri";
			private int _fontSize = 22;
			private Color _fontColor = Color.White;

			public D2DFont(Texture2D backBuffer)
			{
				var d2dFactory = new SharpDX.Direct2D1.Factory();
				var d2dSurface = backBuffer.QueryInterface<Surface>();
				_direct2DRenderTarget = new RenderTarget(d2dFactory, d2dSurface, new SharpDX.Direct2D1.
					RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));
				d2dSurface.Dispose();
				d2dFactory.Dispose();

				InitFont();
			}

			public void SetFont(Color fontColor, string fontName, int fontSize)
			{
				_fontColor = fontColor;
				_fontName = fontName;
				_fontSize = fontSize;

				InitFont();
			}

			private void InitFont()
			{
				var directWriteFactory = new SharpDX.DirectWrite.Factory();
				_directWriteTextFormat = new TextFormat(directWriteFactory, _fontName, _fontSize) { TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading, ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near };
				_directWriteFontColor = new SolidColorBrush(_direct2DRenderTarget, _fontColor);
				directWriteFactory.Dispose();
			}

			public void DrawString(string text, int x, int y, int width = 1280, int height = 720)
			{
				_direct2DRenderTarget.DrawText(text, _directWriteTextFormat, new RawRectangleF(x, y, width, height), _directWriteFontColor);
			}

			public void Dispose()
			{
				Utilities.Dispose(ref _directWriteTextFormat);
				Utilities.Dispose(ref _directWriteFontColor);
				Utilities.Dispose(ref _direct2DRenderTarget);
			}

			public void End()
			{
				_direct2DRenderTarget.EndDraw();
			}

			public void Begin()
			{
				_direct2DRenderTarget.BeginDraw();
			}
		}
	}
}