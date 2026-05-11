using API.DTOs.Responses.Auth;
using AutoMapper;
using Services.Models;

namespace API.Profiles;

public class AuthProfile : Profile
{
	public AuthProfile()
	{
		CreateMap<AuthResult, AuthResponse>();
	}
}