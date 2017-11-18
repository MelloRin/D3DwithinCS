using System.Collections;
using System.Collections.Generic;

namespace MelloRin.FileManager
{
	public class SaveFileDataSet
	{
		private Dictionary<string, Hashtable> dataSet = new Dictionary<string, Hashtable>();

		public Hashtable getData(string key)
		{
			if(dataSet.ContainsKey(key))
			{
				return dataSet[key];
			}

			return null;
		}

		public void addData(string key, Hashtable data) => dataSet.Add(key, data);

		public Dictionary<string, Hashtable>.KeyCollection getDataKey() => dataSet.Keys;
	}
}