using SharpDX.Direct2D1;
using System;

namespace MelloRin.CSd3d.Lib
{
	public class SpriteData : ListData, IDisposable
	{
		public BitmapBrush bitmapBrush { get; set; }

		public SpriteData(BitmapBrush bitmapBrush, int x, int y)
		{
			base.x = x;
			base.y = y;

			this.bitmapBrush = bitmapBrush;
		}

		public void Dispose()
		{
			if (bitmapBrush != null)
				bitmapBrush.Dispose();
		}
	}

	public class ClickableSprite : SpriteData
	{
		public int priority { get; private set; }

		public event EventHandler OnMouseClick;

		public ClickableSprite(BitmapBrush bitmapBrush, int x, int y, int priority) : base(bitmapBrush, x, y)
		{
			this.priority = priority;
		}

		public void pointCheck(int pointX, int pointY)
		{
			if (pointX >= x && pointX <= x + (int)bitmapBrush.Bitmap.Size.Width)
			{
				if (pointY >= y && pointY <= y + (int)bitmapBrush.Bitmap.Size.Height)
				{
					OnMouseClick?.Invoke(null, null);
				}
			}
		}

		new public void Dispose()
		{
			base.Dispose();
			if (OnMouseClick != null)
				OnMouseClick = null;
		}
	}
}
