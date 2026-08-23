namespace Template.BlazorWasm.Infrastructure.Security;

public interface IPasswordProvider
{
    bool Match(string password, byte[] hash);

    byte[] Generate(string password);
}
