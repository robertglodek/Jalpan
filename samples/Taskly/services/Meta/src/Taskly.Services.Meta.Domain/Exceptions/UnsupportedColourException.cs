namespace Taskly.Services.Meta.Domain.Exceptions;

public sealed class UnsupportedColourException(string code) : DomainException($"Colour: {code} is unsupported.")
{
    public override string Code => "unsupported_colour";
}