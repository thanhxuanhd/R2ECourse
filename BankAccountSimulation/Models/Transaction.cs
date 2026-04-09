namespace BankAccountSimulation.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; }
    public string AccountNumber { get; set; }

    public override string ToString()
    {
        return $"{TransactionDate,-20:yyyy-MM-dd HH:mm:ss} | {Type,-10} | {Amount,-12:C} | {Description}";
    }
}

public enum TransactionType
{
    Deposit,
    Withdraw,
    Transfer
}