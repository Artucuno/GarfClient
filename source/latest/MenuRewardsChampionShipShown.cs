public class MenuRewardsChampionShipShown : MenuRewards
{
	private string _championShip;

	private EDifficulty _difficulty;

	public override void OnEnter()
	{
		base.OnEnter();
		Tuple<string, EDifficulty> tuple = Singleton<RewardManager>.Instance.PopLockedChampionShip();
		_championShip = tuple.Item1;
		_difficulty = tuple.Item2;
		LbRewardName.text = Localization.instance.Get("CHAMPIONSHIP_NAME_" + _championShip[_championShip.Length - 1]);
		LbMessage.text = string.Format(Localization.instance.Get("MENU_REWARDS_NOUVEAU_CHAMPIONNAT_APPARUT"), Localization.instance.Get("MENU_DIFFICULTY_" + _difficulty.ToString().ToUpper()));
	}

	public override void OnGoNext()
	{
		Singleton<GameSaveManager>.Instance.SetChampionShipState(_championShip, _difficulty, E_UnlockableItemSate.NewLocked, false);
		base.OnGoNext();
	}
}
