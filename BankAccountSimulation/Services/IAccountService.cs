using BankAccountSimulation.Models;

namespace BankAccountSimulation.Services;

public interface IAccountService
{
    Account GetAccount(string accountNumber);
    bool IsExistingAccount(string accountNumber);
    void AddAccount(Account account);
    void UpdateBalance(string accountNumber, decimal newBalance);
    void UpdateStatus(string accountNumber, AccountStatus status);
    List<Account> GetAllAccounts();
}
