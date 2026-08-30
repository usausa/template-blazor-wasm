namespace Template.BlazorWasm.Backend.Host.Endpoints;

using Microsoft.FeatureManagement;

using Template.BlazorWasm.Backend.Host.Application;

public sealed record FeatureResponse(bool CustomOption);

public static class FeatureEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapFeatureEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Features);

        group.MapGet("/", HandleGetAsync)
            .WithName("GetFeatures")
            .Produces<FeatureResponse>();
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleGetAsync(IFeatureManager featureManager) =>
        TypedResults.Ok(new FeatureResponse(await featureManager.IsEnabledAsync(FeatureFlags.CustomOption)));
}
