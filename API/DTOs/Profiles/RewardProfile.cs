using API.DTOs.Requests.Rewards;
using API.DTOs.Responses.Rewards;
using AutoMapper;
using Data.Entities;

namespace API.Profiles;

public class RewardProfile : Profile
{
	public RewardProfile()
	{
		CreateMap<CreateRewardRequest, Reward>();

		CreateMap<Reward, RewardResponse>();
	}
}