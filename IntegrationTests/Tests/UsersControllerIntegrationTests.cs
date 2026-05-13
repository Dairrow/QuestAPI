using System.Net;
using System.Net.Http.Json;
using API.DTOs.Responses.Users;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests;

public class UsersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;
	private HttpClient _anonymousClient;

	public UsersControllerIntegrationTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_factory.ResetDatabaseAsync().GetAwaiter().GetResult();
		_anonymousClient = _factory.AnonymousClient;
	}


	[Fact]
	public async Task Admin_GetAll_Returns_All_Users()
	{
		var client = await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");
		var response = await client.GetAsync("/api/users");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
		Assert.NotNull(users);
		Assert.True(users.Count >= 3);
	}

	[Fact]
	public async Task Regular_User_GetAll_Returns_Forbidden()
	{
		var client = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var response = await client.GetAsync("/api/users");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Get_Any_User_By_Id()
	{
		var adminClient = await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");
		var allResponse = await adminClient.GetFromJsonAsync<List<UserResponse>>("/api/users");
		var playerId = allResponse!.First(u => u.Email == "player@example.com").Id;

		var response = await adminClient.GetAsync($"/api/users/{playerId}");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var user = await response.Content.ReadFromJsonAsync<UserResponse>();
		Assert.Equal("player", user!.Username);
	}

	[Fact]
	public async Task Regular_User_Can_Get_Own_Profile()
	{
		var playerClient = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var authResponse = await playerClient.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = authResponse.UserId;

		var response = await playerClient.GetAsync($"/api/users/{playerId}");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var user = await response.Content.ReadFromJsonAsync<UserResponse>();
		Assert.Equal("player", user!.Username);
	}

	[Fact]
	public async Task Regular_User_Cannot_Get_Other_User_Profile()
	{
		var client = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var adminId = 1;
		var response = await client.GetAsync($"/api/users/{adminId}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Create_User()
	{
		var adminClient = await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");
		var newUser = new
		{
			Username = "newplayer",
			Email = "newplayer@example.com",
			Password = "NewPlayer123",
			Role = 3
		};
		var response = await adminClient.PostAsJsonAsync("/api/users", newUser);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var created = await response.Content.ReadFromJsonAsync<UserResponse>();
		Assert.Equal("newplayer", created!.Username);
	}

	[Fact]
	public async Task Regular_User_Cannot_Create_User()
	{
		var client = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var response = await client.PostAsJsonAsync("/api/users", new
		{
			Username = "newuser",
			Email = "new@example.com",
			Password = "Pass1234",
			Role = "User"
		});
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Update_User_Info_Without_Changing_Password()
	{
		var adminClient = await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");
		var allUsers = await adminClient.GetFromJsonAsync<List<UserResponse>>("/api/users");
		var playerId = allUsers!.First(u => u.Email == "player@example.com").Id;

		var updateData = new
		{
			Username = "player_updated",
			Email = "player_updated@example.com",
			Role = 2
		};
		var response = await adminClient.PutAsJsonAsync($"/api/users/{playerId}/admin", updateData);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var updated = await response.Content.ReadFromJsonAsync<UserResponse>();
		Assert.Equal("player_updated", updated!.Username);
		Assert.Equal("Moderator", updated.Role.ToString());
	}

	[Fact]
	public async Task Regular_User_Cannot_Update_Other_User()
	{
		var playerClient = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var adminId = 1;
		var response = await playerClient.PutAsJsonAsync($"/api/users/{adminId}", new
		{
			Username = "hack",
			Email = "hack@example.com",
			Role = "Admin"
		});
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Admin_Update_NonExistent_User_Returns_NotFound()
	{
		var adminClient = await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");
		var response = await adminClient.PutAsJsonAsync("/api/users/9999", new
		{
			Username = "ghost",
			Email = "ghost@example.com",
			Role = "User"
		});
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}


	[Fact]
	public async Task User_Can_Change_Own_Password()
	{
		var playerClient = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var authResponse = await playerClient.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = authResponse.UserId;

		var changeData = new
		{
			OldPassword = "Player1234",
			NewPassword = "NewPlayerPass123"
		};
		var response = await playerClient.PutAsJsonAsync($"/api/users/{playerId}/change-password", changeData);
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		var loginResponse = await _anonymousClient.PostAsJsonAsync("/api/auth/login", new
		{
			Email = "player@example.com",
			Password = "Player1234"
		});
		Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);

		loginResponse = await _anonymousClient.PostAsJsonAsync("/api/auth/login", new
		{
			Email = "player@example.com",
			Password = "NewPlayerPass123"
		});
		Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
	}

	[Fact]
	public async Task User_Cannot_Change_Password_With_Wrong_OldPassword()
	{
		var playerClient = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var playerId = (await playerClient.LoginAndGetResponseAsync("player@example.com", "Player1234")).UserId;

		var response = await playerClient.PutAsJsonAsync($"/api/users/{playerId}/change-password", new
		{
			OldPassword = "WrongOldPass",
			NewPassword = "NewPass123"
		});
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task User_Cannot_Change_Password_Of_Another_User()
	{
		var playerClient = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var adminId = 1;
		var response = await playerClient.PutAsJsonAsync($"/api/users/{adminId}/change-password", new
		{
			OldPassword = "Admin1234",
			NewPassword = "HackedPass1"
		});
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Delete_User()
	{
		var adminClient = await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");
		var allUsers = await adminClient.GetFromJsonAsync<List<UserResponse>>("/api/users");
		var playerId = allUsers!.First(u => u.Email == "player@example.com").Id;

		var response = await adminClient.DeleteAsync($"/api/users/{playerId}");
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		response = await adminClient.GetAsync($"/api/users/{playerId}");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Regular_User_Cannot_Delete_User()
	{
		var playerClient = await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");
		var adminId = 1;
		var response = await playerClient.DeleteAsync($"/api/users/{adminId}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}
}