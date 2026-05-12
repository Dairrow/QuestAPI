using Microsoft.AspNetCore.Http;

namespace Services.Interfaces;

public interface IFileStorageService
{
	Task<string?> SaveAsync(IFormFile? file, string folder);

	void Delete(string? relativePath);
}