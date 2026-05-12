using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Data.Entities;
using Data.Enums;
using Microsoft.Extensions.Configuration;
using Services.Implementations;
using Xunit;

namespace UnitTests.Services
{
	public class TokenServiceTests
	{
		private readonly TokenService _tokenService;

		public TokenServiceTests()
		{
			var inMemorySettings = new Dictionary<string, string?>
			{
				{"Jwt:SecretKey", "ThisIsAVeryLongSecretKeyForTesting123!"},
				{"Jwt:Issuer", "TestIssuer"},
				{"Jwt:Audience", "TestAudience"},
				{"Jwt:ExpirationMinutes", "15"}
			};
			IConfiguration config = new ConfigurationBuilder()
				.AddInMemoryCollection(inMemorySettings)
				.Build();

			_tokenService = new TokenService(config);
		}

		[Fact]
		public void GenerateAccessToken_ContainsExpectedClaims()
		{
			var user = new User { Id = 123, Username = "testuser", Role = UserRole.Admin };

			var token = _tokenService.GenerateAccessToken(user);
			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.ReadJwtToken(token);

			Assert.Equal("TestIssuer", jwt.Issuer);
			Assert.Equal("123", jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
			Assert.Equal("testuser", jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value);
			Assert.Equal("Admin", jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value);
		}

		[Fact]
		public void GenerateRefreshToken_ReturnsNonEmptyString_OfLength()
		{
			var token = _tokenService.GenerateRefreshToken();
			Assert.NotEmpty(token);
			Assert.True(token.Length >= 64);
		}
	}
}