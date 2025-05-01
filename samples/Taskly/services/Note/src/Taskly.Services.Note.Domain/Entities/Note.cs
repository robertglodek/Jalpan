using Taskly.Services.Note.Domain.Exceptions;

namespace Taskly.Services.Note.Domain.Entities;

public sealed class Note : AggregateRoot
{
    public Note(Guid id, Guid userId, string name, string content, IEnumerable<Link>? links = null)
    {
        if (Guid.Empty == userId)
        {
            throw new InvalidUserIdException();
        }

        Id = id;
        UpdateName(name);
        UpdateContent(content);
        UpdateLinks(links);
        UserId = userId;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidNoteNameException();
        }

        Name = name;
    }

    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidNoteContentException();
        }

        Content = content;
    }

    public void UpdateLinks(IEnumerable<Link>? links) => Links = links ?? [];
    
    public IEnumerable<Link>? Links { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string Content { get; private set; }
}