namespace BankAccountSimulationMvc.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; }
    public string AccountNumber { get; set; }
}

public enum TransactionType
{
    Deposit,
    Withdraw,
    Transfer
}