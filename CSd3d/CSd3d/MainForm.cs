using MelloRin.CSd3d.Lib;
using MelloRin.FileManager;
using SharpDX.Windows;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MelloRin.CSd3d
{
	class MainForm : RenderForm
	{
		public MainForm()
		{
			windowsize_adjust();
			
			FormClosed += new FormClosedEventHandler(_formClosed);
		}


		public void windowsize_adjust()
		{
			AutoScaleDimensions = new SizeF(7F, 12F);
			AutoScaleMode = AutoScaleMode.Font;
			FormBorderStyle = FormBorderStyle.FixedSingle;

			uint width, height;
			if (!UInt32.TryParse(PublicData_manager.settings.get_setting("width"), out width)
			|| !UInt32.TryParse(PublicData_manager.settings.get_setting("height"), out height))
			{
				width = 1280;
				height = 720;
			}
			ClientSize = new Size((int)width, (int)height);
		}

		private void _formClosed (object sender, FormClosedEventArgs e)
		{
			DataSet dataSet = new DataSet();

			dataSet.adddata("Display", PublicData_manager.settings.getDisplaytable());
			dataSet.adddata("Input", PublicData_manager.settings.getKeytable());
			dataSet.adddata("Score", PublicData_manager.score.getScoretable());

			File_manager.save_data(dataSet);
			PublicData_manager.device_created = false;

			Dispose(true);
		}
	}
}