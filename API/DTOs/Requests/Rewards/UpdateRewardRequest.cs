using System.ComponentModel.DataAnnotations;
using Data.Enums;

namespace API.DTOs.Requests.Rewards;

public class UpdateRewardRequest
{
	[Required]
	[StringLength(100)]
	public string Name { get; set; } = string.Empty;


	[Required]
	public RewardType Type { get; set; }


	[Range(1, int.MaxValue)]
	public int Value { get; set; }
}