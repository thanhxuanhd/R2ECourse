namespace BankAccountSimulationMvc.Models
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
    }
}
