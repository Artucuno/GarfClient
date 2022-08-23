public class MenuRewardsCustomDuplicate : MenuRewards
{
	private string _custom;

	private ERarity _rarity;

	public int ResellPrice;

	public int TradePrice;

	public UILabel LTradePrice;

	public UILabel LResellPrice;

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
		LbMessage.text = Localization.instance.Get("MENU_REWARDS_CUSTOMITATIONS_DOUBLON");
		LTradePrice.text = TradePrice.ToString();
		LResellPrice.text = ResellPrice.ToString();
	}

	public void OnTrade()
	{
		if (Singleton<GameSaveManager>.Instance.GetCoins() > TradePrice)
		{
			Singleton<GameSaveManager>.Instance.SpendCoins(TradePrice, false);
			Singleton<RewardManager>.Instance.TradeReward(_rarity);
		}
		OnGoNext();
	}

	public void OnResell()
	{
		Singleton<GameSaveManager>.Instance.EarnCoins(ResellPrice, false, false);
		OnGoNext();
	}
}
