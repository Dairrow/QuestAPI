using System.Net.Http.Headers;
using System.Net.Http.Json;
using API.DTOs.Requests.Auth;
using API.DTOs.Responses.Auth;
using API.DTOs.Responses.UserQuests;
using API.DTOs.Responses.UserRewards;

namespace IntegrationTests.Helpers;

public static class HttpClientExtensions
{
	public static async Task AuthenticateAsync(this HttpClient client, string email, string password)
	{
		var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
		{
			Email = email,
			Password = password
		});
		response.EnsureSuccessStatusCode();
		var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse!.AccessToken);
	}

	public static async Task<AuthResponse> LoginAndGetResponseAsync(this HttpClient client, string email, string password)
	{
		var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
		{
			Email = email,
			Password = password
		});
		response.EnsureSuccessStatusCode();
		return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
	}

	public static async Task<AuthResponse> RegisterAndGetResponseAsync(this HttpClient client, string username, string email, string password)
	{
		var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
		{
			Username = username,
			Email = email,
			Password = password
		});
		response.EnsureSuccessStatusCode();
		return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
	}

	public static async Task<HttpClient> CreateAuthenticatedClientAsync(
	this CustomWebApplicationFactory factory, string email, string password)
	{
		var client = factory.CreateClient();
		await client.AuthenticateAsync(email, password);
		return client;
	}

	public static async Task<int> CompleteQuestAndGetRewardIdAsync(HttpClient playerClient, int playerId, CustomWebApplicationFactory factory)
	{
		var userQuests = await playerClient.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var userQuest = userQuests!.First();

		for (int order = 1; order <= 3; order++)
		{
			var putResponse = await playerClient.PutAsJsonAsync($"/api/users/{playerId}/quests/{userQuest.Id}",
				new { TaskOrder = order });
			putResponse.EnsureSuccessStatusCode();
		}

		var rewards = await playerClient.GetFromJsonAsync<List<UserRewardResponse>>($"/api/users/{playerId}/rewards");
		return rewards!.First().Id;
	}

	public static async Task<int> CompleteQuestAndGetFirstRewardIdAsync(this HttpClient playerClient, int playerId)
	{
		var userQuests = await playerClient.GetFromJsonAsync<List<UserQuestResponse>>($"/api/users/{playerId}/quests");
		var userQuest = userQuests!.First();

		for (int order = 1; order <= 3; order++)
		{
			var putResponse = await playerClient.PutAsJsonAsync($"/api/users/{playerId}/quests/{userQuest.Id}",
				new { TaskOrder = order });
			putResponse.EnsureSuccessStatusCode();
		}

		var rewards = await playerClient.GetFromJsonAsync<List<UserRewardResponse>>($"/api/users/{playerId}/rewards");
		return rewards!.First().Id;
	}
}