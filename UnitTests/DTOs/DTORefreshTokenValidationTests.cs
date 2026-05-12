using System.ComponentModel.DataAnnotations;
using API.DTOs.Requests.Auth;

namespace UnitTests.DTOs
{
	public class RefreshDtoTests
	{
		[Fact]
		public void RefreshRequest_Valid_DTO_PassesValidation()
		{
			var dto = new RefreshRequest { RefreshToken = "someTokenValue" };
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void RefreshRequest_Empty_Token_ReturnsError()
		{
			var dto = new RefreshRequest { RefreshToken = "" };
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("RefreshToken"));
		}
	}
}