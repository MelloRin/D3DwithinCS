using System.Collections;

namespace CSd3d
{
    public static class PublicData_manager
    {
        public static bool device_created = false;
        public static bool render_paused = false;

        public static readonly int render_Delay = 10;

        public static Hashtable settings = new Hashtable();
        public static readonly string[] settings_key = new string[] { "width", "height", "windowded", "up", "down", "left", "right" };
    }
}