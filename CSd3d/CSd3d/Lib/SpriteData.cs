using SharpDX.Direct2D1;
using System;

namespace MelloRin.CSd3d.Lib
{
	public class SpriteData : ListData, IDisposable
	{
		public int x;
		public int y;
		public int priority { get; private set; }

		public BitmapBrush bitmapBrush { get; private set; }

		public event EventHandler OnMouseClick;

		public SpriteData(BitmapBrush bitmapBrush, int x, int y, int priority)
		{
			this.x = x;
			this.y = y;

			this.bitmapBrush = bitmapBrush;

			this.priority = priority;
		}



		public void collisionCheck(int pointX, int pointY)
		{
			if (pointX >= x && pointX <= x + (int)bitmapBrush.Bitmap.Size.Width)
			{
				if (pointY >= y && pointY <= y + (int)bitmapBrush.Bitmap.Size.Height)
				{
					OnMouseClick?.Invoke(null, null);
				}
			}
		}

		public void Dispose()
		{
			if (bitmapBrush != null)
				bitmapBrush.Dispose();
			if (OnMouseClick != null)
				OnMouseClick = null;
		}
	}
}
