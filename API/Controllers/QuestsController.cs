using API.DTOs.Responses.Quests;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers;

public class QuestsController
	: BaseApiController
{
	private readonly IQuestService _questService;

	private readonly IMapper _mapper;


	public QuestsController(
		IQuestService questService,
		IMapper mapper)
	{
		_questService = questService;
		_mapper = mapper;
	}


	[HttpGet]
	public async Task<ActionResult<
		IEnumerable<QuestResponse>>> GetAll(
		CancellationToken cancellationToken)
	{
		var quests = await _questService
			.GetAllAsync(cancellationToken);


		var response = _mapper.Map<
			IEnumerable<QuestResponse>>(quests);


		return Ok(response);
	}

    [Authorize]
    [HttpGet("{id:int}")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<
		QuestResponse>> GetById(
		int id,
		CancellationToken cancellationToken)
	{
		var quest = await _questService
			.GetByIdAsync(
				id,
				cancellationToken);


		var response = _mapper.Map<
			QuestResponse>(quest);


		return Ok(response);
	}
}