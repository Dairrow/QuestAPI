using System.ComponentModel.DataAnnotations;
using API.DTOs.Requests.Auth;

namespace UnitTests.DTOs
{
	public class AuthDtoTests
	{
		[Fact]
		public void LoginRequest_Valid_DTO_PassesValidation()
		{
			var dto = new LoginRequest { Email = "user@example.com", Password = "12345678" };
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void LoginRequest_Invalid_Email_ReturnsError()
		{
			var dto = new LoginRequest { Email = "invalid-email", Password = "12345678" };
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Email"));
		}

		[Fact]
		public void LoginRequest_Missing_Password_ReturnsError()
		{
			var dto = new LoginRequest { Email = "user@example.com", Password = "" };
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Password"));
		}

		[Fact]
		public void RegisterRequest_Valid_DTO_PassesValidation()
		{
			var dto = new RegisterRequest
			{
				Username = "newuser",
				Email = "new@example.com",
				Password = "StrongPass1"
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void RegisterRequest_TooShort_Username_ReturnsError()
		{
			var dto = new RegisterRequest
			{
				Username = "ab", 
				Email = "new@example.com",
				Password = "StrongPass1"
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Username"));
		}

		[Fact]
		public void RegisterRequest_TooShort_Password_ReturnsError()
		{
			var dto = new RegisterRequest
			{
				Username = "validUser",
				Email = "new@example.com",
				Password = "1234567" 
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Password"));
		}

		[Fact]
		public void RegisterRequest_Invalid_Email_ReturnsError()
		{
			var dto = new RegisterRequest
			{
				Username = "validUser",
				Email = "not-an-email",
				Password = "StrongPass1"
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Email"));
		}
	}
}