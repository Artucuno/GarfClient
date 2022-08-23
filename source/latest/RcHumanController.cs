using System;
using UnityEngine;

public class RcHumanController : RcController
{
	protected int m_iInputIndex;

	protected bool m_bMirrorMode;

	protected bool m_bPreviousOnGround;

	private RcVirtualController m_pAutopilotController;

	protected float m_fTakeOffSteer;

	protected float m_fPrevDriftInput;

	protected Gyroscope m_pGyroscope;

	public float DegreeThresholdGyroMin = 15f;

	public float DegreeThresholdGyroMax = 35f;

	private float RadianThreshold;

	private float m_fLastGyroValue;

	public float ProcessNoise = 0.1f;

	public float SensorNoise = 0.05f;

	public float EstimatedError = 0.1f;

	public float KalmanFilterGain = 0.5f;

	private int m_iLogBrake;

	private bool m_bLogBrake;

	public bool toggleAutoPilot = false;

	public int LogBrake
	{
		get
		{
			return m_iLogBrake;
		}
	}

	public RcHumanController()
	{
		m_pAutopilotController = null;
		m_bPreviousOnGround = false;
		m_fTakeOffSteer = 0f;
		m_fPrevDriftInput = 0f;
		m_pGyroscope = null;
	}

	public override float GetSteer()
	{
		return 0f - Steer();
	}

	public override float GetSpeedBehaviour()
	{
		return Accelerate();
	}

	public void SetMirrorMode(bool _mirror)
	{
		m_bMirrorMode = _mirror;
	}

	public RcVirtualController GetAutopilot()
	{
		return m_pAutopilotController;
	}

	public virtual void Start()
	{
		m_pAutopilotController = base.transform.parent.GetComponentInChildren<RcVirtualController>();
		if (m_pAutopilotController != null)
		{
			m_pAutopilotController.SetDrivingEnabled(m_pVehicle.GetControlType() == RcVehicle.ControlType.AI);
		}
		RcVehicle pVehicle = m_pVehicle;
		pVehicle.OnRaceEnded = (Action<RcVehicle>)Delegate.Combine(pVehicle.OnRaceEnded, new Action<RcVehicle>(RaceEnd));
		if (SystemInfo.supportsGyroscope)
		{
			m_pGyroscope = Input.gyro;
			m_pGyroscope.enabled = true;
			RadianThreshold = (float)Math.PI * DegreeThresholdGyroMax / 180f;
			m_fLastGyroValue = 0f;
		}
	}

	public virtual void Stop()
	{
		RcVehicle pVehicle = m_pVehicle;
		pVehicle.OnRaceEnded = (Action<RcVehicle>)Delegate.Remove(pVehicle.OnRaceEnded, new Action<RcVehicle>(RaceEnd));
	}

	public override void Reset()
	{
		base.Reset();
		if ((bool)m_pAutopilotController)
		{
			m_pAutopilotController.Reset();
		}
	}

	public virtual void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			if (GetVehicle().IsAutoPilot())
			{
				SetAutopilotEnabled(false);
			}
			else
			{
				SetAutopilotEnabled(true);
			}
		}

		if (GetVehicle().GetControlType() != 0 || GetVehicle().IsAutoPilot())
		{
			return;
		}
		if (m_bDrivingEnabled)
		{
			float num = 0f;
			float num2 = 0f;
			if (GetVehicle().IsOnGround())
			{
				num = Accelerate();
			}
			num2 = Steer();
			if (!GetVehicle().IsOnGround() && m_bPreviousOnGround)
			{
				m_fTakeOffSteer = num2;
			}
			num2 = ((!(num2 > 0f)) ? ((0f - num2) * num2) : (num2 * num2));
			if (num > 0.1f)
			{
				GetVehicle().Accelerate(num);
				if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
				{
					(Singleton<GameManager>.Instance.GameMode as TutorialGameMode).Accelerate();
				}
				if (LogManager.Instance != null)
				{
					m_bLogBrake = false;
				}
			}
			else if (num < -0.1f)
			{
				GetVehicle().Brake(0f - num);
				if (LogManager.Instance != null && !m_bLogBrake)
				{
					m_iLogBrake++;
					m_bLogBrake = true;
				}
				if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
				{
					(Singleton<GameManager>.Instance.GameMode as TutorialGameMode).Brake();
				}
			}
			else
			{
				GetVehicle().Decelerate();
			}
			Turn(num2);
			float action = Singleton<InputManager>.Instance.GetAction(EAction.Drift);
			if (action > 0f && m_fPrevDriftInput == 0f)
			{
				if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
				{
					(Singleton<GameManager>.Instance.GameMode as TutorialGameMode).DriftAttempt();
				}
				GetVehicle().BArcadeDriftLock = false;
			}
			m_fPrevDriftInput = action;
			if (action == 1f && num >= 0f && !GetVehicle().BArcadeDriftLock && GetVehicle().GetWheelSpeedMS() * 3.6f > GetVehicle().m_fDriftMinSpeedKph)
			{
				if (GetVehicle().IsOnGround() && GetVehicle().GetArcadeDriftFactor() == 0f && !m_bPreviousOnGround)
				{
					if (num2 > 0f)
					{
						GetVehicle().SetArcadeDriftFactor(1f);
					}
					else if (num2 < 0f)
					{
						GetVehicle().SetArcadeDriftFactor(-1f);
					}
					else if (m_fTakeOffSteer > 0f)
					{
						GetVehicle().SetArcadeDriftFactor(1f);
					}
					else if (m_fTakeOffSteer < 0f)
					{
						GetVehicle().SetArcadeDriftFactor(-1f);
					}
					else
					{
						GetVehicle().SetArcadeDriftFactor(0f);
					}
				}
			}
			else
			{
				GetVehicle().SetArcadeDriftFactor(0f);
			}
			if (Singleton<InputManager>.Instance.GetAction(EAction.Respawn) == 1f)
			{
				GetVehicle().ForceRespawn();
			}
		}
		m_bPreviousOnGround = GetVehicle().IsOnGround();
	}

	public virtual void Turn(float _Steer)
	{
		GetVehicle().Turn(0f - _Steer, 0f == _Steer);
	}

	public void SetAutopilotEnabled(bool _enabled)
	{
		SetDrivingEnabled(!_enabled);
		if ((bool)m_pAutopilotController)
		{
			m_pAutopilotController.SetDrivingEnabled(_enabled);
		}
	}

	public override void SetVehicle(RcVehicle _pVehicle)
	{
		base.SetVehicle(_pVehicle);
		if ((bool)m_pAutopilotController)
		{
			m_pAutopilotController.SetVehicle(_pVehicle);
		}
	}

	public float Accelerate()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			float action = Singleton<InputManager>.Instance.GetAction(EAction.Accelerate);
			if (action != -1f)
			{
				return 1f;
			}
			return action;
		}
		return Singleton<InputManager>.Instance.GetAction(EAction.Accelerate);
	}

	public float Steer()
	{
		float num = 0f;
		if (m_pGyroscope != null && Singleton<GameOptionManager>.Instance.GetInputType() == E_InputType.Gyroscopic)
		{
			float gyroSensibility = Singleton<GameOptionManager>.Instance.GetGyroSensibility();
			float num2 = DegreeThresholdGyroMin + (1f - gyroSensibility) * (DegreeThresholdGyroMax - DegreeThresholdGyroMin);
			RadianThreshold = (float)Math.PI * num2 / 180f;
			Quaternion attitude = m_pGyroscope.attitude;
			float num3 = Mathf.Asin(2f * (attitude.x * attitude.z - attitude.w * attitude.y));
			if (m_fLastGyroValue == 0f)
			{
				m_fLastGyroValue = num3;
			}
			float fLastGyroValue = m_fLastGyroValue;
			EstimatedError += ProcessNoise;
			KalmanFilterGain = EstimatedError / (EstimatedError + SensorNoise);
			fLastGyroValue += KalmanFilterGain * (num3 - fLastGyroValue);
			EstimatedError = (1f - KalmanFilterGain) * EstimatedError;
			m_fLastGyroValue = fLastGyroValue;
			fLastGyroValue = Mathf.Clamp(fLastGyroValue, 0f - RadianThreshold, RadianThreshold) / RadianThreshold;
			num = 0f - fLastGyroValue;
		}
		else
		{
			num = Singleton<InputManager>.Instance.GetAction(EAction.Steer);
		}
		if (m_bMirrorMode)
		{
			return 0f - num;
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
		{
			(Singleton<GameManager>.Instance.GameMode as TutorialGameMode).Direction(num);
		}
		return num;
	}

	public void SetInputIndex(int _inputIndex)
	{
		m_iInputIndex = _inputIndex;
	}

	public void RaceEnd(RcVehicle pVehicle)
	{
		if (GetVehicle().GetControlType() == RcVehicle.ControlType.Human)
		{
			GetVehicle().SetArcadeDriftFactor(0f);
			SetAutopilotEnabled(true);
		}
	}
}
