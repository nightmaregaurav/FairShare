using Core.Models;

namespace Core.Helpers
{
    public static class OutputProvider
    {
        public static void PrintExpenseMatrix(string type, IEnumerable<double[]> matrix)
        {
            System.Console.WriteLine($"Expense Matrix ({type}):");
            matrix.Print();
        }

        public static void PrintTransactionMatrix(IEnumerable<double> matrix)
        {
            System.Console.WriteLine("Transaction Matrix:");
            matrix.Print();
        }

        public static void PrintExpenses(string type, List<Expense> expenses)
        {
            System.Console.WriteLine($"Expenses ({type}):");
            foreach (var expense in expenses) System.Console.WriteLine($"\t{expense.Head}: {expense.Amount}  |  {expense.By.Name}");
        }

        public static void PrintDistributionInfo(string type, List<ExpenseDistribution> distributions)
        {
            System.Console.WriteLine($"Expense distributions ({type}):");
            foreach (var distribution in distributions) System.Console.WriteLine($"\t{distribution.Sender.Name} => {distribution.Receiver.Name} [{distribution.Amount}]");
        }
    }
}
