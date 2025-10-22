using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Identity;
using Rise.Shared.Deadlines;

namespace Rise.Services.Deadlines;

/// <summary>
/// Service class for managing student deadlines in the application.
/// This service handles fetching paginated deadlines for authenticated students and toggling their completion status.
/// It ensures secure access by validating user authentication and linking to the student's account.
/// Deadlines are queried via the StudentDeadlines junction table to support per-student completion tracking.
/// Follows the service layer pattern in Clean Architecture, encapsulating business logic and data access.
/// </summary>
/// <param name="dbContext">The Entity Framework Core database context for data operations.</param>
/// <param name="sessionProvider">The provider for session context, used to access the current user's claims principal.</param>
public class DeadlineService(ApplicationDbContext dbContext, ISessionContextProvider sessionProvider) : IDeadlineService
{
    /// <summary>
    /// Retrieves a paginated index of deadlines assigned to the authenticated student.
    /// Supports searching, ordering, and pagination. Ensures the user is authenticated and linked to a student record.
    /// Queries use the StudentDeadlines junction to fetch only relevant assignments, including completion status.
    /// </summary>
    /// <param name="request">The request object containing search term, pagination (skip/take), and ordering parameters.</param>
    /// <param name="ctx">Cancellation token for asynchronous operations.</param>
    /// <returns>A Result containing the index response with deadlines and total count, or an error (e.g., unauthorized, not found).</returns>
    public async Task<Result<DeadlineResponse.Index>> GetIndexAsync(DeadlineRequest.GetForStudent request, CancellationToken ctx = default)
    {
        // Validate authenticated user
        var user = sessionProvider.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            Log.Warning("Unauthenticated access attempt to student deadlines.");
            return Result.Unauthorized("User must be authenticated to access deadlines.");
        }

        // Get user ID from claims (ASP.NET Identity)
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            Log.Warning("User ID not found in claims for authenticated user.");
            return Result.Conflict("Unable to retrieve user ID.");
        }

        // Find student linked to user
        var student = await dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.AccountId == userId, ctx);
        if (student == null)
        {
            Log.Warning("No student account linked to user ID '{UserId}'.", userId);
            return Result.NotFound("Student account not found.");
        }

        // Query deadlines tied to student via junction
        var query = dbContext.StudentDeadlines
            .Where(sd => sd.StudentId == student.Id)
            .Include(sd => sd.Deadline)
                .ThenInclude(d => d.Course)  // For Course.Name in DTO
            .Select(sd => new { sd, Deadline = sd.Deadline });

        // Apply search filter on Deadline properties (including Course.Name if available)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => 
                x.Deadline.Title.Contains(request.SearchTerm) || 
                x.Deadline.Description.Contains(request.SearchTerm) ||
                (x.Deadline.Course != null && x.Deadline.Course.Name.Contains(request.SearchTerm)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(ctx);

        // Apply ordering on Deadline properties
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            // Handle special cases like ordering by Course.Name
            if (request.OrderBy.Equals("Course", StringComparison.OrdinalIgnoreCase))
            {
                query = request.OrderDescending
                    ? query.OrderByDescending(x => x.Deadline.Course != null ? x.Deadline.Course.Name : string.Empty)
                    : query.OrderBy(x => x.Deadline.Course != null ? x.Deadline.Course.Name : string.Empty);
            }
            else
            {
                query = request.OrderDescending
                    ? query.OrderByDescending(x => EF.Property<object>(x.Deadline, request.OrderBy))
                    : query.OrderBy(x => EF.Property<object>(x.Deadline, request.OrderBy));
            }
        }
        else
        {
            query = query.OrderBy(x => x.Deadline.DueDate);  // Default order
        }

        // Fetch paginated results
        var deadlines = await query
            .AsNoTracking()
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(x => new DeadlineDto.Index
            {
                Id = x.Deadline.Id,
                Title = x.Deadline.Title,
                IsCompleted = x.sd.IsCompleted,
                Description = x.Deadline.Description,
                DueDate = x.Deadline.DueDate,
                StartDate = x.Deadline.StartDate,
                Course = x.Deadline.Course != null ? x.Deadline.Course.Name : null
            })
            .ToListAsync(ctx);

        return Result.Success(new DeadlineResponse.Index
        {
            Deadlines = deadlines,
            TotalCount = totalCount
        });
    }

    /// <summary>
    /// Toggles the completion status of a specific deadline assignment for the authenticated student.
    /// Updates the StudentDeadlines junction record with the new status and completion date if applicable.
    /// Ensures the user is authenticated, linked to a student, and the assignment exists.
    /// </summary>
    /// <param name="request">The request object containing the DeadlineId and new IsCompleted value.</param>
    /// <param name="ctx">Cancellation token for asynchronous operations.</param>
    /// <returns>A Result indicating success or an error (e.g., unauthorized, not found).</returns>
    public async Task<Result> ToggleCompletionAsync(DeadlineRequest.ToggleCompletion request, CancellationToken ctx = default)
    {
        // Validate authenticated user
        var user = sessionProvider.User;
        if (user == null || !user.Identity.IsAuthenticated)
        {
            Log.Warning("Unauthenticated access attempt to toggle deadline completion.");
            return Result.Unauthorized("User must be authenticated to toggle completion.");
        }

        // Get user ID from claims (ASP.NET Identity)
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            Log.Warning("User ID not found in claims for authenticated user.");
            return Result.Conflict("Unable to retrieve user ID.");
        }

        // Find student linked to user
        var student = await dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.AccountId == userId, ctx);
        if (student == null)
        {
            Log.Warning("No student account linked to user ID '{UserId}'.", userId);
            return Result.NotFound("Student account not found.");
        }

        // Find the junction record
        var studentDeadline = await dbContext.StudentDeadlines
            .FirstOrDefaultAsync(sd => sd.StudentId == student.Id && sd.DeadlineId == request.DeadlineId, ctx);

        if (studentDeadline == null)
        {
            Log.Warning("No assignment found for student ID '{StudentId}' and deadline ID '{DeadlineId}'.", student.Id, request.DeadlineId);
            return Result.NotFound("Assignment not found for this student.");
        }

        studentDeadline.IsCompleted = request.IsCompleted;
      

        await dbContext.SaveChangesAsync(ctx);
        return Result.Success();
    }
}