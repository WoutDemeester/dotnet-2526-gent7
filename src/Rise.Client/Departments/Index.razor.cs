using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.Departments;

namespace Rise.Client.Departments;

public partial class Index
{
    private IEnumerable<DepartmentDto.Index>? departments;

    [Inject] public required IDepartmentService DepartmentService { get; set; }

    /*
    protected override async Task OnInitializedAsync()
    {
        var request = new QueryRequest.SkipTake
        {
            Skip = 0,
            Take = 50,
            OrderBy = "Id",
        };

        var result = await DepartmentService.GetIndexAsync(request);
        departments = result.Value.Departments;
    }
    */
}

