namespace VillaAPI.Infrastructure;

public class IdentityException : Exception
{
    public IEnumerable<string> Errors { get; set; }
    public IdentityException()
    {
    }

    public IdentityException(string message) : base(message)
    {
    }
    public IdentityException(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors;
    }
}