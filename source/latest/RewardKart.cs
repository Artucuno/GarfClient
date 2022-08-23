using System;

[Serializable]
public class RewardKart : RewardUnlockableItem
{
	public ECharacter Kart;

	protected override void GetReward()
	{
		E_UnlockableItemSate kartState = Singleton<GameSaveManager>.Instance.GetKartState(Kart);
		if (kartState == E_UnlockableItemSate.Locked)
		{
			Singleton<RewardManager>.Instance.UnlockKart(Kart);
		}
	}
}
