using System.Collections;
using FileManager;
using System.Collections.Generic;

namespace CSd3d.Lib
{
    public class Setting_manager
    {
        private Hashtable settings = new Hashtable();
        private Hashtable input_keys = new Hashtable();

        public static readonly string[] settings_key = new string[] { "width", "height", "windowded" };
        public static readonly string[] input_keys_key = new string[] { "up", "down", "left", "right" };

        public bool setSetting(DataSet dataSet)
        {
            try
            {
                if (dataSet.getdata("Display").hashtable.ContainsKey(settings_key[0]))
                {
                    settings = PublicData_manager.dataSet.getdata("Display").hashtable;
                    input_keys = PublicData_manager.dataSet.getdata("Input").hashtable;
                }
            }
            catch(KeyNotFoundException e)
            {
                set_default_settings();
            }
            return true;
        }

        public void config_setting(string key, string value)
        {
            if (settings.ContainsKey(key))
            {
                settings[key] = value;
            }
        }

        public void config_input_keys(string key, string value)
        {
            if (input_keys.ContainsKey(key))
            {
                input_keys[key] = value;
            }
        }

        public Hashtable getDisplaytable() => settings;

        public Hashtable getKeytable() => input_keys;

        public string get_setting(string key) => settings[key].ToString();

        public string get_input_keys(string key) => input_keys[key].ToString();

        public bool input_key_search(string value) => input_keys.ContainsValue(value);

        private void set_default_settings()
        {
            settings.Add("width", "640");
            settings.Add("height", "480");
            settings.Add("windowded", "true");

            input_keys.Add("up", "w");
            input_keys.Add("down", "s");
            input_keys.Add("left", "a");
            input_keys.Add("right", "d");
        }
    }
}