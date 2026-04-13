using System.Text.Json;

namespace NotesCli;

public sealed class NoteStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string path;

    public NoteStore(string path)
    {
        this.path = path;
    }

    public async Task<IReadOnlyList<Note>> ListAsync(bool includeArchived)
    {
        var notes = await LoadAsync();

        return notes
            .Where(note => includeArchived || !note.IsArchived)
            .OrderBy(note => note.IsArchived)
            .ThenBy(note => note.Id)
            .ToList();
    }

    public async Task<Note> AddAsync(string title, string category, string body)
    {
        var notes = await LoadAsync();
        var nextId = notes.Count == 0 ? 1 : notes.Max(note => note.Id) + 1;

        var note = new Note
        {
            Id = nextId,
            Title = title,
            Category = category,
            Body = body,
            CreatedAtUtc = DateTime.UtcNow,
            IsArchived = false
        };

        notes.Add(note);
        await SaveAsync(notes);

        return note;
    }

    public async Task<Note?> GetAsync(int id)
    {
        var notes = await LoadAsync();
        return notes.SingleOrDefault(note => note.Id == id);
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        var notes = await LoadAsync();
        var noteIndex = notes.FindIndex(note => note.Id == id);

        if (noteIndex < 0 || notes[noteIndex].IsArchived)
        {
            return false;
        }

        notes[noteIndex] = notes[noteIndex] with { IsArchived = true };
        await SaveAsync(notes);

        return true;
    }

    private async Task<List<Note>> LoadAsync()
    {
        if (!File.Exists(path))
        {
            var seedNotes = CreateSeedNotes();
            await SaveAsync(seedNotes);
            return seedNotes;
        }

        await using var stream = File.OpenRead(path);
        var notes = await JsonSerializer.DeserializeAsync<List<Note>>(stream, JsonOptions);

        return notes ?? new List<Note>();
    }

    private async Task SaveAsync(List<Note> notes)
    {
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, notes, JsonOptions);
    }

    private static List<Note> CreateSeedNotes()
    {
        return new List<Note>
        {
            new()
            {
                Id = 1,
                Title = "Welcome to Notes CLI",
                Category = "workshop",
                Body = "Use 'dotnet run -- list' to explore the sample data.",
                CreatedAtUtc = new DateTime(2026, 4, 13, 9, 0, 0, DateTimeKind.Utc),
                IsArchived = false
            },
            new()
            {
                Id = 2,
                Title = "Review demo prompts",
                Category = "demo",
                Body = "Ask Copilot how Program.cs and NoteStore.cs work together.",
                CreatedAtUtc = new DateTime(2026, 4, 13, 9, 15, 0, DateTimeKind.Utc),
                IsArchived = false
            },
            new()
            {
                Id = 3,
                Title = "Archive after the session",
                Category = "ops",
                Body = "This note starts archived so the --all flag has something to show.",
                CreatedAtUtc = new DateTime(2026, 4, 13, 9, 30, 0, DateTimeKind.Utc),
                IsArchived = true
            }
        };
    }

}
