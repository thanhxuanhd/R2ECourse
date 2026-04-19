namespace BankAccountSimulationMvc.Models;

public class DataResourceOptions
{
    public const string DataResource = "DataResource";

    public string FolderPath { get; set; } = string.Empty;
    public string AccountFile { get; set; } = string.Empty;
    public string TransactionFile { get; set; } = string.Empty;
}
