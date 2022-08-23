using System;
using UnityEngine;

public class HUDTutorial : MonoBehaviour
{
	public Action<bool> Next;

	private bool m_bShowAgain;

	public void Awake()
	{
		if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
		{
			OnNext();
		}
	}

	public void OnEnter()
	{
	}

	public void OnExit()
	{
	}

	public void OnNext()
	{
		if (Next != null)
		{
			Next(m_bShowAgain);
		}
	}

	public void OnActivate(bool Checked)
	{
		m_bShowAgain = !Checked;
	}
}
