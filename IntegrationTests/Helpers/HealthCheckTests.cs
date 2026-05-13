using System.Net;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests;

public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;

	public HealthCheckTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
	}

	[Fact]
	public async Task Application_Starts_And_Responds()
	{
		var client = _factory.AnonymousClient;
		var response = await client.GetAsync("/api/auth/register");
		Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
	}

	[Fact]
	public async Task Admin_Can_Login_After_Seeding()
	{
		var client = _factory.CreateClient();
		await client.AuthenticateAsync("admin@example.com", "Admin1234");

		var response = await client.GetAsync("/api/users");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}