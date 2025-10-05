using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Batyr.Scripts
{
    public class SaveLoadSystem : Singleton<SaveLoadSystem>
    {
        public void Save<T>(T data, string fileName)
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ColorConverter());
            
            string json = JsonConvert.SerializeObject(data, settings);

            string path = GetSavePath(fileName);
            File.WriteAllText(path, json);
            Debug.Log($"Saved by path: {path}");
        }

        // Загрузка данных любого типа
        public T Load<T>(string fileName)
        {
            string path = GetSavePath(fileName);

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }

            Debug.LogWarning("Save file not found in " + path);
            return default; // null для ссылочных типов, 0 для чисел и т.д.
        }

        // Вспомогательный метод для получения пути к файлу
        private string GetSavePath(string fileName)
        {
            // Убедимся, что имя файла имеет расширение
            if (!fileName.EndsWith(".json"))
                fileName += ".json";

            return Path.Combine(Application.persistentDataPath, fileName);
        }
        
        // Удаление файла сохранения
        public bool Delete(string fileName)
        {
            string path = GetSavePath(fileName);

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    Debug.Log($"Deleted file: {path}");
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error deleting file: {e.Message}");
                    return false;
                }
            }

            Debug.LogWarning($"File not found: {path}");
            return false;
        }
    }
}