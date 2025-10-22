using Ardalis.Result;
using Rise.Domain.Users;
using Rise.Shared.Departments;

namespace Rise.Domain.Departments;

public class Department : Entity
{
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => _description = Guard.Against.NullOrWhiteSpace(value);
    }
    
    public Employee? Manager { get; set; }


    private List<User> members = [];
    public IReadOnlyList<User> Members => members.AsReadOnly();
    public IReadOnlyList<User> Students => members.OfType<Student>().ToList().AsReadOnly();
    public IReadOnlyList<Employee> Employees => members.OfType<Employee>().ToList().AsReadOnly();
    
    public DepartmentType DepartmentType { get; set; }

    
    public Result AddEmployee(Employee employee )
    {
        if(Employees.Contains(employee))
            return Result.Conflict("Employee already exists in this department");
            
        return Result.Success();
    }
}