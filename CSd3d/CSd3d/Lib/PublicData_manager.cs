using System.Diagnostics;

namespace MelloRin.CSd3d.Lib
{
    public static class PublicData_manager
    {
        public static bool device_created = false;
        public static bool game_started = false;

        public static readonly int render_Delay = 2;
        public static readonly int event_Delay = 2;

        public static Setting_manager settings = null;
        public static Savedata_manager score = null;

		public static TaskQueue currentTaskQueue = new TaskQueue(2);

		public static Stopwatch sw = new Stopwatch();
		public static uint frame = 0;
	}
}