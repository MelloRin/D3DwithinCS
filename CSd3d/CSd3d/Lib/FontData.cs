using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;

using Color = SharpDX.Color4;
using Factory = SharpDX.DirectWrite.Factory;

namespace MelloRin.CSd3d.Lib
{
	public class FontData : ListData,IDisposable
	{
		private RenderTarget renderTarget;

		public string text;

		private Color fontColor;
		private string fontName;
		private float fontSize;

		public TextFormat _directWriteTextFormat { get; private set; }
		public SolidColorBrush _directWriteFontColor { get; private set; }

		public FontData(string text, RenderTarget renderTarget, Color fontColor, int x = 0, int y = 0,
			float fontSize = 28, string fontName = "font.ttf")
		{
			this.renderTarget = renderTarget;
			this.text = text;
			this.fontName = fontName;
			base.x = x;
			base.y = y;

			setFont(fontColor, fontName, fontSize);
		}

		public void setFont(Color fontColor, string fontName, float fontSize)
		{
			this.fontColor = fontColor;
			this.fontName = fontName;
			this.fontSize = fontSize;

			InitFont();
		}

		private void InitFont()
		{
			Factory directWriteFactory = new Factory();

			/*FontLoader fontDataLoader = new FontLoader(directWriteFactory);
			FontCollection fontCollection = new FontCollection(directWriteFactory, fontDataLoader, fontDataLoader.getDataStream());*/

			//CurrentTextFormat = new TextFormat(FactoryDWrite, FontFamilyName, CurrentFontCollection, FontWeight.Normal, FontStyle.Normal, FontStretch.Normal, 64) 
			//{ TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };

			_directWriteTextFormat = new TextFormat(directWriteFactory, fontName, fontSize)
			{ TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };

			Console.WriteLine("현재 폰트명 : {0}",_directWriteTextFormat.FontFamilyName);

			_directWriteFontColor = new SolidColorBrush(renderTarget, fontColor);
			directWriteFactory.Dispose();
		}

		public void Dispose()
		{
			if(_directWriteFontColor != null)
				_directWriteFontColor.Dispose();
			if(_directWriteTextFormat != null)
			_directWriteTextFormat.Dispose();
		}
	}
}
