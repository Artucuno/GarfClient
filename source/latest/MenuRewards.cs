using System.Collections.Generic;
using UnityEngine;

public class MenuRewards : AbstractMenu
{
	public UILabel LbRewardName;

	public UILabel LbMessage;

	public UISprite Sprite;

	public UITexturePattern SpriteRarity;

	private AudioSource m_pAudioSource;

	public List<Animation> Animations = new List<Animation>();

	private bool m_bPlayedAnim;

	public override void Awake()
	{
		base.Awake();
		m_pAudioSource = GetComponent<AudioSource>();
	}

	public override void OnEnter()
	{
		base.OnEnter();
	}

	public virtual void OnGoNext()
	{
		m_pMenuEntryPoint.SetStateDelay(Singleton<RewardManager>.Instance.GetState(), 0.1f);
	}

	public new void Update()
	{
		if (m_bPlayedAnim || LoadingManager.IsLoading())
		{
			return;
		}
		m_bPlayedAnim = true;
		if ((bool)m_pAudioSource)
		{
			m_pAudioSource.Play();
		}
		foreach (Animation animation in Animations)
		{
			if (animation.gameObject.activeSelf)
			{
				animation.Play();
			}
		}
	}

	public Tuple<string, UIAtlas, string, ERarity> GetInfos(string Name, E_RewardType Type)
	{
		Object @object = null;
		Tuple<string, UIAtlas, string, ERarity> tuple = new Tuple<string, UIAtlas, string, ERarity>();
		switch (Type)
		{
		case E_RewardType.Custom:
			@object = Resources.Load("Kart/" + Name);
			if (@object != null)
			{
				KartCustom component2 = ((GameObject)@object).GetComponent<KartCustom>();
				tuple.Item1 = Localization.instance.Get(component2.m_TitleTextId);
				tuple.Item3 = component2.spriteName;
				tuple.Item4 = component2.Rarity;
			}
			break;
		case E_RewardType.Hat:
			@object = Resources.Load("Hat/" + Name);
			if (@object != null)
			{
				BonusCustom component3 = ((GameObject)@object).GetComponent<BonusCustom>();
				tuple.Item1 = Localization.instance.Get(component3.m_TitleTextId);
				tuple.Item3 = component3.spriteName;
				tuple.Item4 = component3.Rarity;
			}
			break;
		case E_RewardType.Kart:
			@object = Resources.Load("Kart/K" + Name[0] + "_body");
			if (@object != null)
			{
				KartCarac component = ((GameObject)@object).GetComponent<KartCarac>();
				tuple.Item1 = Localization.instance.Get(component.m_TitleTextId);
				tuple.Item3 = component.spriteName;
				tuple.Item4 = ERarity.Unique;
			}
			break;
		}
		return tuple;
	}
}
