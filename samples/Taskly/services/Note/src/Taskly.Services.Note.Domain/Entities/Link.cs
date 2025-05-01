using Taskly.Services.Note.Domain.Exceptions;

namespace Taskly.Services.Note.Domain.Entities;

public sealed class Link
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
}