using System;
using System.IO;

namespace CSd3d
{
    class File_manager
    {
        private readonly string settingFile_name = "settings.ini";
        private readonly string saveFile_name = "savedata.mlr";

        //private readonly string settings_default = "width=640\r\nheight=480\r\nwindowded=true\r\nup=w\r\ndown=s\r\nleft=a\r\nright=d";

        public bool read_settings()
        {
            set_default_settings();

            if (File.Exists(settingFile_name))
            {
                StreamReader reader = new StreamReader(settingFile_name);
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] temp = line.Split('=');
                    
                    try
                    {
                        for (int i = 0; i < PublicData_manager.settings_key.Length; i++)
                        {
                            if (PublicData_manager.settings.ContainsKey(temp[0]))
                            {
                                PublicData_manager.settings[temp[0]] = temp[1];
                            }
                        }
                    }
                    catch (IndexOutOfRangeException) { }
                }
                reader.Close();
            }
            return true;
        }

        private void set_default_settings()
        {
            PublicData_manager.settings.Add("width", "640");
            PublicData_manager.settings.Add("height", "480");
            PublicData_manager.settings.Add("windowded", "true");
            PublicData_manager.settings.Add("up", "w");
            PublicData_manager.settings.Add("down", "s");
            PublicData_manager.settings.Add("left", "a");
            PublicData_manager.settings.Add("right", "d");
        }
    }
}