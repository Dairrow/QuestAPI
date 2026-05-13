using System.Net;
using System.Net.Http.Json;
using System.Text;
using API.DTOs.Responses.Rewards;
using API.DTOs.Responses.UserRewards;
using API.DTOs.Responses.Users;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests;

public class UserRewardsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;

	public UserRewardsControllerIntegrationTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_factory.ResetDatabaseAsync().GetAwaiter().GetResult();
	}

	private async Task<HttpClient> GetAdminClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");

	private async Task<HttpClient> GetPlayerClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");


	[Fact]
	public async Task Player_Sees_Own_Rewards_After_Quest_Completion()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;

		var initial = await player.GetAsync($"/api/users/{playerId}/rewards");
		var initialList = await initial.Content.ReadFromJsonAsync<List<UserRewardResponse>>();
		Assert.Empty(initialList!);

		await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var after = await player.GetFromJsonAsync<List<UserRewardResponse>>($"/api/users/{playerId}/rewards");
		Assert.Single(after!);
		Assert.False(after[0].IsClaimed);
	}

	[Fact]
	public async Task Admin_Can_View_Any_User_Rewards()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var admin = await GetAdminClientAsync();
		var response = await admin.GetFromJsonAsync<List<UserRewardResponse>>($"/api/users/{playerId}/rewards");
		Assert.NotEmpty(response!);
	}

	[Fact]
	public async Task Player_Cannot_View_Other_User_Rewards()
	{
		var player = await GetPlayerClientAsync();
		var moderatorId = 2;
		var response = await player.GetAsync($"/api/users/{moderatorId}/rewards");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Player_Can_Get_Specific_Reward()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var rewardId = await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var response = await player.GetAsync($"/api/users/{playerId}/rewards/{rewardId}");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var reward = await response.Content.ReadFromJsonAsync<UserRewardResponse>();
		Assert.Equal(rewardId, reward!.Id);
	}

	[Fact]
	public async Task Player_Cannot_Get_Reward_Of_Other_User()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var rewardId = await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var moderator = await _factory.CreateAuthenticatedClientAsync("moderator@example.com", "Moder1234");
		var response = await moderator.GetAsync($"/api/users/{playerId}/rewards/{rewardId}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Create_Reward_Manually()
	{
		var admin = await GetAdminClientAsync();
		var playerId = (await GetAdminClientAsync())
			.GetFromJsonAsync<List<UserResponse>>("/api/users").Result!
			.First(u => u.Email == "player@example.com").Id;

		var rewards = await admin.GetFromJsonAsync<List<RewardResponse>>("/api/rewards");
		var rewardId = rewards!.First().Id;

		var response = await admin.PostAsJsonAsync($"/api/users/{playerId}/rewards", new
		{
			RewardId = rewardId
		});
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var created = await response.Content.ReadFromJsonAsync<UserRewardResponse>();
		Assert.Equal(rewardId, created!.RewardId);
	}

	[Fact]
	public async Task Regular_User_Cannot_Create_Reward_For_Other()
	{
		var moderator = await _factory.CreateAuthenticatedClientAsync("moderator@example.com", "Moder1234");
		var playerId = 3;
		var admin = await GetAdminClientAsync();
		var users = await admin.GetFromJsonAsync<List<UserResponse>>("/api/users");
		var realPlayerId = users!.First(u => u.Email == "player@example.com").Id;

		var rewards = await admin.GetFromJsonAsync<List<RewardResponse>>("/api/rewards");
		var response = await moderator.PostAsJsonAsync($"/api/users/{realPlayerId}/rewards", new
		{
			RewardId = rewards!.First().Id
		});
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Claim_Reward_Successfully()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var rewardId = await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var claimContent = new StringContent("{}", Encoding.UTF8, "application/json");
		var response = await player.PutAsync($"/api/users/{playerId}/rewards/{rewardId}", claimContent);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var claimed = await response.Content.ReadFromJsonAsync<UserRewardResponse>();
		Assert.True(claimed!.IsClaimed);
	}

	[Fact]
	public async Task Cannot_Claim_Already_Claimed_Reward()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var rewardId = await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var claimContent = new StringContent("{}", Encoding.UTF8, "application/json");
		await player.PutAsync($"/api/users/{playerId}/rewards/{rewardId}", claimContent);

		var secondResponse = await player.PutAsync($"/api/users/{playerId}/rewards/{rewardId}", claimContent);
		Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
	}

	[Fact]
	public async Task Cannot_Claim_Other_User_Reward()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var rewardId = await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var moderator = await _factory.CreateAuthenticatedClientAsync("moderator@example.com", "Moder1234");
		var claimContent = new StringContent("{}", Encoding.UTF8, "application/json");
		var response = await moderator.PutAsync($"/api/users/{playerId}/rewards/{rewardId}", claimContent);
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Delete_Any_User_Reward()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var rewardId = await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var admin = await GetAdminClientAsync();
		var response = await admin.DeleteAsync($"/api/users/{playerId}/rewards/{rewardId}");
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		var after = await admin.GetFromJsonAsync<List<UserRewardResponse>>($"/api/users/{playerId}/rewards");
		Assert.Empty(after!);
	}

	[Fact]
	public async Task Player_Cannot_Delete_Other_User_Reward()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var rewardId = await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var moderator = await _factory.CreateAuthenticatedClientAsync("moderator@example.com", "Moder1234");
		var response = await moderator.DeleteAsync($"/api/users/{playerId}/rewards/{rewardId}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Player_Can_Delete_Own_Reward()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var rewardId = await player.CompleteQuestAndGetFirstRewardIdAsync(playerId);

		var response = await player.DeleteAsync($"/api/users/{playerId}/rewards/{rewardId}");
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}
}