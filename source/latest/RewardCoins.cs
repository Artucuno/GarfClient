public class RewardCoins : RewardBase
{
	public int Quantity;

	protected override void GetReward()
	{
		Singleton<RewardManager>.Instance.EarnCoins(Quantity);
	}
}
