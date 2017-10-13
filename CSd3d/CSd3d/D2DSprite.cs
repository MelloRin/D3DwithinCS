using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Concurrent;
using System.Drawing.Imaging;
using System.IO;

using Bitmap = System.Drawing.Bitmap;
using D2Bitmap = SharpDX.Direct2D1.Bitmap;

namespace MelloRin.CSd3d
{
	public class SpriteData : ListData, IDisposable
	{
		private int x, y;
		private int priority;
		public D2Bitmap bitmap { get; private set; }

		public event EventHandler OnMouseClick;

		public SpriteData(D2Bitmap bitmap, int x, int y, int priority)
		{
			this.x = x;
			this.y = y;

			this.bitmap = bitmap;

			this.priority = priority;
		}

		public void collisionCheck(int pointX, int pointY)
		{
			if (pointX >= x && pointX <= x + (int)bitmap.Size.Width / 4)
			{
				if (pointY >= y && pointY <= y + (int)bitmap.Size.Height / 4)
				{
					OnMouseClick?.Invoke(null, null);
				}
			}
		}

		public void Dispose()
		{
			if (bitmap != null)
				bitmap.Dispose();
			if (OnMouseClick != null)
				OnMouseClick = null;
		}
	}

	public interface IListable
	{
		void add(string tag, ListData data);

		void delete(string tag);
	}

	public interface IDrawable
	{
		void draw();
	}

	public class ListData { }

	public class D2DSprite : IDisposable, IListable, IDrawable
	{
		public static ConcurrentDictionary<string, SpriteData> _LSprite = new ConcurrentDictionary<string, SpriteData>();
		public RenderTarget renderTarget { get; private set; }
		static private BitmapBrush bitmapBrush = null;

		public static D2Bitmap makeBitmap(RenderTarget renderTarget, string imgSource)
		{
			try
			{
				ImagingFactory imagingFactory = new ImagingFactory();
				NativeFileStream fileStream = new NativeFileStream(imgSource,
					NativeFileMode.Open, NativeFileAccess.Read);
				BitmapDecoder bitmapDecoder = new BitmapDecoder(imagingFactory, fileStream, DecodeOptions.CacheOnDemand);

				BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);

				FormatConverter converter = new FormatConverter(imagingFactory);
				converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA);

				D2Bitmap bitmap = D2Bitmap.FromWicBitmap(renderTarget, converter);

				Utilities.Dispose(ref bitmapDecoder);
				Utilities.Dispose(ref fileStream);
				Utilities.Dispose(ref imagingFactory);

				bitmapBrush = new BitmapBrush(renderTarget, bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });

				return bitmap;

				/*
				Bitmap bmp = new Bitmap(imgSource);
				
				BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

				if (System.Drawing.Image.IsAlphaPixelFormat(bmp.PixelFormat))
					Console.WriteLine("알파 ok");

				DataStream stream = new DataStream(bmpData.Scan0, bmpData.Stride * bmpData.Height, true, false);
				SharpDX.Direct2D1.PixelFormat pFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
				BitmapProperties bmpProps = new BitmapProperties(pFormat);

				D2Bitmap bitmap = new D2Bitmap(renderTarget, new Size2(bmp.Width, bmp.Height), stream, bmpData.Stride, bmpProps);
				
				bmp.UnlockBits(bmpData);

				stream.Dispose();
				bmp.Dispose();

				bitmapBrush = new BitmapBrush(renderTarget, bitmap, new BitmapBrushProperties() { ExtendModeX = ExtendMode.Wrap, ExtendModeY = ExtendMode.Wrap });
				
				return bitmap;*/
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("{0} file is missing", imgSource);

				return null;
			}
		}


		public D2DSprite(SharpDX.Direct3D11.Texture2D backBuffer)
		{
			var d2dFactory = new SharpDX.Direct2D1.Factory();
			var d2dSurface = backBuffer.QueryInterface<Surface>();
			renderTarget = new RenderTarget(d2dFactory, d2dSurface, new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied)));

			d2dSurface.Dispose();
			d2dFactory.Dispose();
		}

		public void add(string tag, ListData data)
		{
			if (_LSprite.ContainsKey(tag))
			{
				_LSprite[tag] = (SpriteData)data;
			}
			else
			{
				_LSprite.TryAdd(tag, (SpriteData)data);
			}

			((SpriteData)data).OnMouseClick += (Object sender, EventArgs e) =>
			{
				Console.WriteLine("{0} 클릭됨.", tag);
			};
		}

		public void delete(string tag)
		{
			if (_LSprite.ContainsKey(tag))
			{
				_LSprite.TryRemove(tag, out SpriteData temp);
			}
		}

		public void draw()
		{
			renderTarget.BeginDraw();

			renderTarget.Transform = Matrix3x2.Translation(300, 300);
			renderTarget.FillRectangle(new RectangleF(0, 0, (256), (256)), bitmapBrush);

			foreach (string key in _LSprite.Keys)
			{
				//SpriteData drawTarget = _LSprite[key];
				//renderTarget.DrawBitmap(drawTarget.bitmap, 1f, BitmapInterpolationMode.Linear);
				//renderTarget.DrawBitmap(drawTarget.bitmap, new RawRectangleF(300, 300, 300 + (drawTarget.bitmap.Size.Width / 4), 300 + (drawTarget.bitmap.Size.Height / 4)), 1f, BitmapInterpolationMode.Linear);
			}
			renderTarget.EndDraw();
		}

		public void Dispose()
		{
			foreach (string key in _LSprite.Keys)
			{
				SpriteData drawTarget = _LSprite[key];

				drawTarget.Dispose();
			}
			renderTarget.Dispose();
		}
	}
}