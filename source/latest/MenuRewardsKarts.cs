public class MenuRewardsKarts : MenuRewards
{
	private ECharacter _kart;

	public override void OnEnter()
	{
		base.OnEnter();
		_kart = Singleton<RewardManager>.Instance.PopKart();
		Tuple<string, UIAtlas, string, ERarity> infos = GetInfos(_kart.ToString(), E_RewardType.Kart);
		LbRewardName.text = infos.Item1;
		if (Sprite != null)
		{
			Sprite.spriteName = infos.Item3;
		}
	}

	public override void OnGoNext()
	{
		Singleton<GameSaveManager>.Instance.SetKartState(_kart, E_UnlockableItemSate.NewUnlocked, false);
		base.OnGoNext();
	}
}
