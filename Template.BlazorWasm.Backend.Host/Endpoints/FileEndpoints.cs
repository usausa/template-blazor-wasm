namespace Template.BlazorWasm.Backend.Host.Endpoints;

using Template.BlazorWasm.Backend.Host.Application;
using Template.BlazorWasm.Backend.Host.Infrastructure.Filters;
using Template.BlazorWasm.Backend.Host.Models.File;
using Template.BlazorWasm.Infrastructure.Storage;

public static class FileEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapFileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Files)
            .RequireAuthorization()
            .AddEndpointFilter<StorageExceptionFilter>();

        group.MapGet("/list/{**path}", HandleListAsync)
            .WithName("ListFiles")
            .Produces<FileListResponse>()
            .Produces(StatusCodes.Status404NotFound);
        group.MapGet("/download/{**path}", HandleDownloadAsync)
            .WithName("DownloadFile")
            .Produces(StatusCodes.Status200OK, contentType: "application/octet-stream")
            .Produces(StatusCodes.Status404NotFound);
        group.MapDelete("/{**path}", HandleDeleteAsync)
            .RequireAuthorization(Policies.Administrator)
            .WithName("DeleteFile")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleListAsync(
        IStorage storage,
        string? path,
        CancellationToken cancellationToken)
    {
        path ??= string.Empty;

        if (!await storage.DirectoryExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var entries = await storage.ListAsync(path, cancellationToken);
        return TypedResults.Ok(new FileListResponse(entries));
    }

    private static async ValueTask<IResult> HandleDownloadAsync(
        IStorage storage,
        string path,
        CancellationToken cancellationToken)
    {
        if (!await storage.FileExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var stream = await storage.ReadAsync(path, cancellationToken);
        return TypedResults.Stream(stream, "application/octet-stream", Path.GetFileName(path));
    }

    private static async ValueTask<IResult> HandleDeleteAsync(
        IStorage storage,
        string path,
        CancellationToken cancellationToken)
    {
        if (!await storage.FileExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        await storage.DeleteAsync(path, cancellationToken);
        return TypedResults.NoContent();
    }
}
