using API.DTOs.Requests.UserQuests;
using API.DTOs.Requests.UserRewards;
using API.DTOs.Responses.UserQuests;
using API.DTOs.Responses.UserRewards;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Exceptions;
using Services.Interfaces;

namespace API.Controllers;

[Authorize]
[Route("api/users/{userId:int}/rewards")]
public class UserRewardsController : BaseApiController
{
	private readonly IUserRewardService _service;
	private readonly IMapper _mapper;

	public UserRewardsController(IUserRewardService service, IMapper mapper)
	{
		_service = service;
		_mapper = mapper;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll(int userId, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var entities = await _service.GetByUserIdAsync(userId, cancellationToken);
		return Ok(_mapper.Map<IEnumerable<UserRewardResponse>>(entities));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int userId, int id, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var entity = await _service.GetByIdAsync(id, cancellationToken);
		if (entity.UserId != userId)
			throw new Services.Exceptions.ForbiddenException("Access denied");
		return Ok(_mapper.Map<UserRewardResponse>(entity));
	}

	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create(int userId, CreateUserRewardRequest request, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var entity = new UserReward
		{
			UserId = userId,
			RewardId = request.RewardId,
			ReceivedAt = DateTime.UtcNow
		};
		var result = await _service.CreateAsync(entity, cancellationToken);
		return Ok(_mapper.Map<UserRewardResponse>(result));
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Claim(int userId, int id, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var existing = await _service.GetByIdAsync(id, cancellationToken);
		if (existing.UserId != userId)
			throw new ForbiddenException("Access denied");

		var claimed = await _service.ClaimAsync(id, cancellationToken);
		return Ok(_mapper.Map<UserRewardResponse>(claimed));
	}

	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int userId, int id, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var entity = await _service.GetByIdAsync(id, cancellationToken);
		if (entity.UserId != userId)
			throw new Services.Exceptions.ForbiddenException("Access denied");
		await _service.DeleteAsync(id, cancellationToken);
		return NoContent();
	}
}