using System.ComponentModel.DataAnnotations;
using Core.Models;
using Sharprompt;
using SystemTextJsonHelper;

namespace Core.Helpers
{
    public abstract class InputHelper
    {
        public static string GetArtifactName(bool usePreviousArtifact)
        {
            return !usePreviousArtifact
                ? "artifact-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json"
                : Prompt.Input<string>(
                    "Provide name of the artifact:",
                    "artifact.json",
                    "artifact.json",
                    new[]
                    {
                        Validators.Required(),
                        name => name is string s && File.Exists(s) ? ValidationResult.Success : new ValidationResult("File does not exist."),
                        name => name is string s && s.EndsWith(".json") ? ValidationResult.Success : new ValidationResult("File must be a json file."),
                        name => {
                            try{
                                var jsonContent = File.ReadAllText(name as string ?? string.Empty);
                                _ = JsonHelper.Deserialize<Artifact>(jsonContent) ?? throw new Exception();
                                return ValidationResult.Success;
                            } catch (Exception) {
                                return new ValidationResult($"{name} is not a valid artifact json file.");
                            }
                        }
                    }
                );
        }
    }
}
