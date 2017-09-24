namespace MelloRin.CSd3d.Lib
{
	public static class PublicData_manager
    {
        public static bool device_created = false;

        public static Setting_manager settings = null;
        public static Savedata_manager score = null;

		public static TaskQueue currentTaskQueue = new TaskQueue();
	}
}