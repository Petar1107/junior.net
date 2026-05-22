using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace AbySalto.Junior.IntegrationTests.Fixtures;

public sealed class AbySaltoApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public new HttpClient CreateClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://localhost"),
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgres.GetConnectionString(),
                ["Seed:AdminEmail"] = TestCredentials.AdminEmail,
                ["Seed:AdminPassword"] = TestCredentials.AdminPassword,
                ["Seed:AdminFirstName"] = TestCredentials.AdminFirstName,
                ["Seed:AdminLastName"] = TestCredentials.AdminLastName,
            });
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }
}
