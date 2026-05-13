using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Repository.Context;
using Respawn;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
		.WithImage("postgres:15-alpine")
		.WithDatabase("testdb")
		.WithUsername("postgres")
		.WithPassword("postgres")
		.Build();

	private string _dbConnectionString = null!;
	private Respawner? _respawner;

	public HttpClient AnonymousClient { get; private set; } = null!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureAppConfiguration((context, config) =>
		{
			config.AddInMemoryCollection(new Dictionary<string, string?>
			{
				{ "ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString() }
			});
		});

		builder.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(
				d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
			if (descriptor != null)
				services.Remove(descriptor);

			services.AddDbContext<AppDbContext>(options =>
				options.UseNpgsql(_dbContainer.GetConnectionString())
					   .UseSnakeCaseNamingConvention());
		});
	}

	public async Task InitializeAsync()
	{
		await _dbContainer.StartAsync();
		_dbConnectionString = _dbContainer.GetConnectionString();

		using var scope = Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await db.Database.EnsureCreatedAsync();
		await Repository.Seed.SeedData.InitializeAsync(scope.ServiceProvider);

		AnonymousClient = CreateClient();
	}

	public async Task ResetDatabaseAsync()
	{
		await using var connection = new NpgsqlConnection(_dbConnectionString);
		await connection.OpenAsync();

		_respawner ??= await Respawner.CreateAsync(connection, new RespawnerOptions
		{
			DbAdapter = DbAdapter.Postgres,
			SchemasToInclude = new[] { "public" }
		});

		await _respawner.ResetAsync(connection);

		using var scope = Services.CreateScope();
		await Repository.Seed.SeedData.InitializeAsync(scope.ServiceProvider);
	}

	public async Task DisposeAsync()
	{
		AnonymousClient.Dispose();
		await _dbContainer.DisposeAsync();
		base.Dispose();
	}
}