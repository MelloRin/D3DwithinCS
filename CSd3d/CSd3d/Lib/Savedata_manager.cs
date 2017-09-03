using MelloRin.FileManager;
using System.Collections;

namespace MelloRin.CSd3d.Lib
{
    public class Savedata_manager
    {
        private Hashtable scoreTable = new Hashtable();
        public Hashtable getScoretable() => scoreTable;

        public static readonly string[] music_name = new string[] { "a", "b", "c" };

        public Savedata_manager(DataSet dataSet)
        {
            try
            {
                scoreTable = dataSet.getdata("Score");
            }
            catch (DatasetException)
            {
				for (int i = 0; i < music_name.Length; i++) 
                {
					scoreTable.Add(music_name[i], i);
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