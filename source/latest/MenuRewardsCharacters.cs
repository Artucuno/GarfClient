public class MenuRewardsCharacters : MenuRewards
{
	private ECharacter _character;

	public override void OnEnter()
	{
		base.OnEnter();
		_character = Singleton<RewardManager>.Instance.PopCharacter();
		LbRewardName.text = _character.ToString();
		if (Sprite != null)
		{
			Sprite.GetComponent<UITexturePattern>().ChangeTexture((int)_character);
		}
		LbMessage.text = Localization.instance.Get("MENU_REWARDS_PERSONNAGE");
	}

	public override void OnGoNext()
	{
		Singleton<GameSaveManager>.Instance.SetCharacterState(_character, E_UnlockableItemSate.NewUnlocked, false);
		base.OnGoNext();
	}
}
