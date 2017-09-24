using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;

namespace MelloRin.CSd3d
{
	public class D2DFont : IDisposable
	{
		private TextFormat _directWriteTextFormat;
		private SolidColorBrush _directWriteFontColor;
		private RenderTarget _direct2DRenderTarget;

		private Color _fontColor = Color.White;
		private string _fontName = "Calibri";
		private int _fontSize = 22;

		private Dictionary<string, FontData> drawList = new Dictionary<string, FontData>();

		private class FontData
		{
			public string text { get; private set; }
			public int x { get; private set; }
			public int y { get; private set; }

			public FontData(string text, int targetX = 0, int targetY = 0)
			{
				this.text = text;
				x = targetX;
				y = targetY;
			}
		}

		public D2DFont(Texture2D backBuffer)
		{
			var d2dFactory = new SharpDX.Direct2D1.Factory();
			var d2dSurface = backBuffer.QueryInterface<Surface>();
			_direct2DRenderTarget = new RenderTarget(d2dFactory, d2dSurface, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));

			d2dSurface.Dispose();
			d2dFactory.Dispose();

			InitFont();
		}

		public void SetFont(Color fontColor, string fontName = "Calibri", int fontSize = 22)
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

		public void addTextList(string key, string text, int targetX = 0, int targetY = 0)
		{
			if (drawList.ContainsKey(key))
			{
				drawList[key] = new FontData(text, targetX, targetY);
			}
			else
			{
				drawList.Add(key, new FontData(text, targetX, targetY));
			}
		}

		public void deleteTextList(string key)
		{
			if (drawList.ContainsKey(key))
			{
				drawList.Remove(key);
			}
		}

		public void drawStrings(int width = 1280, int height = 720)
		{
			_direct2DRenderTarget.BeginDraw();

			lock(drawList)
			{
				foreach (string key in drawList.Keys)
				{
					FontData drawTarget = drawList[key];
					_direct2DRenderTarget.DrawText(drawTarget.text, _directWriteTextFormat, new RawRectangleF(drawTarget.x, drawTarget.y, width, height), _directWriteFontColor);
				}
			}

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