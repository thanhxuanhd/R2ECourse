namespace BankAccountSimulation.Models;

public class Account
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public void UpdateBalance(decimal balance)
    {
        Balance = balance;
    }

    public bool IsAccountActive => Status == AccountStatus.Active;

    public override string ToString()
    {
        return $"""
            -----------------------------
                Account Number: {AccountNumber}
                Owner Name:     {OwnerName}
                Balance:        {Balance:C}
                Status:         {Status}
                Created Date:   {CreatedAt:yyyy-MM-dd HH:mm:ss}
            ----------------------------- 
            """;

    }
}

public enum AccountStatus
{
    Active,
    Frozen
}