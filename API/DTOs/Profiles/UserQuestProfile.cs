using API.DTOs.Requests.UserQuests;
using API.DTOs.Responses.UserQuests;
using AutoMapper;
using Data.Entities;

namespace API.Profiles;

public class UserQuestProfile : Profile
{
	public UserQuestProfile()
	{
		CreateMap<CreateUserQuestRequest, UserQuest>();

		CreateMap<UserQuest, UserQuestResponse>();

		CreateMap<UpdateUserQuestRequest, UserQuest>();
	}
}