using System;
using System.Collections;

namespace CSd3d.Lib
{
    public class Setting_manager
    {
        private Hashtable data = new Hashtable();
        public static readonly string[] settings_key = new string[] { "width", "height", "windowded", "up", "down", "left", "right" };

        public Setting_manager()
        {
            set_default_settings();
        }

        public void change_setting(string key,string value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
        }

        public string get_setting(string key) => data[key].ToString();

        private void set_default_settings()
        {
            data.Add("width", "640");
            data.Add("height", "480");
            data.Add("windowded", "true");
            data.Add("up", "w");
            data.Add("down", "s");
            data.Add("left", "a");
            data.Add("right", "d");
        }
    }
}