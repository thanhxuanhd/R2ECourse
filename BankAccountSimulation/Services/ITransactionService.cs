using BankAccountSimulation.Models;

namespace BankAccountSimulation.Services;

public interface ITransactionService
{
    void AddTransaction(Transaction transaction);
    IEnumerable<Transaction> GetTransactionsByAccount(string accountNumber);
    IEnumerable<Transaction> GetFilteredTransactions(string accountNumber, TransactionType? type);
    int GetNextTransactionId();
}
