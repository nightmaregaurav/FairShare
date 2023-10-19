namespace Core.Models
{
    public class ExpenseDistribution
    {
        public Member Sender { get; set; }
        public Member Receiver { get; set; }
        public double Amount { get; set; }
    }
}
