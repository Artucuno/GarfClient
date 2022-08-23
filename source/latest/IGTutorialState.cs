using System.Collections.Generic;
using UnityEngine;

public class IGTutorialState : MonoBehaviour
{
	public ETutorialState ID;

	public ETutorialState ENextState;

	public List<GameObject> PCSpecificObjects;

	public List<GameObject> MobileSpecificObjects;

	public float AfterSuccessDelay = 5f;

	public float MaxStateTime = 30f;

	public bool DisablePanelOnTouch = true;

	public float DisablePanelOnTouchTimer = 2f;

	private float m_fDisablePanelOnTouchTimer;

	[HideInInspector]
	public TutorialGameMode GameMode;

	public virtual ETutorialState NextState
	{
		get
		{
			return ENextState;
		}
	}

	public virtual bool CanBeDisabled()
	{
		if (Time.realtimeSinceStartup >= m_fDisablePanelOnTouchTimer + DisablePanelOnTouchTimer)
		{
			return DisablePanelOnTouch;
		}
		return false;
	}

	public virtual void OnEnable()
	{
		if (!GameMode)
		{
			GameMode = (TutorialGameMode)Object.FindObjectOfType(typeof(TutorialGameMode));
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			foreach (GameObject pCSpecificObject in PCSpecificObjects)
			{
				pCSpecificObject.SetActive(false);
			}
		}
		else
		{
			foreach (GameObject mobileSpecificObject in MobileSpecificObjects)
			{
				mobileSpecificObject.SetActive(false);
			}
		}
		m_fDisablePanelOnTouchTimer = Time.realtimeSinceStartup;
	}

	public virtual void OnEnter()
	{
	}

	public virtual void OnExit()
	{
	}

	public virtual void OnDisable()
	{
	}
}
