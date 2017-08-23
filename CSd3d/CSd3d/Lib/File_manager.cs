using System;
using System.IO;

namespace CSd3d.Lib
{
    class File_manager
    {
        static public bool load_settings()
        {
            if (File.Exists(PublicData_manager.settingsFile_name))
            {
                string tag = "Display";
                StreamReader reader = new StreamReader(PublicData_manager.settingsFile_name);
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();

                    if(line.StartsWith("[") && line.EndsWith("]"))
                    {
                        tag = line.Substring(1, line.Length - 2);
                        continue;
                    }

                    try
                    {
                        string[] data = line.Split('=');
                        switch (tag)
                        {
                            case "Display":
                                PublicData_manager.settings.config_setting(data[0], data[1]);
                                break;
                            case "Input":
                                PublicData_manager.settings.config_input_keys(data[0], data[1].ToLower());
                                break;
                        }
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
            if (File.Exists(PublicData_manager.settingsFile_name))
            {
                File.Delete(PublicData_manager.settingsFile_name);
            }

            StreamWriter writer = new StreamWriter(PublicData_manager.settingsFile_name);

            writer.WriteLine("[Display]");
            for (int i = 0; i < Setting_manager.settings_key.Length; i++)
            {
                string key = Setting_manager.settings_key[i];
                writer.WriteLine(key + "=" + PublicData_manager.settings.get_setting(key));
            }

            writer.WriteLine("[Input]");
            for (int i = 0; i < Setting_manager.input_keys_key.Length; i++)
            {
                string key = Setting_manager.input_keys_key[i];
                writer.WriteLine(key + "=" + PublicData_manager.settings.get_input_keys(key));
            }

            writer.Close();
        }
    }
}