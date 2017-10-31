namespace MelloRin.CSd3d.Lib
{
	public static class PublicDataManager
    {
        public static bool deviceCreated = false;

        public static SettingManager settings = null;
        public static SaveDataManager score = null;

		public static TaskQueue currentTaskQueue = new TaskQueue();

		public static bool mouseCaptureState = true;
	}
}