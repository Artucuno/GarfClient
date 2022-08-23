using UnityEngine;

public class HUDControls : MonoBehaviour
{
	private GameObject m_pSteerLeft;

	private GameObject m_pSteerRight;

	private GameObject m_pBrake;

	private GameObject m_pDrift;

	private GameObject m_pPause;

	public void Awake()
	{
		m_pSteerLeft = null;
		m_pSteerRight = null;
		m_pBrake = null;
		m_pDrift = null;
		m_pPause = null;
		m_pSteerLeft = GameObject.Find("SteerLeft");
		m_pSteerRight = GameObject.Find("SteerRight");
		m_pBrake = GameObject.Find("Brake");
		m_pDrift = GameObject.Find("Drift");
		m_pPause = GameObject.Find("Pause");
	}

	public void Start()
	{
		StartGame();
	}

	public void StartGame()
	{
		ShowExceptPause(false);
	}

	public void StartRace()
	{
		ShowExceptPause(true);
	}

	public void Pause(bool _Pause)
	{
		ShowAll(!_Pause);
	}

	public void ShowExceptPause(bool _Show)
	{
		m_pSteerLeft.SetActive(_Show);
		m_pSteerRight.SetActive(_Show);
		if (_Show && Singleton<GameOptionManager>.Instance.GetInputType() == E_InputType.Gyroscopic)
		{
			m_pSteerLeft.gameObject.SetActive(false);
			m_pSteerRight.gameObject.SetActive(false);
		}
		m_pBrake.SetActive(_Show);
		m_pDrift.SetActive(_Show);
	}

	public void ShowAll(bool _Show)
	{
		ShowExceptPause(_Show);
		m_pPause.SetActive(_Show);
	}

	public void OnAction(EInputAction _Action)
	{
		switch (_Action)
		{
		case EInputAction.SteerLeft:
			Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.Steer, -1f);
			break;
		case EInputAction.SteerRight:
			Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.Steer, 1f);
			break;
		case EInputAction.Brake:
			Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.Accelerate, -1f);
			break;
		case EInputAction.Drift:
			Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.Drift, 1f);
			break;
		}
	}

	public void OnRespawn()
	{
		Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.Respawn, 1f);
	}

	public void OnQuit()
	{
		LoadingManager.LoadLevel("MenuRoot");
	}

	public void OnLaunchBonus()
	{
		Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.LaunchBonus, 1f);
	}

	public void OnDrift()
	{
		Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.DriftJump, 1f);
	}

	public void OnPause()
	{
		Singleton<InputManager>.Instance.NullSafeAct(Singleton<InputManager>.Instance.SetAction, EAction.Pause, 1f);
	}
}
