    using System.Net.Http.Json;
    using Rise.Shared.Common;
    using Rise.Shared.Departments;

    namespace Rise.Client.Products;

    public class DepartmentService(HttpClient httpClient) : IDepartmentService
    {

        public async Task<Result<DepartmentResponse.Create>> CreateAsync(DepartmentRequest.Create request, CancellationToken ctx = default)
        {
            var response = await httpClient.PostAsJsonAsync("/api/departments", request, ctx);
            var result = await response.Content.ReadFromJsonAsync<Result<DepartmentResponse.Create>>(cancellationToken: ctx);
            return result!;
        }

        public async Task<Result<DepartmentResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
        {
            var result = await httpClient.GetFromJsonAsync<Result<DepartmentResponse.Index>>("/api/departments", cancellationToken: ctx);
            return result!;
        }
    }
