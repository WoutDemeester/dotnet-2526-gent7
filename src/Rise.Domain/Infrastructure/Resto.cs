using Rise.Domain.Entities;
namespace Rise.Domain.Infrastructure;

public class Resto : Entity
{
    public Menu _menu;
    public required Menu Menu
    {
        get => _menu;
        set => _menu = value;
    }
    public String _coordinates = "";
    public required String Coordinates
    {
        get => _coordinates;
        set => _coordinates = value;
    }
    private String _name = "";
    public String Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }
    public Building _building;
    public required Building Building
    {
        get => _building;
        set => _building = value;
    }
    public List<Openingtime> _openingtimes;
     public required List<Openingtime> Openingtimes
    {
        get => _openingtimes;
        set => _openingtimes = value;
    }

}