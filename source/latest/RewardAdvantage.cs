using System.Collections.Generic;

public class RewardAdvantage : RewardBase
{
	public List<EAdvantage> Advantages = new List<EAdvantage>();

	protected override void GetReward()
	{
		if (Singleton<GameSaveManager>.Instance.GetAdvantageState(Advantages[0]) != 0)
		{
			return;
		}
		foreach (EAdvantage advantage in Advantages)
		{
			Singleton<RewardManager>.Instance.ShowAdvantage(advantage);
		}
	}
}
