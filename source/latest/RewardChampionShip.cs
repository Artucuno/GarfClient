public class RewardChampionShip : RewardUnlockableItem
{
	public EDifficulty Difficulty;

	public string ChampionShip;

	protected override void GetReward()
	{
		E_UnlockableItemSate championShipState = Singleton<GameSaveManager>.Instance.GetChampionShipState(ChampionShip, Difficulty);
		if (championShipState == E_UnlockableItemSate.Hidden && State == E_UnlockableItemSate.NewLocked)
		{
			Singleton<RewardManager>.Instance.ShowChampionShip(ChampionShip, Difficulty);
		}
		else if (State == E_UnlockableItemSate.NewUnlocked && (championShipState == E_UnlockableItemSate.NewLocked || championShipState == E_UnlockableItemSate.Locked || Singleton<RewardManager>.Instance.ContainsShownChampionShip(ChampionShip, Difficulty)))
		{
			Singleton<RewardManager>.Instance.UnlockChampionShip(ChampionShip, Difficulty);
		}
	}
}
