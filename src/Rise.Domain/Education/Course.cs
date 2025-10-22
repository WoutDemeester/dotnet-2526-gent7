using Ardalis.Result;
using Ardalis.GuardClauses;
using Rise.Domain.Users;

namespace Rise.Domain.Education;

/// <summary>
/// Represents a course entity in the education domain.
/// A course is part of a study field and can have multiple deadlines and enrolled students.
/// This entity enforces business rules for enrollment and deadline association.
/// </summary>
public class Course : Entity
{
    /// <summary>
    /// The name of the course, required and non-empty.
    /// </summary>
    private string _name = string.Empty;
    public required string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// An optional description of the course.
    /// </summary>
    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => _description = value; // Optional, so no guard
    }

    /// <summary>
    /// The study field associated with this course.
    /// Assumes StudyField is defined elsewhere (e.g., as an entity or enum).
    /// </summary>
    public StudyField StudyField { get; set; } // Assuming StudyField is defined elsewhere (e.g., enum or entity)

    /// <summary>
    /// Backing field for the list of deadlines associated with this course.
    /// </summary>
    private List<Deadline> deadlines = [];
    
    /// <summary>
    /// Read-only list of deadlines for this course.
    /// </summary>
    public IReadOnlyList<Deadline> Deadlines => deadlines.AsReadOnly();

    /// <summary>
    /// Backing field for the list of enrolled students in this course.
    /// </summary>
    private List<Student> students = [];
    
    /// <summary>
    /// Read-only list of students enrolled in this course.
    /// </summary>
    public IReadOnlyList<Student> Students => students.AsReadOnly();

    /// <summary>
    /// Enrolls a student in this course.
    /// Ensures the student is not already enrolled and maintains bidirectional relationships.
    /// </summary>
    /// <param name="student">The student to enroll.</param>
    /// <returns>A Result indicating success or conflict if already enrolled.</returns>
    public Result EnrollStudent(Student student)
    {
        if (students.Contains(student))
            return Result.Conflict("Student already enrolled in this course");

        students.Add(student);
        student.EnrollInCourse(this); // Make the relationship bidirectional
        return Result.Success();
    }

    /// <summary>
    /// Adds a deadline to this course.
    /// Ensures the deadline is not already associated and sets the course reference.
    /// </summary>
    /// <param name="deadline">The deadline to add.</param>
    /// <returns>A Result indicating success or conflict if already associated.</returns>
    public Result AddDeadline(Deadline deadline)
    {
        if (deadlines.Contains(deadline))
            return Result.Conflict("Deadline already associated with this course");

        deadlines.Add(deadline);
        deadline.Course = this;
        return Result.Success();
    }
}