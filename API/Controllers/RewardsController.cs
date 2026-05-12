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
	private readonly IFileStorageService _fileStorage;
	private readonly IMapper _mapper;

	public RewardsController(IRewardService service, IFileStorageService fileStorage, IMapper mapper)
	{
		_service = service;
		_fileStorage = fileStorage;
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
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> Create([FromForm] CreateRewardRequest request, IFormFile? image, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<Reward>(request);
		entity.ImagePath = await _fileStorage.SaveAsync(image, "uploads/rewards");

		var result = await _service.CreateAsync(entity, cancellationToken);
		return Ok(_mapper.Map<RewardResponse>(result));
	}

	[HttpPut("{id:int}")]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> Update(int id, [FromForm] UpdateRewardRequest request, IFormFile? image, CancellationToken cancellationToken)
	{
		var existing = await _service.GetByIdAsync(id, cancellationToken);

		var entity = _mapper.Map<Reward>(request);
		if (image != null)
		{
			_fileStorage.Delete(existing.ImagePath);
			entity.ImagePath = await _fileStorage.SaveAsync(image, "uploads/rewards");
		}
		else
		{
			entity.ImagePath = existing.ImagePath;
		}

		var result = await _service.UpdateAsync(id, entity, cancellationToken);
		return Ok(_mapper.Map<RewardResponse>(result));
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
	{
		var entity = await _service.GetByIdAsync(id, cancellationToken);
		_fileStorage.Delete(entity.ImagePath);
		await _service.DeleteAsync(id, cancellationToken);
		return NoContent();
	}
}