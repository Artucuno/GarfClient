using System;
using UnityEngine;

public class HUDFade : MonoBehaviour
{
	public enum E_FadeType
	{
		FadeIn,
		FadeOut
	}

	public UITexture FadeTexture;

	public float FadeInDuration = 0.25f;

	public float FadeOutDuration = 0.25f;

	private float _elapsedTime;

	private bool _enabled;

	private float m_fTimer;

	private Kart _kart;

	private E_FadeType _fadeType;

	private float _Duration
	{
		get
		{
			return m_fTimer;
		}
	}

	private void OnDestroy()
	{
		if (_kart != null)
		{
			Kart kart = _kart;
			kart.OnRespawn = (Action)Delegate.Remove(kart.OnRespawn, new Action(FadeOut));
			Kart kart2 = _kart;
			kart2.OnKilled = (Action)Delegate.Remove(kart2.OnKilled, new Action(FadeIn));
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (_kart == null)
		{
			_kart = Singleton<GameManager>.Instance.GameMode.GetHumanKart();
			if (_kart != null)
			{
				Kart kart = _kart;
				kart.OnRespawn = (Action)Delegate.Combine(kart.OnRespawn, new Action(FadeOut));
				Kart kart2 = _kart;
				kart2.OnKilled = (Action)Delegate.Combine(kart2.OnKilled, new Action(FadeIn));
			}
		}
		if (_enabled)
		{
			_elapsedTime += Time.deltaTime;
			float duration = _Duration;
			if (_elapsedTime >= duration)
			{
				_elapsedTime = duration;
				_enabled = false;
			}
			float time = GetTime(duration);
			FadeTexture.alpha = Mathf.Lerp(0f, 1f, time);
		}
	}

	public void FadeIn()
	{
		DoFadeIn(FadeInDuration);
	}

	public void DoFadeIn(float _Time)
	{
		_enabled = true;
		_elapsedTime = 0f;
		_fadeType = E_FadeType.FadeIn;
		FadeTexture.alpha = 0f;
		m_fTimer = _Time;
	}

	public void FadeOut()
	{
		DoFadeOut(FadeOutDuration);
	}

	public void DoFadeOut(float _Time)
	{
		_enabled = true;
		_elapsedTime = 0f;
		_fadeType = E_FadeType.FadeOut;
		FadeTexture.alpha = 1f;
		m_fTimer = _Time;
	}

	private float GetTime(float pDuration)
	{
		float num = _elapsedTime / pDuration;
		if (_fadeType == E_FadeType.FadeIn)
		{
			return num;
		}
		if (_fadeType == E_FadeType.FadeOut)
		{
			return 1f - num;
		}
		return 0f;
	}

	public void ForceIn()
	{
		FadeTexture.alpha = 1f;
	}
}
