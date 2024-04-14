using System.ComponentModel.DataAnnotations;
using Core.Helpers;
using Sharprompt;
using SystemTextJsonHelper;
using Console = System.Console;

namespace Core.Models
{
    public class Artifact
    {
        public string Name { get; set; } = "artifact.json";
        public List<Member> Members { get; set; } = [];
        public List<Expense> Expenses { get; set; } = [];

        public void GetGroupMembers()
        {
            var selectedOption = string.Empty;
            while (selectedOption != "Continue with current members")
            {
                Console.WriteLine("Current members:");
                Members.PrintAsTable();

                var options = new SelectOptions<string>
                {
                    Message = "Select an option",
                    Items = [ "Add new member", "Edit existing member", "Delete existing member", "Continue with current members" ],
                    DefaultValue = Members.Count <= 1 ? "Add new member" : "Continue with current members"
                };
                selectedOption = Prompt.Select(options);

                switch (selectedOption)
                {
                    case "Add new member":
                        var name = Prompt.Input<string>("Enter name of the new member", null, null, new[] { Validators.Required() });
                        Members.Add(new Member { Id = Members.Count, Name = name });
                        break;
                    case "Edit existing member":
                        var memberToEdit = Prompt.Select(GetMemberSelectOptions());
                        var newName = Prompt.Input<string>("Enter new name for the member", memberToEdit.Name, memberToEdit.Name, new[] { Validators.Required() });
                        memberToEdit.Name = newName;
                        break;
                    case "Delete existing member":
                        var memberToDelete = Prompt.Select(GetMemberSelectOptions());
                        Members.Remove(memberToDelete);
                        break;
                    default:
                        continue;
                }
                UpdateArtifact();
            }
        }

        public void GetExpenses()
        {
            var selectedOption = string.Empty;
            while (selectedOption != "Continue with current expenses")
            {
                Console.WriteLine("Current expenses:");
                Expenses.PrintAsTable(Members);

                var options = new SelectOptions<string>
                {
                    Message = "Select an option",
                    Items = [ "Add new expense", "Edit existing expense", "Delete existing expense", "Continue with current expenses" ],
                    DefaultValue = Expenses.Sum(x => x.Amount) == 0 ? "Add new expense" : "Continue with current expenses"
                };
                selectedOption = Prompt.Select(options);
                switch (selectedOption)
                {
                    case "Add new expense":
                        var by = Prompt.Select(GetMemberSelectOptions());
                        var head = Prompt.Input<string>("Enter head of the new expense", null, null, new[] { Validators.Required() });
                        var amount = Prompt.Input<double>("Enter amount of the new expense", null, null, new[]
                        {
                            Validators.Required(),
                            v => v is double and >= 0 ? ValidationResult.Success : new ValidationResult("Amount must be a positive number.")
                        });
                        Expenses.Add(new Expense { Id = Expenses.Count, By = by.Id, Head = head, Amount = amount });
                        break;
                    case "Edit existing expense":
                        var expenseToEdit = Prompt.Select(GetExpenseSelectOptions());
                        var newHead = Prompt.Input<string>("Enter new head for the expense", expenseToEdit.Head, expenseToEdit.Head, new[] { Validators.Required() });
                        var newAmount = Prompt.Input<double>("Enter new amount for the expense", expenseToEdit.Amount, expenseToEdit.Amount + "", new[]
                        {
                            Validators.Required(),
                            v => v is double and >= 0 ? ValidationResult.Success : new ValidationResult("Amount must be a positive number.")
                        });
                        var newBy = Prompt.Select(GetMemberSelectOptions());
                        expenseToEdit.Head = newHead;
                        expenseToEdit.Amount = newAmount;
                        expenseToEdit.By = newBy.Id;
                        break;
                    case "Delete existing expense":
                        var expenseToDelete = Prompt.Select(GetExpenseSelectOptions());
                        Expenses.Remove(expenseToDelete);
                        break;
                    default:
                        continue;
                }
                UpdateArtifact();
            }
        }

        private SelectOptions<Member> GetMemberSelectOptions()
        {
            return new SelectOptions<Member>
            {
                Message = "Select member",
                Items = Members,
                DefaultValue = Members[0],
                TextSelector = member => $"{member.Id}: {member.Name}"
            };
        }

        private SelectOptions<Expense> GetExpenseSelectOptions()
        {
            return new SelectOptions<Expense>
            {
                Message = "Select expense",
                Items = Expenses,
                DefaultValue = Expenses[0],
                TextSelector = expense => $"{expense.Id}: {expense.Head} worth {expense.Amount} by {Members.First(x => x.Id == expense.By).Name}:{Members.First(x => x.Id == expense.By).Id}"
            };
        }

        private void UpdateArtifact() => File.WriteAllText(Name, JsonHelper.Serialize(this));
    }
}
