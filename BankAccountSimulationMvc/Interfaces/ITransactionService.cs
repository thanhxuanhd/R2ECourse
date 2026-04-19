using BankAccountSimulationMvc.Models;

namespace BankAccountSimulationMvc.Interfaces;

public interface ITransactionService
{
    void AddTransactions(IEnumerable<Transaction> transactions);
    IEnumerable<Transaction> GetTransactionsByAccount(string accountNumber);
    IEnumerable<Transaction> GetFilteredTransactions(string? searchString, TransactionType? type);
    int GetNextTransactionId();
}
