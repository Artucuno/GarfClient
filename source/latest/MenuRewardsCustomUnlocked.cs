public class MenuRewardsCustomUnlocked : MenuRewards
{
	private bool _keepCusto = true;

	private string _custom;

	private ERarity _rarity;

	public int TradePrice;

	public UILabel LTradePrice;

	public override void OnEnter()
	{
		base.OnEnter();
		Tuple<string, ERarity> tuple = Singleton<RewardManager>.Instance.PopUnlockedCustom();
		_custom = tuple.Item1;
		_rarity = tuple.Item2;
		Tuple<string, UIAtlas, string, ERarity> infos = GetInfos(_custom, E_RewardType.Custom);
		LbRewardName.text = infos.Item1;
		if (Sprite != null)
		{
			Sprite.spriteName = infos.Item3;
		}
		if (SpriteRarity != null)
		{
			SpriteRarity.ChangeTexture(Tricks.LogBase2((int)infos.Item4));
		}
		LbMessage.text = Localization.instance.Get("MENU_REWARDS_CUSTOMITATIONS_UNITAIRE");
		LTradePrice.text = TradePrice.ToString();
	}

	public void OnTrade()
	{
		_keepCusto = false;
		if (Singleton<GameSaveManager>.Instance.GetCoins() > TradePrice)
		{
			Singleton<GameSaveManager>.Instance.SpendCoins(TradePrice, false);
			Singleton<RewardManager>.Instance.TradeReward(_rarity);
		}
		OnGoNext();
	}

	public override void OnGoNext()
	{
		if (_keepCusto)
		{
			Singleton<GameSaveManager>.Instance.SetCustomState(_custom, E_UnlockableItemSate.NewUnlocked, false);
		}
		base.OnGoNext();
	}
}
