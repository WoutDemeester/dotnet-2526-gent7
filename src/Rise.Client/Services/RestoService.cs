using System.Net.Http.Json;
using Rise.Shared.Common;
using Rise.Shared.Departments;
using Rise.Shared.Resto;

namespace Rise.Client.Services;

public class RestoService(HttpClient httpClient) : IRestoService
{


    public async Task<Result<RestoResponse.Index>> GetIndexAsync(QueryRequest.SkipTake request, CancellationToken ctx = default)
    {
        var result = await httpClient.GetFromJsonAsync<Result<RestoResponse.Index>>("/api/restos", cancellationToken: ctx);
        return result!;
    }
}
