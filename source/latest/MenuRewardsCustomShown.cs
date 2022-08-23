public class MenuRewardsCustomShown : MenuRewards
{
	private string[] _customs;

	public override void OnEnter()
	{
		base.OnEnter();
		_customs = Singleton<RewardManager>.Instance.PopLockedCustoms();
		LbMessage.text = string.Format(Localization.instance.Get("MENU_REWARDS_CUSTOMISATION_GROUPE"), Singleton<GameSaveManager>.Instance.GetCollectedCoins());
	}

	public override void OnGoNext()
	{
		string[] customs = _customs;
		foreach (string text in customs)
		{
			if (text != null)
			{
				Singleton<GameSaveManager>.Instance.SetCustomState(text, E_UnlockableItemSate.NewLocked, false);
			}
		}
		base.OnGoNext();
	}
}
