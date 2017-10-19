using System;

using Bitmap = SharpDX.Direct2D1.Bitmap;

namespace MelloRin.CSd3d.Lib
{
	public class SpriteData : ListData, IDisposable
	{
		public int x { get; private set; }
		public int y { get; private set; }
		public int priority { get; private set; }
		public Bitmap bitmap { get; private set; }

		public event EventHandler OnMouseClick;

		public SpriteData(Bitmap bitmap, int x, int y, int priority)
		{
			this.x = x;
			this.y = y;

			this.bitmap = bitmap;

			this.priority = priority;
		}

		public void collisionCheck(int pointX, int pointY)
		{
			if (pointX >= x && pointX <= x + (int)bitmap.Size.Width)
			{
				if (pointY >= y && pointY <= y + (int)bitmap.Size.Height)
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
}
