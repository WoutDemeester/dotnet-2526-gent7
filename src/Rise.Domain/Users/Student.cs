using Ardalis.Result;
using Ardalis.GuardClauses;
using Rise.Domain.Education;

namespace Rise.Domain.Users;

/// <summary>
/// Represents a student entity in the users domain, inheriting from the base User class.
/// A student is associated with courses and deadlines via many-to-many relationships.
/// This entity enforces business rules for enrollment and deadline assignment.
/// </summary>
public class Student : User
{
    /// <summary>
    /// The unique student number, required and non-empty.
    /// </summary>
    private string _studentnumber = string.Empty;

    public required string StudentNumber
    {
        get => _studentnumber;
        init => _studentnumber = Guard.Against.NullOrWhiteSpace(value);
    }

    /// <summary>
    /// Backing field for the list of courses the student is enrolled in.
    /// </summary>
    private List<Course> courses = [];
    
    /// <summary>
    /// Read-only list of courses the student is enrolled in.
    /// </summary>
    public IReadOnlyList<Course> Courses => courses.AsReadOnly();

    /// <summary>
    /// Backing field for the list of deadline assignments for this student.
    /// </summary>
    private List<StudentDeadline> studentDeadlines = [];
    
    /// <summary>
    /// Read-only list of deadline assignments (junction entities) for this student.
    /// </summary>
    public IReadOnlyList<StudentDeadline> StudentDeadlines => studentDeadlines.AsReadOnly();

    /// <summary>
    /// Enrolls the student in a course.
    /// Internal access to allow calling from the Course entity for bidirectional consistency.
    /// Ensures the student is not already enrolled.
    /// </summary>
    /// <param name="course">The course to enroll in.</param>
    /// <returns>A Result indicating success or conflict if already enrolled.</returns>
    internal Result EnrollInCourse(Course course)
    {
        if (courses.Contains(course))
            return Result.Conflict("Student already enrolled in this course");

        courses.Add(course);
        return Result.Success();
    }

    /// <summary>
    /// Adds a deadline assignment to this student.
    /// Internal access to allow calling from the Deadline entity for bidirectional consistency.
    /// Ensures the deadline is not already assigned.
    /// </summary>
    /// <param name="studentDeadline">The StudentDeadline junction entity to add.</param>
    /// <returns>A Result indicating success or conflict if already assigned.</returns>
    internal Result AddStudentDeadline(StudentDeadline studentDeadline)
    {
        if (studentDeadlines.Contains(studentDeadline))
            return Result.Conflict("Deadline already assigned to this student");

        studentDeadlines.Add(studentDeadline);
        return Result.Success();
    }
}