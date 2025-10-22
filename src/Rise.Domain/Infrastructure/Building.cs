using Rise.Domain.Infrastructure;
namespace Rise.Domain.Infrastructure;

public class Building : Entity
{
    // --- Classrooms ---
    private List<Classroom> _classrooms = new();
    public required List<Classroom> Classrooms
    {
        get => _classrooms;
        set => _classrooms = value;
    }

    private String _name = "";
    public String Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private List<Openingtime> _openingtimes = new();
    public required List<Openingtime> Openingtimes
    {
        get => _openingtimes;
        set => _openingtimes = value;
    }

    private Campus _campus = null!;
    public required Campus Campus
    {
        get => _campus;
        set => _campus = value;
    }

    private List<Resto> _restos = new();
    public required List<Resto> Restos
    {
        get => _restos;
        set => _restos = value;
    }
}
