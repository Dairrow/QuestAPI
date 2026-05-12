using API.DTOs.Requests.UserQuests;
using API.DTOs.Responses.UserQuests;
using API.Extensions;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Exceptions;
using Services.Interfaces;

namespace API.Controllers;

[Authorize]
[Route("api/users/{userId:int}/quests")]
public class UserQuestsController : BaseApiController
{
	private readonly IUserQuestService _service;
	private readonly IMapper _mapper;

	public UserQuestsController(IUserQuestService service, IMapper mapper)
	{
		_service = service;
		_mapper = mapper;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll(int userId, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var entities = await _service.GetByUserIdAsync(userId, cancellationToken);
		return Ok(_mapper.Map<IEnumerable<UserQuestResponse>>(entities));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int userId, int id, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var entity = await _service.GetByIdAsync(id, cancellationToken);
		if (entity.UserId != userId)
			throw new Services.Exceptions.ForbiddenException("Access denied");
		return Ok(_mapper.Map<UserQuestResponse>(entity));
	}

	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create(int userId, CreateUserQuestRequest request, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var entity = _mapper.Map<UserQuest>(request);
		entity.UserId = userId;
		var result = await _service.CreateAsync(entity, cancellationToken);
		return Ok(_mapper.Map<UserQuestResponse>(result));
	}

	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> UpdateProgress(int userId, int id, UpdateUserQuestRequest request, CancellationToken cancellationToken)
	{
		EnsureUserAccess(userId);
		var userQuest = await _service.GetByIdAsync(id, cancellationToken);
		if (userQuest.UserId != userId)
			throw new ForbiddenException("Access denied");

		var result = await _service.UpdateProgressAsync(id, request.TaskOrder, cancellationToken);
		return Ok(_mapper.Map<UserQuestResponse>(result));
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