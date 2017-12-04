using MelloRin.FileManager;
using System;
using System.Collections.Generic;
using System.IO;

namespace MelloRin.CSd3d.Lib.NoteData
{
	public class MinData
	{
		public Dictionary<int, SecData> secData { get; private set; }

		public MinData()
		{
			secData = new Dictionary<int, SecData>();
		}

		public void addMinData(int lineNum, int sec, int ms)
		{
			if (secData.ContainsKey(sec % 60))
			{
				secData[sec % 60].addSecData(lineNum, sec, ms);
			}
			else
			{
				secData.Add(sec, new SecData());
				secData[sec % 60].addSecData(lineNum,sec,ms);
			}
		}
	}

	public class SecData
	{
		public Dictionary<int, NoteData> msData { get; private set; }

		public SecData()
		{
			msData = new Dictionary<int, NoteData>();
		}

		public void addSecData(int lineNum, int sec, int ms)
		{
			msData.Add(ms, new NoteData(lineNum, sec, ms));
		}
	}

	public class NoteData
	{
		public int sec { get; }
		public int ms { get; }
		public int lineNum { get; }

		public NoteData(int lineNum, int sec, int ms)
		{
			this.lineNum = lineNum;
			this.sec = sec;
			this.ms = ms;
		}
	}

	public class NoteManager
	{
		public int noteCount { get; }
		public Dictionary<int, MinData> data { get; private set; }

		public NoteManager(string musicName)
		{
			data = new Dictionary<int, MinData>();
			string musicPath = Program.musicFileDir + musicName + ".mlr";

			if (File.Exists(musicPath))
			{
				StreamReader reader = new StreamReader(musicPath);

				string rawData = reader.ReadLine();
				reader.Close();
				string key = rawData.Substring(0, 32);
				rawData = rawData.Substring(32);

				string[] line = AES256_manager.decrypt(rawData, key).Split('\n');
				int temp = 0;
				foreach (string currentLine in line)
				{
					string[] data = currentLine.Split('/');

					try
					{
						int lineNum = Int32.Parse(data[0]);
						int min = Int32.Parse(data[1]);
						int sec = Int32.Parse(data[2]);
						int ms = Int32.Parse(data[3]);

						++temp;
						addData(lineNum, min, sec, ms);
					}
					catch (FormatException) { }
				}

				noteCount = temp;
			}
			else
			{
				throw new Exception();
			}
		}

		public void addData(int lineNum, int min, int sec, int ms)
		{
			if (data.ContainsKey(min))
			{
				data[min].addMinData(lineNum, (60 * min) + sec, ms);
			}
			else
			{
				data.Add(min, new MinData());
				data[min].addMinData(lineNum, (60 * min) + sec, ms);
			}
		}

		public SecData getSecData(int min, int sec)
		{
			if (data[min].secData.ContainsKey(sec))
			{
				return data[min].secData[sec];
			}
			else
			{
				return null;
			}
		}
	}
}