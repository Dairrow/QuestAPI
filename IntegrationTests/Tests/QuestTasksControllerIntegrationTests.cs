using System.Net;
using System.Net.Http.Json;
using API.DTOs.Responses.Quests;
using API.DTOs.Responses.QuestTasks;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests;

public class QuestTasksControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;

	public QuestTasksControllerIntegrationTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_factory.ResetDatabaseAsync().GetAwaiter().GetResult();
	}

	private async Task<HttpClient> GetAdminClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");

	private async Task<HttpClient> GetPlayerClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");

	private async Task<int> GetPlayerQuestIdAsync(HttpClient adminClient)
	{
		var quests = await adminClient.GetFromJsonAsync<List<QuestResponse>>("/api/quests");
		return quests!.First(q => q.Title == "Первое приключение").Id;
	}


	[Fact]
	public async Task Admin_Can_Get_All_Tasks_For_Any_Quest()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var response = await admin.GetAsync($"/api/quests/{questId}/tasks");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var tasks = await response.Content.ReadFromJsonAsync<List<QuestTaskResponse>>();
		Assert.NotEmpty(tasks!);
		Assert.Equal(3, tasks.Count);
	}

	[Fact]
	public async Task Player_Can_Get_Tasks_Of_Own_Quest()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var player = await GetPlayerClientAsync();

		var response = await player.GetAsync($"/api/quests/{questId}/tasks");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var tasks = await response.Content.ReadFromJsonAsync<List<QuestTaskResponse>>();
		Assert.NotEmpty(tasks!);
	}

	[Fact]
	public async Task Player_Cannot_Get_Tasks_Of_Unassigned_Quest()
	{
		var admin = await GetAdminClientAsync();
		var quests = await admin.GetFromJsonAsync<List<QuestResponse>>("/api/quests");
		var otherQuestId = quests!.First(q => q.Title == "Опасный лес").Id;

		var player = await GetPlayerClientAsync();
		var response = await player.GetAsync($"/api/quests/{otherQuestId}/tasks");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Get_Specific_Task()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);

		var tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{questId}/tasks");
		var taskId = tasks!.First().Id;

		var response = await admin.GetAsync($"/api/quests/{questId}/tasks/{taskId}");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var task = await response.Content.ReadFromJsonAsync<QuestTaskResponse>();
		Assert.Equal(taskId, task!.Id);
	}

	[Fact]
	public async Task Admin_Get_Task_With_Wrong_QuestId_Returns_NotFound()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{questId}/tasks");
		var taskId = tasks!.First().Id;

		var otherQuestId = (await admin.GetFromJsonAsync<List<QuestResponse>>("/api/quests"))!
			.First(q => q.Title == "Опасный лес").Id;

		var response = await admin.GetAsync($"/api/quests/{otherQuestId}/tasks/{taskId}");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Player_Can_Get_Own_Task()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{questId}/tasks");
		var taskId = tasks!.First().Id;

		var player = await GetPlayerClientAsync();
		var response = await player.GetAsync($"/api/quests/{questId}/tasks/{taskId}");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task Player_Cannot_Get_Task_Of_Other_Quest()
	{
		var admin = await GetAdminClientAsync();
		var playerQuestId = await GetPlayerQuestIdAsync(admin);
		var tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{playerQuestId}/tasks");
		var taskId = tasks!.First().Id;

		var player = await GetPlayerClientAsync();
		var otherQuestId = (await admin.GetFromJsonAsync<List<QuestResponse>>("/api/quests"))!
			.First(q => q.Title == "Опасный лес").Id;

		var response = await player.GetAsync($"/api/quests/{otherQuestId}/tasks/{taskId}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Create_Task()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);

		var newTask = new
		{
			Title = "Новая задача",
			Description = "Описание",
			Order = 10
		};

		var response = await admin.PostAsJsonAsync($"/api/quests/{questId}/tasks", newTask);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var task = await response.Content.ReadFromJsonAsync<QuestTaskResponse>();
		Assert.Equal("Новая задача", task!.Title);
		Assert.Equal(10, task.Order);
	}

	[Fact]
	public async Task Admin_Create_Task_With_Invalid_Data_Returns_BadRequest()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);

		var response = await admin.PostAsJsonAsync($"/api/quests/{questId}/tasks", new
		{
			Description = "Без названия",
			Order = 1
		});
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Player_Cannot_Create_Task()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var player = await GetPlayerClientAsync();

		var response = await player.PostAsJsonAsync($"/api/quests/{questId}/tasks", new
		{
			Title = "Хак",
			Description = "Хак",
			Order = 99
		});
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Update_Task()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{questId}/tasks");
		var taskId = tasks!.First().Id;

		var update = new
		{
			Title = "Обновлённая задача",
			Description = "Новое описание",
			Order = 15
		};

		var response = await admin.PutAsJsonAsync($"/api/quests/{questId}/tasks/{taskId}", update);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var updated = await response.Content.ReadFromJsonAsync<QuestTaskResponse>();
		Assert.Equal("Обновлённая задача", updated!.Title);
		Assert.Equal(15, updated.Order);
	}

	[Fact]
	public async Task Player_Cannot_Update_Task()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{questId}/tasks");
		var taskId = tasks!.First().Id;

		var player = await GetPlayerClientAsync();
		var response = await player.PutAsJsonAsync($"/api/quests/{questId}/tasks/{taskId}", new
		{
			Title = "Хак",
			Description = "Хак",
			Order = 1
		});
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Delete_Task()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{questId}/tasks");
		var countBefore = tasks!.Count;
		var taskId = tasks.First().Id;

		var response = await admin.DeleteAsync($"/api/quests/{questId}/tasks/{taskId}");
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{questId}/tasks");
		Assert.Equal(countBefore - 1, tasks!.Count);
	}

	[Fact]
	public async Task Player_Cannot_Delete_Task()
	{
		var admin = await GetAdminClientAsync();
		var questId = await GetPlayerQuestIdAsync(admin);
		var tasks = await admin.GetFromJsonAsync<List<QuestTaskResponse>>($"/api/quests/{questId}/tasks");
		var taskId = tasks!.First().Id;

		var player = await GetPlayerClientAsync();
		var response = await player.DeleteAsync($"/api/quests/{questId}/tasks/{taskId}");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}
}