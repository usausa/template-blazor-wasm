namespace Template.BlazorWasm.Frontend.App.Infrastructure.Authentication;

public static class JwtParser
{
    // 期限マージン(残り時間がこれを切ったら失効扱いにする)
    private static readonly TimeSpan ExpireMargin = TimeSpan.FromSeconds(30);

    public static ClaimsPrincipal? Parse(string token, DateTimeOffset now)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(Base64UrlDecode(parts[1]));
            var root = document.RootElement;

            // 期限判定
            if (!root.TryGetProperty("exp", out var expProperty) ||
                (DateTimeOffset.FromUnixTimeSeconds(expProperty.GetInt64()) <= now.Add(ExpireMargin)))
            {
                return null;
            }

            var claims = new List<Claim>();
            foreach (var property in root.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    claims.AddRange(property.Value.EnumerateArray().Select(x => new Claim(property.Name, x.ToString())));
                }
                else
                {
                    claims.Add(new Claim(property.Name, property.Value.ToString()));
                }
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "jwt", nameType: "sub", roleType: "role"));
        }
        catch (JsonException)
        {
            return null;
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        return Convert.FromBase64String(base64.PadRight(base64.Length + ((4 - (base64.Length % 4)) % 4), '='));
    }
}
