using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;

namespace MelloRin.CSd3d
{
	class D2DFont : IDisposable
	{
		private TextFormat _directWriteTextFormat;
		private SolidColorBrush _directWriteFontColor;
		private RenderTarget _direct2DRenderTarget;

		private Color _fontColor = Color.White;
		private string _fontName = "Calibri";
		private int _fontSize = 22;


		public D2DFont(Texture2D backBuffer)
		{
			var d2dFactory = new SharpDX.Direct2D1.Factory();
			var d2dSurface = backBuffer.QueryInterface<Surface>();
			_direct2DRenderTarget = new RenderTarget(d2dFactory, d2dSurface, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));

			d2dSurface.Dispose();
			d2dFactory.Dispose();

			InitFont();
		}

		public void SetFont(Color fontColor , string fontName = "Calibri", int fontSize = 22)
		{
			_fontColor = fontColor;
			_fontName = fontName;
			_fontSize = fontSize;

			InitFont();
		}

		private void InitFont()
		{
			var directWriteFactory = new SharpDX.DirectWrite.Factory();
			_directWriteTextFormat = new TextFormat(directWriteFactory, _fontName, _fontSize)
			{ TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
			_directWriteFontColor = new SolidColorBrush(_direct2DRenderTarget, _fontColor);
			directWriteFactory.Dispose();
		}

		public void DrawString(string text, int x, int y, int width = 1280, int height = 720)
		{
			_direct2DRenderTarget.DrawText(text, _directWriteTextFormat, new RawRectangleF(x, y, width, height), _directWriteFontColor);
		}

		public void Begin()
		{
			_direct2DRenderTarget.BeginDraw();
		}

		public void End()
		{
			_direct2DRenderTarget.EndDraw();
		}

		public void Dispose()
		{
			Utilities.Dispose(ref _directWriteTextFormat);
			Utilities.Dispose(ref _directWriteFontColor);
			Utilities.Dispose(ref _direct2DRenderTarget);
		}
	}
}