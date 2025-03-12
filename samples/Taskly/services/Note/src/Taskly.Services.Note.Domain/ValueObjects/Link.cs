using Taskly.Services.Note.Domain.Exceptions;

namespace Taskly.Services.Note.Domain.ValueObjects;

public sealed class Link : ValueObject
{
    public Link(string url, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidLinkUrlException();
        }
        
        Url = url;
        Name = name;
    }
    public string Url { get; private set; }
    public string? Name { get; private set; }
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Url;
        yield return Name;
    }
}