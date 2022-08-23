public class RewardConditionCoins : RewardConditionBase
{
	public int CollectedCoins;

	public override bool CanGiveReward()
	{
		int collectedCoins = Singleton<GameSaveManager>.Instance.GetCollectedCoins();
		int coins = Singleton<RewardManager>.Instance.Coins;
		return collectedCoins < CollectedCoins && collectedCoins + coins >= CollectedCoins;
	}
}
