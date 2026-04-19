using BankAccountSimulationMvc.Interfaces;
using BankAccountSimulationMvc.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BankAccountSimulationMvc.Services;

public class AccountService : IAccountService
{
    private List<Account> _accounts = [];
    private readonly string _filePath;

    private readonly DataResourceOptions _options;
    private readonly IWebHostEnvironment _env;

    public AccountService(IOptions<DataResourceOptions> options, IWebHostEnvironment env)
    {
        _options = options.Value;
        _env = env;

        string folderPath = string.IsNullOrEmpty(_options.FolderPath) ? "Data" : _options.FolderPath;
        string accountFile = string.IsNullOrEmpty(_options.AccountFile) ? "account.json" : _options.AccountFile;
        string wwwrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        _filePath = Path.Combine(wwwrootPath, folderPath.TrimStart('/'), accountFile);

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
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_accounts, options));
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

    public void UpdateAccount(Account account)
    {
        var existing = GetAccount(account.AccountNumber);
        if (existing != null)
        {
            existing.OwnerName = account.OwnerName;
            existing.Status = account.Status;
            SaveData();
        }
    }

    public IEnumerable<Account> GetFilteredAccounts(string? searchString, AccountStatus? statusFilter)
    {
        var accounts = _accounts.AsEnumerable();

        if (!string.IsNullOrEmpty(searchString))
        {
            accounts = accounts.Where(a =>
                (a.AccountNumber != null && a.AccountNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                (a.OwnerName != null && a.OwnerName.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
        }

        if (statusFilter.HasValue)
        {
            accounts = accounts.Where(a => a.Status == statusFilter.Value);
        }

        return accounts;
    }
}