using BetterConsoles.Tables;
using BetterConsoles.Tables.Configuration;
using BetterConsoles.Tables.Models;
using Core.Models;

namespace Core.Helpers
{
    public static class OutputProvider
    {
        public static void Print(this IEnumerable<double[]> matrix)
        {
            foreach (var row in matrix)
            {
                foreach (var element in row) Console.Write(element.ToString("0.00").PadLeft(8) + "\t");
                Console.WriteLine();
            }
        }

        public static void Print(this IEnumerable<double> matrix)
        {
            foreach (var element in matrix)
            {
                Console.Write(element.ToString("0.00").PadLeft(8) + "\t");
            }
            Console.WriteLine();
        }

        public static void PrintAsTable(this List<Member> members)
        {
            IColumn[] columns = [
                new Column("Id", new CellFormat(Alignment.Center), new CellFormat(Alignment.Center)),
                new Column("Name", new CellFormat(Alignment.Center), new CellFormat(Alignment.Left))
            ];
            var table = new Table(columns)
            {
                Config = TableConfig.UnicodeAlt()
            };

            members.ForEach(member => table.AddRow(member.Id, member.Name));
            if (members.Count == 0) table.AddRow("*", "No members recorded");

            Console.WriteLine(table.ToString());
        }

        public static void PrintAsTable(this List<Expense> expenses, List<Member> members)
        {
            IColumn[] columns = [
                new Column("By", new CellFormat(Alignment.Center), new CellFormat(Alignment.Left)),
                new Column("Head", new CellFormat(Alignment.Center), new CellFormat(Alignment.Left)),
                new Column("Amount", new CellFormat(Alignment.Center), new CellFormat(Alignment.Right))
            ];
            var table = new Table(columns)
            {
                Config = TableConfig.UnicodeAlt()
            };
            expenses.ForEach(expense => table.AddRow(members.First(x => x.Id == expense.By).Id + ": " + members.First(x => x.Id == expense.By).Name, expense.Head, expense.Amount));
            if (expenses.Count == 0) table.AddRow("*", "No expenses recorded", "*");

            Console.WriteLine(table.ToString());
        }

        public static void Print(this List<ExpenseDistribution> distributions, string type)
        {
            Console.WriteLine($"Expense distributions ({type}):");
            foreach (var distribution in distributions) Console.WriteLine($"\t{distribution.Sender.Name} => {distribution.Receiver.Name} [{distribution.Amount}]");
        }

        public static void PrintHorizontalRule(char lineCharacter = '-')
        {
            var consoleWidth = Console.WindowWidth;
            var line = new string(lineCharacter, consoleWidth);
            Console.WriteLine(line);
        }
    }
}
