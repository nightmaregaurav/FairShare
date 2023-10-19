using System.Text;
using Core.Models;
using Core.Types;

namespace Core.Helpers
{
    public abstract class DataCollector
    {
        public static Size GetGroupSize() => new()
        {
            Total = UserInputHelper.GetNumberInput("Enter the number of people in the group: ", "2")
        };

        public static List<Member> GetGroupMembers(int groupSize)
        {
            var groupMemberNames = new List<Member>();
            for (var i = 0; i < groupSize; i++)
            {
                var name = UserInputHelper.GetStringInput($"Enter name of person {i + 1}: ", $"Person {i + 1}");
                groupMemberNames.Add(new Member
                {
                    Id = i,
                    Name = name
                });
            }
            return groupMemberNames;
        }

        public static List<Expense> RecordExpenses(List<Member> groupMembers)
        {
            var expenses = new List<Expense>();

            var i = 1;
            while (true)
            {
                var continueFlag = UserInputHelper.GetBoolInput($"Expense Record #{i}, continue adding records? (yes/no)", "yes");
                if (!continueFlag) break;

                var promptBuilder = new StringBuilder("Who did the expense? ");
                foreach (var member in groupMembers) promptBuilder.Append($"[{member.Id}] {member.Name}, ");
                var prompt = "\t" + promptBuilder.ToString().Trim().Trim(',');

                var head = UserInputHelper.GetStringInput("\tEnter expense head: ", $"Expense {i}");
                var cost = UserInputHelper.GetDecimalInput("\tEnter the cost: ", "0.00");
                var groupMemberId = -1;
                while (groupMemberId < 0 || groupMemberId >= groupMembers.Count)
                {
                    if(groupMemberId != -1) Console.WriteLine("Invalid index selected! Retry...");
                    groupMemberId = UserInputHelper.GetNumberInput(prompt, "0");
                }

                var expense = new Expense
                {
                    By = groupMembers.First(x => x.Id == groupMemberId),
                    Head = head,
                    Amount = cost
                };
                expenses.Add(expense);

                i++;
            }
            return expenses;
        }
    }
}
