using Core.Helpers;
using Core.Models;
using Console = Core.Helpers.Console;

// Initial Env Setup
var useArtifact = Console.GetBool("Do you want to use previous run artifact? ", "no");
var artifactFolder = useArtifact ? Console.GetString("Provide name of the folder: ", "artifacts") : FileHelper.CreateTimestampedFolderAndGetName("artifact");

// Only used to get Raw data
var groupSize = FileHelper.ChooseArtifactOrGet(artifactFolder, "groupSize", DataCollector.GetGroupSize);
var members = FileHelper.ChooseArtifactOrGet(artifactFolder, "members", () => DataCollector.GetGroupMembers(groupSize));
var expenses = FileHelper.ChooseArtifactOrGet<List<Expense>>(artifactFolder, "expenses", prev =>
{
    prev.AddRange(DataCollector.RecordExpenses(members, prev.Count));
    return prev;
});


// Step 0: Raw data of total expenses
var perMemberTotal = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "perMemberTotal", () => SplitHelper.GetPerMemberTotal(expenses, members));
// Step 1: Sort the raw data based on expense [Ascending]
perMemberTotal = perMemberTotal.OrderBy(x => x.Amount).ToList();
members.ForEach(x =>
{
    x.Id = perMemberTotal.Select(y => y.By).ToList().IndexOf(x);
});
// Step 2: Form a per-member split matrix A
var perMemberSplit = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "perMemberSplit", () => SplitHelper.GetPerMemberSplit(perMemberTotal));
// Step 3: Transpose the per-member split matrix A to create new matrix A'
var transposedPerMemberSplit = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "transposedPerMemberSplit", () => perMemberSplit.Transpose());
// Step 4: Create a new matrix B such that B = A-A'
var subtractedPerMemberSplit = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "subtractedPerMemberSplit", () => perMemberSplit.Subtract(transposedPerMemberSplit));
// Step 5: Create another matrix C such that C = Max(0, B)
var reducedPerMemberSplit = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "reducedPerMemberSplit", () => subtractedPerMemberSplit.Max(subtractedPerMemberSplit.CreateFlatMatrix(0)));
// Step 6: Create new reduced 1*X transaction matrix D such as D[i] = Sum(Row[i]) - Sum(Col[i])
var transactions = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "transactions", () => SplitHelper.ReduceAndGetTransactionMatrix(reducedPerMemberSplit));
// Step 7: Every i in matrix D will pay i+1 X amount where X = (D1i + D1i-1)
var finalDistributions = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "finalDistributions", () => SplitHelper.GetDistributionsFromTransactions(transactions, members));


// Only used to print
var perMemberSplitDistributions = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "perMemberSplitDistributions", () => SplitHelper.GetDistributions(perMemberSplit, members));
var reducedPerMemberSplitDistributions = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "reducedPerMemberSplitDistributions", () => SplitHelper.GetDistributions(reducedPerMemberSplit, members));

// To Pretty Print calculations
Console.PrintHorizontalRule();
Console.PrintHorizontalRule();
OutputProvider.PrintExpenses("all", expenses);
Console.PrintHorizontalRule();
Console.PrintHorizontalRule();
OutputProvider.PrintExpenses("total", perMemberTotal);
Console.PrintHorizontalRule();
Console.PrintHorizontalRule();
OutputProvider.PrintExpenseMatrix("Split", perMemberSplit);
Console.PrintHorizontalRule();
OutputProvider.PrintDistributionInfo("per member split", perMemberSplitDistributions);
Console.PrintHorizontalRule();
Console.PrintHorizontalRule();
OutputProvider.PrintExpenseMatrix("Reduced", reducedPerMemberSplit);
Console.PrintHorizontalRule();
OutputProvider.PrintDistributionInfo("per member reduced", reducedPerMemberSplitDistributions);
Console.PrintHorizontalRule();
Console.PrintHorizontalRule();
OutputProvider.PrintTransactionMatrix(transactions);
Console.PrintHorizontalRule();
OutputProvider.PrintDistributionInfo("final", finalDistributions);
Console.PrintHorizontalRule();
Console.PrintHorizontalRule();
