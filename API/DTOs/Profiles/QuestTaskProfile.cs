using API.DTOs.Requests.QuestTasks;
using API.DTOs.Responses.QuestTasks;
using AutoMapper;
using Data.Entities;

namespace API.Profiles;

public class QuestTaskProfile : Profile
{
	public QuestTaskProfile()
	{
		CreateMap<CreateQuestTaskRequest, QuestTask>();

		CreateMap<UpdateQuestTaskRequest, QuestTask>();

		CreateMap<QuestTask, QuestTaskResponse>();
	}
}