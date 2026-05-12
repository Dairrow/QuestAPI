using Microsoft.AspNetCore.Http;
using Services.Interfaces;

namespace Services.Implementations;

public class FileStorageService : IFileStorageService
{
	private readonly string _webRootPath;

	public FileStorageService(string? webRootPath = null)
	{
		_webRootPath = webRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
	}

	public async Task<string?> SaveAsync(IFormFile? file, string folder)
	{
		if (file == null || file.Length == 0) return null;

		var uploadsFolder = Path.Combine(_webRootPath, folder.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
		Directory.CreateDirectory(uploadsFolder);

		var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
		var filePath = Path.Combine(uploadsFolder, uniqueName);

		using (var stream = new FileStream(filePath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}

		return $"/{folder.TrimEnd('/')}/{uniqueName}";
	}

	public void Delete(string? relativePath)
	{
		if (string.IsNullOrWhiteSpace(relativePath)) return;
		var fullPath = Path.Combine(_webRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
		if (System.IO.File.Exists(fullPath))
			System.IO.File.Delete(fullPath);
	}
}