using API.DTOs.Requests.Rewards;
using API.DTOs.Responses.Rewards;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class RewardsController : BaseApiController
{
	private readonly IRewardService _service;
	private readonly IMapper _mapper;

	public RewardsController(IRewardService service, IMapper mapper)
	{
		_service = service;
		_mapper = mapper;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var entities = await _service.GetAllAsync(cancellationToken);
		return Ok(_mapper.Map<IEnumerable<RewardResponse>>(entities));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		var entity = await _service.GetByIdAsync(id, cancellationToken);
		return Ok(_mapper.Map<RewardResponse>(entity));
	}

	[HttpPost]
	public async Task<IActionResult> Create(CreateRewardRequest request, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<Reward>(request);
		var result = await _service.CreateAsync(entity, cancellationToken);
		return Ok(_mapper.Map<RewardResponse>(result));
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, UpdateRewardRequest request, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<Reward>(request);
		var result = await _service.UpdateAsync(id, entity, cancellationToken);
		return Ok(_mapper.Map<RewardResponse>(result));
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
	{
		await _service.DeleteAsync(id, cancellationToken);
		return NoContent();
	}
}