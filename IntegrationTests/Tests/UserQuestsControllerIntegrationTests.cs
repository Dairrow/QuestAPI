using System.Net;
using System.Net.Http.Json;
using API.DTOs.Responses.Quests;
using API.DTOs.Responses.UserQuests;
using API.DTOs.Responses.UserRewards;
using API.DTOs.Responses.Users;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests;

public class UserQuestsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;

	public UserQuestsControllerIntegrationTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_factory.ResetDatabaseAsync().GetAwaiter().GetResult();
	}

	private async Task<HttpClient> GetAdminClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");

	private async Task<HttpClient> GetPlayerClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");

	private async Task<int> GetPlayerUserIdAsync(HttpClient adminClient)
	{
		var users = await adminClient.GetFromJsonAsync<List<UserResponse>>("/api/users");
		return users!.First(u => u.Email == "player@example.com").Id;
	}

	private async Task<int> GetFirstQuestIdAsync(HttpClient adminClient)
	{
		var quests = await adminClient.GetFromJsonAsync<List<QuestResponse>>("/api/quests");
		return quests!.First(q => q.Title == "Первое приключение").Id;
	}


	[Fact]
	public async Task Admin_Can_Get_Any_User_Quests()
	{
		var admin = await GetAdminClientAsync();
		var playerId = await GetPlayerUserIdAsync(admin);

		var response = await admin.GetAsync($"/api/users/{playerId}/quests");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var quests = await response.Content.ReadFromJsonAsync<List<UserQuestResponse>>();
		Assert.NotEmpty(quests!);
	}

	[Fact]
	public async Task Player_Can_Get_Own_Quests()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;

		var response = await player.GetAsync($"/api/users/{playerId}/quests");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var quests = await response.Content.ReadFromJsonAsync<List<UserQuestResponse>>();
		Assert.NotEmpty(quests!);
	}

	[Fact]
	public async Task Player_Cannot_Get_Other_User_Quests()
	{
		var player = await GetPlayerClientAsync();
		var adminId = 1;
		var response = await player.GetAsync($"/api/users/{adminId}/quests");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Assign_Quest_To_User()
	{
		var admin = await GetAdminClientAsync();
		var playerId = await GetPlayerUserIdAsync(admin);
		var questId = (await admin.GetFromJsonAsync<List<QuestResponse>>("/api/quests"))!
			.First(q => q.Title == "Опасный лес").Id;

		var response = await admin.PostAsJsonAsync($"/api/users/{playerId}/quests", new
		{
			QuestId = questId
		});
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var userQuest = await response.Content.ReadFromJsonAsync<UserQuestResponse>();
		Assert.Equal(questId, userQuest!.QuestId);
		Assert.False(userQuest.IsCompleted);
	}

	[Fact]
	public async Task Player_Can_Assign_Quest_To_Himself()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var admin = await GetAdminClientAsync();
		var quests = await admin.GetFromJsonAsync<List<QuestResponse>>("/api/quests");
		var otherQuestId = quests!.First(q => q.Title == "Подземелье ужаса").Id;

		var response = await player.PostAsJsonAsync($"/api/users/{playerId}/quests", new
		{
			QuestId = otherQuestId
		});
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task Assign_Duplicate_Quest_Returns_BadRequest()
	{
		var admin = await GetAdminClientAsync();
		var playerId = await GetPlayerUserIdAsync(admin);
		var questId = await GetFirstQuestIdAsync(admin);

		var response = await admin.PostAsJsonAsync($"/api/users/{playerId}/quests", new
		{
			QuestId = questId
		});
		Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
	}


	[Fact]
	public async Task Complete_Quest_Step_By_Step_And_Receive_Reward()
	{
		var admin = await GetAdminClientAsync();
		var playerId = await GetPlayerUserIdAsync(admin);
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerIdFromLogin = auth.UserId;

		var userQuests = await player.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerIdFromLogin}/quests");
		var userQuest = userQuests!.First();

		for (int order = 1; order <= 3; order++)
		{
			var putResponse = await player.PutAsJsonAsync($"/api/users/{playerIdFromLogin}/quests/{userQuest.Id}",
				new { TaskOrder = order });
			Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
			var updated = await putResponse.Content.ReadFromJsonAsync<UserQuestResponse>();
			Assert.Equal(order, updated!.LastCompletedTaskOrder);
			if (order < 3)
				Assert.False(updated.IsCompleted);
			else
				Assert.True(updated.IsCompleted);
		}

		var rewardsResponse = await player.GetAsync($"/api/users/{playerIdFromLogin}/rewards");
		Assert.Equal(HttpStatusCode.OK, rewardsResponse.StatusCode);
		var rewards = await rewardsResponse.Content.ReadFromJsonAsync<List<InventoryResponse>>();
		Assert.Single(rewards!);
		Assert.False(rewards[0].IsClaimed);
	}

	[Fact]
	public async Task Reset_Completed_Quest_Sets_IsCompleted_False_And_Resets_Claim()
	{
		var admin = await GetAdminClientAsync();
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;

		var userQuests = await player.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var userQuest = userQuests!.First();
		for (int i = 1; i <= 3; i++)
			await player.PutAsJsonAsync($"/api/users/{playerId}/quests/{userQuest.Id}", new { TaskOrder = i });

		var rewards = await player.GetFromJsonAsync<List<InventoryResponse>>($"/api/users/{playerId}/rewards");
		var rewardId = rewards!.First().Id;

		await player.PutAsync($"/api/users/{playerId}/rewards/{rewardId}", null);
		var rewardAfterClaim = await player.GetFromJsonAsync<InventoryResponse>($"/api/users/{playerId}/rewards/{rewardId}");
		Assert.True(rewardAfterClaim!.IsClaimed);

		var resetResponse = await player.PutAsJsonAsync($"/api/users/{playerId}/quests/{userQuest.Id}",
			new { TaskOrder = 0 });
		Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);
		var updatedQuest = await resetResponse.Content.ReadFromJsonAsync<UserQuestResponse>();
		Assert.False(updatedQuest!.IsCompleted);
		Assert.Null(updatedQuest.LastCompletedTaskOrder);

		var rewardAfterReset = await player.GetFromJsonAsync<InventoryResponse>($"/api/users/{playerId}/rewards/{rewardId}");
		Assert.NotNull(rewardAfterReset);
		Assert.False(rewardAfterReset!.IsClaimed);
	}

	[Fact]
	public async Task Cannot_Skip_Task_Order()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var userQuests = await player.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var userQuest = userQuests!.First();

		var response = await player.PutAsJsonAsync($"/api/users/{playerId}/quests/{userQuest.Id}",
			new { TaskOrder = 2 });
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Cannot_Reset_Non_Completed_Quest()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var userQuests = await player.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var userQuest = userQuests!.First();
		var response = await player.PutAsJsonAsync($"/api/users/{playerId}/quests/{userQuest.Id}",
			new { TaskOrder = 0 });
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Player_Cannot_Update_Other_User_Quest()
	{
		var admin = await GetAdminClientAsync();
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var userQuests = await player.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var userQuest = userQuests!.First();

		var moderator = await _factory.CreateAuthenticatedClientAsync("moderator@example.com", "Moder1234");
		var modAuth = await moderator.LoginAndGetResponseAsync("moderator@example.com", "Moder1234");
		var modId = modAuth.UserId;
		var response = await moderator.PutAsJsonAsync($"/api/users/{playerId}/quests/{userQuest.Id}",
			new { TaskOrder = 1 });
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Delete_User_Quest()
	{
		var admin = await GetAdminClientAsync();
		var playerId = await GetPlayerUserIdAsync(admin);
		var userQuests = await admin.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var idToDelete = userQuests!.First().Id;

		var response = await admin.DeleteAsync($"/api/users/{playerId}/quests/{idToDelete}");
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		var remaining = await admin.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		Assert.Empty(remaining!);
	}

	[Fact]
	public async Task Player_Can_Delete_Own_Quest()
	{
		var player = await GetPlayerClientAsync();
		var auth = await player.LoginAndGetResponseAsync("player@example.com", "Player1234");
		var playerId = auth.UserId;
		var userQuests = await player.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var idToDelete = userQuests!.First().Id;

		var response = await player.DeleteAsync($"/api/users/{playerId}/quests/{idToDelete}");
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}

	[Fact]
	public async Task Player_Cannot_Delete_Other_User_Quest()
	{
		var admin = await GetAdminClientAsync();
		var playerId = await GetPlayerUserIdAsync(admin);
		var moderator = await _factory.CreateAuthenticatedClientAsync("moderator@example.com", "Moder1234");

		var userQuests = await admin.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var idToDelete = userQuests!.First().Id;

		var response = await moderator.DeleteAsync($"/api/users/{playerId}/quests/{idToDelete}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}
}