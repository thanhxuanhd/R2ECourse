using BankAccountSimulation.Models;
using System.Text.Json;

namespace BankAccountSimulation.Services;

public class TransactionService : ITransactionService
{
    private List<Transaction> _transactions = [];
    private readonly string _filePath;

    public TransactionService(string filePath)
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
                _transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? [];
            }
        }
    }

    public void SaveData()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(_transactions, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void AddTransaction(Transaction transaction)
    {
        // Auto-generate ID if needed, similar to Program.cs logic
        if (transaction.TransactionId == 0)
        {
            transaction.TransactionId = _transactions.Count > 0 ? _transactions.Max(t => t.TransactionId) + 1 : 1;
        }
        
        _transactions.Add(transaction);
        SaveData();
    }

    public IEnumerable<Transaction> GetTransactionsByAccount(string accountNumber)
    {
        return _transactions.Where(t => t.AccountNumber == accountNumber);
    }

    public IEnumerable<Transaction> GetFilteredTransactions(string accountNumber, TransactionType? type)
    {
        var query = _transactions.Where(t => t.AccountNumber == accountNumber);
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
