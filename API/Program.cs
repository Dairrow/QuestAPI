using Microsoft.EntityFrameworkCore;
using Repository.Context;
using API.Extensions;
using Services.Extensions;
using API.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(
		builder.Configuration.GetConnectionString(
			"DefaultConnection"))
	.UseSnakeCaseNamingConvention());


builder.Services.AddRepositories();
builder.Services.AddBusinessServices();
builder.Services.AddAutoMapper(typeof(UserProfile));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
