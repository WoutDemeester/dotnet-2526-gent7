using Microsoft.AspNetCore.Components;
using Rise.Shared.Deadlines;

namespace Rise.Client.Deadlines; // Adjust if the folder/namespace doesn't match the file location

/// <summary>
/// Partial class for the index Razor page component.
/// This component manages the display of a student's deadlines, including authentication checks,
/// data fetching, filtering, sorting, and rendering of upcoming and completed deadlines.
/// Deadlines are fetched via the IDeadlineService and displayed using the DeadlineBlock component.
/// The database structure uses a StudentDeadlines junction table to associate students with deadlines,
/// allowing for per-student completion tracking.
/// </summary>
public partial class Index : ComponentBase  // Change to 'Index' if capitalizing
{
    /// <summary>
    /// Injected authentication state provider to check user login status.
    /// </summary>
    /// Flag indicating if the user is authenticated.
    /// </summary>
    private bool isAuthenticated = false;

    /// <summary>
    /// List of all deadlines fetched for the current student.
    /// </summary>
    private List<DeadlineDto.Index> deadlines = new();

    /// <summary>
    /// Search query string for filtering deadlines by title, description, or course.
    /// </summary>
    private string searchQuery = string.Empty;

    /// <summary>
    /// Selected course for filtering deadlines.
    /// </summary>
    private string selectedCourse = string.Empty;

    /// <summary>
    /// Selected sorting option for deadlines (e.g., "DateAscending").
    /// </summary>
    private string sortOption = "DateAscending";

    /// <summary>
    /// Distinct list of courses from the fetched deadlines, used for the course filter dropdown.
    /// </summary>
    private IEnumerable<string> Courses => deadlines.Select(d => d.Course ?? "").Distinct();

    /// <summary>
    /// Filtered deadlines based on search query and selected course.
    /// Includes checks for title, description, and course matching.
    /// </summary>
    private IEnumerable<DeadlineDto.Index> FilteredDeadlines => deadlines.Where(d =>
        (string.IsNullOrEmpty(searchQuery) ||
         d.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
         d.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
         (d.Course?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false)) &&
        (string.IsNullOrEmpty(selectedCourse) || (d.Course ?? "") == selectedCourse));

    /// <summary>
    /// Upcoming (incomplete) deadlines, filtered and sorted.
    /// </summary>
    private IEnumerable<DeadlineDto.Index> UpcomingDeadlines => SortDeadlines(FilteredDeadlines.Where(d => !d.IsCompleted));

    /// <summary>
    /// Completed deadlines, filtered and sorted.
    /// </summary>
    private IEnumerable<DeadlineDto.Index> CompletedDeadlines => SortDeadlines(FilteredDeadlines.Where(d => d.IsCompleted));

    /// <summary>
    /// Sorts the provided deadlines based on the current sort option.
    /// Defaults to ascending by due date if no valid option is selected.
    /// </summary>
    /// <param name="dl">The collection of deadlines to sort.</param>
    /// <returns>The sorted collection of deadlines.</returns>
    private IEnumerable<DeadlineDto.Index> SortDeadlines(IEnumerable<DeadlineDto.Index> dl) => sortOption switch
    {
        "DateAscending" => dl.OrderBy(d => d.DueDate),
        "DateDescending" => dl.OrderByDescending(d => d.DueDate),
        _ => dl.OrderBy(d => d.DueDate)
    };

    /// <summary>
    /// Lifecycle method called when the component is initialized.
    /// Checks authentication status and fetches deadlines if the user is logged in.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Get the current authentication state
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;

        if (isAuthenticated)
        {
            // Prepare request to fetch all deadlines for the student
            var request = new DeadlineRequest.GetForStudent
            {
                SearchTerm = string.Empty,
                Skip = 0,
                Take = 1000, // Fetch a large number to load all; consider pagination for very large datasets
                OrderBy = string.Empty,
                OrderDescending = false
            };

            // Fetch deadlines via the service
            var result = await DeadlineService.GetIndexAsync(request);

            if (result.IsSuccess)
            {
                // Store fetched deadlines in the local list
                deadlines = result.Value.Deadlines.ToList();
            }
            // Optionally handle errors, e.g., display a notification to the user
        }
    }

    /// <summary>
    /// Triggers a UI update by calling StateHasChanged.
    /// Called when a deadline's completion status changes to refresh the display.
    /// </summary>
    private void UpdateUI()
    {
        StateHasChanged();
    }
}