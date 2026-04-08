using CsvHelper;
using System.Globalization;
using TodoAppConsole;

const string fileName = "Notes.csv";
const string folderPath = "Data";
string filePath = Path.Combine(folderPath, fileName);

List<Note> notes = [];

bool running = true;
while (running)
{
    CreateStorageFile();
    LoadDataIntoMemory();
    PrintMenuOption();
    HandleOption();
}

void LoadDataIntoMemory()
{
    notes.Clear();

    if (!File.Exists(filePath)) return;
    using var reader = new StreamReader(filePath);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    notes = csv.GetRecords<Note>().ToList();

}

void CreateStorageFile()
{
    if (!Directory.Exists(folderPath))
    {
        Directory.CreateDirectory(folderPath);
    }

    if (!File.Exists(filePath))
    {
        File.Create(filePath);
    }
}

static void PrintMenuOption()
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("=== Notes Application ===");
    Console.WriteLine("""
        1. Add a new note 
        2. View notes 
        3. Mark note as done 
        4. Delete a note by ID 
        5. Clear all notes 
        6. Exit 
    """);
    Console.WriteLine("Select an option:");
    Console.ResetColor();
}

void HandleOption()
{
    var input = Console.ReadLine();
    switch (input)
    {
        case "1":
            HandleAddNote();
            break;
        case "2":
            HandleViewNotes();
            break;
        case "3":
            HandleMakeAsDone();
            break;
        case "4":
            HandleDeleteNote();
            break;
        case "5":
            HandleCleanAllNotes();
            break;
        case "6":
            running = false;
            Console.WriteLine("Exiting...");
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

void HandleCleanAllNotes()
{
    notes.Clear();
    SaveToCsv();
}

void HandleDeleteNote()
{
    Console.Write("Enter ID to delete: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        Note? note = notes.Find(c => c.Id == id);
        if (note is not null)
        {
            HandleDeleteConfirm(note);
        }
        else
        {
            Console.WriteLine("ID not found.");
        }
    }
}

void HandleDeleteConfirm(Note note)
{
    Console.WriteLine($"\nFound Note: \"{note.Content}\"");
    Console.Write("Are you sure you want to mark this as DONE? (y/n): ");
    string confirm = Console.ReadLine()?.ToLower();
    if (confirm == "y" || confirm == "yes")
    {
        notes.Remove(note);
        SaveToCsv();
        Console.WriteLine("Deleted.");
    }
    else
    {
        Console.WriteLine("Action cancelled");
    }
}

void HandleMakeAsDone()
{
    Console.Write("Enter ID to mark as DONE: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        var note = notes.FirstOrDefault(n => n.Id == id);
        if (note != null)
        {
            note.Status = NoteStatus.DONE;
            SaveToCsv();
            Console.WriteLine("Updated.");
        }
        else
        {
            Console.WriteLine("ID not found.");
        }
    }
}

void HandleViewNotes()
{
    Console.WriteLine("\n--- VIEW OPTIONS ---");
    Console.WriteLine("1. View All Notes");
    Console.WriteLine("2. View DONE Only");
    Console.WriteLine("3. View NOT_DONE Only");
    Console.Write("Selection: ");

    string choice = Console.ReadLine();

    IEnumerable<Note> filteredNotes;

    switch (choice)
    {
        case "2":
            filteredNotes = notes.Where(n => n.Status == NoteStatus.DONE);
            break;
        case "3":
            filteredNotes = notes.Where(n => n.Status == NoteStatus.NOT_DONE);
            break;
        default:
            filteredNotes = notes;
            break;
    }

    DisplayTable(filteredNotes);
}

void DisplayTable(IEnumerable<Note> filteredNotes)
{
    if (!filteredNotes.Any())
    {
        Console.WriteLine("No notes match this criteria.");
        return;
    }

    foreach (var note in filteredNotes)
    {
        if (note.Status == NoteStatus.DONE)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }

        Console.WriteLine(note.ToString());
        Console.ResetColor();
    }
}

void HandleAddNote()
{
    string content = string.Empty;
    while (true)
    {
        Console.Write("Enter note content: ");
        content = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(content))
        {
            Console.Write("Please enter note content: ");
        }
        else
        {
            break;
        }
    }

    int nextId = notes.Count > 0 ? notes.Max(n => n.Id) + 1 : 1;

    notes.Add(new Note
    {
        Id = nextId,
        Status = NoteStatus.NOT_DONE,
        Content = content!
    });

    SaveToCsv();
    Console.WriteLine("Save Note.");
}

void SaveToCsv()
{
    using (var writer = new StreamWriter(filePath))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.WriteRecords(notes);
    }
}