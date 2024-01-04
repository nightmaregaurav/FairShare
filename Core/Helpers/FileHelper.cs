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

        public static bool FileExists(string folder, string filename)
        {
            var filePath = Path.Combine(folder, filename);
            return File.Exists(filePath);
        }

        public static void StoreDataAsJson<T>(string folder, string filename, T data)
        {
            Directory.CreateDirectory(folder);
            var jsonRepresentation = JsonHelper.Serialize(new JsonNode<T>{ Value = data });
            File.WriteAllText(Path.Combine(folder, filename), jsonRepresentation);
        }

        public static T ReadDataFromJson<T>(string folder, string filename)
        {
            var filePath = Path.Combine(folder, filename);
            var jsonContent = File.ReadAllText(filePath);
            var jsonResult = JsonHelper.Deserialize<JsonNode<T>>(jsonContent) ?? throw new Exception($"Cannot read {folder}/{filename} as json.");
            return jsonResult.Value;
        }

        public static T ChooseArtifactOrGet<T>(string folder, string filename, Func<T, T> appendValuesToArtifact) where T : new()
        {
            if (!FileExists(folder, filename)) return SaveResultAsArtifactAndGet(folder, filename, () => appendValuesToArtifact(new T()));

            var useArtifact = Console.GetBool(
                $"An artifact {folder}/{filename} already exists. Do you want to load data from it? (yes/no)",
                "yes"
            );
            if (!useArtifact) return SaveResultAsArtifactAndGet(folder, filename, () => appendValuesToArtifact(new T()));

            var initialValue = ReadDataFromJson<T>(folder, filename);
            return appendValuesToArtifact(initialValue);
        }

        public static T ChooseArtifactOrGet<T>(string folder, string filename, Func<T> ifUserDeny)
        {
            if (!FileExists(folder, filename)) return SaveResultAsArtifactAndGet(folder, filename, ifUserDeny);

            var useArtifact = Console.GetBool(
                $"An artifact {folder}/{filename} already exists. Do you want to load data from it? (yes/no)",
                "yes"
            );
            if (!useArtifact) return SaveResultAsArtifactAndGet(folder, filename, ifUserDeny);

            var value = ReadDataFromJson<T>(folder, filename);
            return value;
        }

        public static T SaveResultAsArtifactAndGet<T>(string folder, string filename, Func<T> ifUserDeny)
        {
            var valueFromFunc = ifUserDeny();
            StoreDataAsJson(folder, filename, valueFromFunc);
            return valueFromFunc;
        }
    }
}
