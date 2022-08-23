using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGameMode : InGameGameMode
{
	public float AfterFailTimer = 2f;

	private bool m_bLaunched;

	private bool m_bSuccess;

	private bool m_bFail;

	private float m_fRedisplayTimer;

	private float m_fAfterSuccessTimer;

	private float m_fAfterFailTimer;

	private ETutorialState m_eCurrentState;

	private GameObject[] m_oPanels = new GameObject[Enum.GetNames(typeof(ETutorialState)).Length];

	private IGTutorialState[] m_oPanelScripts = new IGTutorialState[Enum.GetNames(typeof(ETutorialState)).Length];

	private GameObject m_oSuccesLabel;

	private HUDFinish m_oFinish;

	private float m_fSuccessAnimDuration;

	private bool m_bInstructionShown;

	private bool m_bShowHUDBonus;

	private List<GameObject> m_cBBEs = new List<GameObject>();

	public float DriftAttemptTimer = 10f;

	private bool m_bDriftAttempt;

	private bool m_bBlueDrift;

	private bool m_bRedDrift;

	private float m_fDriftAttemptTimer;

	public int DriftTraining = 2;

	private int m_iDriftCount;

	private int m_iLeftGyro;

	private int m_iRightGyro;

	private int m_iLeft;

	private int m_iRight;

	private bool m_bWannaItem;

	private EITEM m_eDesiredItem;

	public bool ShowHUDBonus
	{
		get
		{
			return m_bShowHUDBonus;
		}
		set
		{
			m_bShowHUDBonus = value;
			base.Hud.ActivateHUDBonus(value);
		}
	}

	public bool Ended
	{
		get
		{
			return m_eCurrentState == ETutorialState.End;
		}
	}

	public int DriftSuccessLevel
	{
		get
		{
			if (m_bRedDrift)
			{
				return 3;
			}
			if (m_bBlueDrift)
			{
				return 2;
			}
			if (m_bDriftAttempt)
			{
				return 1;
			}
			return 0;
		}
	}

	public bool WannaItem
	{
		get
		{
			return m_bWannaItem;
		}
		set
		{
			m_bWannaItem = value;
		}
	}

	public EITEM DesiredItem
	{
		get
		{
			return m_eDesiredItem;
		}
		set
		{
			m_eDesiredItem = value;
		}
	}

	public bool Launched
	{
		get
		{
			return m_bLaunched;
		}
	}

	public bool InstructionShown
	{
		get
		{
			return m_bInstructionShown;
		}
	}

	public new ETutorialState State
	{
		get
		{
			return m_eCurrentState;
		}
	}

	public override void Awake()
	{
		base.Awake();
	}

	public override void InitState()
	{
		_state = E_GameState.RaceTutorial;
	}

	protected override void Start()
	{
		base.Start();
		Transform parent = base.Hud.transform.Find("Camera");
		m_oFinish = base.Hud.HUDFinish;
		m_fSuccessAnimDuration = m_oFinish.AnimDuration;
		UnityEngine.Object[] array = Resources.LoadAll("TutorialStates", typeof(GameObject));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject = (GameObject)array2[i];
			IGTutorialState component = gameObject.GetComponent<IGTutorialState>();
			if ((bool)component)
			{
				m_oPanels[(int)component.ID] = (GameObject)UnityEngine.Object.Instantiate(gameObject);
				m_oPanels[(int)component.ID].SetActive(false);
				m_oPanels[(int)component.ID].transform.parent = parent;
				component = m_oPanels[(int)component.ID].GetComponent<IGTutorialState>();
				component.GameMode = this;
				m_oPanelScripts[(int)component.ID] = component;
			}
		}
		UnityEngine.Object[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(BoxBonusEntity));
		UnityEngine.Object[] array4 = array3;
		for (int j = 0; j < array4.Length; j++)
		{
			BoxBonusEntity boxBonusEntity = (BoxBonusEntity)array4[j];
			m_cBBEs.Add(boxBonusEntity.gameObject);
			boxBonusEntity.gameObject.SetActive(false);
		}
		UnityEngine.Object[] array5 = UnityEngine.Object.FindObjectsOfType(typeof(PuzzlePiece));
		UnityEngine.Object[] array6 = array5;
		for (int k = 0; k < array6.Length; k++)
		{
			PuzzlePiece puzzlePiece = (PuzzlePiece)array6[k];
			puzzlePiece.gameObject.SetActive(false);
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			Singleton<GameOptionManager>.Instance.SetInputType(E_InputType.Gyroscopic, false);
		}
		m_bInstructionShown = false;
	}

	public override void StartScene()
	{
		base.StartScene();
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(RcVehicleRaceStats));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RcVehicleRaceStats rcVehicleRaceStats = (RcVehicleRaceStats)array2[i];
			rcVehicleRaceStats.CanCompleteLap = false;
		}
		SetAIActive(false);
		Singleton<GameManager>.Instance.GameMode.GetHumanKart().SetLocked(false);
	}

	public void FirstPanel()
	{
		if (!m_bLaunched)
		{
			StartState(ETutorialState.Acceleration);
			m_bLaunched = true;
			_hasStart = true;
		}
	}

	public new void Update()
	{
		base.Update();
		if (m_oPanels[(int)m_eCurrentState].activeSelf && m_oPanelScripts[(int)m_eCurrentState].CanBeDisabled() && Input.anyKeyDown)
		{
			m_oPanels[(int)m_eCurrentState].SetActive(false);
			Time.timeScale = 1f;
			m_bInstructionShown = true;
		}
		m_fRedisplayTimer += Time.deltaTime;
		if (!m_bSuccess && m_fRedisplayTimer >= m_oPanelScripts[(int)m_eCurrentState].MaxStateTime)
		{
			StartState(m_eCurrentState);
		}
		if ((bool)m_oFinish && m_oFinish.isDisplaying)
		{
			m_fAfterSuccessTimer += Time.deltaTime;
			if (m_fAfterSuccessTimer >= m_oPanelScripts[(int)m_eCurrentState].AfterSuccessDelay && m_fAfterSuccessTimer >= m_fSuccessAnimDuration)
			{
				NextState();
				m_oFinish.Hide();
			}
		}
		if (m_bFail)
		{
			m_fAfterFailTimer += Time.deltaTime;
			if (m_fAfterFailTimer >= AfterFailTimer)
			{
				m_bFail = false;
				m_fAfterFailTimer = 0f;
				NextState();
			}
		}
		if (!m_bDriftAttempt)
		{
			return;
		}
		if (m_eCurrentState != ETutorialState.Drift)
		{
			m_bDriftAttempt = false;
			return;
		}
		m_fDriftAttemptTimer += Time.deltaTime;
		if (m_fDriftAttemptTimer >= DriftAttemptTimer)
		{
			NextState();
			m_bDriftAttempt = false;
		}
	}

	public void NextState()
	{
		StartState(m_oPanels[(int)m_eCurrentState].GetComponent<IGTutorialState>().NextState);
	}

	private void StartState(ETutorialState State)
	{
		Time.timeScale = 0f;
		m_oPanels[(int)m_eCurrentState].SetActive(false);
		m_oPanelScripts[(int)m_eCurrentState].OnExit();
		m_eCurrentState = State;
		m_oPanelScripts[(int)m_eCurrentState].OnEnter();
		m_oPanels[(int)m_eCurrentState].SetActive(true);
		m_bInstructionShown = false;
		m_fRedisplayTimer = 0f;
		m_fAfterSuccessTimer = 0f;
		m_bSuccess = false;
		if (m_eCurrentState == ETutorialState.End)
		{
			GrandFinal();
		}
	}

	public void SetAIActive(bool bActivate)
	{
		for (int i = 0; i < 6; i++)
		{
			if (i != Singleton<GameManager>.Instance.GameMode.GetHumanPlayerVehicleId())
			{
				Kart kartWithVehicleId = Singleton<GameManager>.Instance.GameMode.GetKartWithVehicleId(i);
				kartWithVehicleId.SetLocked(!bActivate);
			}
		}
	}

	public void OnSuccess()
	{
		if (Launched && (bool)m_oFinish)
		{
			m_oFinish.Show();
			m_fAfterSuccessTimer = 0f;
			m_bSuccess = true;
		}
	}

	public void OnFail()
	{
		m_fAfterFailTimer = 0f;
		m_bFail = true;
	}

	public void Accelerate()
	{
		if (m_eCurrentState == ETutorialState.Acceleration && !m_bSuccess)
		{
			OnSuccess();
		}
	}

	public void Direction(float Steer)
	{
		if (m_eCurrentState < ETutorialState.Direction_PC || m_eCurrentState > ETutorialState.Direction_Tactile || m_bSuccess)
		{
			return;
		}
		if (Singleton<GameOptionManager>.Instance.GetInputType() == E_InputType.Gyroscopic)
		{
			if (Steer < 0f)
			{
				m_iLeftGyro++;
			}
			else if (Steer > 0f)
			{
				m_iRightGyro++;
			}
			if (m_iLeftGyro > 0 && m_iRightGyro > 0)
			{
				OnSuccess();
			}
			return;
		}
		if (Steer < 0f)
		{
			m_iLeft++;
		}
		else if (Steer > 0f)
		{
			m_iRight++;
		}
		if (m_iLeft > 0 && m_iRight > 0)
		{
			m_iLeft = 0;
			m_iRight = 0;
			OnSuccess();
		}
	}

	public void Brake()
	{
		if (m_eCurrentState == ETutorialState.Brake)
		{
			OnSuccess();
		}
	}

	public void DriftAttempt()
	{
		if (m_eCurrentState == ETutorialState.Drift)
		{
			m_bDriftAttempt = true;
		}
	}

	public void BlueDrift()
	{
		if ((m_eCurrentState != ETutorialState.Drift && m_eCurrentState != ETutorialState.Drift_TryAgain && m_eCurrentState != ETutorialState.Drift_Perfect) || m_bSuccess)
		{
			return;
		}
		m_bBlueDrift = true;
		if (m_eCurrentState == ETutorialState.Drift_Perfect)
		{
			m_iDriftCount++;
			if (m_iDriftCount >= DriftTraining)
			{
				OnSuccess();
			}
		}
		else
		{
			OnSuccess();
		}
	}

	public void RedDrift()
	{
		if ((m_eCurrentState != ETutorialState.Drift && m_eCurrentState != ETutorialState.Drift_TryAgain && m_eCurrentState != ETutorialState.Drift_NotBad && m_eCurrentState != ETutorialState.Drift_Perfect) || m_bSuccess)
		{
			return;
		}
		m_bRedDrift = true;
		if (m_eCurrentState == ETutorialState.Drift_Perfect)
		{
			m_iDriftCount++;
			if (m_iDriftCount >= DriftTraining)
			{
				OnSuccess();
			}
		}
		else
		{
			OnSuccess();
		}
	}

	public void GotItem(EITEM Item)
	{
		if ((m_eCurrentState == ETutorialState.Bonus || m_eCurrentState == ETutorialState.Bonus_2) && !m_bSuccess && WannaItem && Item == DesiredItem)
		{
			OnSuccess();
			WannaItem = false;
		}
	}

	public void UsedBonus(EITEM Item, bool Behind)
	{
		if ((m_eCurrentState == ETutorialState.Bonus_Lasagna || m_eCurrentState == ETutorialState.Bonus_Pie || m_eCurrentState == ETutorialState.Bonus_PieFailed || m_eCurrentState == ETutorialState.Bonus_PieBehind) && !m_bSuccess && Item == DesiredItem)
		{
			((BonusTutorialState)m_oPanelScripts[(int)m_eCurrentState]).LaunchedBehind = Behind;
			LaunchType type = ((BonusTutorialState)m_oPanelScripts[(int)m_eCurrentState]).Type;
			if (type == LaunchType.ANY || (type == LaunchType.FRONT && !Behind) || (type == LaunchType.BACK && Behind))
			{
				OnSuccess();
			}
			else
			{
				OnFail();
			}
		}
	}

	public void GrandFinal()
	{
		SetAIActive(true);
		base.Hud.ShowEndTutorialHUD();
	}

	public void ReactivateBBEs()
	{
		foreach (GameObject cBBE in m_cBBEs)
		{
			cBBE.gameObject.SetActive(true);
		}
	}

	public override void PlaceVehiclesOnStartLine()
	{
		GameObject gameObject = GameObject.Find("StartTuto");
		if (gameObject != null)
		{
			for (int i = 0; i < m_pPlayers.Length; i++)
			{
				StartPositions[i] = gameObject.transform.GetChild(i);
			}
		}
	}

	public void Pause(bool _Pause)
	{
		if (!m_bInstructionShown)
		{
			m_oPanels[(int)m_eCurrentState].SetActive(!_Pause);
		}
	}
}
