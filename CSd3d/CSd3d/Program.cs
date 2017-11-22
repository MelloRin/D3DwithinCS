using MelloRin.CSd3d.Lib;
using MelloRin.CSd3d.Scenes;
using MelloRin.FileManager;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MelloRin.CSd3d
{
	class Program
	{
		#region File Directories
		public static readonly string currentDir = Directory.GetCurrentDirectory();

		public static readonly string logFileDir = String.Format(@"{0}\res\", currentDir);
		public static readonly string logFileName = logFileDir + "log.txt";

		public static readonly string spriteFileDir = String.Format(@"{0}\res\sprite\", currentDir);

		public static readonly string musicFileDir = String.Format(@"{0}\res\music\", currentDir);

		public static readonly string fontFileDir = String.Format(@"{0}\res\fonts\", currentDir);
		#endregion

		[STAThread]
		static void Main(string[] args)
		{
			FileManager.FileManager.loadData(out SaveFileDataSet dataSet);

			Parallel.Invoke
			(
				() => { PublicDataManager.settings = new SettingManager(dataSet); },
				() => { PublicDataManager.score = new SaveDataManager(dataSet); }
			);

			MainForm mainForm = new MainForm();
			PublicDataManager.currentTaskQueue.addTask(mainForm);
			D3Dhandler drawer = new D3Dhandler(mainForm);
			PublicDataManager.currentTaskQueue.addTask(drawer);
			PublicDataManager.currentTaskQueue.addTask(new StartPage(drawer));

			//PublicDataManager.currentTaskQueue.addTask(new Game(drawer));
		}
	}
}