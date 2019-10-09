using System;
using System.Collections;
using System.IO;
using System.Text;

namespace MelloRin.FileManager
{
	public static class FileManager
	{
		
        public static readonly string currentDir = Directory.GetCurrentDirectory();
        public static readonly string saveFileDir = Path.Combine(currentDir, "res");

        public static readonly string saveFileName = "savedata.mlr";

        public static void loadData(out SaveFileDataSet dataSet)
		{

            Directory.CreateDirectory(saveFileDir);
            if (File.Exists(saveFileName))
            {
                StreamReader reader = new StreamReader(Path.Combine(saveFileDir, saveFileName));

                string rawData = reader.ReadLine();
				reader.Close();
				string key = rawData.Substring(0, 32);
				rawData = rawData.Substring(32);

				string[] line = AES256_manager.decrypt(rawData, key).Split('\n');

				string tag = null;

				dataSet = new SaveFileDataSet();
				Hashtable data = null;

				for (int i = 0; i < line.Length - 1; i++)
				{
					string nowLine = line[i];

					if (nowLine.StartsWith("[") && nowLine.EndsWith("]"))
					{
						if (data == null)
							data = new Hashtable();
						else
						{
							dataSet.addData(tag, data);
							data = new Hashtable();
						}

						tag = nowLine.Substring(1, nowLine.Length - 2);
						continue;
					}

					try
					{
						string[] temp = nowLine.Split('=');

						data.Add(temp[0], temp[1]);
					}
					catch (IndexOutOfRangeException) { }
				}
				dataSet.addData(tag, data);
			}
			else
			{
				dataSet = null;
			}
		}

		public static void saveData(SaveFileDataSet dataSet)
		{
			Guid guid = Guid.NewGuid();

			string[] temp = guid.ToString().Split('-');
			string uuid = "";

			for (int i = 0; i < temp.Length; i++)
			{
				uuid += temp[i];
			}

			string raw_data = "";
			foreach (string tag_key in dataSet.getDataKey())
			{
				raw_data += String.Format("[{0}]\n", tag_key);
				Hashtable data = dataSet.getData(tag_key);

				foreach (object key in data.Keys)
				{
					raw_data += String.Format("{0}={1}\n", key, data[key.ToString()]);
				}
			}
			string output = uuid + AES256_manager.encrypt(raw_data, uuid);


            Directory.CreateDirectory(saveFileDir);
            string dirTemp = Path.Combine(saveFileDir, saveFileName);
            if (File.Exists(dirTemp))
            {
                File.Delete(dirTemp);
            }

            

            StreamWriter writer = new StreamWriter(dirTemp);



            //byte[] byteArr = Encoding.UTF8.GetBytes(output);

            //File.WriteAllBytes(saveFileName, byteArr);

            writer.Write(output);
			writer.Close();
		}
	}
}