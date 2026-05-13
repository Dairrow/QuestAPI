using System.Net;
using System.Net.Http.Json;
using API.DTOs.Responses.Rewards;
using IntegrationTests.Helpers;
using Xunit;

namespace IntegrationTests;

public class RewardsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;

	public RewardsControllerIntegrationTests(CustomWebApplicationFactory factory)
	{
		_factory = factory;
		_factory.ResetDatabaseAsync().GetAwaiter().GetResult();
	}

	private async Task<HttpClient> GetAdminClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("admin@example.com", "Admin1234");

	private async Task<HttpClient> GetPlayerClientAsync() =>
		await _factory.CreateAuthenticatedClientAsync("player@example.com", "Player1234");


	[Fact]
	public async Task Admin_GetAll_Returns_All_Rewards()
	{
		var client = await GetAdminClientAsync();
		var response = await client.GetAsync("/api/rewards");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var rewards = await response.Content.ReadFromJsonAsync<List<RewardResponse>>();
		Assert.NotNull(rewards);
		Assert.Equal(3, rewards.Count);
	}

	[Fact]
	public async Task Regular_User_GetAll_Returns_Forbidden()
	{
		var client = await GetPlayerClientAsync();
		var response = await client.GetAsync("/api/rewards");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_GetById_Returns_Reward()
	{
		var client = await GetAdminClientAsync();
		var rewards = await client.GetFromJsonAsync<List<RewardResponse>>("/api/rewards");
		var rewardId = rewards!.First().Id;

		var response = await client.GetAsync($"/api/rewards/{rewardId}");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var reward = await response.Content.ReadFromJsonAsync<RewardResponse>();
		Assert.Equal(rewardId, reward!.Id);
	}

	[Fact]
	public async Task Regular_User_GetById_Returns_Forbidden()
	{
		var client = await GetPlayerClientAsync();
		var response = await client.GetAsync("/api/rewards/1");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Admin_GetById_NonExistent_Returns_NotFound()
	{
		var client = await GetAdminClientAsync();
		var response = await client.GetAsync("/api/rewards/9999");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Create_Reward_With_Image()
	{
		var client = await GetAdminClientAsync();

		using var form = new MultipartFormDataContent();
		form.Add(new StringContent("Test Reward"), "Name");
		form.Add(new StringContent("1"), "Type");
		form.Add(new StringContent("100"), "Value");

		var imageBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
		var imageContent = new ByteArrayContent(imageBytes);
		imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
		form.Add(imageContent, "image", "test.png");

		var response = await client.PostAsync("/api/rewards", form);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var reward = await response.Content.ReadFromJsonAsync<RewardResponse>();
		Assert.Equal("Test Reward", reward!.Name);
		Assert.NotNull(reward.ImagePath);
		Assert.StartsWith("/uploads/rewards/", reward.ImagePath);
	}

	[Fact]
	public async Task Admin_Create_Reward_Without_Image_Sets_NullPath()
	{
		var client = await GetAdminClientAsync();

		using var form = new MultipartFormDataContent();
		form.Add(new StringContent("No Image Reward"), "Name");
		form.Add(new StringContent("2"), "Type");
		form.Add(new StringContent("500"), "Value");

		var response = await client.PostAsync("/api/rewards", form);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var reward = await response.Content.ReadFromJsonAsync<RewardResponse>();
		Assert.Null(reward!.ImagePath);
	}

	[Fact]
	public async Task Regular_User_Cannot_Create_Reward()
	{
		var client = await GetPlayerClientAsync();
		var response = await client.PostAsync("/api/rewards", new MultipartFormDataContent());
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Update_Reward_And_Change_Image()
	{
		var client = await GetAdminClientAsync();
		var rewards = await client.GetFromJsonAsync<List<RewardResponse>>("/api/rewards");
		var rewardId = rewards!.First().Id;

		using var form = new MultipartFormDataContent();
		form.Add(new StringContent("Updated Reward"), "Name");
		form.Add(new StringContent("3"), "Type");
		form.Add(new StringContent("10"), "Value");

		var newImageBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
		var imageContent = new ByteArrayContent(newImageBytes);
		imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
		form.Add(imageContent, "image", "updated.png");

		var response = await client.PutAsync($"/api/rewards/{rewardId}", form);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var updated = await response.Content.ReadFromJsonAsync<RewardResponse>();
		Assert.Equal("Updated Reward", updated!.Name);
		Assert.NotEqual(rewards.First().ImagePath, updated.ImagePath);
	}

	[Fact]
	public async Task Regular_User_Cannot_Update_Reward()
	{
		var client = await GetPlayerClientAsync();
		var response = await client.PutAsync("/api/rewards/1", new MultipartFormDataContent());
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}


	[Fact]
	public async Task Admin_Can_Delete_Reward_And_Removes_ImageFile()
	{
		var client = await GetAdminClientAsync();
		using var form = new MultipartFormDataContent();
		form.Add(new StringContent("ToDelete"), "Name");
		form.Add(new StringContent("1"), "Type");
		form.Add(new StringContent("1"), "Value");
		var img = new ByteArrayContent(new byte[] { 0x89, 0x50, 0x4E, 0x47 });
		img.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
		form.Add(img, "image", "del.png");
		var createResponse = await client.PostAsync("/api/rewards", form);
		var reward = await createResponse.Content.ReadFromJsonAsync<RewardResponse>();

		var deleteResponse = await client.DeleteAsync($"/api/rewards/{reward!.Id}");
		Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

		var getResponse = await client.GetAsync($"/api/rewards/{reward.Id}");
		Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

		var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", reward.ImagePath?.TrimStart('/') ?? "");
		if (!string.IsNullOrEmpty(reward.ImagePath))
			Assert.False(File.Exists(filePath));
	}

	[Fact]
	public async Task Player_Cannot_Delete_Reward()
	{
		var client = await GetPlayerClientAsync();
		var response = await client.DeleteAsync("/api/rewards/1");
		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}
}