using System.Collections;
using System.Collections.Generic;

namespace MelloRin.FileManager
{
	public class DataSet
	{
		private Dictionary<string, Hashtable> dataSet = new Dictionary<string, Hashtable>();

		public Hashtable getData(string key)
		{
			try
			{
				return dataSet[key];
			}
			catch (KeyNotFoundException)
			{
				throw new DatasetException();
			}
		}

		public void addData(string key, Hashtable data) => dataSet.Add(key, data);

		public Dictionary<string, Hashtable>.KeyCollection getDataKey() => dataSet.Keys;
	}
}