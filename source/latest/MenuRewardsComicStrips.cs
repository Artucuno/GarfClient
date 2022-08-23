public class MenuRewardsComicStrips : MenuRewards
{
	private string _comicStrip;

	public override void OnEnter()
	{
		base.OnEnter();
		_comicStrip = Singleton<RewardManager>.Instance.PopComicStrip();
		LbRewardName.text = Localization.instance.Get("TRACK_NAME_" + _comicStrip);
	}

	public override void OnGoNext()
	{
		Singleton<GameSaveManager>.Instance.SetComicStripState(_comicStrip, E_UnlockableItemSate.NewUnlocked, false);
		base.OnGoNext();
	}
}
