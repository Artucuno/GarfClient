public class RewardConditionTimeTrial : RewardConditionBase
{
	public string Track;

	public E_TimeTrialMedal Medal;

	public override bool CanGiveReward()
	{
		E_TimeTrialMedal medal = Singleton<GameSaveManager>.Instance.GetMedal(Track, false);
		E_TimeTrialMedal medal2 = Singleton<RewardManager>.Instance.Medal;
		return medal < Medal && medal2 >= Medal;
	}
}
