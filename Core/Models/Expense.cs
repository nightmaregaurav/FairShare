namespace Core.Models
{
    public class Expense
    {
        public Member By { get; set; }
        public string Head { get; set; }
        public double Amount { get; set; }
    }
}
