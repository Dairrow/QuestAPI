using API.DTOs.Responses.Users;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers;

public class UsersController
	: BaseApiController
{
	private readonly IUserService _userService;

	private readonly IMapper _mapper;


	public UsersController(
		IUserService userService,
		IMapper mapper)
	{
		_userService = userService;
		_mapper = mapper;
	}


	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<
		IEnumerable<UserResponse>>> GetAll(
		CancellationToken cancellationToken)
	{
		var users = await _userService
			.GetAllAsync(cancellationToken);


		var response = _mapper.Map<
			IEnumerable<UserResponse>>(users);


		return Ok(response);
	}


	[HttpGet("{id:int}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<
		UserResponse>> GetById(
		int id,
		CancellationToken cancellationToken)
	{
		var user = await _userService
			.GetByIdAsync(
				id,
				cancellationToken);


		var response = _mapper.Map<
			UserResponse>(user);


		return Ok(response);
	}
}