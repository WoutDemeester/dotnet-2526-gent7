namespace Rise.Domain.Infrastructure;

public class Event : Entity
{
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = value;
    }

    private string _description = string.Empty;
    public required string Description
    {
        get => _description;
        set => _description = value;
    }

    public DateTime DateAndTime { get; set; }

    private Campus _campus;
    public required Campus Campus
    {
        get => _campus;
        set => _campus = value;
    }
}
