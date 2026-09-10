namespace Template.BlazorWasm.Accessors;

[DataAccessor]
public sealed partial class RefreshTokenAccessor
{
    [Execute]
    public partial void Create();

    [Execute]
    public partial ValueTask<int> InsertAsync(byte[] tokenHash, string accountName, DateTime expireAt, DateTime createdAt);

    [QueryFirst]
    public partial ValueTask<RefreshTokenEntity?> QueryByHashAsync(byte[] tokenHash);

    [Execute]
    public partial ValueTask<int> DeleteAsync(long id);

    [Execute]
    public partial ValueTask<int> DeleteByHashAsync(byte[] tokenHash);

    [Execute]
    public partial ValueTask<int> DeleteExpiredAsync(DateTime now);
}
