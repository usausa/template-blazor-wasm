namespace Template.BlazorWasm.Backend.Host.Endpoints;

using Microsoft.FeatureManagement;

using Template.BlazorWasm.Backend.Host.Application;

public sealed record FeatureGetResponse(bool CustomOption);

public static class FeatureEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapFeatureEndpoints(this WebApplication app)
    {
        var group = app.MapApiGroup(ApiRoutes.Features);

        group.MapGet("/", HandleGetAsync)
            .WithName("FeatureGet")
            .Produces<FeatureGetResponse>();
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleGetAsync(IFeatureManager featureManager) =>
        TypedResults.Ok(new FeatureGetResponse(await featureManager.IsEnabledAsync(FeatureFlags.CustomOption)));
}
