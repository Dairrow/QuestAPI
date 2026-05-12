using System.ComponentModel.DataAnnotations;
using API.DTOs.Requests.Users;
using Data.Enums;

namespace UnitTests.DTOs
{
	public class UserDtoTests
	{
		[Fact]
		public void CreateUserRequest_Valid_DTO_PassesValidation()
		{
			var dto = new CreateUserRequest
			{
				Username = "john_doe",
				Email = "john@example.com",
				Password = "Str0ng!Pass",
				Role = UserRole.User
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void CreateUserRequest_Username_TooShort_ReturnsError()
		{
			var dto = new CreateUserRequest
			{
				Username = "ab", 
				Email = "john@example.com",
				Password = "Str0ng!Pass",
				Role = UserRole.User
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Username"));
		}

		[Fact]
		public void CreateUserRequest_Password_TooShort_ReturnsError()
		{
			var dto = new CreateUserRequest
			{
				Username = "johndoe",
				Email = "john@example.com",
				Password = "1234567", 
				Role = UserRole.User
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Password"));
		}

		[Fact]
		public void CreateUserRequest_Invalid_Email_ReturnsError()
		{
			var dto = new CreateUserRequest
			{
				Username = "johndoe",
				Email = "invalid",
				Password = "Str0ng!Pass",
				Role = UserRole.User
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Email"));
		}

		[Fact]
		public void UpdateUserRequest_Valid_DTO_PassesValidation()
		{
			var dto = new UpdateUserRequest
			{
				Username = "jane_doe",
				Email = "jane@example.com"
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void UpdateUserRequest_Missing_Username_ReturnsError()
		{
			var dto = new UpdateUserRequest
			{
				Username = "",
				Email = "jane@example.com"
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("Username"));
		}

		[Fact]
		public void ChangePasswordRequest_Valid_DTO_PassesValidation()
		{
			var dto = new ChangePasswordRequest
			{
				OldPassword = "OldPass1",
				NewPassword = "NewPass2"
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.True(isValid);
			Assert.Empty(results);
		}

		[Fact]
		public void ChangePasswordRequest_NewPassword_TooShort_ReturnsError()
		{
			var dto = new ChangePasswordRequest
			{
				OldPassword = "OldPass1",
				NewPassword = "1234567" 
			};
			var context = new ValidationContext(dto);
			var results = new List<ValidationResult>();
			bool isValid = Validator.TryValidateObject(dto, context, results, true);
			Assert.False(isValid);
			Assert.Contains(results, r => r.MemberNames.Contains("NewPassword"));
		}
	}
}