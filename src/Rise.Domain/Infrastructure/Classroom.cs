using Rise.Domain.Education;

namespace Rise.Domain.Infrastructure;

public class Classroom : Entity
{
    private string _coordinates = string.Empty;
    public required string Coordinates
    {
        get => _coordinates;
        set => _coordinates = value;
    }

    private Building _building;
    public required Building Building
    {
        get => _building;
        set => _building = value;
    }

    private Lesson _lesson;
    public required Lesson Lesson
    {
        get => _lesson;
        set => _lesson = value;
    }
}
