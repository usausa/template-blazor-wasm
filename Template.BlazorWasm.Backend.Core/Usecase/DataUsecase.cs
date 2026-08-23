namespace Template.BlazorWasm.Usecase;

using Template.BlazorWasm.Models;
using Template.BlazorWasm.Models.Entity;
using Template.BlazorWasm.Services;

public sealed class DataUsecase
{
    private readonly DataService dataService;

    public DataUsecase(DataService dataService)
    {
        this.dataService = dataService;
    }

    public async ValueTask<PagedResult<DataEntity>> QueryPageAsync(string? name, int page, int size)
    {
        var total = await dataService.CountAsync(name);
        var items = await dataService.QueryPageAsync(name, null, false, page * size, size);
        return new PagedResult<DataEntity>(total, page, size, items);
    }
}
