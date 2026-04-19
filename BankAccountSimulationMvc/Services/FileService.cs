using BankAccountSimulationMvc.Models;
using Microsoft.Extensions.Options;

namespace BankAccountSimulationMvc.Services
{
    public class FileService(IOptions<DataResourceOptions> options, IWebHostEnvironment env)
    {
        private readonly DataResourceOptions _options = options.Value;
        private readonly IWebHostEnvironment _env = env;

        public void Initialize()
        {
            string folderPath = string.IsNullOrEmpty(_options.FolderPath) ? "Data" : _options.FolderPath;
            string accountFile = string.IsNullOrEmpty(_options.AccountFile) ? "account.json" : _options.AccountFile;
            string transactionFile = string.IsNullOrEmpty(_options.TransactionFile) ? "transaction.json" : _options.TransactionFile;

            string wwwrootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string fullFolderPath = Path.Combine(wwwrootPath, folderPath.TrimStart('/'));
            string fullAccountPath = Path.Combine(fullFolderPath, accountFile);
            string fullTransactionPath = Path.Combine(fullFolderPath, transactionFile);

            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            if (!File.Exists(fullAccountPath))
            {
                File.WriteAllText(fullAccountPath, "[]");
            }

            if (!File.Exists(fullTransactionPath))
            {
                File.WriteAllText(fullTransactionPath, "[]");
            }
        }
    }
}
