using Core.Models;

namespace Core.Helpers
{
    public static class OutputProvider
    {
        public static void PrintMatrix(string type, IEnumerable<double[]> matrix)
        {
            Console.WriteLine($"Expense Matrix ({type}):");
            foreach (var row in matrix)
            {
                foreach (var element in row) Console.Write(element.ToString("0.00").PadLeft(8) + "\t");
                Console.WriteLine();
            }
        }

        public static void PrintExpenses(string type, List<Expense> expenses)
        {
            Console.WriteLine($"Expenses ({type}):");
            foreach (var expense in expenses) Console.WriteLine($"\t{expense.Head}: {expense.Amount}  |  {expense.By.Name}");
        }

        public static void PrintDistributionInfo(string type, List<ExpenseDistribution> distributions)
        {
            Console.WriteLine($"Expense distributions ({type}):");
            foreach (var distribution in distributions) Console.WriteLine($"\t{distribution.Sender.Name} => {distribution.Receiver.Name} [{distribution.Amount}]");
        }

        public static void PrintHorizontalRule(char lineCharacter)
        {
            var consoleWidth = Console.WindowWidth;
            var line = new string(lineCharacter, consoleWidth);
            Console.WriteLine(line);
        }
    }
}
