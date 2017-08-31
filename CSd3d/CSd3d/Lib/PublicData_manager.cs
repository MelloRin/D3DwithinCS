using CSd3d.Lib;

namespace CSd3d
{
    public static class PublicData_manager
    {
        public static bool device_created = false;
        public static readonly int render_Delay = 2;
        public static readonly int event_Delay = 2;

        public static Setting_manager settings = null;
        public static Savedata_manager score = null;
    }
}