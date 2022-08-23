using System;

[Serializable]
public class RewardCharacter : RewardBase
{
	public ECharacter Character;

	protected override void GetReward()
	{
		if (Singleton<GameSaveManager>.Instance.GetCharacterState(Character) == E_UnlockableItemSate.Hidden)
		{
			Singleton<RewardManager>.Instance.UnlockCharacter(Character);
		}
	}
}
