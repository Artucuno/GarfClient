using System.Collections.Generic;

public abstract class RewardRandomBase : RewardUnlockableItem
{
	public List<string> Items = new List<string>();

	public List<ERarity> Rarities = new List<ERarity>();

	public RewardRandomBase()
	{
	}
}
