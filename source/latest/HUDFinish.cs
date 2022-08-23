using UnityEngine;

public class HUDFinish : MonoBehaviour
{
	public GameObject Finish;

	public GameObject Victory;

	public UILabel Message;

	public float LabelHideDelay = 1f;

	private int m_iRank;

	private EndRaceGameState m_pState;

	private float m_fLabelHideDelay;

	private bool m_bDelayedHideMessage;

	public bool isDisplaying
	{
		get
		{
			return Finish.activeSelf;
		}
	}

	public float AnimDuration
	{
		get
		{
			return Finish.animation.clip.length;
		}
	}

	public EndRaceGameState EndState
	{
		set
		{
			m_pState = value;
		}
	}

	private void Awake()
	{
		Finish.SetActive(false);
		Victory.SetActive(false);
	}

	public void FillRank(int _iRank)
	{
		m_iRank = _iRank;
	}

	public void Show()
	{
		Finish.SetActive(true);
		Victory.SetActive(false);
		m_bDelayedHideMessage = true;
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
		{
			Message.text = Localization.instance.Get("HUD_SUCCESS");
			m_bDelayedHideMessage = false;
		}
		else if (m_iRank == 0)
		{
			Victory.SetActive(true);
			if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
			{
				Message.text = Localization.instance.Get("HUD_FINISHTIMETRIAL_RECORD");
			}
			else
			{
				Message.text = Localization.instance.Get("HUD_FINISHRACE_FINISHFIRST");
			}
		}
		else
		{
			Message.text = Localization.instance.Get("HUD_FINISHRACE_FINISH");
		}
	}

	public void ShowLap(int iLap)
	{
		Finish.SetActive(true);
		Victory.SetActive(false);
		Message.text = Localization.instance.Get("HUD_LAP_" + iLap);
		m_bDelayedHideMessage = true;
	}

	public void Update()
	{
		if (m_bDelayedHideMessage)
		{
			m_fLabelHideDelay += Time.deltaTime;
			if (m_fLabelHideDelay >= LabelHideDelay)
			{
				Hide();
			}
		}
	}

	public void Hide()
	{
		Finish.SetActive(false);
		m_fLabelHideDelay = 0f;
		m_bDelayedHideMessage = false;
	}

	public void Next()
	{
		if ((bool)m_pState)
		{
			m_pState.Next();
		}
	}
}
