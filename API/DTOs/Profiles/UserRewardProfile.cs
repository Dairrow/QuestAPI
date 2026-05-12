using API.DTOs.Requests.UserRewards;
using API.DTOs.Responses.UserRewards;
using AutoMapper;
using Data.Entities;

namespace API.Profiles;

public class UserRewardProfile : Profile
{
	public UserRewardProfile()
	{
		CreateMap<CreateUserRewardRequest, UserReward>();

		CreateMap<UserReward, UserRewardResponse>();
	}
}