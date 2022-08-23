public class RewardCustom : RewardRandomBase
{
	protected override void GetReward()
	{
		if (State == E_UnlockableItemSate.NewLocked && Singleton<GameSaveManager>.Instance.GetCustomState(Items[0]) == E_UnlockableItemSate.Hidden)
		{
			foreach (string item in Items)
			{
				Singleton<RewardManager>.Instance.ShowCustom(item);
			}
			return;
		}
		if (State == E_UnlockableItemSate.NewUnlocked)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				Singleton<RewardManager>.Instance.UnlockCustom(Items[i], Rarities[i]);
			}
		}
	}
}
