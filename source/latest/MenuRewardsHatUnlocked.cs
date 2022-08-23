public class MenuRewardsHatUnlocked : MenuRewards
{
	private string _hat;

	private ERarity _rarity;

	private bool _keepHat = true;

	public int TradePrice;

	public UILabel LTradePrice;

	public override void OnEnter()
	{
		base.OnEnter();
		Tuple<string, ERarity> tuple = Singleton<RewardManager>.Instance.PopUnlockedHat();
		_hat = tuple.Item1;
		_rarity = tuple.Item2;
		Tuple<string, UIAtlas, string, ERarity> infos = GetInfos(_hat, E_RewardType.Hat);
		LbRewardName.text = infos.Item1;
		if (Sprite != null)
		{
			Sprite.spriteName = infos.Item3;
		}
		if (SpriteRarity != null)
		{
			SpriteRarity.ChangeTexture(Tricks.LogBase2((int)infos.Item4));
		}
		LTradePrice.text = TradePrice.ToString();
	}

	public void OnTrade()
	{
		_keepHat = false;
		if (Singleton<GameSaveManager>.Instance.GetCoins() > TradePrice)
		{
			Singleton<GameSaveManager>.Instance.SpendCoins(TradePrice, false);
			Singleton<RewardManager>.Instance.TradeReward(_rarity);
		}
		OnGoNext();
	}

	public override void OnGoNext()
	{
		if (_keepHat)
		{
			Singleton<GameSaveManager>.Instance.SetHatState(_hat, E_UnlockableItemSate.NewUnlocked, false);
		}
		base.OnGoNext();
	}
}
