using System.Collections.Generic;

namespace FileManager
{
    public class DataSet
    {
        private Dictionary<string, Data> dataSet = new Dictionary<string, Data>();

        public Data getdata(string key) => dataSet[key];
        public void adddata(string key, Data data) => dataSet.Add(key, data);
        public Dictionary<string, Data>.KeyCollection getdataKey() => dataSet.Keys;
    }
}