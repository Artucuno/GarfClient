public class MenuRewardsHatShown : MenuRewards
{
	private string[] _hats;

	public override void OnEnter()
	{
		base.OnEnter();
		_hats = Singleton<RewardManager>.Instance.PopLockedHats();
		LbMessage.text = string.Format(Localization.instance.Get("MENU_REWARDS_HATS_GROUPE"), Singleton<GameSaveManager>.Instance.GetCollectedCoins());
	}

	public override void OnGoNext()
	{
		string[] hats = _hats;
		foreach (string text in hats)
		{
			if (text != null)
			{
				Singleton<GameSaveManager>.Instance.SetHatState(text, E_UnlockableItemSate.NewLocked, false);
			}
		}
		base.OnGoNext();
	}
}
