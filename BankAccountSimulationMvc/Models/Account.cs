using System.Text.Json.Serialization;

namespace BankAccountSimulationMvc.Models;

public class Account
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    [JsonInclude]
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public void UpdateBalance(decimal balance)
    {
        Balance = balance;
    }

    public bool IsAccountActive => Status == AccountStatus.Active;
}

public enum AccountStatus
{
    Active,
    Frozen
}