using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using System;
using System.Drawing.Imaging;
using System.IO;
using Bitmap = System.Drawing.Bitmap;

namespace MelloRin.CSd3d
{
	public class D2DSprite
	{
		public static SharpDX.Direct2D1.Bitmap LoadFromFile(RenderTarget renderTarget, string fileName)
		{
			try
			{
				Bitmap bmp = new Bitmap(fileName);

				BitmapData bmpData =
					bmp.LockBits(
						new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
						ImageLockMode.ReadOnly, bmp.PixelFormat);

				if (System.Drawing.Image.IsAlphaPixelFormat(bmp.PixelFormat))
					Console.WriteLine("알파 ok");

				DataStream stream = new DataStream(bmpData.Scan0, bmpData.Stride * bmpData.Height, true, false);
				SharpDX.Direct2D1.PixelFormat pFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
				BitmapProperties bmpProps = new BitmapProperties(pFormat);

				SharpDX.Direct2D1.Bitmap result = new SharpDX.Direct2D1.Bitmap(renderTarget, new Size2(bmp.Width, bmp.Height), stream, bmpData.Stride, bmpProps);
				bmp.UnlockBits(bmpData);

				stream.Dispose();
				bmp.Dispose();
				


				return result;
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("{0} file is missing", fileName);
				return null;
			}
		}
	}
}