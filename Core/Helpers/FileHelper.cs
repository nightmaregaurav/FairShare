using System.Collections;
using Core.Types;
using SystemTextJsonHelper;

namespace Core.Helpers
{
    public abstract class FileHelper
    {
        public static string CreateTimestampedFolderAndGetName(string prefix = "", string suffix = "")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var folderName = $"{prefix}-{timestamp}-{suffix}";
            folderName = folderName.Trim().Trim('-');
            Directory.CreateDirectory(folderName);
            return folderName;
        }

        public static void StoreDataAsJson<T>(string folder, string filename, T data)
        {
            Directory.CreateDirectory(folder);
            var jsonRepresentation = JsonHelper.Serialize(new JsonNode<T>{ Value = data });
            File.WriteAllText(Path.Combine(folder, filename), jsonRepresentation);
        }

        public static T? ReadJsonData<T>(string folder, string filename)
        {
            var filePath = Path.Combine(folder, filename);
            if (!File.Exists(filePath)) return default;
            var jsonContent = File.ReadAllText(filePath);
            var jsonResult = JsonHelper.Deserialize<JsonNode<T>>(jsonContent);
            return jsonResult != null ? jsonResult.Value : default;
        }

        public static T ChooseArtifactOrGet<T>(string folder, string filename, Func<T> ifUserDeny, bool rePrompt = false) where T : class
        {
            var filePath = Path.Combine(folder, filename);
            if (!File.Exists(filePath)) return SaveResultAsArtifactAndGet(folder, filename, ifUserDeny);

            var useArtifact = UserInputHelper.GetBoolInput(
                $"An artifact {folder}/{filename} already exists. Do you want to load data from it? (yes/no)",
                "yes"
            );
            if (!useArtifact) return SaveResultAsArtifactAndGet(folder, filename, ifUserDeny);

            var value = ReadJsonData<T>(folder, filename);
            if (value == null) Console.WriteLine("Failed to retrieve data from artifact!");
            else if(!rePrompt) return value;

            return SaveResultAsArtifactThenGet(folder, filename, ifUserDeny, value);
        }

        public static T SaveResultAsArtifactAndGet<T>(string folder, string filename, Func<T> ifUserDeny) where T : class
        {
            var valueFromFunc = ifUserDeny();
            StoreDataAsJson(folder, filename, valueFromFunc);
            return valueFromFunc;
        }

        public static T SaveResultAsArtifactThenGet<T>(string folder, string filename, Func<T> ifUserDeny, T value) where T : class
        {
            var valueFromFunc = ifUserDeny();

            if (value is IList a)
            {
                var actualValueFromFunc = valueFromFunc as IList;
                foreach (var entry in a) actualValueFromFunc?.Add(entry);
                StoreDataAsJson(folder, filename, actualValueFromFunc);
                return actualValueFromFunc as T ?? valueFromFunc;
            }

            StoreDataAsJson(folder, filename, valueFromFunc);
            return valueFromFunc;
        }
    }
}
