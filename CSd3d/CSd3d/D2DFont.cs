using MelloRin.CSd3d.Lib;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MelloRin.CSd3d
{
	public class D2DFont : IDisposable, IDrawable, IListable
	{
		public RenderTarget renderTarget { get; private set; }
		private Dictionary<string, FontData> _Ldraw = new Dictionary<string, FontData>();

		static public D2DSprite test;

		public D2DFont(Texture2D backBuffer)
		{
			var d2dFactory = new SharpDX.Direct2D1.Factory();
			var d2dSurface = backBuffer.QueryInterface<Surface>();
			renderTarget = new RenderTarget(d2dFactory, d2dSurface, new RenderTargetProperties(new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));

			d2dSurface.Dispose();
			d2dFactory.Dispose();
		}

		public void add(string tag, ListData data)
		{
			if (_Ldraw.ContainsKey(tag))
			{
				_Ldraw[tag] = (FontData)data;
			}
			else
			{
				_Ldraw.Add(tag, (FontData)data);
			}
		}

		public void delete(string tag)
		{
			if (_Ldraw.ContainsKey(tag))
			{
				_Ldraw.Remove(tag);
			}
		}

		//usage = {only change x,y => modString("tag", x: 9, y: 10);} {only change text => modString("tag",text:" ");}
		public void modString(string tag, string text = "", int x = -1, int y = -1)
		{
			if (_Ldraw.ContainsKey(tag))
			{
				if (text != "")
					_Ldraw[tag].text = text;

				if (x != -1)
					_Ldraw[tag].x = x;

				if (y != -1)
					_Ldraw[tag].y = y;
			}
		}

		public void draw()
		{
			renderTarget.BeginDraw();

			foreach (string key in _Ldraw.Keys.ToArray())
			{
				FontData drawTarget = _Ldraw[key];
				renderTarget.DrawText(drawTarget.text, drawTarget._directWriteTextFormat, new RawRectangleF(drawTarget.x, drawTarget.y, float.Parse(PublicDataManager.settings.getSetting("width")) , float.Parse(PublicDataManager.settings.getSetting("width"))), drawTarget._directWriteFontColor);
			}
			renderTarget.EndDraw();
		}

		public void Dispose()
		{
			foreach (string key in _Ldraw.Keys.ToArray())
			{
				FontData drawTarget = _Ldraw[key];

				drawTarget.Dispose();
			}

			renderTarget.Dispose();
		}
	}
}