using FileManager;
using System;
using System.Collections;

namespace CSd3d.Lib
{
    public class Setting_manager
    {
        private Hashtable settings = new Hashtable();
        private Hashtable inputKeys = new Hashtable();

        public Hashtable getDisplaytable() => settings;
        public Hashtable getKeytable() => inputKeys;

        public static readonly string[] settings_key = new string[] { "width", "height", "windowded" };
        public static readonly string[] input_keys_key = new string[] { "up", "down", "left", "right" };

        public Setting_manager(DataSet dataSet)
        {
            try
            {
                settings = dataSet.getdata("Display");
                inputKeys = dataSet.getdata("Input");
            }
            catch (DatasetException)
            {
                set_default_settings();
            }
        }

        public string get_setting(string key) => settings[key].ToString();
        public void configSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
            {
				Console.WriteLine("초기세팅");
                settings[key] = value;
            }
        }

        public string get_input_keys(string key) => inputKeys[key].ToString();
        public bool input_key_search(string value) => inputKeys.ContainsValue(value);
        public void configInputKeys(string key, string value)
        {
            if (inputKeys.ContainsKey(key))
            {
                inputKeys[key] = value;
            }
        }

        private void set_default_settings()
        {
            settings.Add("width", "1280");
            settings.Add("height", "720");
            settings.Add("windowded", "true");

            inputKeys.Add("up", "w");
            inputKeys.Add("down", "s");
            inputKeys.Add("left", "a");
            inputKeys.Add("right", "d");
        }
    }
}