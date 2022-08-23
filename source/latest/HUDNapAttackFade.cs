using System;
using UnityEngine;

public class HUDNapAttackFade : MonoBehaviour
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

	private E_FadeType _fadeType;

	private float _Duration
	{
		get
		{
			return (_fadeType != 0) ? FadeOutDuration : FadeInDuration;
		}
	}

	private void OnDestroy()
	{
		NapBonusEffect.OnLaunched = (Action)Delegate.Remove(NapBonusEffect.OnLaunched, new Action(FadeIn));
	}

	private void Start()
	{
		NapBonusEffect.OnLaunched = (Action)Delegate.Combine(NapBonusEffect.OnLaunched, new Action(FadeIn));
	}

	private void Update()
	{
		if (!_enabled)
		{
			return;
		}
		_elapsedTime += Time.deltaTime;
		float duration = _Duration;
		if (_elapsedTime >= duration)
		{
			_elapsedTime = duration;
			_enabled = false;
			if (_fadeType == E_FadeType.FadeIn)
			{
				FadeOut();
			}
		}
		float time = GetTime(duration);
		FadeTexture.alpha = Mathf.Lerp(0f, 1f, time);
	}

	public void FadeIn()
	{
		_enabled = true;
		_elapsedTime = 0f;
		_fadeType = E_FadeType.FadeIn;
		FadeTexture.alpha = 0f;
	}

	public void FadeOut()
	{
		_enabled = true;
		_elapsedTime = 0f;
		_fadeType = E_FadeType.FadeOut;
		FadeTexture.alpha = 1f;
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
}
