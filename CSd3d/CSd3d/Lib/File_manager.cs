using System;
using System.IO;

namespace CSd3d
{
    class File_manager
    {
        private readonly string settingsFile_name = "settings.ini";
        private readonly string saveFile_name = "savedata.mlr";
       
        public bool read_settings()
        {
            set_default_settings();

            if (File.Exists(settingsFile_name))
            {
                StreamReader reader = new StreamReader(settingsFile_name);
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

        public bool save_settigs()
        {
            Console.WriteLine("데이터 저장");
            if(File.Exists(settingsFile_name))
            {
                File.Delete(settingsFile_name);
            }

            StreamWriter writer = new StreamWriter(settingsFile_name);

            for(int i = 0; i < PublicData_manager.settings_key.Length; i++)
            {
                string key = PublicData_manager.settings_key[i].ToString();
                writer.WriteLine(key + "=" + PublicData_manager.settings[key]);
            }

            writer.Close();

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