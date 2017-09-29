using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Concurrent;

using MelloRin.CSd3d.Lib;

namespace MelloRin.CSd3d
{
	public class D2DFont : IDisposable
	{
		public RenderTarget _renderTarget { get; private set; }
		private ConcurrentDictionary<string, FontData> drawList = new ConcurrentDictionary<string, FontData>();

		private Bitmap test;

		public D2DFont(Texture2D backBuffer)
		{
			var d2dFactory = new SharpDX.Direct2D1.Factory();
			var d2dSurface = backBuffer.QueryInterface<Surface>();
			_renderTarget = new RenderTarget(d2dFactory, d2dSurface, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));

			d2dSurface.Dispose();
			d2dFactory.Dispose();

			string imageSrc = String.Format("{0}\\res\\sprite\\{1}", System.IO.Directory.GetCurrentDirectory(), "note_blue.png");
			test = D2DSprite.LoadFromFile(_renderTarget, imageSrc);
		}

		public void addTextList(string key, FontData fontData)
		{
			if (drawList.ContainsKey(key))
			{
				drawList[key] = fontData;
			}
			else
			{
				drawList.TryAdd(key, fontData);
			}
		}

		public void deleteTextList(string key)
		{
			if (drawList.ContainsKey(key))
			{
				drawList.TryRemove(key, out FontData temp);
			}
		}
		public void drawStrings(int width = 1280, int height = 720)
		{
			_renderTarget.BeginDraw();

			foreach (string key in drawList.Keys)
			{
				FontData drawTarget = drawList[key];
				_renderTarget.DrawText(drawTarget.text, drawTarget._directWriteTextFormat, new RawRectangleF(drawTarget.x, drawTarget.y, width, height), drawTarget._directWriteFontColor);
			}

			//_renderTarget.DrawBitmap(test,new RawRectangleF(300,300,test.Size.Width,test.Size.Height), 0.1f, BitmapInterpolationMode.Linear);

			//_renderTarget.DrawBitmap(test, 1f, BitmapInterpolationMode.Linear);
			//SharpDX.Direct2D1.BitmapRenderTarget _bmpRenderTarget = new SharpDX.Direct2D1.BitmapRenderTarget(_renderTarget,CompatibleRenderTargetOptions.GdiCompatible);

			_renderTarget.EndDraw();
		}

		public void Dispose()
		{
			foreach (string key in drawList.Keys)
			{
				FontData drawTarget = drawList[key];

				drawTarget.Dispose();
			}

			_renderTarget.Dispose();
		}
	}
}