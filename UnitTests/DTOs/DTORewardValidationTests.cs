using System.ComponentModel.DataAnnotations;
using API.DTOs.Requests.Rewards;
using Data.Enums;

namespace UnitTests.DTOs
{
	public class RewardDtoTests
	{
		[Fact]
		public void CreateRewardRequest_Valid_DTO_PassesValidation()
		{
			var dto = new CreateRewardRequest
			{
				Name = "Gold Coin",
				Type = RewardType.Currency,
				Value = 100
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void CreateRewardRequest_Missing_Name_ReturnsError()
		{
			var dto = new CreateRewardRequest
			{
				Name = "",
				Type = RewardType.Item,
				Value = 1
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Name"));
		}

		[Fact]
		public void CreateRewardRequest_Value_Zero_ReturnsError()
		{
			var dto = new CreateRewardRequest
			{
				Name = "Silver",
				Type = RewardType.Currency,
				Value = 0 
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Value"));
		}

		[Fact]
		public void UpdateRewardRequest_Valid_DTO_PassesValidation()
		{
			var dto = new UpdateRewardRequest
			{
				Name = "Silver Coin",
				Type = RewardType.Experience,
				Value = 500
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}
	}
}