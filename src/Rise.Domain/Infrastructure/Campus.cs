using Rise.Domain.Education;
using Rise.Domain.Departments;
namespace Rise.Domain.Infrastructure;

public class Campus : Entity
{
    private bool _isOpen;
    public bool IsOpen
    {
        get => _isOpen;
        set => _isOpen = value;
    }
    private String _name = "";
    public String Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _map = string.Empty;
    public required string Map
    {
        get => _map;
        set => _map = value;
    }

    private List<Department> _departements = new();
    public required List<Department> Departements
    {
        get => _departements;
        set => _departements = value;
    }

    private List<Openingtime> _openingtimes = new();
    public required List<Openingtime> Openingtimes
    {
        get => _openingtimes;
        set => _openingtimes = value;
    }

    private List<Emergency> _emergencies = new();
    public required List<Emergency> Emergencies
    {
        get => _emergencies;
        set => _emergencies = value;
    }

    private List<Event> _events = new();
    public required List<Event> Events
    {
        get => _events;
        set => _events = value;
    }

    private List<Building> _buildings = new();
    public required List<Building> Buildings
    {
        get => _buildings;
        set => _buildings = value;
    }
}
