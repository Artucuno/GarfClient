public class RewardConditionChampionShip : RewardConditionBase
{
	public string ChampionShip;

	public EDifficulty Difficulty;

	public int Rank;

	public override bool CanGiveReward()
	{
		int num = Singleton<GameSaveManager>.Instance.GetRank(ChampionShip, Difficulty);
		if (ChampionShip.Equals(Singleton<GameConfigurator>.Instance.ChampionShipData.name) && Difficulty == Singleton<GameConfigurator>.Instance.Difficulty)
		{
			int playerRank = Singleton<RewardManager>.Instance.PlayerRank;
			if (num == -1 || playerRank < num)
			{
				num = playerRank;
			}
		}
		return num >= 0 && num <= Rank;
	}
}
