namespace Template.BlazorWasm.Backend.Host.Settings;

public sealed class AuthSetting
{
    [Required]
    [MinLength(32)]
    public string SecretKey { get; set; } = default!;

    [Required]
    public string Issuer { get; set; } = default!;

    [Required]
    public string Audience { get; set; } = default!;

    [Range(1, 1440)]
    public int ExpireMinutes { get; set; }

    // リフレッシュトークンの有効期間(日)。アクセストークンより十分長くする
    [Range(1, 365)]
    public int RefreshExpireDays { get; set; }

    [Required]
    public string InitialId { get; set; } = default!;

    [Required]
    public string InitialPassword { get; set; } = default!;
}
