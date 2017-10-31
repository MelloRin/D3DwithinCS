using MelloRin.CSd3d.Lib;
using MelloRin.FileManager;
using System;
using System.Threading.Tasks;

namespace MelloRin.CSd3d
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			File_manager.loadData(out DataSet dataSet);

			Parallel.Invoke
			(
				() => { PublicDataManager.settings = new SettingManager(dataSet); },
				() => { PublicDataManager.score = new SaveDataManager(dataSet); }
			);

			MainForm mainForm = new MainForm();
			PublicDataManager.currentTaskQueue.addTask(mainForm);
			D3Dhandler drawer = new D3Dhandler(mainForm);
			PublicDataManager.currentTaskQueue.addTask(drawer);
			PublicDataManager.currentTaskQueue.addTask(new Game(drawer));
		}
	}
}