using Core.Helpers;
using Core.Models;
using Sharprompt;
using SystemTextJsonHelper;

var useArtifact = Prompt.Confirm("Do you want to use previous artifact?", false);
var artifactName = InputHelper.GetArtifactName(useArtifact);

if (useArtifact)
{
    Console.WriteLine($"Using artifact: {artifactName}");
    Console.WriteLine("Available data from artifact will be set as default values.");
}

var artifact = useArtifact
    ? JsonHelper.Deserialize<Artifact>(File.ReadAllText(artifactName)) ?? new Artifact()
    : new Artifact {Name = artifactName};

artifact.GetGroupMembers();
if (artifact.Members.Count <= 1)
{
    Console.WriteLine("Please add at least 2 members to proceed.");
    return;
}
artifact.GetExpenses();
if (artifact.Expenses.Sum(x =>  x.Amount) == 0)
{
    Console.WriteLine("Please add at least 1 non-zero expense to proceed.");
    return;
}

// Step 0: Raw data of total expenses
var perMemberTotal = SplitHelper.GetPerMemberTotal(artifact.Expenses, artifact.Members);

// Step 1: Sort the raw data based on expense [Ascending]
perMemberTotal = perMemberTotal.OrderBy(x => x.Amount).ToList();
artifact.Members = artifact.Members.OrderBy(x => perMemberTotal.First(y => y.By == x.Id).Amount).ToList();

// Step 2: Form a per-member split matrix A
var perMemberSplit = SplitHelper.GetPerMemberSplit(perMemberTotal);


// Step 3: Transpose the per-member split matrix A to create new matrix A'
var transposedPerMemberSplit = perMemberSplit.Transpose();

// Step 4: Create a new matrix B such that B = A-A'
var subtractedPerMemberSplit = perMemberSplit.Subtract(transposedPerMemberSplit);

// Step 5: Create another matrix C such that C = Max(0, B)
var reducedPerMemberSplit = subtractedPerMemberSplit.Max(subtractedPerMemberSplit.CreateFlatMatrix(0));

// Step 6: Create new reduced 1*X transaction matrix D such as D[i] = Sum(Row[i]) - Sum(Col[i])
var transactions = SplitHelper.ReduceAndGetTransactionMatrix(reducedPerMemberSplit);

// Step 7: Every i in matrix D will pay i+1 X amount where X = (D1i + D1i-1)
var finalDistributions = SplitHelper.GetDistributionsFromTransactions(transactions, artifact.Members);


// Only used to print
var perMemberSplitDistributions = SplitHelper.GetDistributions(perMemberSplit, artifact.Members);
var reducedPerMemberSplitDistributions = SplitHelper.GetDistributions(reducedPerMemberSplit, artifact.Members);

// To Pretty Print calculations
OutputProvider.PrintHorizontalRule();
OutputProvider.PrintHorizontalRule();
Console.WriteLine();
Console.WriteLine("Members:");
artifact.Members.OrderBy(x => x.Id).ToList().PrintAsTable();
Console.WriteLine("Expenses:");
artifact.Expenses.OrderBy(x => x.By).ThenBy(x => x.Head).ThenBy(x => x.Amount).ToList().PrintAsTable(artifact.Members);
Console.WriteLine("Per Member Total Expenses:");
perMemberTotal.OrderBy(x => x.Amount).ToList().PrintAsTable(artifact.Members);
OutputProvider.PrintHorizontalRule();
Console.WriteLine();
Console.WriteLine("Expense Matrix (Split):");
perMemberSplit.Print();
Console.WriteLine();
perMemberSplitDistributions.Print("Per Member Split");
Console.WriteLine();
OutputProvider.PrintHorizontalRule();
Console.WriteLine();
Console.WriteLine("Expense Matrix (Reduced):");
reducedPerMemberSplit.Print();
Console.WriteLine();
reducedPerMemberSplitDistributions.Print("Per Member Reduced");
Console.WriteLine();
OutputProvider.PrintHorizontalRule();
Console.WriteLine();
Console.WriteLine("Transaction Matrix:");
transactions.Print();
Console.WriteLine();
finalDistributions.Print("Final");
Console.WriteLine();
OutputProvider.PrintHorizontalRule();
OutputProvider.PrintHorizontalRule();
