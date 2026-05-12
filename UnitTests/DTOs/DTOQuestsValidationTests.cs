using System.ComponentModel.DataAnnotations;
using API.DTOs.Requests.Quests;
using Data.Enums;

namespace UnitTests.DTOs
{
	public class QuestDtoTests
	{
		[Fact]
		public void CreateQuestRequest_Valid_DTO_PassesValidation()
		{
			var dto = new CreateQuestRequest
			{
				Title = "Valid Quest Title",
				Description = "This is a valid description.",
				Difficulty = QuestDifficulty.Medium
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void CreateQuestRequest_Missing_Title_ReturnsError()
		{
			var dto = new CreateQuestRequest
			{
				Title = "",
				Description = "Description",
				Difficulty = QuestDifficulty.Easy
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Title"));
		}

		[Fact]
		public void CreateQuestRequest_Title_TooShort_ReturnsError()
		{
			var dto = new CreateQuestRequest
			{
				Title = "ab", 
				Description = "Description",
				Difficulty = QuestDifficulty.Easy
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Title"));
		}

		[Fact]
		public void CreateQuestRequest_Description_TooLong_ReturnsError()
		{
			var dto = new CreateQuestRequest
			{
				Title = "Valid",
				Description = new string('A', 501), 
				Difficulty = QuestDifficulty.Easy
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Description"));
		}

		[Fact]
		public void UpdateQuestRequest_Valid_DTO_PassesValidation()
		{
			var dto = new UpdateQuestRequest
			{
				Title = "Updated Quest",
				Description = "Updated description.",
				Difficulty = QuestDifficulty.Hard
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void UpdateQuestRequest_Missing_Description_ReturnsError()
		{
			var dto = new UpdateQuestRequest
			{
				Title = "Valid",
				Description = "",
				Difficulty = QuestDifficulty.Easy
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Description"));
		}
	}
}