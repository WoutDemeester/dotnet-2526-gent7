namespace Rise.Shared.Deadlines;

/// <summary>
/// Defines the interface for deadline-related services.
/// This interface is shared between client and server layers to ensure consistent API contracts.
/// It includes methods for fetching paginated deadlines and toggling completion status.
/// </summary>
public interface IDeadlineService
{
    /// <summary>
    /// Asynchronously retrieves a paginated index of deadlines for the authenticated student.
    /// </summary>
    /// <param name="request">The request parameters including search, pagination, and ordering.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result with the index response.</returns>
    Task<Result<DeadlineResponse.Index>> GetIndexAsync(DeadlineRequest.GetForStudent request,
        CancellationToken ctx = default);

    /// <summary>
    /// Asynchronously toggles the completion status of a specific deadline for the authenticated student.
    /// </summary>
    /// <param name="request">The request containing the deadline ID and new completion status.</param>
    /// <param name="ctx">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing a Result indicating success or failure.</returns>
    Task<Result> ToggleCompletionAsync(DeadlineRequest.ToggleCompletion request,
        CancellationToken ctx = default);
}