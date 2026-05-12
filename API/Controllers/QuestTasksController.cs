using API.DTOs.Requests.QuestTasks;
using API.DTOs.Responses.QuestTasks;
using API.Extensions;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Exceptions;
using Services.Implementations;
using Services.Interfaces;

namespace API.Controllers;

[Authorize]
[Route("api/quests/{questId:int}/tasks")]
public class QuestTasksController
	: BaseApiController
{
	private readonly
		IQuestTaskService _service;

	private readonly 
		IUserQuestService _userQuestService;

	private readonly
		IMapper _mapper;


	public QuestTasksController(
		IQuestTaskService service,
		IUserQuestService userQuestService,
		IMapper mapper)
	{
		_service = service;
		_userQuestService = userQuestService;
		_mapper = mapper;
	}

	private async Task EnsureQuestAccess(int questId)
	{
		if (User.IsAdmin()) return;
		bool hasAccess = await _userQuestService.HasUserQuestAsync(User.GetUserId(), questId);
		if (!hasAccess)
			throw new ForbiddenException("Access denied to this quest");
	}

	[HttpGet]
	public async Task<IActionResult>
		GetAll(
		int questId,
		CancellationToken cancellationToken)
	{
		await EnsureQuestAccess(questId);

		var entities =
			await _service
				.GetByQuestIdAsync(
					questId,
					cancellationToken);

		return Ok(
			_mapper.Map<
				IEnumerable<QuestTaskResponse>>(
				entities));
	}


	[HttpGet("{id:int}")]
	public async Task<IActionResult>
		GetById(
		int questId,
		int id,
		CancellationToken cancellationToken)
	{
		await EnsureQuestAccess(questId);

		var entity =
			await _service
				.GetByIdAsync(
					id,
					cancellationToken);

		if (entity.QuestId != questId)
			throw new NotFoundException($"Task {id} not found in quest {questId}");

		return Ok(
			_mapper.Map<
				QuestTaskResponse>(
				entity));
	}


	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult>
		Create(
		int questId,
		CreateQuestTaskRequest request,
		CancellationToken cancellationToken)
	{
		var entity =
			_mapper.Map<QuestTask>(
				request);

		entity.QuestId =
			questId;


		var result =
			await _service
				.CreateAsync(
					entity,
					cancellationToken);

		return Ok(
			_mapper.Map<
				QuestTaskResponse>(
				result));
	}


	[HttpPut("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult>
		Update(
		int id,
		UpdateQuestTaskRequest request,
		CancellationToken cancellationToken)
	{
		var entity =
			_mapper.Map<QuestTask>(
				request);

		var result =
			await _service
				.UpdateAsync(
					id,
					entity,
					cancellationToken);

		return Ok(
			_mapper.Map<
				QuestTaskResponse>(
				result));
	}


	[HttpDelete("{id:int}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult>
		Delete(
		int id,
		CancellationToken cancellationToken)
	{
		await _service
			.DeleteAsync(
				id,
				cancellationToken);

		return NoContent();
	}
}