using Core.Extensions;
using Core.Helpers;


var useArtifact = UserInputHelper.GetBoolInput("Do you want to use previous run artifact? ", "no");
var artifactFolder = useArtifact ? UserInputHelper.GetStringInput("Provide name of the folder: ", "artifacts") : FileHelper.CreateTimestampedFolderAndGetName("artifact");

var groupSize = FileHelper.ChooseArtifactOrGet(artifactFolder, "groupSize", DataCollector.GetGroupSize);
var members = FileHelper.ChooseArtifactOrGet(artifactFolder, "members", () => DataCollector.GetGroupMembers(groupSize.Total));
var expenses = FileHelper.ChooseArtifactOrGet(artifactFolder, "expenses", () => DataCollector.RecordExpenses(members), true);

var pmTotal = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "pmTotal", () => SplitHelper.GetPerMemberTotal(expenses, members));
var pmSplit = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "pmSplit", () => SplitHelper.GetPerMemberSplit(pmTotal));
var reducedPmSplit = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "reducedPmSplit", () => SplitHelper.ReduceSplitMatrix(pmSplit.Copy()));

var transposePmSplit = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "transposePmSplit", () => pmSplit.Transpose());
var pmSplitDistributions = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "distributions", () => SplitHelper.GetDistributions(transposePmSplit.Copy(), members));

var transposeReducedSplit = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "transposePmSplit", () => reducedPmSplit.Transpose());
var reducedSplitDistributions = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "distributions", () => SplitHelper.GetDistributions(transposeReducedSplit.Copy(), members));

var linearReduce = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "linearReduce", () => SplitHelper.LinearSplitReduce(transposeReducedSplit.Copy()));
var finalDistributions = FileHelper.SaveResultAsArtifactAndGet(artifactFolder, "distributions", () => SplitHelper.GetDistributions(linearReduce.Copy(), members));

OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintExpenses("all", expenses);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintExpenses("total", pmTotal);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintMatrix("Split", pmSplit);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintDistributionInfo("per member split", pmSplitDistributions);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintMatrix("Reduced", reducedPmSplit);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintDistributionInfo("per member reduced", reducedSplitDistributions);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintMatrix("Transpose", transposeReducedSplit);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintMatrix("Linear", linearReduce);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintDistributionInfo("final", finalDistributions);
OutputProvider.PrintHorizontalRule('-');
OutputProvider.PrintHorizontalRule('-');
