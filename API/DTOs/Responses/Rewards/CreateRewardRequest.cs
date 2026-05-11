using Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Rewards;

public class CreateRewardRequest
{
	[Required]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;


	[Required]
	public RewardType Type { get; set; }


	[Range(1, int.MaxValue)]
	public int Value { get; set; }
}