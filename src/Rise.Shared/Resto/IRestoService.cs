using Rise.Shared.Common;

namespace Rise.Shared.Resto;

/// <summary>
/// Provides methods for managing resto-related operations.
/// </summary>
public interface IRestoService
{
    //Task<Result<Departments.DepartmentResponse.Create>> CreateAsync(DepartmentRequest.Create request, CancellationToken ctx = default);
    Task<Result<Resto.RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default);
}