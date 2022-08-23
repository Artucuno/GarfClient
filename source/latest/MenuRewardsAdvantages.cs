using System;

public class MenuRewardsAdvantages : MenuRewards
{
	private string[] _advantages;

	public override void OnEnter()
	{
		base.OnEnter();
		_advantages = Singleton<RewardManager>.Instance.PopAdvantage();
		string key = string.Empty;
		switch (Singleton<GameConfigurator>.Instance.Difficulty)
		{
		case EDifficulty.NORMAL:
			key = "MENU_DIFFICULTY_NORMAL";
			break;
		case EDifficulty.HARD:
			key = "MENU_DIFFICULTY_HARD";
			break;
		case EDifficulty.EASY:
			key = "MENU_DIFFICULTY_EASY";
			break;
		}
		LbMessage.text = string.Format(Localization.instance.Get("MENU_REWARDS_AVANTAGES_GROUP"), Localization.instance.Get(key));
	}

	public override void OnGoNext()
	{
		string[] advantages = _advantages;
		foreach (string text in advantages)
		{
			if (text != null)
			{
				EAdvantage eAdvantage = (EAdvantage)(int)Enum.Parse(typeof(EAdvantage), text);
				if (eAdvantage != EAdvantage.None)
				{
					Singleton<GameSaveManager>.Instance.SetAdvantageState(eAdvantage, E_UnlockableItemSate.NewLocked, false);
				}
			}
		}
		Singleton<GameSaveManager>.Instance.Save();
		base.OnGoNext();
	}
}
