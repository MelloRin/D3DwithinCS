using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;

using Color = SharpDX.Color4;

namespace MelloRin.CSd3d.Lib
{
	public class FontData : IDisposable
	{
		private RenderTarget renderTarget;

		public string text { get; private set; }
		public int x { get; private set; }
		public int y { get; private set; }

		private Color fontColor;
		private string fontName = "Calibri";
		private int fontSize = 22;

		public TextFormat _directWriteTextFormat { get; private set; }
		public SolidColorBrush _directWriteFontColor { get; private set; }

		public FontData(string text, RenderTarget renderTarget, Color fontColor, int targetX = 0, int targetY = 0,
			int fontSize = 22, string fontName = "Calibri")
		{
			this.renderTarget = renderTarget;
			this.text = text;
			x = targetX;
			y = targetY;

			setFont(fontColor, fontName, fontSize);
		}

		public void setFont(Color fontColor, string fontName, int fontSize)
		{
			this.fontColor = fontColor;
			this.fontName = fontName;
			this.fontSize = fontSize;

			InitFont();
		}

		private void InitFont()
		{
			SharpDX.DirectWrite.Factory directWriteFactory = new SharpDX.DirectWrite.Factory();
			_directWriteTextFormat = new TextFormat(directWriteFactory, fontName, fontSize)
			{ TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
			_directWriteFontColor = new SolidColorBrush(renderTarget, fontColor);
			directWriteFactory.Dispose();
		}

		public void Dispose()
		{
			_directWriteFontColor.Dispose();
			_directWriteTextFormat.Dispose();
		}
	}
}
