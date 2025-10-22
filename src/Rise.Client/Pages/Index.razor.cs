using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rise.Shared.Deadlines;

namespace Rise.Client.Pages; // Adjust if the folder/namespace doesn't match the file location

/// <summary>
/// Partial class for the Index Razor page component.
/// This component fetches the upcoming 2 incomplete deadlines for the logged-in user.
/// Deadlines are fetched via the IDeadlineService.
/// The database structure uses a StudentDeadlines junction table to associate students with deadlines,
/// allowing for per-student completion tracking.
/// </summary>
public partial class Index : ComponentBase
{
    /// <summary>
    /// Injected authentication state provider to check user login status.
    /// </summary>
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;

    /// <summary>
    /// Injected deadline service to fetch deadlines.
    /// </summary>
    [Inject] private IDeadlineService DeadlineService { get; set; } = default!;

    /// <summary>
    /// Flag indicating if the user is authenticated.
    /// </summary>
    private bool isAuthenticated = false;
    
    /// <summary>
    /// Lifecycle method called when the component is initialized.
    /// Checks authentication status and fetches the top 2 upcoming incomplete deadlines if the user is logged in.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Get the current authentication state
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

        if (isAuthenticated)
        {
            await FetchDeadlinesAsync();
        }
    }

    private async Task FetchDeadlinesAsync()
    {
        // Prepare request to fetch deadlines for the student (fetch a reasonable number and filter client-side)
        var request = new DeadlineRequest.GetForStudent
        {
            SearchTerm = string.Empty,
            Skip = 0,
            Take = 1000, // Increased to ensure future deadlines are included even if there are many past ones
            OrderBy = "DueDate",
            OrderDescending = false
        };

        // Fetch deadlines via the service
        var result = await DeadlineService.GetIndexAsync(request);

        if (result.IsSuccess)
        {
            // Filter for upcoming incomplete deadlines (DueDate >= now, !IsCompleted), already sorted by DueDate asc, take top 2
            var now = DateTime.Now;
            _deadlines = result.Value.Deadlines
                .Where(d => !d.IsCompleted && d.DueDate >= now)
                .Take(2)
                .ToList();
        }
        // Optionally handle errors, e.g., display a notification to the user
    }

    private async Task UpdateUi()
    {
        await FetchDeadlinesAsync();
    }
}