namespace NotesCli;

public sealed record Note
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }

    public bool IsArchived { get; init; }
}
