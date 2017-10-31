using MelloRin.FileManager;
using System.Collections;

using DataSet = MelloRin.FileManager.DataSet;

namespace MelloRin.CSd3d.Lib
{
    public class SaveDataManager
    {
        private Hashtable scoreTable = new Hashtable();
        public Hashtable getScoretable() => scoreTable;

        public static readonly string[] musicName = new string[] { "a", "b", "c" };

        public SaveDataManager(DataSet dataSet)
        {
            try
            {
                scoreTable = dataSet.getData("Score");
            }
            catch (DatasetException)
            {
				for (int i = 0; i < musicName.Length; i++) 
                {
					scoreTable.Add(musicName[i], i);
                }
            }
        }

        public bool updateScore(string music_key, int score)
        {
            if (scoreTable.ContainsKey(music_key))
                scoreTable[music_key] = score;
            else
                scoreTable.Add(music_key, score);

            return true;
        }
    }
}