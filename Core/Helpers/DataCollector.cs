using System.Text;
using Core.Models;

namespace Core.Helpers
{
    public abstract class DataCollector
    {
        public static int GetGroupSize() => Console.GetNumber("Enter the number of people in the group: ", "2");

        public static List<Member> GetGroupMembers(int groupSize)
        {
            var groupMemberNames = new List<Member>();
            for (var i = 0; i < groupSize; i++)
            {
                var name = Console.GetString($"Enter name of person {i + 1}: ", $"Person {i + 1}");
                groupMemberNames.Add(new Member
                {
                    Id = i,
                    Name = name
                });
            }
            return groupMemberNames;
        }

        public static List<Expense> RecordExpenses(List<Member> groupMembers, int prevRecordLength = 0)
        {
            var expenses = new List<Expense>();

            var i = prevRecordLength + 1;
            while (true)
            {
                var continueFlag = Console.GetBool($"Expense Record #{i}, continue adding records? (yes/no)", "yes");
                if (!continueFlag) break;

                var promptBuilder = new StringBuilder("Who did the expense? ");
                foreach (var member in groupMembers) promptBuilder.Append($"[{member.Id}] {member.Name}, ");
                var prompt = "\t" + promptBuilder.ToString().Trim().Trim(',');

                var head = Console.GetString("\tEnter expense head: ", $"Expense {i}");
                var cost = Console.GetDecimal("\tEnter the cost: ", "0.00");
                int? groupMemberId = null;
                while (groupMemberId == null || groupMemberId < 0 || groupMemberId >= groupMembers.Count)
                {
                    if(groupMemberId != null) System.Console.WriteLine("Invalid index selected! Retry...");
                    groupMemberId = Console.GetNumber(prompt, "0");
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
