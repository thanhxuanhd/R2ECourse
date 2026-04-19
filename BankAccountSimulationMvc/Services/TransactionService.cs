using BankAccountSimulationMvc.Interfaces;
using BankAccountSimulationMvc.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BankAccountSimulationMvc.Services;

public class TransactionService : ITransactionService
{
    private List<Transaction> _transactions = [];
    private readonly string _filePath;
    private readonly DataResourceOptions _options;
    private readonly IWebHostEnvironment _env;

    public TransactionService(IOptions<DataResourceOptions> options, IWebHostEnvironment env)
    {
        _options = options.Value;
        _env = env;

        string folderPath = string.IsNullOrEmpty(_options.FolderPath) ? "Data" : _options.FolderPath;
        string transactionFile = string.IsNullOrEmpty(_options.TransactionFile) ? "transaction.json" : _options.TransactionFile;
        string wwwrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        _filePath = Path.Combine(wwwrootPath, folderPath.TrimStart('/'), transactionFile);

        LoadData();
    }

    private void LoadData()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            if (!string.IsNullOrEmpty(json))
            {
                _transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? [];
            }
        }
    }

    public void SaveData()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_transactions, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void AddTransactions(IEnumerable<Transaction> transactions)
    {
        int nextId = _transactions.Count > 0 ? _transactions.Max(t => t.TransactionId) + 1 : 1;

        foreach (var transaction in transactions)
        {
            if (transaction.TransactionId == 0)
            {
                transaction.TransactionId = nextId++;
            }
            _transactions.Add(transaction);
        }

        SaveData();
    }

    public IEnumerable<Transaction> GetTransactionsByAccount(string accountNumber)
    {
        return _transactions.Where(t => t.AccountNumber == accountNumber);
    }

    public IEnumerable<Transaction> GetFilteredTransactions(string? searchString, TransactionType? type)
    {
        var query = _transactions.AsEnumerable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(t => t.AccountNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        }

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        return query.OrderByDescending(t => t.TransactionDate);
    }

    public int GetNextTransactionId()
    {
        return _transactions.Count > 0 ? _transactions.Max(t => t.TransactionId) + 1 : 1;
    }
}