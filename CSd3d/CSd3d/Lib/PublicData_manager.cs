namespace CSd3d
{
    public static class PublicData_manager
    {
        public static bool device_created = false;
        public static readonly int render_Delay = 10;

        public static readonly string settingsFile_name = "settings.ini";
        public static readonly string saveFile_name = "savedata.mlr";

        public static Lib.Setting_manager settings = new Lib.Setting_manager();
    }
}