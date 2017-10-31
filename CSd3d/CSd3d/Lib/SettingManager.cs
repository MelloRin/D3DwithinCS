using MelloRin.FileManager;
using System;
using System.Collections;

using DataSet = MelloRin.FileManager.DataSet;

namespace MelloRin.CSd3d.Lib
{
    public class SettingManager
    {
        private Hashtable settings = new Hashtable();
        private Hashtable inputKeys = new Hashtable();

        public Hashtable getDisplaytable() => settings;
        public Hashtable getKeytable() => inputKeys;

        public static readonly string[] settingsKey = new string[] { "width", "height", "windowded" };
        public static readonly string[] inputKeysKey = new string[] { "up", "down", "left", "right" };

        public SettingManager(DataSet dataSet)
        {
            try
            {
                settings = dataSet.getData("Display");
                inputKeys = dataSet.getData("Input");
            }
            catch (DatasetException)
            {
                setDefaultSettings();
            }
        }

        public string getSetting(string key) => settings[key].ToString();
        public void configSetting(string key, string value)
        {
            if (settings.ContainsKey(key))
            {
				Console.WriteLine("초기세팅");
                settings[key] = value;
            }
        }

        public string getInputKeys(string key) => inputKeys[key].ToString();
        public bool inputKeySearch(string value) => inputKeys.ContainsValue(value);
        public void configInputKeys(string key, string value)
        {
            if (inputKeys.ContainsKey(key))
            {
                inputKeys[key] = value;
            }
        }

        private void setDefaultSettings()
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