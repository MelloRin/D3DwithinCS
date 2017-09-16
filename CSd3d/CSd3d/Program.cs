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
			File_manager.load_data(out DataSet dataSet);

			Parallel.Invoke
			(
				() => { PublicData_manager.settings = new Setting_manager(dataSet); },
				() => { PublicData_manager.score = new Savedata_manager(dataSet); }
			);

			MainForm mainForm = new MainForm();
			PublicData_manager.currentTaskQueue.addTask(mainForm);
			PublicData_manager.currentTaskQueue.addTask(new D3D_handler(mainForm));
		}
	}
}