namespace TodoAppConsole;

public class Note
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public NoteStatus Status { get; set; }
    public string CreatedDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

    public override string ToString()
    {
        return $"""
              [ID: {Id}] [Created: {CreatedDate}] [Status: {Status}] {Content}
           """;
    }
    public string ToCsvRow()
    {
        return $"{Id},{CreatedDate},{Status},\"{Content.Replace("\"", "\"\"")}\"";
    }
}

public enum NoteStatus { DONE, NOT_DONE }