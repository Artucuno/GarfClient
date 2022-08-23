public class RewardConditionChallenge : RewardConditionBase
{
	public EDifficulty Difficulty;

	public override bool CanGiveReward()
	{
		return false;
	}
}
