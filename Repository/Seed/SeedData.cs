using Data.Entities;
using Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.Context;
using System;

namespace Repository.Seed;

public static class SeedData
{
	public static async Task InitializeAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await context.Database.EnsureCreatedAsync();

		if (!await context.Users.AnyAsync())
		{
			var admin = new User
			{
				Username = "admin",
				Email = "admin@example.com",
				PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1234"),
				Role = UserRole.Admin
			};

			var moderator = new User
			{
				Username = "moderator",
				Email = "moderator@example.com",
				PasswordHash = BCrypt.Net.BCrypt.HashPassword("Moder1234"),
				Role = UserRole.Moderator
			};

			var user = new User
			{
				Username = "player",
				Email = "player@example.com",
				PasswordHash = BCrypt.Net.BCrypt.HashPassword("Player1234"),
				Role = UserRole.User
			};

			context.Users.AddRange(admin, moderator, user);
			await context.SaveChangesAsync();
		}

		if (!await context.Rewards.AnyAsync())
		{
			var reward1 = new Reward
			{
				Name = "Золотая монета",
				Type = RewardType.Currency,
				Value = 100
			};
			var reward2 = new Reward
			{
				Name = "Опыт 500",
				Type = RewardType.Experience,
				Value = 500
			};
			var reward3 = new Reward
			{
				Name = "Меч новичка",
				Type = RewardType.Item,
				Value = 1
			};

			context.Rewards.AddRange(reward1, reward2, reward3);
			await context.SaveChangesAsync();
		}

		if (!await context.Quests.AnyAsync())
		{
			var reward1 = await context.Rewards.FirstAsync(r => r.Name == "Золотая монета");
			var reward2 = await context.Rewards.FirstAsync(r => r.Name == "Опыт 500");
			var reward3 = await context.Rewards.FirstAsync(r => r.Name == "Меч новичка");

			var quest1 = new Quest
			{
				Title = "Первое приключение",
				Description = "Пройдите обучение и завершите три простых задания.",
				Difficulty = QuestDifficulty.Easy,
				RewardId = reward1.Id,
				Tasks = new List<QuestTask>
				{
					new QuestTask { Title = "Поговорить с наставником", Description = "Найти старика в деревне", Order = 1 },
					new QuestTask { Title = "Собрать 5 яблок", Description = "Яблоки растут в саду", Order = 2 },
					new QuestTask { Title = "Победить слайма", Description = "Слайм прячется за мельницей", Order = 3 }
				}
			};

			var quest2 = new Quest
			{
				Title = "Опасный лес",
				Description = "Исследуйте лес и найдите древний артефакт.",
				Difficulty = QuestDifficulty.Medium,
				RewardId = reward2.Id,
				Tasks = new List<QuestTask>
				{
					new QuestTask { Title = "Найти вход в пещеру", Description = "Вход скрыт за водопадом", Order = 1 },
					new QuestTask { Title = "Победить паука", Description = "Гигантский паук охраняет проход", Order = 2 },
					new QuestTask { Title = "Взять артефакт", Description = "Древний амулет на алтаре", Order = 3 }
				}
			};

			var quest3 = new Quest
			{
				Title = "Подземелье ужаса",
				Description = "Погрузитесь в подземелье и сразитесь с боссом.",
				Difficulty = QuestDifficulty.Hard,
				RewardId = reward3.Id,
				Tasks = new List<QuestTask>
				{
					new QuestTask { Title = "Пройти ловушки", Description = "Уклонитесь от стрел и огня", Order = 1 },
					new QuestTask { Title = "Победить минотавра", Description = "Минотавр ждёт в главном зале", Order = 2 },
					new QuestTask { Title = "Забрать сокровище", Description = "Сундук в дальнем углу", Order = 3 }
				}
			};

			context.Quests.AddRange(quest1, quest2, quest3);
			await context.SaveChangesAsync();
		}

		if (!await context.UserQuests.AnyAsync())
		{
			var player = await context.Users.FirstAsync(u => u.Email == "player@example.com");
			var quest1 = await context.Quests.FirstAsync(q => q.Title == "Первое приключение");

			var userQuest = new UserQuest
			{
				UserId = player.Id,
				QuestId = quest1.Id,
				IsCompleted = false,
				LastCompletedTaskOrder = null
			};

			context.UserQuests.Add(userQuest);
			await context.SaveChangesAsync();
		}
	}
}