using API.DTOs.Responses.Rewards;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers;

public class RewardsController
	: BaseApiController
{
	private readonly IRewardService _rewardService;

	private readonly IMapper _mapper;


	public RewardsController(
		IRewardService rewardService,
		IMapper mapper)
	{
		_rewardService = rewardService;
		_mapper = mapper;
	}


	[HttpGet]
	public async Task<ActionResult<
		IEnumerable<RewardResponse>>> GetAll(
		CancellationToken cancellationToken)
	{
		var rewards = await _rewardService
			.GetAllAsync(cancellationToken);


		var response = _mapper.Map<
			IEnumerable<RewardResponse>>(rewards);


		return Ok(response);
	}
}