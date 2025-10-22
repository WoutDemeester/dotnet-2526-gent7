// DeadlineBlock.razor.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Shared.Deadlines;

namespace Rise.Client.Deadlines; // Namespace for client-side deadline components

/// <summary>
/// Partial class for the DeadlineBlock Razor component, handling logic for displaying and interacting with a single deadline.
/// This component manages expansion, completion toggling, and file uploads for a student's deadline.
/// </summary>
public partial class DeadlineBlock
{
    /// <summary>
    /// The deadline data transfer object to display.
    /// </summary>
    [Parameter] public DeadlineDto.Index Deadline { get; set; } = new();

    /// <summary>
    /// Event callback invoked when the completion status changes.
    /// </summary>
    [Parameter] public EventCallback OnCompletedChanged { get; set; }

    /// <summary>
    /// Event callback invoked when a file is uploaded.
    /// </summary>

    /// <summary>
    /// Flag indicating if the deadline details are expanded.
    /// </summary>
    private bool IsExpanded { get; set; } = false;

    /// <summary>
    /// Flag indicating if the completion animation is in progress.
    /// </summary>
    private bool IsCompleting { get; set; } = false;

    /// <summary>
    /// Flag indicating if a loading operation (e.g., API call) is in progress.
    /// </summary>
    private bool IsLoading { get; set; } = false;

    /// <summary>
    /// Toggles the expansion state of the deadline details.
    /// </summary>
    private void ToggleExpand()
    {
        IsExpanded = !IsExpanded;
    }

    /// <summary>
    /// Handles the change event for the completion checkbox.
    /// Updates the deadline's completion status via the service and triggers UI updates.
    /// </summary>
    /// <param name="e">The change event arguments.</param>
    private async Task HandleChange(ChangeEventArgs e)
    {
        if (e.Value is bool newValue && newValue != Deadline.IsCompleted)
        {
            IsLoading = true;

            // Create a request to toggle the completion status
            var request = new DeadlineRequest.ToggleCompletion { DeadlineId = Deadline.Id, IsCompleted = newValue };
            
            // Call the service to update the status on the server
            var result = await DeadlineService.ToggleCompletionAsync(request);

            if (result.IsSuccess)
            {
                // Update local state on success
                Deadline.IsCompleted = newValue;

                if (newValue)
                {
                    // Trigger completion animation and collapse the details
                    IsCompleting = true;
                    await Task.Delay(500); // Delay to match CSS transition duration
                    IsCompleting = false;
                    IsExpanded = false;
                }

                // Notify parent component of the change
                await OnCompletedChanged.InvokeAsync();
            }
            else
            {
                // Optionally handle error, e.g., show a message
                // The checkbox will revert due to StateHasChanged and unchanged Deadline.IsCompleted
            }

            IsLoading = false;
        }
    }
}