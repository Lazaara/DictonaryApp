namespace WinFormsApp2
{
    public class Dictionary
    {
        private List<string> keys;
        private List<string> values;

        public Dictionary()
        {
            keys = new List<string>();
            values = new List<string>();
        }
        
        public void Add(string key, string value)
        {
            int index = keys.IndexOf(key);
            if (index != -1)
            {
                values[index] = value;
            }
            else
            {
                keys.Add(key);
                values.Add(value);
            }
        }

        public void Remove(string key)
        {
            int index = keys.IndexOf(key);
            if (index != -1)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
            else
            {
                Console.WriteLine("Key not found.");
            }
        }

        public string GetValue(string key)
        {
            int index = keys.IndexOf(key);
            if (index != -1)
            {
                return values[index];
            }
            return null;
        }
        
        public bool ContainsKey(string key)
        {
            return keys.Contains(key);
        }

    }
}