using NotesCli;

var store = new NoteStore(Path.Combine(Environment.CurrentDirectory, "notes.json"));
return await RunAsync(args, store);

static async Task<int> RunAsync(string[] args, NoteStore store)
{
    if (args.Length == 0)
    {
        return ShowHelp();
    }

    return args[0].ToLowerInvariant() switch
    {
        "list" => await ListNotesAsync(args, store),
        "add" => await AddNoteAsync(args, store),
        "show" => await ShowNoteAsync(args, store),
        "archive" => await ArchiveNoteAsync(args, store),
        "help" or "--help" or "-h" => ShowHelp(),
        _ => UnknownCommand(args[0])
    };
}

static async Task<int> ListNotesAsync(string[] args, NoteStore store)
{
    var includeArchived = args.Any(argument => string.Equals(argument, "--all", StringComparison.OrdinalIgnoreCase));
    var notes = await store.ListAsync(includeArchived);

    if (notes.Count == 0)
    {
        Console.WriteLine("No notes found.");
        return 0;
    }

    foreach (var note in notes)
    {
        var status = note.IsArchived ? "archived" : "active";
        Console.WriteLine($"{note.Id,2} | {status,-8} | {note.Category,-10} | {note.Title}");
    }

    return 0;
}

static async Task<int> AddNoteAsync(string[] args, NoteStore store)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- add <title> [--category <category>] [--body <body>]");
        return 1;
    }

    var title = args[1];
    var category = ReadOption(args, "--category") ?? "general";
    var body = ReadOption(args, "--body") ?? "Fill in the details for this note.";
    var note = await store.AddAsync(title, category, body);

    Console.WriteLine($"Added note {note.Id}: {note.Title}");
    return 0;
}

static async Task<int> ShowNoteAsync(string[] args, NoteStore store)
{
    if (!TryReadId(args, out var noteId))
    {
        Console.WriteLine("Usage: dotnet run -- show <id>");
        return 1;
    }

    var note = await store.GetAsync(noteId);

    if (note is null)
    {
        Console.WriteLine($"Note {noteId} was not found.");
        return 1;
    }

    var status = note.IsArchived ? "archived" : "active";
    Console.WriteLine($"{note.Title} ({status})");
    Console.WriteLine($"Category: {note.Category}");
    Console.WriteLine($"Created: {note.CreatedAtUtc:yyyy-MM-dd HH:mm} UTC");
    Console.WriteLine();
    Console.WriteLine(note.Body);

    return 0;
}

static async Task<int> ArchiveNoteAsync(string[] args, NoteStore store)
{
    if (!TryReadId(args, out var noteId))
    {
        Console.WriteLine("Usage: dotnet run -- archive <id>");
        return 1;
    }

    var archived = await store.ArchiveAsync(noteId);

    if (!archived)
    {
        Console.WriteLine($"Note {noteId} was not found or is already archived.");
        return 1;
    }

    Console.WriteLine($"Archived note {noteId}.");
    return 0;
}

static bool TryReadId(string[] args, out int noteId)
{
    noteId = 0;
    return args.Length >= 2 && int.TryParse(args[1], out noteId);
}

static string? ReadOption(string[] args, string optionName)
{
    for (var index = 0; index < args.Length - 1; index++)
    {
        if (string.Equals(args[index], optionName, StringComparison.OrdinalIgnoreCase))
        {
            return args[index + 1];
        }
    }

    return null;
}

static int ShowHelp()
{
    Console.WriteLine("Notes CLI");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  list [--all]");
    Console.WriteLine("  add <title> [--category <category>] [--body <body>]");
    Console.WriteLine("  show <id>");
    Console.WriteLine("  archive <id>");
    return 0;
}

static int UnknownCommand(string command)
{
    Console.WriteLine($"Unknown command: {command}");
    Console.WriteLine("Run 'dotnet run -- help' to see the available commands.");
    return 1;
}
