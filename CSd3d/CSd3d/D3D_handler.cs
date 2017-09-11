﻿using MelloRin.CSd3d.Lib;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using System;
using System.Threading;
using System.Windows.Forms;
using Device = SharpDX.Direct3D11.Device;

namespace MelloRin.CSd3d
{
	class D3D_handler : IDisposable, Itask
	{
		private RenderForm targetForm;

		private Device _device;
		private SwapChain _swapChain;
		private RenderTargetView _backbufferView;
		private DepthStencilView _zbufferView;
		private DeviceContext _deviceContext;

		private SwapChainDescription desc;

		private readonly int targetFPS = 60;
		
		private bool b_up = false;
		private float B = 0f;

		public D3D_handler(RenderForm mainForm)
		{
			targetForm = mainForm;
		}

		public void run()
		{
			createDevice();
			Thread _Td3d = new Thread(() =>
			{
				PublicData_manager.sw.Start();
				int lastFrameTime = DateTime.Now.Millisecond;
				int msPerFPS = 1000 / targetFPS;

				while (targetForm.Created)
				{
					int currentFrameTime = DateTime.Now.Millisecond;
					int renderTime;

					loop();

					if (lastFrameTime > currentFrameTime)
					{
						renderTime = (currentFrameTime + 1000) - lastFrameTime;
					}
					else
					{
						renderTime = currentFrameTime - lastFrameTime;
					}
					if (renderTime < msPerFPS)
					{
						Thread.Sleep((int)((msPerFPS - renderTime) * 2.1));
					}
					lastFrameTime = currentFrameTime;

					++PublicData_manager.frame;

					if (PublicData_manager.sw.ElapsedMilliseconds >= 1000)
					{
						Console.WriteLine("{0}ms {1}fps", PublicData_manager.sw.ElapsedMilliseconds, PublicData_manager.frame);
						PublicData_manager.sw.Restart();
						PublicData_manager.frame = 0;
					}
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
			var _backBufferTexture = _swapChain.GetBackBuffer<Texture2D>(0);

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

			SetDefaultTargers();
		}

		public void Clear(RawColor4 color)
		{
			_deviceContext.ClearRenderTargetView(_backbufferView, color);
			_deviceContext.ClearDepthStencilView(_zbufferView, DepthStencilClearFlags.Depth, 1.0F, 0);
		}

		public void Present()
		{
			_swapChain.Present(0, PresentFlags.None);
		}

		public void SetDefaultTargers()
		{
			_deviceContext.Rasterizer.SetViewport(0, 0, targetForm.ClientSize.Width, targetForm.ClientSize.Height);
			_deviceContext.OutputMerger.SetTargets(_zbufferView, _backbufferView);
		}

		public void loop()
		{
			if (PublicData_manager.device_created)
			{
				try
				{
					background_Render();
					//Console.WriteLine("{0},{1},{2},{3}", color.A, color.R, color.G, color.B);
					/*_device.BeginScene();

					draw_Text();

					_device.EndScene();*/
					Present();
				}
				catch (SharpDXException e)
				{
					Console.WriteLine("D3D 에러" + e.ToString());
					return;
				}
			}
		}

		private void background_Render()
		{			
			if (b_up)
			{
				if (B < 1f)
					B += 0.005f;
				else
				{
					b_up = false;
				}
			}
			else
			{
				if (B >= 0f)
					B -= 0.005f;
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
	}
}