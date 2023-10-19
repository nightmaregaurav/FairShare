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
                    if (i == j)
                        splitExpenses[i][j] = 0.0;
                    else
                        splitExpenses[i][j] = split;
                }
            }

            return splitExpenses;
        }

        public static double[][] ReduceSplitMatrix(double[][] matrix)
        {
            var matrixSize = matrix.Length;
            for (var i = 0; i < matrixSize; i++)
            {
                var senderArraySize = matrix[i].Length;
                for (var j = 0; j < senderArraySize; j++)
                {
                    if (!(matrix[i][j] > matrix[j][i]) || matrix[j][i] == 0) continue;

                    matrix[i][j] -= matrix[j][i];
                    matrix[j][i] = 0.0;
                }
            }
            return matrix;
        }

        public static int CountZeros(IEnumerable<double> row) => row.Count(val => val == 0);

        public static double[][] LinearSplitReduce(double[][] matrix)
        {
            var assignmentOrderDict = new Dictionary<int, int>();
            for (var index = 0; index < matrix.Length; index++)
            {
                var row = matrix[index];
                assignmentOrderDict.Add(index, CountZeros(row));
            }

            var assignmentOrder = assignmentOrderDict.OrderBy(x => x.Value).ToList();

            for (var index = 0; index < assignmentOrder.Count; index++)
            {
                if (index == assignmentOrder.Count - 1 || index == assignmentOrder.Count - 2) continue;

                var i = assignmentOrder[index].Key;
                var assignmentTargetIndex = assignmentOrder[index + 1].Key;
                var allAmountToPay = matrix[i].Sum();

                for (var j = 0; j < matrix[i].Length; j++)
                {
                    if (j == assignmentTargetIndex) matrix[i][j] = allAmountToPay;
                    else
                    {
                        var replacement = matrix[i][j];
                        matrix[i][j] = 0.0;
                        matrix[assignmentTargetIndex][j] += replacement;
                    }
                }
            }

            return matrix;
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
    }
}
