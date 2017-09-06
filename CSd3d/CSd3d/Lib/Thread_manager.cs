using MelloRin.FileManager;
using System.Threading.Tasks;

using DataSet = MelloRin.FileManager.DataSet;

namespace MelloRin.CSd3d.Lib
{
	class Thread_manager : Itask
	{
		public bool run()
		{
			File_manager.load_data(out DataSet dataSet);

			Parallel.Invoke
			(
				() => { PublicData_manager.settings = new Setting_manager(dataSet); },
				() => { PublicData_manager.score = new Savedata_manager(dataSet); }
			);

			/*Thread _TmainThread = new Thread(mainLoop);
			_TmainThread.Start();*/

			return true;
		}

		public Thread_manager()
		{
			//PublicData_manager.currentTaskQueue.addTask(this);

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