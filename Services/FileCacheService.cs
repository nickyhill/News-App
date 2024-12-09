using System.Text.Json;

namespace CyberNewsApp.Services
{
    public class FileService
    {
        private string GetFilePath(string fileName) => Path.Combine(FileSystem.AppDataDirectory, fileName);

        public async Task SaveToFileAsync<T>(string fileName, T data)
        {
            var filePath = GetFilePath(fileName);
            var jsonData = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(filePath, jsonData);
        }

        public async Task<T?> LoadFromFileAsync<T>(string fileName)
        {
            var filePath = GetFilePath(fileName);
            if (File.Exists(filePath))
            {
                var jsonData = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            return default;
        }

        public void DeleteFile(string fileName)
        {
            var filePath = GetFilePath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

