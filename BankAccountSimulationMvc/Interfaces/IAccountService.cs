using BankAccountSimulationMvc.Models;

namespace BankAccountSimulationMvc.Interfaces;

public interface IAccountService
{
    Account GetAccount(string accountNumber);
    bool IsExistingAccount(string accountNumber);
    void AddAccount(Account account);
    void UpdateBalance(string accountNumber, decimal newBalance);
    void UpdateStatus(string accountNumber, AccountStatus status);
    void UpdateAccount(Account account);
    IEnumerable<Account> GetFilteredAccounts(string? searchString, AccountStatus? statusFilter);
}
