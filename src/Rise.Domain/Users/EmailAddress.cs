namespace Rise.Domain.Users;

public class EmailAddress : ValueObject
{
    public string Value { get; }

    /// <summary>
    /// Entity Framework Core Constructor
    /// </summary>
    private EmailAddress() 
    {
        
    }
    
    public EmailAddress(string email)
    {
        Value = email.ToLower().Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLower(); // Checks
    }
}