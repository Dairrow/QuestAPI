using API.DTOs.Requests.Users;
using API.DTOs.Responses.Users;
using API.Extensions;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
	private readonly IUserService _service;
	private readonly IMapper _mapper;

	public UsersController(IUserService service, IMapper mapper)
	{
		_service = service;
		_mapper = mapper;
	}

	[HttpGet]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var entities = await _service.GetAllAsync(cancellationToken);
		return Ok(_mapper.Map<IEnumerable<UserResponse>>(entities));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		EnsureUserAccess(id);
		var entity = await _service.GetByIdAsync(id, cancellationToken);
		return Ok(_mapper.Map<UserResponse>(entity));
	}

	[HttpPost]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<User>(request);
		entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
		var result = await _service.CreateAsync(entity, cancellationToken);
		return Ok(_mapper.Map<UserResponse>(result));
	}
		
	[HttpPut("{id:int}")]
	public async Task<IActionResult> Update(int id, UpdateUserRequest request, CancellationToken cancellationToken)
	{
		EnsureUserAccess(id);
		var entity = _mapper.Map<User>(request);
		var result = await _service.UpdateAsync(id, entity, cancellationToken);
		return Ok(_mapper.Map<UserResponse>(result));
	}

	[HttpPut("{id:int}/change-password")]
	public async Task<IActionResult> ChangePassword(int id, ChangePasswordRequest request, CancellationToken cancellationToken)
	{
		EnsureUserAccess(id);
		await _service.ChangePasswordAsync(id, request.OldPassword, request.NewPassword, cancellationToken);
		return NoContent();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
	{
		EnsureUserAccess(id);
		await _service.DeleteAsync(id, cancellationToken);
		return NoContent();
	}
}