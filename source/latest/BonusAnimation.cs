using System;
using System.Collections.Generic;
using UnityEngine;

public class BonusAnimation : MonoBehaviour
{
	public Action<int> OnAnimationFinished;

	public Action InformSwapOk;

	public Action InformLaunchFinished;

	public float TimerBeforeDeceleration = 1f;

	private BonusAnimation_State m_eState;

	private float CurrentTimer;

	public float SlowDownTimer = 1.5f;

	public float m_BaseFps;

	private int m_WantedBonus;

	protected float m_BonusIndex;

	private int m_LastBonusIndex = -1;

	protected float m_CustomFPS = 30f;

	protected UISprite mSprite;

	protected List<string> mSpriteNames = new List<string>();

	public int SlotIndex;

	public EITEM WantedBonus
	{
		get
		{
			return (EITEM)m_WantedBonus;
		}
		set
		{
			m_WantedBonus = (int)value;
		}
	}

	public BonusAnimation_State State
	{
		get
		{
			return m_eState;
		}
	}

	public void Start()
	{
		m_eState = BonusAnimation_State.STOPPED;
		CurrentTimer = TimerBeforeDeceleration;
		m_WantedBonus = 0;
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
		}
		mSpriteNames.Clear();
		if (mSprite != null && mSprite.atlas != null)
		{
			List<UIAtlas.Sprite> spriteList = mSprite.atlas.spriteList;
			int i = 0;
			for (int count = spriteList.Count; i < count; i++)
			{
				UIAtlas.Sprite sprite = spriteList[i];
				if (string.IsNullOrEmpty("Bonus") || sprite.name.StartsWith("Bonus"))
				{
					mSpriteNames.Add(sprite.name);
				}
			}
		}
		base.gameObject.SetActive(false);
	}

	public void Update()
	{
		if (Singleton<InputManager>.Instance.GetAction(EAction.LaunchBonus) == 1f)
		{
			OnTouch();
		}
		if (mSpriteNames.Count <= 1 || !Application.isPlaying || !(m_CustomFPS > 0f) || m_eState <= BonusAnimation_State.STOPPED)
		{
			return;
		}
		m_BonusIndex += m_CustomFPS * Time.deltaTime * 0.5f;
		m_BonusIndex %= mSpriteNames.Count;
		if (m_LastBonusIndex != (int)m_BonusIndex)
		{
			m_LastBonusIndex = (int)m_BonusIndex;
			mSprite.spriteName = mSpriteNames[(int)m_BonusIndex];
			mSprite.MakePixelPerfect();
			Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.BonusRoulette);
		}
		if (m_eState == BonusAnimation_State.DECELERATED)
		{
			if (m_CustomFPS > 0f)
			{
				m_CustomFPS -= Time.deltaTime * m_BaseFps / SlowDownTimer;
			}
			if (m_CustomFPS <= 0f)
			{
				if ((int)m_BonusIndex != m_WantedBonus - 1 && m_WantedBonus > 0)
				{
					mSprite.spriteName = mSpriteNames[m_WantedBonus - 1];
					mSprite.MakePixelPerfect();
				}
				m_eState = BonusAnimation_State.STOPPED;
				if (OnAnimationFinished != null)
				{
					OnAnimationFinished(SlotIndex);
				}
			}
		}
		if (m_CustomFPS > 0f)
		{
			m_BonusIndex += m_CustomFPS * Time.deltaTime * 0.5f;
		}
		if (CurrentTimer >= 0f && m_eState == BonusAnimation_State.STARTED)
		{
			CurrentTimer -= Time.deltaTime;
			if (CurrentTimer < 0f)
			{
				DoStopAnim();
			}
		}
	}

	public void Affect(BonusAnimation _ba)
	{
		m_CustomFPS = _ba.m_CustomFPS;
		m_BaseFps = _ba.m_BaseFps;
		SlowDownTimer = _ba.SlowDownTimer;
		m_eState = _ba.m_eState;
		m_BonusIndex = _ba.m_BonusIndex;
		CurrentTimer = _ba.CurrentTimer;
		m_WantedBonus = _ba.m_WantedBonus;
		if (m_eState == BonusAnimation_State.STOPPED && m_WantedBonus > 0)
		{
			mSprite.spriteName = mSpriteNames[m_WantedBonus - 1];
			mSprite.MakePixelPerfect();
			base.gameObject.SetActive(true);
		}
		else if (m_eState == BonusAnimation_State.DECELERATED && m_WantedBonus > 0)
		{
			base.gameObject.SetActive(true);
		}
		else if (m_WantedBonus == 0)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnTouch()
	{
		if (m_eState == BonusAnimation_State.STARTED)
		{
			DoStopAnim();
		}
	}

	public void DoStopAnim()
	{
		m_eState = BonusAnimation_State.DECELERATED;
		m_BonusIndex = ((float)(m_WantedBonus - 1) - 0.5f * m_BaseFps * SlowDownTimer + m_BaseFps * SlowDownTimer * (float)mSpriteNames.Count) % (float)mSpriteNames.Count + 0.5f;
	}

	public void Reset()
	{
		m_CustomFPS = m_BaseFps;
		m_eState = BonusAnimation_State.STOPPED;
		m_BonusIndex = 0f;
		CurrentTimer = TimerBeforeDeceleration;
		m_WantedBonus = 0;
	}

	public void Deactivate()
	{
		base.gameObject.SetActive(false);
	}

	public void Launch(EITEM _WantedBonus)
	{
		Reset();
		base.gameObject.SetActive(true);
		WantedBonus = _WantedBonus;
		m_eState = BonusAnimation_State.STARTED;
	}

	public void OnSwapOk()
	{
		if (InformSwapOk != null)
		{
			InformSwapOk();
		}
	}

	public void OnLaunchFinished()
	{
		if (InformLaunchFinished != null)
		{
			InformLaunchFinished();
		}
	}
}
