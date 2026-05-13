using System.Net;
using System.Net.Http.Json;
using API.DTOs.Responses.Quests;
using API.DTOs.Responses.Rewards;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests;

public class QuestsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;

	public QuestsControllerIntegrationTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_factory.ResetDatabaseAsync().GetAwaiter().GetResult();
	}

	private async Task<HttpClient> GetAdminClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");

	private async Task<HttpClient> GetPlayerClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");


	[Fact]
	public async Task Admin_GetAll_Returns_Three_Quests()
	{
		var client = await GetAdminClientAsync();
		var response = await client.GetAsync("/api/quests");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var quests = await response.Content.ReadFromJsonAsync<List<QuestResponse>>();
		Assert.NotNull(quests);
		Assert.Equal(3, quests.Count);
	}

	[Fact]
	public async Task Regular_User_GetAll_Returns_Forbidden()
	{
		var client = await GetPlayerClientAsync();
		var response = await client.GetAsync("/api/quests");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_GetById_Returns_Existing_Quest()
	{
		var client = await GetAdminClientAsync();
		var quests = await client.GetFromJsonAsync<List<QuestResponse>>("/api/quests");
		var target = quests!.First(q => q.Title == "Первое приключение");

		var response = await client.GetAsync($"/api/quests/{target.Id}");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var quest = await response.Content.ReadFromJsonAsync<QuestResponse>();
		Assert.Equal(target.Id, quest!.Id);
	}

	[Fact]
	public async Task Regular_User_GetById_Returns_Forbidden()
	{
		var player = await GetPlayerClientAsync();
		var response = await player.GetAsync("/api/quests/1");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Admin_GetById_NonExistent_Returns_NotFound()
	{
		var client = await GetAdminClientAsync();
		var response = await client.GetAsync("/api/quests/9999");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Create_Quest_Without_Reward()
	{
		var client = await GetAdminClientAsync();
		var newQuest = new
		{
			Title = "New Test Quest",
			Description = "Created in integration test",
			Difficulty = 2
		};

		var response = await client.PostAsJsonAsync("/api/quests", newQuest);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var quest = await response.Content.ReadFromJsonAsync<QuestResponse>();
		Assert.Equal("New Test Quest", quest!.Title);
		Assert.Null(quest.RewardId);
	}

	[Fact]
	public async Task Admin_Create_Quest_With_Reward()
	{
		var client = await GetAdminClientAsync();

		var rewards = await client.GetFromJsonAsync<List<RewardResponse>>("/api/rewards");
		Assert.NotEmpty(rewards!);
		var rewardId = rewards!.First(r => r.Name == "Золотая монета").Id;

		var newQuest = new
		{
			Title = "Reward Quest",
			Description = "Quest with reward",
			Difficulty = 3,
			RewardId = rewardId
		};

		var response = await client.PostAsJsonAsync("/api/quests", newQuest);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var quest = await response.Content.ReadFromJsonAsync<QuestResponse>();
		Assert.Equal(rewardId, quest!.RewardId);
	}

	[Fact]
	public async Task Regular_User_Cannot_Create_Quest()
	{
		var client = await GetPlayerClientAsync();
		var response = await client.PostAsJsonAsync("/api/quests", new
		{
			Title = "Hack",
			Description = "Hack",
			Difficulty = 1
		});
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Update_Quest_And_Change_Reward()
	{
		var client = await GetAdminClientAsync();
		var quests = await client.GetFromJsonAsync<List<QuestResponse>>("/api/quests");
		var target = quests!.First(q => q.Title == "Опасный лес");

		var rewards = await client.GetFromJsonAsync<List<RewardResponse>>("/api/rewards");
		var rewardId = rewards!.First(r => r.Name == "Опыт 500").Id;

		var update = new
		{
			Title = "Лес обновлён",
			Description = "Новое описание",
			Difficulty = 4,
			RewardId = rewardId
		};

		var response = await client.PutAsJsonAsync($"/api/quests/{target.Id}", update);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var updated = await response.Content.ReadFromJsonAsync<QuestResponse>();
		Assert.Equal("Лес обновлён", updated!.Title);
		Assert.Equal(rewardId, updated.RewardId);
	}

	[Fact]
	public async Task Regular_User_Cannot_Update_Quest()
	{
		var player = await GetPlayerClientAsync();
		var response = await player.PutAsJsonAsync("/api/quests/1", new
		{
			Title = "Hack",
			Description = "Hack",
			Difficulty = 1
		});
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Admin_Update_NonExistent_Returns_NotFound()
	{
		var client = await GetAdminClientAsync();
		var response = await client.PutAsJsonAsync("/api/quests/9999", new
		{
			Title = "Ghost",
			Description = "Ghost",
			Difficulty = 1
		});
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Delete_Quest()
	{
		var client = await GetAdminClientAsync();
		var quests = await client.GetFromJsonAsync<List<QuestResponse>>("/api/quests");
		var target = quests!.First(q => q.Title == "Подземелье ужаса");

		var response = await client.DeleteAsync($"/api/quests/{target.Id}");
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		response = await client.GetAsync($"/api/quests/{target.Id}");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Regular_User_Cannot_Delete_Quest()
	{
		var player = await GetPlayerClientAsync();
		var response = await player.DeleteAsync("/api/quests/1");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Admin_Delete_NonExistent_Returns_NotFound()
	{
		var client = await GetAdminClientAsync();
		var response = await client.DeleteAsync("/api/quests/9999");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}