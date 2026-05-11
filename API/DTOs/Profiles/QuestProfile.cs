using API.DTOs.Requests.Quests;
using API.DTOs.Responses.Quests;
using AutoMapper;
using Data.Entities;

namespace API.Profiles;

public class QuestProfile : Profile
{
	public QuestProfile()
	{
		CreateMap<CreateQuestRequest, Quest>();

		CreateMap<Quest, QuestResponse>();
	}
}