using UnityEngine;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SaveSystem
{
    public static class SaveSystem
    {
        private const string KEYWORD = "DNAS*&1i nx712asas0-1==a a";

        public static object LoadData<T>(string fileName, bool encrypt = true)
        {
            string path = GetFilePath(fileName);

            if (File.Exists(path))
            {
                try
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };

                    string json = File.ReadAllText(path);
                    if (encrypt)
                        json = EncryptDecrypt(json);
                    T data = JsonConvert.DeserializeObject<T>(json, settings);
                    return data;
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to load data from " + path + " with exception: " + e);
                    return null;
                }
            }
            else
            {
                Debug.LogWarning("Save file not found at " + path);
                return null;
            }
        }

        public static void SaveData(object data, string fileName, bool encrypt = true)
        {
            string path = GetFilePath(fileName);

            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                string json = JsonConvert.SerializeObject(data, settings);
                if (encrypt)
                    json = EncryptDecrypt(json);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save data to " + path + " with exception: " + e);
            }
        }

        private static string EncryptDecrypt(string data)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                result.Append((char)(data[i] ^ KEYWORD[i % KEYWORD.Length]));
            }

            return result.ToString();
        }

        public static void DeleteData(string fileName)
        {
            string path = GetFilePath(fileName);

            if (File.Exists(path))
                File.Delete(path);
        }

        private static string GetFilePath(string fileName)
        {
            return $"{Application.persistentDataPath}/{fileName}.json";
        }

        public static bool SaveDataExists(string fileName)
        {
            return File.Exists(GetFilePath(fileName));
        }
    }
}