using Rise.Domain.Departments;

namespace Rise.Domain.Users;

public abstract class User : Entity
{
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    
    public required string AccountId { get; init; }
    public Department? Department { get; set; }
    public required EmailAddress Email { get; init; }
    
}