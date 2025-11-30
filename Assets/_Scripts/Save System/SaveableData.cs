using System.Collections.Generic;

namespace SaveSystem
{
    /// <summary>
    /// Example of data which can be saved with <see cref="SaveSystem"/>.
    /// </summary>
    [System.Serializable]
    public class SaveableData
    {
        public Dictionary<string, object> SavedDatas;

        public SaveableData()
        {
            SavedDatas = new Dictionary<string, object>();
        }

        /// <summary>
        /// Caches the given object in dictionary.
        /// </summary>
        /// <param name="key">The unique name of the object.</param>
        /// <param name="obj">The object to be saved.</param>
        public void SaveObject(string key, object obj)
        {
            SavedDatas[key] = obj;
        }

        /// <summary>
        /// Retrieves the cached object from a dictionary.
        /// </summary>
        /// <param name="key">The unique name of saved object.</param>
        /// <returns>The cached object.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when given name is not present in cached dictionary.</exception>
        public object GetObject(string key)
        {
            if (!SavedDatas.ContainsKey(key))
                throw new KeyNotFoundException();

            return SavedDatas[key];
        }
    }
}
