using BankAccountSimulation.Models;
using System.Text.Json;

namespace BankAccountSimulation.Services;

public class AccountService : IAccountService
{
    private List<Account> _accounts = [];
    private readonly string _filePath;

    public AccountService(string filePath)
    {
        _filePath = filePath;
        LoadData();
    }

    private void LoadData()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            if (!string.IsNullOrEmpty(json))
            {
                _accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? [];
            }
        }
    }

    public void SaveData()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_accounts, new JsonSerializerOptions { WriteIndented = true }));
    }

    public Account GetAccount(string accountNumber)
    {
        return _accounts.Find(a => a.AccountNumber == accountNumber);
    }

    public bool IsExistingAccount(string accountNumber)
    {
        return _accounts.Any(a => a.AccountNumber == accountNumber);
    }

    public void AddAccount(Account account)
    {
        _accounts.Add(account);
        SaveData();
    }

    public void UpdateBalance(string accountNumber, decimal newBalance)
    {
        var account = GetAccount(accountNumber);
        if (account != null)
        {
            account.UpdateBalance(newBalance);
            SaveData();
        }
    }

    public void UpdateStatus(string accountNumber, AccountStatus status)
    {
        var account = GetAccount(accountNumber);
        if (account != null)
        {
            account.Status = status;
            SaveData();
        }
    }

    public List<Account> GetAllAccounts() => _accounts;
}
