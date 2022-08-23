public class MenuRewardsChampionShipUnlocked : MenuRewards
{
	private string _championShip;

	private EDifficulty _difficulty;

	public override void OnEnter()
	{
		base.OnEnter();
		Tuple<string, EDifficulty> tuple = Singleton<RewardManager>.Instance.PopUnlockedChampionShip();
		_championShip = tuple.Item1;
		_difficulty = tuple.Item2;
		LbRewardName.text = Localization.instance.Get("CHAMPIONSHIP_NAME_" + _championShip[_championShip.Length - 1]);
		LbMessage.text = string.Format(Localization.instance.Get("MENU_REWARDS_NOUVEAU_CHAMPIONNAT_DEBLOQUE"), Localization.instance.Get("MENU_DIFFICULTY_" + _difficulty.ToString().ToUpper()));
		Sprite.GetComponent<UITexturePattern>().ChangeTexture(int.Parse(_championShip[_championShip.Length - 1].ToString()) - 1);
	}

	public override void OnGoNext()
	{
		Singleton<GameSaveManager>.Instance.SetChampionShipState(_championShip, _difficulty, E_UnlockableItemSate.NewUnlocked, false);
		base.OnGoNext();
	}
}
