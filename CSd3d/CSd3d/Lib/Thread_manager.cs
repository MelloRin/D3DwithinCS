using MelloRin.FileManager;
using System.Threading.Tasks;

using DataSet = MelloRin.FileManager.DataSet;

namespace MelloRin.CSd3d.Lib
{
	class Thread_manager : Itask
	{
		public void run()
		{
			File_manager.load_data(out DataSet dataSet);

			Parallel.Invoke
			(
				() => { PublicData_manager.settings = new Setting_manager(dataSet); },
				() => { PublicData_manager.score = new Savedata_manager(dataSet); }
			);
		}

		public Thread_manager()
		{

		}
	}
}