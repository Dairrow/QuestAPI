using API.DTOs.Requests.Auth;
using API.DTOs.Responses.Auth;
using AutoMapper;
using Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers;

public class AuthController
	: BaseApiController
{
	private readonly
		IAuthService _authService;

	private readonly
		IMapper _mapper;


	public AuthController(
		IAuthService authService,
		IMapper mapper)
	{
		_authService =
			authService;

		_mapper =
			mapper;
	}


	[HttpPost("register")]
	public async Task<ActionResult<
		AuthResponse>>
		Register(
		RegisterRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _authService
				.RegisterAsync(
					request.Username,

					request.Email,

					request.Password,

					UserRole.User,

					cancellationToken);


		return Ok(
			_mapper.Map<
				AuthResponse>(
				result));
	}


	[HttpPost("login")]
	public async Task<ActionResult<
		AuthResponse>>
		Login(
		LoginRequest request,
		CancellationToken cancellationToken)
	{
		var result =
			await _authService
				.LoginAsync(
					request.Email,

					request.Password,

					cancellationToken);


		return Ok(
			_mapper.Map<
				AuthResponse>(
				result));
	}

	[HttpPost("refresh")]
	public async Task<ActionResult<AuthResponse>> 
		Refresh(RefreshRequest request, 
		CancellationToken cancellationToken)
	{
	var result = await _authService
			.RefreshAsync(
			request.RefreshToken, 
			cancellationToken);

	return Ok(_mapper.Map<AuthResponse>(result));
	}

	[Authorize(Roles = "Admin")]
	[HttpPut("revoke")]
	public async Task<IActionResult>
		RevokeRefreshTokens(
		RevokeRefreshTokenRequest request,
		CancellationToken cancellationToken)
	{
	await _authService
	.RevokeUserTokensAsync(
	request.UserId,
	cancellationToken);

	return NoContent();
	}
}