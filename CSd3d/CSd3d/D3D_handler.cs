using MelloRin.CSd3d.Lib;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using System;
using System.Drawing;
using Device = SharpDX.Direct3D11.Device;

namespace MelloRin.CSd3d
{
	class D3D_handler : IDisposable
	{
		private RenderForm targetForm;

		private Device _device;
		private SwapChain _swapChain;
		private RenderTargetView _backbufferView;
		private DepthStencilView _zbufferView;
		private DeviceContext _deviceContext;

		private bool b_up = true;
		private int B = 1;

		public D3D_handler(RenderForm mainForm)
		{
			targetForm = mainForm;

			createDevice(mainForm);

			mainForm.Show();
		}

		private void createDevice(RenderForm mainForm)
		{
			var desc = new SwapChainDescription()
			{
				BufferCount = 1,//buffer count
				ModeDescription = new ModeDescription(targetForm.ClientSize.Width, targetForm.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),//sview
				IsWindowed = Boolean.Parse(PublicData_manager.settings.get_setting("windowded")),
				OutputHandle = targetForm.Handle,
				SampleDescription = new SampleDescription(1, 0),
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput
			};

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
				Width = targetForm.ClientSize.Width,
				Height = targetForm.ClientSize.Height,
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
			//RawColor4 color = new RawColor4(0, 0, 1, 1);

			Clear( b_up ? new RawColor4(0, 0, 1, 1) : new RawColor4(1, 0, 0, 1)) ;
			b_up = !b_up;
			/*
			if (b_up)
			{
				_device.Clear(ClearFlags.Target, color, 1.0f, 0);

				if (B < 255)
					B += 2;
				else
				{
					B = 255;
					b_up = false;
				}
			}
			else
			{
				_device.Clear(ClearFlags.Target, color, 1.0f, 0);

				if (B >= 2)
					B -= 2;
				else
				{
					B = 1;
					b_up = true;
				}
			}*/
		}

		public void Dispose()
		{
			if (_device != null)
				_device.Dispose();
		}
	}
}