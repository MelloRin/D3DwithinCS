using System;
using System.IO;

namespace CSd3d.Lib
{
    class File_manager
    {
        private readonly string settingsFile_name = "settings.ini";
        private readonly string saveFile_name = "savedata.mlr";
       
        public bool load_settings()
        {
            if (File.Exists(settingsFile_name))
            {
                StreamReader reader = new StreamReader(settingsFile_name);
                while (reader.Peek() >= 0)
                {
                    try
                    {
                        string[] line = reader.ReadLine().Split('=');
                        PublicData_manager.settings.change_setting(line[0], line[1]);
                    }
                    catch (IndexOutOfRangeException) { }
                }
                reader.Close();
            }
            return true;
        }

        public void save_settigs()
        {
            Console.WriteLine("데이터 저장");
            if(File.Exists(settingsFile_name))
            {
                File.Delete(settingsFile_name);
            }

            StreamWriter writer = new StreamWriter(settingsFile_name);

            for(int i = 0; i < Setting_manager.settings_key.Length; i++)
            {
                string key = Setting_manager.settings_key[i];
                writer.WriteLine(key + "=" + PublicData_manager.settings.get_setting(key));
            }

            writer.Close();
        }
    }
}