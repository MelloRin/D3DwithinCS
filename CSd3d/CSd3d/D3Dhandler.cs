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
	public class RenderTaskerHandler
	{
		public D2DFont font { get; protected set; }
		public D2DSprite sprite { get; protected set; }
		public RenderForm targetForm { get; protected set; }
	}

	public class D3Dhandler : RenderTaskerHandler, IDisposable, ITask
	{
		#region field members
		//D3D device
		private Device _device;
		private SwapChain _swapChain;
		private RenderTargetView _backbufferView;
		private DepthStencilView _zbufferView;
		private DeviceContext _deviceContext;
		private Texture2D _backBufferTexture;
		private SwapChainDescription desc;

		private Timer timer;

		private int frame = 0;
		#endregion

		public D3Dhandler(RenderForm mainForm)
		{
			targetForm = mainForm;

			timer = new Timer(1000d);
			timer.Elapsed += new ElapsedEventHandler((sender, e) =>
			{
				font.modString("fps", frame + " fps");
				//font.modString("nowTime", "Current Time " + DateTime.Now.ToString());

				frame = 0;
			});
		}

		public void run(TaskQueue taskQueue)
		{
			createDevice();

			Thread _Td3d = new Thread(() =>
			{
				timer.Start();

				while (targetForm.Created)
				{
					try
					{
						Clear(new RawColor4(0f, 0f, 0f, 1f));
						sprite.draw();
						font.draw();
						Present();

						++frame;
					}
					catch (SharpDXException e)
					{
						Console.WriteLine("D3D 에러" + e.ToString());
						return;
					}
				}
				Dispose();
			});
			_Td3d.Start();

			taskQueue.runNext();
		}

		private void createDevice()
		{
			targetForm.Invoke(new MethodInvoker(() =>
			{
				desc = new SwapChainDescription()
				{
					BufferCount = 1,//buffer count
					ModeDescription = new ModeDescription(targetForm.ClientSize.Width, targetForm.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),//sview
					IsWindowed = Boolean.Parse(PublicDataManager.settings.getSetting("windowded")),
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
			font.add("fps", new FontData(frame + " fps", font.renderTarget, Color.White, fontName: "applemint"));
			sprite = new D2DSprite(_backBufferTexture);

			_backbufferView = new RenderTargetView(_device, _backBufferTexture);

			_backBufferTexture.Dispose();

			var _zbufferTexture = new Texture2D(_device, new Texture2DDescription()
			{
				Format = Format.D16_UNorm,
				ArraySize = 1,
				MipLevels = 1,
				Width = Int32.Parse(PublicDataManager.settings.getSetting("width")),
				Height = Int32.Parse(PublicDataManager.settings.getSetting("height")),
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

			PublicDataManager.deviceCreated = true;
		}

		private void Clear(RawColor4 color)
		{
			_deviceContext.ClearRenderTargetView(_backbufferView, color);
			_deviceContext.ClearDepthStencilView(_zbufferView, DepthStencilClearFlags.Depth, 1.0F, 0);
		}

		private void Present() { _swapChain.Present(1, PresentFlags.None); }


		public void Dispose()
		{
			if (_device != null)
				_device.Dispose();
			if (font != null)
				font.Dispose();
			if (timer != null)
				timer.Dispose();
		}
	}
}