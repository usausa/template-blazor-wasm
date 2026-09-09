namespace Template.BlazorWasm.Services;

using System.Buffers.Text;
using System.Security.Cryptography;

using Template.BlazorWasm.Accessors;

// リフレッシュトークンは乱数値をそのまま返し、DBにはハッシュのみ保存する(漏洩時の再利用を防ぐ)
public sealed class RefreshTokenService
{
    private const int TokenSize = 32;

    private readonly RefreshTokenAccessor refreshTokenAccessor;

    private readonly TimeProvider timeProvider;

    public RefreshTokenService(
        RefreshTokenAccessor refreshTokenAccessor,
        TimeProvider timeProvider)
    {
        this.refreshTokenAccessor = refreshTokenAccessor;
        this.timeProvider = timeProvider;
    }

    public void CreateTable() =>
        refreshTokenAccessor.Create();

    public async ValueTask<string> IssueAsync(string accountName, int expireDays)
    {
        var now = timeProvider.GetLocalNow().DateTime;

        // 期限切れの行をここでまとめて掃除する(専用のバックグラウンド処理は設けない)
        await refreshTokenAccessor.DeleteExpiredAsync(now);

        var token = RandomNumberGenerator.GetBytes(TokenSize);
        await refreshTokenAccessor.InsertAsync(SHA256.HashData(token), accountName, now.AddDays(expireDays), now);

        return Base64Url.EncodeToString(token);
    }

    // 有効なら消費してアカウント名を返す。トークンは1回限り(ローテーション)
    public async ValueTask<string?> ConsumeAsync(string token)
    {
        var hash = HashToken(token);
        if (hash is null)
        {
            return null;
        }

        var entity = await refreshTokenAccessor.QueryByHashAsync(hash);
        if (entity is null)
        {
            return null;
        }

        await refreshTokenAccessor.DeleteAsync(entity.Id);

        return entity.ExpireAt > timeProvider.GetLocalNow().DateTime ? entity.AccountName : null;
    }

    public async ValueTask RevokeAsync(string token)
    {
        var hash = HashToken(token);
        if (hash is not null)
        {
            await refreshTokenAccessor.DeleteByHashAsync(hash);
        }
    }

    // 不正な文字列はTryDecodeFromCharsが例外を投げるため、先に形式と長さを検証する
    private static byte[]? HashToken(string token)
    {
        if (!Base64Url.IsValid(token, out var length) || (length != TokenSize))
        {
            return null;
        }

        Span<byte> buffer = stackalloc byte[TokenSize];
        return Base64Url.TryDecodeFromChars(token, buffer, out _) ? SHA256.HashData(buffer) : null;
    }
}
