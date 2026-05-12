using System.ComponentModel.DataAnnotations;
using API.DTOs.Requests.QuestTasks;

namespace UnitTests.DTOs
{
	public class QuestTaskDtoTests
	{
		[Fact]
		public void CreateQuestTaskRequest_Valid_DTO_PassesValidation()
		{
			var dto = new CreateQuestTaskRequest
			{
				Title = "Task 1",
				Description = "Some description",
				Order = 1
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void CreateQuestTaskRequest_Missing_Title_ReturnsError()
		{
			var dto = new CreateQuestTaskRequest
			{
				Title = "",
				Description = "Description",
				Order = 2
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Title"));
		}

		[Fact]
		public void CreateQuestTaskRequest_Order_LessThanOne_ReturnsError()
		{
			var dto = new CreateQuestTaskRequest
			{
				Title = "Task",
				Description = "Desc",
				Order = 0 
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Order"));
		}

		[Fact]
		public void UpdateQuestTaskRequest_Valid_DTO_PassesValidation()
		{
			var dto = new UpdateQuestTaskRequest
			{
				Title = "Updated Task",
				Description = "New desc",
				Order = 5
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}
	}
}