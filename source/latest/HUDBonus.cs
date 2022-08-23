using System;
using UnityEngine;

public class HUDBonus : MonoBehaviour
{
	private BonusAnimation[] BonusSlots = new BonusAnimation[2];

	private GameObject DropBonus;

	private GameObject Slot;

	private GameObject Quantity;

	public Action<int> OnAnimationFinished;

	private bool ShowBehind;

	private UILabel m_pQuantity;

	private Animation Slot1BackAnim;

	private Animation Slot1ItemAnim;

	private Animation Slot2BackAnim;

	private Animation Slot2ItemAnim;

	private Animation ArrowAnim;

	private UITexturePattern Slot1TexturePattern;

	private UISprite Slot1Sprite;

	private UISprite Slot1SpriteItem;

	private UISprite Slot2Sprite;

	private UISprite Slot2SpriteItem;

	private int m_iIndexToAffectSlot1;

	private int m_iIndexToAffectSlot2;

	private int m_iLogBonus;

	private int m_iLogUsed;

	private int m_iLogUsedBack;

	public int LogBonus
	{
		get
		{
			return m_iLogBonus;
		}
	}

	public int LogUsed
	{
		get
		{
			return m_iLogUsed;
		}
	}

	public int LogUsedBack
	{
		get
		{
			return m_iLogUsedBack;
		}
	}

	private void OnDestroy()
	{
		if (BonusSlots == null)
		{
			return;
		}
		for (int i = 0; i < BonusSlots.Length; i++)
		{
			if (BonusSlots[i] != null)
			{
				BonusAnimation obj = BonusSlots[i];
				obj.OnAnimationFinished = (Action<int>)Delegate.Remove(obj.OnAnimationFinished, new Action<int>(AnimationFinished));
				BonusAnimation obj2 = BonusSlots[i];
				obj2.InformSwapOk = (Action)Delegate.Remove(obj2.InformSwapOk, new Action(OnSwapOk));
				BonusAnimation obj3 = BonusSlots[i];
				obj3.InformLaunchFinished = (Action)Delegate.Remove(obj3.InformLaunchFinished, new Action(OnLaunchFinished));
			}
		}
	}

	private void Awake()
	{
		for (int i = 0; i < BonusSlots.Length; i++)
		{
			BonusSlots[i] = null;
		}
		BonusSlots = base.gameObject.GetComponentsInChildren<BonusAnimation>();
		BonusAnimation[] bonusSlots = BonusSlots;
		foreach (BonusAnimation bonusAnimation in bonusSlots)
		{
			bonusAnimation.OnAnimationFinished = (Action<int>)Delegate.Combine(bonusAnimation.OnAnimationFinished, new Action<int>(AnimationFinished));
			bonusAnimation.InformSwapOk = (Action)Delegate.Combine(bonusAnimation.InformSwapOk, new Action(OnSwapOk));
			bonusAnimation.InformLaunchFinished = (Action)Delegate.Combine(bonusAnimation.InformLaunchFinished, new Action(OnLaunchFinished));
		}
		DropBonus = GameObject.Find("Drop");
		GameObject gameObject = GameObject.Find("DropBackground");
		if ((bool)gameObject)
		{
			ArrowAnim = gameObject.GetComponent<Animation>();
		}
		if (DropBonus != null)
		{
			DropBonus.SetActive(false);
		}
		Slot = GameObject.Find("MainSlotBackground");
		if ((bool)Slot)
		{
			Slot1BackAnim = Slot.GetComponent<Animation>();
			Slot1Sprite = Slot.GetComponent<UISprite>();
			Slot1TexturePattern = Slot.GetComponent<UITexturePattern>();
		}
		Quantity = GameObject.Find("Quantity");
		if (Quantity != null)
		{
			Quantity.SetActive(false);
			m_pQuantity = Quantity.GetComponent<UILabel>();
		}
		GameObject gameObject2 = GameObject.Find("Slot2Background");
		if ((bool)gameObject2)
		{
			Slot2BackAnim = gameObject2.GetComponent<Animation>();
			Slot2Sprite = gameObject2.GetComponent<UISprite>();
		}
		gameObject2 = GameObject.Find("BackgroundSlot2");
		if ((bool)gameObject2)
		{
			Slot2ItemAnim = gameObject2.GetComponent<Animation>();
			Slot2SpriteItem = gameObject2.GetComponent<UISprite>();
		}
		GameObject gameObject3 = GameObject.Find("BackgroundSlot1");
		if ((bool)gameObject3)
		{
			Slot1ItemAnim = gameObject3.GetComponent<Animation>();
			Slot1SpriteItem = gameObject3.GetComponent<UISprite>();
		}
		if ((bool)Slot1Sprite && (bool)Slot2Sprite && (bool)Slot1TexturePattern)
		{
			Slot1Sprite.alpha = 0.1f;
			Slot2Sprite.alpha = 0.1f;
			Slot1TexturePattern.ChangeTexture(0);
		}
		m_iIndexToAffectSlot1 = -1;
		m_iIndexToAffectSlot2 = -1;
	}

	private void AnimationFinished(int _SlotIndex)
	{
		if (OnAnimationFinished != null)
		{
			OnAnimationFinished(_SlotIndex);
			Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.BonusEarned);
		}
		if (_SlotIndex == 0 && DropBonus != null && ShowBehind)
		{
			DropBonus.SetActive(true);
			ShowBehind = false;
		}
		if (_SlotIndex == 0 && (bool)Slot1TexturePattern)
		{
			Slot1TexturePattern.ChangeTexture(1);
		}
	}

	public void StartAnimation(int _Index, EITEM _Item)
	{
		if (_Index < 0 || _Index >= BonusSlots.Length || !(BonusSlots[_Index] != null))
		{
			return;
		}
		switch (_Index)
		{
		case 1:
			if (Slot2ItemAnim.isPlaying && m_iIndexToAffectSlot1 == -1)
			{
				ResetSlot2();
			}
			if (Slot2ItemAnim.isPlaying && m_iIndexToAffectSlot1 != -1)
			{
				ForceSwap();
			}
			break;
		case 0:
			if (Slot1ItemAnim.isPlaying)
			{
				ResetSlot1();
			}
			break;
		}
		UISprite uISprite = null;
		UISprite uISprite2 = null;
		if (_Index == 0)
		{
			uISprite = Slot1Sprite;
			uISprite2 = Slot1SpriteItem;
		}
		else
		{
			uISprite = Slot2Sprite;
			uISprite2 = Slot2SpriteItem;
		}
		if ((bool)uISprite && (bool)uISprite2)
		{
			uISprite.alpha = 1f;
			uISprite2.alpha = 1f;
		}
		BonusSlots[_Index].Launch(_Item);
		ShowBehind = KartBonus.IsBehind(_Item);
		if (LogManager.Instance != null)
		{
			m_iLogBonus++;
		}
	}

	public void ResetSlot(int _Index)
	{
		if (_Index >= 0 && _Index < BonusSlots.Length && BonusSlots[_Index] != null)
		{
			BonusSlots[_Index].Reset();
		}
	}

	public void DeactivateSlot(int _Index)
	{
		if (_Index >= 0 && _Index < BonusSlots.Length && BonusSlots[_Index] != null)
		{
			BonusSlots[_Index].Deactivate();
		}
	}

	public void Launch(int _Index, bool _Behind)
	{
		if (_Index < 0 || _Index >= BonusSlots.Length || !(BonusSlots[_Index] != null))
		{
			return;
		}
		if (m_iIndexToAffectSlot1 != -1)
		{
			ForceSwap();
		}
		if (_Index != 0 || !(DropBonus != null))
		{
			return;
		}
		DropBonus.SetActive(false);
		ShowBehind = false;
		Slot1TexturePattern.ChangeTexture(0);
		Slot1BackAnim.Play("Slot_Used");
		bool flag = KartBonus.IsBehind(BonusSlots[_Index].WantedBonus) && _Behind;
		Slot1ItemAnim.PlayQueued((!flag) ? "Bonus_Used_Front" : "Bonus_Used_Back", QueueMode.CompleteOthers, PlayMode.StopAll);
		if (LogManager.Instance != null)
		{
			m_iLogUsed++;
			if (flag)
			{
				m_iLogUsedBack++;
			}
		}
	}

	public void AffectSlot(int _Index, int _Index2, bool _Behind)
	{
		if (_Index >= 0 && _Index < BonusSlots.Length && BonusSlots[_Index] != null && _Index2 >= 0 && _Index2 < BonusSlots.Length && BonusSlots[_Index2] != null)
		{
			Slot2ItemAnim.Rewind();
			Slot2BackAnim.Rewind();
			Slot2ItemAnim.PlayQueued("Bonus2_Swap", QueueMode.PlayNow);
			Slot2BackAnim.PlayQueued("Slot2_Swap", QueueMode.PlayNow);
			Launch(_Index, _Behind);
			m_iIndexToAffectSlot1 = _Index;
			m_iIndexToAffectSlot2 = _Index2;
		}
	}

	public void SetQuantity(int _Quantity)
	{
		if (!Quantity || !m_pQuantity)
		{
			return;
		}
		if (_Quantity <= 1 && Quantity.activeSelf)
		{
			Quantity.SetActive(false);
		}
		else if (_Quantity > 1)
		{
			if (!Quantity.activeSelf)
			{
				Quantity.SetActive(true);
			}
			m_pQuantity.text = "x " + _Quantity;
		}
	}

	public void OnBonus()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.LaunchBonus, 1f);
		}
	}

	public void OnDropBonus()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.DropBonus, 1f);
		}
	}

	public void OnLaunchFinished()
	{
		Slot1SpriteItem.alpha = 0f;
		if (m_iIndexToAffectSlot1 == -1)
		{
			Slot1Sprite.alpha = 0.1f;
		}
		ResetSlot1();
	}

	public void OnSwapOk()
	{
		Slot2Sprite.alpha = 0.1f;
		BonusSlots[m_iIndexToAffectSlot1].Affect(BonusSlots[m_iIndexToAffectSlot2]);
		ShowBehind = KartBonus.IsBehind(BonusSlots[m_iIndexToAffectSlot1].WantedBonus);
		ChangeArrowTexture();
		Slot2SpriteItem.alpha = 0f;
		Slot1SpriteItem.alpha = 1f;
		ResetSlot(m_iIndexToAffectSlot2);
		m_iIndexToAffectSlot1 = -1;
		m_iIndexToAffectSlot2 = -1;
		ArrowAnim.Play("DropArrow_Turn");
		Slot1ItemAnim.Play("Bonus_Swapped");
	}

	public void ForceSwap()
	{
		ResetSlot1();
		BonusSlots[m_iIndexToAffectSlot1].Affect(BonusSlots[m_iIndexToAffectSlot2]);
		ShowBehind = KartBonus.IsBehind(BonusSlots[m_iIndexToAffectSlot1].WantedBonus);
		ChangeArrowTexture();
		ResetSlot2();
		Slot1SpriteItem.alpha = 1f;
		m_iIndexToAffectSlot1 = -1;
		m_iIndexToAffectSlot2 = -1;
		ArrowAnim.Play("DropArrow_Turn");
		Slot1ItemAnim.Play("Bonus_Swapped");
	}

	public void ChangeArrowTexture()
	{
		if (BonusSlots[m_iIndexToAffectSlot1].State == BonusAnimation_State.STOPPED && m_iIndexToAffectSlot1 == 0 && DropBonus != null)
		{
			DropBonus.SetActive(KartBonus.IsBehind(BonusSlots[0].WantedBonus));
			ShowBehind = false;
			if ((bool)Slot1TexturePattern)
			{
				Slot1TexturePattern.ChangeTexture(1);
			}
		}
	}

	public void ResetSlot1()
	{
		ResetSlot(0);
		Slot1ItemAnim.Play("Bonus_Used_Front");
		Slot1ItemAnim.Rewind();
		Slot1ItemAnim.Play();
		Slot1ItemAnim.Sample();
		Slot1ItemAnim.Stop();
	}

	public void ResetSlot2()
	{
		Slot2Sprite.alpha = 0.1f;
		Slot2SpriteItem.alpha = 0f;
		ResetSlot(1);
		Slot2ItemAnim.Rewind();
		Slot2ItemAnim.Play();
		Slot2ItemAnim.Sample();
		Slot2ItemAnim.Stop();
		Slot2BackAnim.Rewind();
		Slot2BackAnim.Play();
		Slot2BackAnim.Sample();
		Slot2BackAnim.Stop();
	}

	public void ResetSlots()
	{
		ResetSlot1();
		ResetSlot2();
		DeactivateSlot(1);
		Slot1Sprite.alpha = 0.1f;
		Slot1SpriteItem.alpha = 0f;
		DropBonus.SetActive(false);
		ShowBehind = false;
		if ((bool)Slot1TexturePattern)
		{
			Slot1TexturePattern.ChangeTexture(0);
		}
		SetQuantity(0);
	}
}
