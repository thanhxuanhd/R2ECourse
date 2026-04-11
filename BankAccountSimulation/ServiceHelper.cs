using BankAccountSimulation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BankAccountSimulation;

public static class ServiceHelper
{
    public static IServiceProvider ConfigureServices(string accountFilePath, string transactionFilePath)
    {
        return new ServiceCollection()
            .AddSingleton<IAccountService>(new AccountService(accountFilePath))
            .AddSingleton<ITransactionService>(new TransactionService(transactionFilePath))
            .AddSingleton<BankManager>()
            .BuildServiceProvider();
    }
}
