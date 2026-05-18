using API.DTOs.Requests.UserQuests;
using API.DTOs.Requests.UserRewards;
using API.DTOs.Responses.UserRewards;
using AutoMapper;
using Data.Entities;

namespace API.Profiles;

public class InventoryProfile : Profile
{
	public InventoryProfile()
	{
		CreateMap<CreateRewardInventoryRequest, Inventory>();

		CreateMap<Inventory, InventoryResponse>();
	}
}