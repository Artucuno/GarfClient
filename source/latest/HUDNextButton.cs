using System;
using UnityEngine;

public class HUDNextButton : MonoBehaviour
{
	public float Timer = 0.3f;

	public static Action OnNextClick;

	private float m_fElapsedTime;

	private void Update()
	{
		if (m_fElapsedTime < Timer)
		{
			m_fElapsedTime += Time.deltaTime;
		}
	}

	private void OnClick()
	{
		if (OnNextClick != null && m_fElapsedTime > Timer)
		{
			OnNextClick();
			m_fElapsedTime = 0f;
		}
	}
}
