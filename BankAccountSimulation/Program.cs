using BankAccountSimulation;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

string GetSourceFilePath([CallerFilePath] string sourceFilePath = "") => sourceFilePath;
string sourceDirectory = Path.GetDirectoryName(GetSourceFilePath()) ?? string.Empty;

const string accountFile = "account.json";
const string transactionFile = "transaction.json";
string folderPath = Path.Combine(sourceDirectory, "Data");
string accountFilePath = Path.Combine(folderPath, accountFile);
string transactionFilePath = Path.Combine(folderPath, transactionFile);

CreateDataResource();

var serviceProvider = ServiceHelper.ConfigureServices(accountFilePath, transactionFilePath);

var bankManager = serviceProvider.GetRequiredService<BankManager>();
bankManager.Run();

void CreateDataResource()
{
    if (!Directory.Exists(folderPath))
    {
        Directory.CreateDirectory(folderPath);
    }

    if (!File.Exists(accountFilePath))
    {
        File.WriteAllText(accountFilePath, "[]");
    }

    if (!File.Exists(transactionFilePath))
    {
        File.WriteAllText(transactionFilePath, "[]");
    }
}