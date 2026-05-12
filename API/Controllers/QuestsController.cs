using API.DTOs.Requests.Quests;
using API.DTOs.Responses.Quests;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class QuestsController : BaseApiController
{
	private readonly IQuestService _service;
	private readonly IMapper _mapper;

	public QuestsController(IQuestService service, IMapper mapper)
	{
		_service = service;
		_mapper = mapper;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var entities = await _service.GetAllAsync(cancellationToken);
		return Ok(_mapper.Map<IEnumerable<QuestResponse>>(entities));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		var entity = await _service.GetByIdAsync(id, cancellationToken);
		return Ok(_mapper.Map<QuestResponse>(entity));
	}

	[HttpPost]
	public async Task<IActionResult> Create(CreateQuestRequest request, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<Quest>(request);
		var result = await _service.CreateAsync(entity, cancellationToken);
		return Ok(_mapper.Map<QuestResponse>(result));
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, UpdateQuestRequest request, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<Quest>(request);
		var result = await _service.UpdateAsync(id, entity, cancellationToken);
		return Ok(_mapper.Map<QuestResponse>(result));
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
	{
		await _service.DeleteAsync(id, cancellationToken);
		return NoContent();
	}
}