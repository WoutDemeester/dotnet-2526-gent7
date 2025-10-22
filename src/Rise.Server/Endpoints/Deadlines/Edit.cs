using Rise.Shared.Deadlines;

namespace Rise.Server.Endpoints.Deadlines;

/// <summary>
/// Endpoint for toggling the completion status of a student's assigned deadline.
/// This PATCH endpoint allows authenticated students to mark a deadline as completed or incomplete.
/// Utilizes FastEndpoints for request handling and validation.
/// See https://fast-endpoints.com/ for more details on the framework.
/// </summary>
/// <param name="deadlineService">Injected service for deadline operations.</param>
public class Edit(IDeadlineService deadlineService) : Endpoint<DeadlineRequest.ToggleCompletion, Result>
{
    /// <summary>
    /// Configures the endpoint route and HTTP method.
    /// Maps to PATCH /api/deadlines/{DeadlineId}/completion.
    /// </summary>
    public override void Configure()
    {
        Patch("/api/deadlines/{DeadlineId}/completion");
    }

    /// <summary>
    /// Executes the endpoint logic asynchronously.
    /// Delegates to the deadline service to toggle the completion status.
    /// </summary>
    /// <param name="req">The request object containing DeadlineId and IsCompleted.</param>
    /// <param name="ct">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public override async Task<Result> ExecuteAsync(DeadlineRequest.ToggleCompletion req, CancellationToken ct)
    {
        return await deadlineService.ToggleCompletionAsync(req, ct);
    }
}