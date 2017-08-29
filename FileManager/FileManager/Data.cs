using System.Collections;

namespace FileManager
{
    public class Data
    {
        public Hashtable hashtable = new Hashtable();

        public object getdata(string key) => hashtable[key];
        public void adddata(string key, string value) => hashtable.Add(key, value);

        public ICollection getKey => hashtable.Keys;
    }
}