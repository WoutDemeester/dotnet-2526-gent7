using Rise.Domain.Education;

namespace Rise.Domain.Users;

public class Teacher : Employee
{
    private List<Lesson> lessons = [];
    
    public IReadOnlyList<Lesson> Lessons => lessons.AsReadOnly();
}