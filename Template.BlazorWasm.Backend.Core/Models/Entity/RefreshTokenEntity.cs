namespace Template.BlazorWasm.Models.Entity;

public sealed class RefreshTokenEntity
{
    public long Id { get; set; }

#pragma warning disable CA1819
    public byte[] TokenHash { get; set; } = default!;
#pragma warning restore CA1819

    public string AccountName { get; set; } = default!;

    public DateTime ExpireAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
