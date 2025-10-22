using Rise.Domain.Users;

namespace Rise.Domain.Education;

public class Lesson : Entity
{
    private List<Student> students = [];
    
    public IReadOnlyList<Student> Students => students.AsReadOnly();

    private List<Teacher> _teachers = [];
    
    public IReadOnlyList<Teacher> Teachers => _teachers.AsReadOnly();    

}