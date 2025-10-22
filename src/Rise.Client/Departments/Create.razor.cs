using Microsoft.AspNetCore.Components;
using Rise.Shared.Departments;

namespace Rise.Client.Products;


public partial class Create
{
    [Inject] public required IDepartmentService DepartmentService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    private DepartmentRequest.Create Model { get; set; } = new();
    private Result<DepartmentResponse.Create>? _result;
    private async Task CreateProductAsync()
    {
        _result = await DepartmentService.CreateAsync(Model);
        if (_result.IsSuccess)
        {
            NavigationManager.NavigateTo("/product");
        }
    }
}