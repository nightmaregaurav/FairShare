using System.Text.RegularExpressions;

namespace Core.Helpers
{
    public static class Console
    {
        public static void PrintHorizontalRule(char lineCharacter = '-')
        {
            var consoleWidth = System.Console.WindowWidth;
            var line = new string(lineCharacter, consoleWidth);
            System.Console.WriteLine(line);
        }

        public static bool GetBool(string prompt, string? defaultValue = "")
        {
            const string regex = "[yes|no|true|false|Yes|No|True|False|YES|NO|TRUE|FALSE|y|n|t|f|Y|N|T|F]";
            var booleanTrueValues = new List<string> {"true", "yes", "y", "t"};
            var booleanFalseValues = new List<string> {"false", "no", "n", "f"};
            var value = GetInput(prompt, defaultValue, LogicalValidation, regex);
            return booleanTrueValues.Contains(value.ToLowerInvariant());

            bool LogicalValidation(string v) => booleanTrueValues.Contains(v.ToLowerInvariant()) || booleanFalseValues.Contains(v.ToLowerInvariant());
        }

        public static string GetString(string prompt, string? defaultValue = "")
        {
            var value = GetInput(prompt, defaultValue);
            return value;
        }

        public static int GetNumber(string prompt, string? defaultValue = "")
        {
            const string regex = @"\d+";
            var value = GetInput(prompt, defaultValue, LogicalValidation, regex);
            return int.Parse(value);

            bool LogicalValidation(string v) => int.TryParse(v, out _);
        }

        public static double GetDecimal(string prompt, string? defaultValue = "")
        {
            const string regex = @"\d+(.\d+)?";
            var value = GetInput(prompt, defaultValue, LogicalValidation, regex);
            return double.Parse(value);

            bool LogicalValidation(string v) => double.TryParse(v, out _);
        }

        private static bool True(string _) => true;
        private static string GetInput(string prompt, string? defaultValue = "", Func<string, bool>? logicalValidation = null, string regex = ".*")
        {
            logicalValidation ??= True;

            prompt += string.IsNullOrWhiteSpace(defaultValue) ? " >>> " : $" [default: {defaultValue}] >>> ";
            var input = defaultValue ?? "";
            var firstTime = true;
            while (firstTime || !(Regex.IsMatch(input, regex) && logicalValidation(input)))
            {
                if (!firstTime) System.Console.WriteLine($"Invalid input `{input}` for pattern `{regex}`. Please try again.");
                firstTime = false;
                System.Console.Write(prompt);
                var userInput = System.Console.ReadLine() ?? "";
                input = string.IsNullOrWhiteSpace(userInput) ? defaultValue ?? "" : userInput;
            }
            return input.Trim();
        }
    }
}
