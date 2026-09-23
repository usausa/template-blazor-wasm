namespace Template.BlazorWasm.Services;

using Template.BlazorWasm.Accessors;
using Template.BlazorWasm.Infrastructure.Security;
using Template.BlazorWasm.Models.Entity;

public sealed class AccountService
{
    private readonly AccountAccessor accountAccessor;

    private readonly IPasswordProvider passwordProvider;

    private readonly ServiceContextProvider contextProvider;

    public AccountService(
        AccountAccessor accountAccessor,
        IPasswordProvider passwordProvider,
        ServiceContextProvider contextProvider)
    {
        this.accountAccessor = accountAccessor;
        this.passwordProvider = passwordProvider;
        this.contextProvider = contextProvider;
    }

    // Seed initial account
    public async ValueTask InitializeAsync(InitialAccountOption option, string role)
    {
        var count = await accountAccessor.CountAsync();
        if (count == 0)
        {
            var context = contextProvider.Current;
            await accountAccessor.InsertAsync(option.Id, passwordProvider.Generate(option.Password), role, context.Now.DateTime);
        }
    }

    public ValueTask<AccountEntity?> QueryAsync(string name) =>
        accountAccessor.QueryByNameAsync(name);

    public async ValueTask<AccountEntity?> AuthenticateAsync(string name, string password)
    {
        var account = await accountAccessor.QueryByNameAsync(name);
        if (account is null)
        {
            return null;
        }

        return passwordProvider.Match(password, account.Password) ? account : null;
    }
}
