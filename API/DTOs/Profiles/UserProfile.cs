using API.DTOs.Responses.Users;
using API.DTOs.Requests.Users;
using AutoMapper;
using Data.Entities;

namespace API.Profiles;

public class UserProfile : Profile
{
	public UserProfile()
	{
		CreateMap<User, UserResponse>();

		CreateMap<CreateUserRequest, User>()
		.ForMember(
			x => x.PasswordHash,
			opt => opt.Ignore());

		CreateMap<UpdateUserRequest, User>();

	}
}