using Core.Models;

namespace Core.Helpers
{
    public abstract class SplitHelper
    {
        public static List<Expense> GetPerMemberTotal(List<Expense> expenses, List<Member> groupMembers)
        {
            var expensesSum = new Dictionary<int, double>();
            for (var i = 0; i < groupMembers.Count; i++) expensesSum[i] = 0;
            foreach (var expense in expenses)
            {
                var by = expense.By;
                var total = expensesSum[by.Id];
                total += expense.Amount;
                expensesSum[by.Id] = total;
            }

            var totalExpenses = expensesSum.Keys.Select(by => new Expense
            {
                By = groupMembers.First(x => x.Id == by),
                Head = "Total",
                Amount = expensesSum[by]
            }).ToList();

            return totalExpenses;
        }

        public static double[][] GetPerMemberSplit(List<Expense> totalExpenses)
        {
            var totalMembers = totalExpenses.Count;

            var splitExpenses = new double[totalMembers][];
            for (var i = 0; i < totalMembers; i++) splitExpenses[i] = new double[totalMembers];

            for (var i = 0; i < totalMembers; i++)
            {
                var split = totalExpenses[i].Amount / totalMembers;

                for (var j = 0; j < totalMembers; j++)
                {
                    splitExpenses[i][j] = split;
                }
            }

            return splitExpenses;
        }

        public static double[] ReduceAndGetTransactionMatrix(double[][] matrix)
        {
            if (!IsSquareMatrix(matrix)) throw new ArgumentException("Input matrix must be square for creating a reduced matrix.");
            var rows = matrix.Length;
            var result = new double[rows];

            for (var i = 0; i < rows; i++)
            {
                var rowSum = matrix[i].Sum();
                var colSum = matrix.Sum(x => x[i]);
                result[i] = rowSum - colSum;
            }

            return result;
        }

        public static List<ExpenseDistribution> GetDistributionsFromTransactions(double[] transactions, List<Member> groupMembers)
        {
            var distributions = new List<ExpenseDistribution>();
            var sum = 0.0;
            for (var i = 0; i < transactions.Length-1; i++)
            {
                var distribution = new ExpenseDistribution
                {
                    Sender = groupMembers.First(x => x.Id == i),
                    Receiver = groupMembers.First(x => x.Id == i+1),
                    Amount = Math.Abs(sum + transactions[i])
                };
                distributions.Add(distribution);
                sum += transactions[i];
            }

            sum += transactions.Last();
            if (sum != 0) throw new Exception("Final sum must yield 0. Something went wrong.");
            return distributions.OrderBy(x => x.Sender.Id).ThenBy(x => x.Receiver.Id).ThenByDescending(x => x.Amount).ToList();
        }

        public static List<ExpenseDistribution> GetDistributions(double[][] linearSplit, List<Member> groupMembers)
        {
            var distributions = new List<ExpenseDistribution>();
            for (var i = 0; i < linearSplit.Length; i++)
            {
                for (var j = 0; j < linearSplit.Length; j++)
                {
                    if(linearSplit[i][j] == 0) continue;
                    var distribution = new ExpenseDistribution
                    {
                        Sender = groupMembers.First(x => x.Id == i),
                        Receiver = groupMembers.First(x => x.Id == j),
                        Amount = linearSplit[i][j]
                    };
                    distributions.Add(distribution);
                }
            }
            return distributions.OrderBy(x => x.Sender.Id).ThenBy(x => x.Receiver.Id).ThenByDescending(x => x.Amount).ToList();
        }

        public static bool IsSquareMatrix(double[][] matrix)
        {
            var rows = matrix.Length;
            if (rows == 0) return false;
            var columns = matrix[0].Length;
            return rows == columns;
        }
    }
}
