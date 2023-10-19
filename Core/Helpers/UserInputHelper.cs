using System.Text.RegularExpressions;

namespace Core.Helpers
{
    public static class UserInputHelper
    {

        public static bool GetBoolInput(string prompt, string? defaultValue = "")
        {
            const string regex = "[yes|no|true|false|Yes|No|True|False|YES|NO|TRUE|FALSE|y|n|t|f|Y|N|T|F]";
            var booleanTrueValues = new List<string> {"true", "yes", "y", "t"};
            var booleanFalseValues = new List<string> {"false", "no", "n", "f"};
            var value = GetUserInput(prompt, defaultValue, LogicalValidation, regex);
            return booleanTrueValues.Contains(value.ToLowerInvariant());

            bool LogicalValidation(string v) => booleanTrueValues.Contains(v.ToLowerInvariant()) || booleanFalseValues.Contains(v.ToLowerInvariant());
        }

        public static string GetStringInput(string prompt, string? defaultValue = "")
        {
            var value = GetUserInput(prompt, defaultValue);
            return value;
        }

        public static int GetNumberInput(string prompt, string? defaultValue = "")
        {
            const string regex = @"\d+";
            var value = GetUserInput(prompt, defaultValue, LogicalValidation, regex);
            return int.Parse(value);

            bool LogicalValidation(string v) => int.TryParse(v, out _);
        }

        public static double GetDecimalInput(string prompt, string? defaultValue = "")
        {
            const string regex = @"\d+(.\d+)?";
            var value = GetUserInput(prompt, defaultValue, LogicalValidation, regex);
            return double.Parse(value);

            bool LogicalValidation(string v) => double.TryParse(v, out _);
        }

        private static bool True(string _) => true;
        private static string GetUserInput(string prompt, string? defaultValue = "", Func<string, bool>? logicalValidation = null, string regex = ".*")
        {
            logicalValidation ??= True;

            prompt += string.IsNullOrWhiteSpace(defaultValue) ? " >>> " : $" [default: {defaultValue}] >>> ";
            var input = defaultValue ?? "";
            var firstTime = true;
            while (firstTime || !(Regex.IsMatch(input, regex) && logicalValidation(input)))
            {
                if (!firstTime) Console.WriteLine($"Invalid input `{input}` for pattern `{regex}`. Please try again.");
                firstTime = false;
                Console.Write(prompt);
                var userInput = Console.ReadLine() ?? "";
                input = string.IsNullOrWhiteSpace(userInput) ? defaultValue ?? "" : userInput;
            }
            return input.Trim();
        }
    }
}
