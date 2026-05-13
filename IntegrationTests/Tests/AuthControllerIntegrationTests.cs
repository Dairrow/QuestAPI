using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using IntegrationTests.Helpers;
using API.DTOs.Responses.Auth;
using Xunit;

namespace IntegrationTests;

public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;
	private HttpClient _client = null!;

	public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_factory.ResetDatabaseAsync().GetAwaiter().GetResult();
	}

	private HttpClient CreateClient() => _factory.CreateClient();

	[Fact]
	public async Task Register_With_Valid_Data_Returns_Tokens()
	{
		var client = CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/register", new
		{
			Username = "newuser",
			Email = "newuser@example.com",
			Password = "StrongPass1"
		});

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
		Assert.NotNull(auth);
		Assert.NotEmpty(auth.AccessToken);
		Assert.NotEmpty(auth.RefreshToken);
		Assert.Equal("newuser", auth.Username);
	}

	[Fact]
	public async Task Register_With_Existing_Email_Returns_Conflict()
	{
		var client = CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/register", new
		{
			Username = "another",
			Email = "admin@example.com",
			Password = "ValidPass123"
		});

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Register_With_Invalid_Data_Returns_BadRequest()
	{
		var client = CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/register", new
		{
			Username = "user",
			Email = "user@example.com",
			Password = "123"
		});

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Login_With_Valid_Credentials_Returns_Tokens()
	{
		var client = CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/login", new
		{
			Email = "admin@example.com",
			Password = "Admin1234"
		});

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
		Assert.NotEmpty(auth!.AccessToken);
		Assert.NotEmpty(auth.RefreshToken);
		Assert.Equal("admin", auth.Username);
	}

	[Fact]
	public async Task Login_With_Invalid_Credentials_Returns_BadRequest()
	{
		var client = CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/login", new
		{
			Email = "admin@example.com",
			Password = "WrongPassword"
		});

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Login_With_Nonexistent_Email_Returns_BadRequest()
	{
		var client = CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/login", new
		{
			Email = "nonexist@example.com",
			Password = "SomePass1"
		});

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Refresh_With_Valid_Token_Returns_New_Tokens()
	{
		var client = CreateClient();

		var loginResponse = await client.LoginAndGetResponseAsync("admin@example.com", "Admin1234");
		var oldRefresh = loginResponse.RefreshToken;

		await Task.Delay(1000);

		var refreshRequest = new { RefreshToken = oldRefresh };
		var response = await client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var newAuth = await response.Content.ReadFromJsonAsync<AuthResponse>();
		Assert.NotNull(newAuth);
		Assert.NotEqual(oldRefresh, newAuth.RefreshToken);
		Assert.NotEqual(loginResponse.AccessToken, newAuth.AccessToken);
	}

	[Fact]
	public async Task Refresh_With_Invalid_Token_Returns_BadRequest()
	{
		var client = CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/refresh", new
		{
			RefreshToken = "invalid-token-value"
		});

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Refresh_With_Revoked_Token_Returns_BadRequest()
	{
		var client = CreateClient();

		var auth1 = await client.LoginAndGetResponseAsync("admin@example.com", "Admin1234");
		var oldRefresh = auth1.RefreshToken;

		var auth2 = await client.LoginAndGetResponseAsync("admin@example.com", "Admin1234");

		var response = await client.PostAsJsonAsync("/api/auth/refresh", new
		{
			RefreshToken = oldRefresh
		});

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}