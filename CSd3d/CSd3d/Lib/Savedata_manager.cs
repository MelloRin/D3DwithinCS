using System.Collections;
using FileManager;
using System.Collections.Generic;

namespace CSd3d.Lib
{
    public class Savedata_manager
    {
        public static readonly string[] music_name = new string[] { "a", "b", "c" };

        private Hashtable scoreTable = new Hashtable();

        public bool setScore(DataSet dataSet)
        {
            try
            {
                scoreTable = dataSet.getdata("Score").hashtable;
            }
            catch (KeyNotFoundException e)
            {

                for (int i = 0; i < music_name.Length; i++)
                {

                    scoreTable.Add(music_name[i], 0);

                }
            }

            return true;
        }

        public bool updateScore(string music_key, int score)
        {
            if (scoreTable.ContainsKey(music_key))
                scoreTable[music_key] = score;
            else
                scoreTable.Add(music_key, score);

            return true;
        }

        public Hashtable getScoretable() => scoreTable;
    }
}
