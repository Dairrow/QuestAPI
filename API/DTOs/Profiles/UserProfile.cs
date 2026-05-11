using API.DTOs.Responses.Users;
using AutoMapper;
using Data.Entities;

namespace API.Profiles;

public class UserProfile : Profile
{
	public UserProfile()
	{
		CreateMap<User, UserResponse>();
	}
}