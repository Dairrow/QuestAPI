using API.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController
	: ControllerBase
{
	protected bool CanAccessUser(
		int userId)
	{
		if (User.IsAdmin())
			return true;

		return User.GetUserId()
			== userId;
	}


	protected void EnsureUserAccess(
		int userId)
	{
		if (!CanAccessUser(userId))
		{
			throw new Services.Exceptions.ForbiddenException(
				"Access denied");
		}
	}
}