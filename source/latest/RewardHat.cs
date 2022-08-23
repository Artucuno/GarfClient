using System;

[Serializable]
public class RewardHat : RewardRandomBase
{
	protected override void GetReward()
	{
		if (State == E_UnlockableItemSate.NewLocked && Singleton<GameSaveManager>.Instance.GetHatState(Items[0]) == E_UnlockableItemSate.Hidden)
		{
			foreach (string item in Items)
			{
				Singleton<RewardManager>.Instance.ShowHat(item);
			}
			return;
		}
		if (State == E_UnlockableItemSate.NewUnlocked)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				Singleton<RewardManager>.Instance.UnlockHat(Items[i], Rarities[i]);
			}
		}
	}
}
