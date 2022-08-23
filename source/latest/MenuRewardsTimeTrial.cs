public class MenuRewardsTimeTrial : MenuRewards
{
	public UILabel LbNextMedal;

	public UITexturePattern MedalSprite;

	public override void OnEnter()
	{
		base.OnEnter();
		Tuple<int, string> tuple = Singleton<RewardManager>.Instance.PopTimeTrial();
		if ((bool)MedalSprite)
		{
			MedalSprite.ChangeTexture(tuple.Item1 - 1);
		}
		LbRewardName.text = Localization.instance.Get("TRACK_NAME_" + tuple.Item2);
		LbMessage.text = string.Format(Localization.instance.Get("MENU_REWARDS_TIME_TRIAL_MEDAL"), Localization.instance.Get("MENU_REWARDS_MEDAL" + tuple.Item1));
		if (tuple.Item1 == 4)
		{
			LbNextMedal.text = string.Format(Localization.instance.Get("MENU_REWARDS_TIME_TRIAL_NEXT_RECORD"));
		}
		else
		{
			LbNextMedal.text = string.Format(Localization.instance.Get("MENU_REWARDS_TIME_TRIAL_NEXT_MEDAL"), Localization.instance.Get("MENU_REWARDS_MEDAL" + (tuple.Item1 + 1)));
		}
	}
}
