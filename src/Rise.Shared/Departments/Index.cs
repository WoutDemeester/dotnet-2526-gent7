using Rise.Shared.Users;

namespace Rise.Shared.Departments;

/// <summary>
/// Represents the response structure for department-related operations.
/// </summary>
public static partial class DepartmentResponse
{
    public class Index
    {
        public IEnumerable<DepartmentDto.Index> Departments { get; set; } = [];
        public int TotalCount { get; set; }
    }


    
    public class Manager
    {
        public required UserDto.Index DepartmentsManager { get; set; }

    }
}

