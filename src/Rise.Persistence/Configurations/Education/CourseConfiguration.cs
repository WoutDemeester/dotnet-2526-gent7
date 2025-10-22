using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Education;

namespace Rise.Persistence.Configurations.Education;

/// <summary>
/// Entity Framework Core configuration for the Course entity.
/// This class defines the database schema mappings, constraints, and relationships for Courses.
/// Courses are part of the education domain and can be associated with study fields, deadlines, and students.
/// </summary>
internal class CourseConfiguration : EntityConfiguration<Course>
{
    /// <summary>
    /// Configures the Course entity type.
    /// Sets the table name, property constraints, and relationships with other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Course.</param>
    public override void Configure(EntityTypeBuilder<Course> builder)
    {
        base.Configure(builder); // Applies base entity configurations (e.g., common properties like Id, CreatedAt)
        
        // Map to the "Courses" table in the database
        builder.ToTable("Courses");

        // Configure the Name property: required with a maximum length
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100); // Adjust as needed based on domain requirements

        // Configure the Description property: optional with a maximum length
        builder.Property(c => c.Description)
            .HasMaxLength(500); // Optional, adjust as needed for storage constraints

        // Configure the one-to-many relationship to StudyField
        // A Course belongs to one StudyField, and a StudyField can have many Courses
        builder.HasOne(c => c.StudyField)
            .WithMany(sf => sf.Courses) // Assumes StudyField has a navigation property: public IReadOnlyList<Course> Courses
            .HasForeignKey("StudyFieldId") // Uses a shadow foreign key property
            .IsRequired(); // Course must have a StudyField; use .OnDelete(DeleteBehavior.SetNull) if optional

        // Configure the one-to-many relationship with Deadlines
        // A Course can have many Deadlines, and a Deadline optionally belongs to one Course
        builder.HasMany(c => c.Deadlines)
            .WithOne(d => d.Course)
            .HasForeignKey("CourseId") // Uses a shadow foreign key property
            .OnDelete(DeleteBehavior.SetNull); // On course deletion, set Deadline's CourseId to null (optional relationship)

        // Configure the many-to-many relationship with Students
        // Uses an implicit junction table "StudentCourses" for the association
        builder.HasMany(c => c.Students)
            .WithMany(s => s.Courses)
            .UsingEntity(join => join.ToTable("StudentCourses"));
    }
}