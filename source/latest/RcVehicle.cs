using System;
using UnityEngine;

public class RcVehicle : MonoBehaviour, RcCollisionListener
{
	public enum eVehicleState
	{
		S_IS_LOCKED,
		S_IS_AUTOPILOT,
		S_IS_TELEPORTING,
		S_IS_FALLING,
		S_IS_RUNNING,
		S_IS_DISABLED,
		STATE_COUNT
	}

	public enum RcInputs
	{
		ACTION_RACE_ACCELERATE = 100,
		ACTION_RACE_BREAK,
		ACTION_RACE_LEAN_LEFT,
		ACTION_RACE_LEAN_RIGHT,
		ACTION_RACE_LEAN,
		ACTION_RACE_TOGGLE_VIEW,
		ACTION_RACE_REAR_VIEW,
		ACTION_RACE_RESPAWN,
		ACTION_RACE_BOOST,
		ACTION_RACE_SKID,
		ACTION_RACE_LOOK_BACK,
		ACTION_RACE_HAND_BRAKE,
		ACTION_RACE_GEAR_SHIFT_UP,
		ACTION_RACE_GEAR_SHIFT_DOWN,
		ACTION_RACE_TOGGLE_AUTOPILOT
	}

	public enum ControlType
	{
		Human,
		AI,
		Net
	}

	public enum DriftStyle
	{
		Handbrake,
		Ridge_Racer
	}

	private const float DEFAULT_RESPAWN_DELAY = 1.5f;

	public Action OnTeleported;

	public Action OnStateChanged;

	public Action OnAutoPilotChanged;

	public Action OnRaceStated;

	public Action<RcVehicle> OnRaceEnded;

	public Action OnLapEnded;

	public Action OnLapEndedAfterRace;

	public Action OnCheckpointValidated;

	public Action OnCheckpointsReseted;

	public Action OnFirstLapStarted;

	public Action OnCount;

	public float m_fUsualHandicap;

	public float m_fDriftMinSpeedKph;

	public Vector3 m_vCameraOffset;

	public ControlType m_eControlType;

	public DriftStyle m_eDriftStyle;

	protected Vector3 m_vRespawnPosition;

	protected Vector3 m_vPreviousPosition;

	protected Quaternion m_qRespawnOrientation;

	protected bool m_bRaceEnded;

	protected bool[] m_vehicleState = new bool[6];

	protected float m_fTempHandicap;

	protected int m_iInvulnerableStartTimeMs;

	protected float m_fFallStartTimeMs;

	protected float m_fArcadeDriftFactor;

	protected bool m_bArcadeDriftLock;

	protected float m_fMoveFactor;

	protected float m_fSteeringFactor;

	protected bool m_bNoSteeringInput;

	protected float m_fWheelAccelMSS;

	protected float m_fWheelSpeedMS;

	protected float m_fSteeringAngleRad;

	protected eVehicleState m_ePrevState;

	protected RcVehiclePhysic m_pVehiclePhysic;

	protected int m_iVehicleId;

	protected float m_fDriftFactor;

	protected bool m_bFearsVehicleCollisions;

	protected RcVehicleCarac m_pVehicleCarac;

	protected RcGearBox m_pGearBox;

	protected int m_iPlayerNumber;

	protected RcVehicleRaceStats m_pRaceStats;

	protected Transform m_pTransform;

	protected float m_fRespawnDelay = 1.5f;

	protected AudioListener m_pAudioListener;

	private LayerMask m_IgnoreCollision;

	protected int m_iSecureTeleport;

	public Action OnKilled;

	public Action OnRespawn;

	public bool BArcadeDriftLock
	{
		get
		{
			return m_bArcadeDriftLock;
		}
		set
		{
			m_bArcadeDriftLock = value;
		}
	}

	public AudioListener AudioListener
	{
		get
		{
			return m_pAudioListener;
		}
	}

	public RcVehicleRaceStats RaceStats
	{
		get
		{
			return m_pRaceStats;
		}
		set
		{
			m_pRaceStats = value;
		}
	}

	public RcVehicle()
	{
		m_fDriftMinSpeedKph = 10f;
		m_bArcadeDriftLock = false;
		m_fArcadeDriftFactor = 0f;
		m_vCameraOffset = Vector3.zero;
		m_bFearsVehicleCollisions = true;
		m_iPlayerNumber = -1;
		m_pVehicleCarac = null;
		m_pGearBox = null;
		m_pRaceStats = null;
		m_eControlType = ControlType.AI;
		m_eDriftStyle = DriftStyle.Handbrake;
		m_iVehicleId = 0;
		m_fFallStartTimeMs = 0f;
		m_ePrevState = eVehicleState.STATE_COUNT;
		for (int i = 0; i < 6; i++)
		{
			m_vehicleState[i] = false;
		}
		m_vehicleState[1] = true;
		m_bRaceEnded = false;
		m_fSteeringFactor = 0f;
		m_bNoSteeringInput = false;
		m_qRespawnOrientation = Quaternion.identity;
		m_vRespawnPosition = Vector3.zero;
		m_iInvulnerableStartTimeMs = 0;
		m_fUsualHandicap = 0f;
		m_fTempHandicap = 0f;
		m_fMoveFactor = 0f;
		m_fWheelAccelMSS = 0f;
		m_fWheelSpeedMS = 0f;
		m_fSteeringAngleRad = 0f;
		m_pVehiclePhysic = null;
		m_vehicleState[2] = false;
		m_vPreviousPosition = Vector3.zero;
		m_fDriftFactor = 0f;
		m_iSecureTeleport = 0;
	}

	public virtual void SetArcadeDriftFactor(float factor)
	{
		m_fArcadeDriftFactor = factor;
	}

	public void Turn(float _steering, bool _noInput)
	{
		m_fSteeringFactor = Mathf.Clamp(_steering, -1f, 1f);
		m_bNoSteeringInput = _noInput;
	}

	public bool IsLocked()
	{
		return m_vehicleState[0];
	}

	public bool IsAutoPilot()
	{
		return m_vehicleState[1];
	}

	public Vector3 GetLastFramePos()
	{
		return m_vPreviousPosition;
	}

	public float GetWheelSpeedMS()
	{
		return m_fWheelSpeedMS;
	}

	public float GetWheelAccelMSS()
	{
		return m_fWheelAccelMSS;
	}

	public float GetSpeedKPH()
	{
		return 3.6f * m_fWheelSpeedMS;
	}

	public float GetMotorSpeedMS()
	{
		return GetWheelSpeedMS();
	}

	public float GetTempHandicap()
	{
		return m_fTempHandicap;
	}

	public float GetHandicap()
	{
		return Mathf.Min(0.99f, m_fUsualHandicap + m_fTempHandicap);
	}

	public float GetSteeringFactor()
	{
		return m_fSteeringFactor;
	}

	public float GetSteeringAngle()
	{
		return m_fSteeringAngleRad;
	}

	public float GetMoveFactor()
	{
		return m_fMoveFactor;
	}

	public int GetVehicleId()
	{
		return m_iVehicleId;
	}

	public float GetArcadeDriftFactor()
	{
		return m_fArcadeDriftFactor;
	}

	public bool IsRaceEnded()
	{
		return m_bRaceEnded;
	}

	public float GetUsualHandicap()
	{
		return m_fUsualHandicap;
	}

	public Vector3 GetRespawnPos()
	{
		return m_vRespawnPosition;
	}

	public RcVehiclePhysic GetVehiclePhysic()
	{
		return m_pVehiclePhysic;
	}

	public Vector3 GetPosition()
	{
		return m_pTransform.position;
	}

	public float GetDrift()
	{
		return m_fDriftFactor;
	}

	public bool IsFearingVehicleCollisions()
	{
		return m_bFearsVehicleCollisions;
	}

	public Vector3 GetCameraOffset()
	{
		return m_vCameraOffset;
	}

	public int GetPlayerNumber()
	{
		return m_iPlayerNumber;
	}

	public virtual bool IsBoosting()
	{
		return false;
	}

	public virtual float GetSkidMarksDetail()
	{
		return 1f;
	}

	public ControlType GetControlType()
	{
		return m_eControlType;
	}

	public DriftStyle GetDriftStyle()
	{
		return m_eDriftStyle;
	}

	public RcVehicleCarac GetCarac()
	{
		return m_pVehicleCarac;
	}

	public RcGearBox GetGearBox()
	{
		return m_pGearBox;
	}

	public void SetLocked(bool _locked)
	{
		m_vehicleState[0] = _locked;
	}

	public void SetTeleport(bool _bTeleport)
	{
		m_vehicleState[2] = _bTeleport;
		if (!_bTeleport)
		{
			m_iSecureTeleport = 0;
		}
	}

	public void SetRespawnOrientation(Quaternion _orient)
	{
		m_qRespawnOrientation = _orient;
	}

	public void SetRespawnPos(Vector3 pos)
	{
		m_vRespawnPosition = pos;
	}

	public void SetUsualHandicap(float _handicap)
	{
		m_fUsualHandicap = _handicap;
	}

	public void SetTempHandicap(float _handicap)
	{
		m_fTempHandicap = _handicap;
	}

	public void SetWheelSpeedMS(float _speed)
	{
		m_fWheelSpeedMS = _speed;
	}

	public void SetWheelAccelMSS(float _accel)
	{
		m_fWheelAccelMSS = _accel;
	}

	public void SetVehicleId(int _Num)
	{
		m_iVehicleId = _Num;
	}

	public void SetFearsVehicleCollisions(bool fears)
	{
		m_bFearsVehicleCollisions = fears;
	}

	public void SetDriftStyle(DriftStyle eStyle)
	{
		m_eDriftStyle = eStyle;
	}

	public virtual void Awake()
	{
		m_pVehicleCarac = base.transform.parent.GetComponentInChildren<RcVehicleCarac>();
		m_pGearBox = base.transform.parent.GetComponentInChildren<RcGearBox>();
		m_pVehiclePhysic = base.transform.parent.GetComponentInChildren<RcVehiclePhysic>();
		m_pAudioListener = base.transform.parent.GetComponentInChildren<AudioListener>();
		if ((bool)m_pVehiclePhysic)
		{
			m_pVehiclePhysic.AddCollisionListener(this);
		}
		m_pTransform = base.transform;
	}

	public virtual void Start()
	{
		RegisterInputs();
		SetState(eVehicleState.S_IS_RUNNING, true);
		m_vPreviousPosition = m_pTransform.position;
		int deathMaskValue = ((RcKinematicPhysic)m_pVehiclePhysic).DeathMaskValue;
		m_IgnoreCollision = LayerMask.NameToLayer("Everything") & ~LayerMask.NameToLayer("Vehicle") & ~deathMaskValue;
	}

	public void OnDestroy()
	{
		if ((bool)m_pVehiclePhysic)
		{
			m_pVehiclePhysic.RemoveCollisionListener(this);
		}
	}

	public virtual void Update()
	{
		if (m_vehicleState[5])
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (deltaTime <= 0f)
		{
			return;
		}
		if (m_vehicleState[4])
		{
			ManageRunState();
		}
		if (m_vehicleState[3])
		{
			ManageFallState();
		}
		m_vPreviousPosition = m_pTransform.position;
		if (GetState(eVehicleState.S_IS_TELEPORTING))
		{
			m_iSecureTeleport++;
			if (m_iSecureTeleport > 2)
			{
				m_vehicleState[2] = false;
				m_iSecureTeleport = 0;
			}
		}
	}

	public void Disable()
	{
		if ((bool)m_pVehiclePhysic)
		{
			m_pVehiclePhysic.Enable = false;
		}
		SetState(eVehicleState.S_IS_DISABLED, true);
	}

	public void Enable()
	{
		if ((bool)m_pVehiclePhysic)
		{
			m_pVehiclePhysic.Enable = true;
		}
		SetState(eVehicleState.S_IS_DISABLED, false);
	}

	public void InitPosition()
	{
		Vector3 vector = new Vector3(0f, 0f, 1000f);
		Quaternion identity = Quaternion.identity;
		vector = m_pTransform.position;
		identity = m_pTransform.rotation;
		vector.y += 2f;
		m_pTransform.position = vector;
		Teleport(vector, identity);
		m_vPreviousPosition = vector;
	}

	public void Reset()
	{
		if (m_pVehiclePhysic != null)
		{
			m_pVehiclePhysic.Reset();
		}
		m_fFallStartTimeMs = 0f;
		m_ePrevState = eVehicleState.STATE_COUNT;
		for (int i = 0; i < 1; i++)
		{
			m_vehicleState[i] = false;
		}
		for (int j = 2; j < 6; j++)
		{
			m_vehicleState[j] = false;
		}
		m_bRaceEnded = false;
		m_fSteeringFactor = 0f;
		m_bNoSteeringInput = false;
		m_fWheelSpeedMS = 0f;
		m_fWheelAccelMSS = 0f;
		m_fSteeringAngleRad = 0f;
		m_qRespawnOrientation = Quaternion.identity;
		m_vRespawnPosition = Vector3.zero;
	}

	public virtual float GetMaxSpeed()
	{
		return m_pGearBox.GetMaxSpeed();
	}

	public virtual void ComputeWheelSpeed()
	{
		if (m_vehicleState[0])
		{
			m_fWheelSpeedMS = 0f;
			return;
		}
		float deltaTime = Time.deltaTime;
		bool flag = m_pVehiclePhysic.IsGoingTooFast() || m_pGearBox.IsGoingTooFast();
		float num = GetMaxSpeed();
		VehicleHandling _handlingOut;
		m_pVehicleCarac.ComputeHandling(Mathf.Abs(m_fWheelSpeedMS * (1f - GetHandicap())), out _handlingOut);
		if (m_fArcadeDriftFactor == 0f)
		{
			num = RcUtils.LinearInterpolation(0f, num, 1f, num * (1f - _handlingOut.steeringTopSpeedMalus), Mathf.Abs(m_fSteeringFactor), true);
		}
		float num2 = ((!m_pVehiclePhysic.IsGoingTooFast()) ? _handlingOut.brakingMSS : _handlingOut.toofastBrakingMSS);
		if (flag)
		{
			if (m_fWheelSpeedMS >= 0f)
			{
				m_fWheelSpeedMS -= num2 * deltaTime;
			}
			else
			{
				m_fWheelSpeedMS += num2 * deltaTime;
			}
		}
		else if (m_fMoveFactor > 0f)
		{
			if (m_fWheelSpeedMS >= 0f)
			{
				if (m_fWheelSpeedMS < num)
				{
					m_fWheelSpeedMS += m_pGearBox.ComputeAcceleration(Mathf.Abs(m_fWheelSpeedMS)) * deltaTime * m_fMoveFactor;
				}
			}
			else
			{
				m_fWheelSpeedMS += num2 * deltaTime * m_fMoveFactor;
			}
		}
		else if (m_fMoveFactor < 0f)
		{
			if (m_fWheelSpeedMS >= 0f)
			{
				m_fWheelSpeedMS += num2 * deltaTime * m_fMoveFactor;
			}
			else
			{
				m_fWheelSpeedMS += m_pGearBox.ComputeAcceleration(Mathf.Abs(m_fWheelSpeedMS)) * deltaTime * m_fMoveFactor;
			}
		}
		else if (m_fWheelSpeedMS >= 0f)
		{
			m_fWheelSpeedMS -= _handlingOut.decelerationMSS * deltaTime;
			if (m_fWheelSpeedMS < 0f)
			{
				m_fWheelSpeedMS = 0f;
			}
		}
		else
		{
			m_fWheelSpeedMS += _handlingOut.decelerationMSS * deltaTime;
			if (m_fWheelSpeedMS > 0f)
			{
				m_fWheelSpeedMS = 0f;
			}
		}
		if (m_fWheelSpeedMS > num)
		{
			m_fWheelSpeedMS -= 5f * _handlingOut.decelerationMSS * deltaTime;
			if (m_fWheelSpeedMS < num)
			{
				m_fWheelSpeedMS = num;
			}
		}
		else if (m_fWheelSpeedMS < GetMinSpeed())
		{
			m_fWheelSpeedMS = GetMinSpeed();
		}
	}

	public float SteeringRadiusToAngle(float _radius)
	{
		return Mathf.Atan(m_pVehiclePhysic.GetFrontToRearWheelLength() / _radius);
	}

	public void ComputeArcadeDriftSteeringAngle()
	{
		VehicleHandling _handlingOut;
		m_pVehicleCarac.ComputeHandling(Mathf.Abs(m_fWheelSpeedMS * (1f - GetHandicap())), out _handlingOut);
		float driftTurningRadius = _handlingOut.driftTurningRadius;
		float num = SteeringRadiusToAngle(driftTurningRadius);
		float num2 = 0f;
		float y = SteeringRadiusToAngle(_handlingOut.counterSteeringTurningRadius);
		float y2 = SteeringRadiusToAngle(_handlingOut.driftNoInputTurningRadius);
		float num3 = 0f;
		if (IsOnGround())
		{
			num3 = ((!((0f - m_fSteeringFactor) * m_fArcadeDriftFactor > 0f)) ? RcUtils.LinearInterpolation(0f, y2, 1f, y, Mathf.Abs(m_fSteeringFactor), true) : RcUtils.LinearInterpolation(0f, y2, 1f, num, Mathf.Abs(m_fSteeringFactor), true));
			if (m_fArcadeDriftFactor > 0f)
			{
				num3 = 0f - num3;
			}
		}
		num2 = (m_bNoSteeringInput ? (num / _handlingOut.driftResetSteeringNoInput) : ((!(m_fSteeringAngleRad * num3 < 0f)) ? (num / _handlingOut.driftTimeToMaxSteering) : ((_handlingOut.resetSteeringOppositeInput != 0f) ? (num / _handlingOut.resetSteeringOppositeInput) : 1000f)));
		float num4 = num3 - m_fSteeringAngleRad;
		if (num4 > 0f)
		{
			m_fSteeringAngleRad += num2 * Time.deltaTime;
			if (m_fSteeringAngleRad > num3)
			{
				m_fSteeringAngleRad = num3;
			}
		}
		else if (num4 < 0f)
		{
			m_fSteeringAngleRad -= num2 * Time.deltaTime;
			if (m_fSteeringAngleRad < num3)
			{
				m_fSteeringAngleRad = num3;
			}
		}
	}

	public void ComputeSteeringAngle()
	{
		if (m_vehicleState[0])
		{
			m_fSteeringAngleRad = 0f;
			return;
		}
		if (m_fArcadeDriftFactor != 0f)
		{
			ComputeArcadeDriftSteeringAngle();
			return;
		}
		float deltaTime = Time.deltaTime;
		VehicleHandling _handlingOut;
		m_pVehicleCarac.ComputeHandling(Mathf.Abs(m_fWheelSpeedMS * (1f - GetHandicap())), out _handlingOut);
		float minTurningRadius = _handlingOut.minTurningRadius;
		float num = SteeringRadiusToAngle(minTurningRadius);
		float num2 = 0f;
		Vector3 linearVelocity = m_pVehiclePhysic.GetLinearVelocity();
		Vector3 rhs = m_pTransform.rotation * Vector3.forward;
		Vector3 vector = m_pTransform.rotation * Vector3.up;
		float num3 = Vector3.Dot(vector, linearVelocity);
		Vector3 lhs = linearVelocity - num3 * vector;
		float driftRatio = GetDriftRatio();
		if (Mathf.Abs(driftRatio) > 0f && driftRatio * m_fSteeringFactor > 0f)
		{
			lhs.Normalize();
			float value = Vector3.Dot(lhs, rhs);
			value = Mathf.Clamp(value, -1f, 1f);
			float value2 = Mathf.Acos(value);
			value2 = Mathf.Clamp(value2, 0f, (float)Math.PI / 4f);
			if (value2 > num)
			{
				num = value2;
			}
		}
		float num4 = 0f;
		if (IsOnGround())
		{
			num4 = m_fSteeringFactor * num;
		}
		num2 = (m_bNoSteeringInput ? (num / _handlingOut.resetSteeringNoInput) : ((!(m_fSteeringAngleRad * num4 < 0f)) ? (num / _handlingOut.timeToMaxSteering) : ((_handlingOut.resetSteeringOppositeInput != 0f) ? (num / _handlingOut.resetSteeringOppositeInput) : 1000f)));
		float num5 = num4 - m_fSteeringAngleRad;
		if (num5 > 0f)
		{
			m_fSteeringAngleRad += num2 * deltaTime;
			if (m_fSteeringAngleRad > num4)
			{
				m_fSteeringAngleRad = num4;
			}
		}
		else if (num5 < 0f)
		{
			m_fSteeringAngleRad -= num2 * deltaTime;
			if (m_fSteeringAngleRad < num4)
			{
				m_fSteeringAngleRad = num4;
			}
		}
	}

	public virtual void Respawn()
	{
		if (OnRespawn != null)
		{
			OnRespawn();
		}
		int vehicleId = GetVehicleId();
		int num = vehicleId % 3;
		int num2 = vehicleId / 3;
		if (num == 2)
		{
			num = -1;
		}
		Vector3 vector = m_qRespawnOrientation * Vector3.up * 2f;
		Vector3 vector2 = m_qRespawnOrientation * Vector3.left * ((float)num * 1.5f);
		Vector3 vector3 = m_qRespawnOrientation * Vector3.forward * -2f * num2;
		Vector3 vector4 = m_vRespawnPosition + vector2 + vector + vector3;
		Quaternion quaternion = m_qRespawnOrientation;
		if (m_qRespawnOrientation == Quaternion.identity)
		{
			quaternion = m_pTransform.rotation;
		}
		Teleport(vector4, quaternion);
		m_vPreviousPosition = vector4;
		m_fWheelAccelMSS = 0f;
		m_fWheelSpeedMS = 0f;
		if (m_eControlType == ControlType.Human)
		{
			Camera.main.GetComponent<CamStateRespawn>().Setup(vector4, quaternion);
			Camera.main.GetComponent<CameraBase>().SwitchCamera(ECamState.Respawn, ECamState.TransCut);
		}
	}

	public void Accelerate()
	{
		Accelerate(1f);
	}

	public void Accelerate(float _accelerationPrc)
	{
		m_fMoveFactor = _accelerationPrc;
	}

	public void Decelerate()
	{
		m_fMoveFactor = 0f;
	}

	public void Brake()
	{
		Brake(1f);
	}

	public void Brake(float _brakingPrc)
	{
		m_fMoveFactor = 0f - _brakingPrc;
	}

	public void ForceRespawn()
	{
		if (!GetState(eVehicleState.S_IS_FALLING) && !GetState(eVehicleState.S_IS_LOCKED))
		{
			m_ePrevState = eVehicleState.S_IS_FALLING;
			SetState(eVehicleState.S_IS_RUNNING, true);
		}
	}

	public void Teleport(Vector3 _TeleportPos, Quaternion _TeleportOrientation)
	{
		Teleport(_TeleportPos, _TeleportOrientation, Vector3.zero);
	}

	public void Teleport(Vector3 _TeleportPos, Quaternion _TeleportOrientation, Vector3 linearVelocity)
	{
		SetTeleport(true);
		m_fSteeringAngleRad = 0f;
		if ((bool)m_pVehiclePhysic)
		{
			m_pVehiclePhysic.Teleport(_TeleportPos, _TeleportOrientation, linearVelocity);
		}
		if (OnTeleported != null)
		{
			OnTeleported();
		}
	}

	public void SetAutoPilot(bool _bIsAutoPilot)
	{
		m_vehicleState[1] = _bIsAutoPilot;
		if (OnAutoPilotChanged != null)
		{
			OnAutoPilotChanged();
		}
	}

	public void SetState(eVehicleState _State, bool _Active)
	{
		m_vehicleState[(int)_State] = _Active;
		if (OnStateChanged != null)
		{
			OnStateChanged();
		}
		switch (_State)
		{
		case eVehicleState.S_IS_RUNNING:
			if (_Active)
			{
				OnStartRunState();
				if (m_ePrevState < eVehicleState.STATE_COUNT)
				{
					SetState(m_ePrevState, false);
				}
				m_ePrevState = _State;
			}
			else
			{
				OnEndRunState();
			}
			break;
		case eVehicleState.S_IS_FALLING:
			if (_Active)
			{
				OnStartFallState();
				if (m_ePrevState < eVehicleState.STATE_COUNT)
				{
					SetState(m_ePrevState, false);
				}
				m_ePrevState = _State;
			}
			else
			{
				OnEndFallState();
			}
			break;
		case eVehicleState.S_IS_DISABLED:
			break;
		}
	}

	public bool GetState(eVehicleState _State)
	{
		return m_vehicleState[(int)_State];
	}

	public void OnStartFallState()
	{
		m_fFallStartTimeMs = m_fRespawnDelay;
	}

	public bool IsFallFinished()
	{
		return m_fFallStartTimeMs == 0f;
	}

	public void ManageFallState()
	{
		m_fFallStartTimeMs -= Time.deltaTime;
		m_fFallStartTimeMs = Mathf.Clamp(m_fFallStartTimeMs, 0f, m_fRespawnDelay);
		if (IsFallFinished())
		{
			EndOfRespawn();
		}
	}

	public void EndOfRespawn()
	{
		SetState(eVehicleState.S_IS_RUNNING, true);
		if ((bool)m_pVehiclePhysic)
		{
			m_pVehiclePhysic.ResetTouchedDeath();
		}
	}

	public void OnEndFallState()
	{
		m_fFallStartTimeMs = 0f;
		Respawn();
	}

	public void OnStartRunState()
	{
	}

	public virtual void ManageRunState()
	{
		float num = Time.deltaTime * 1000f;
		float num2 = num * 0.001f;
		float fWheelSpeedMS = m_fWheelSpeedMS;
		ComputeWheelSpeed();
		m_fWheelAccelMSS = (m_fWheelSpeedMS - fWheelSpeedMS) / num2;
		ComputeSteeringAngle();
	}

	public void OnEndRunState()
	{
	}

	public int GetGroundSurface()
	{
		return m_pVehiclePhysic.GetGroundSurface();
	}

	public Vector3 ForecastPosition(float dtSec)
	{
		if ((bool)m_pVehiclePhysic)
		{
			return m_pVehiclePhysic.ForecastPosition(dtSec);
		}
		return m_pTransform.position;
	}

	public float GetRealSpeedMs()
	{
		return (!(m_pVehiclePhysic != null)) ? 0f : m_pVehiclePhysic.GetLinearVelocity().magnitude;
	}

	public float GetMinSpeed()
	{
		return m_pGearBox.GetBackwardMaxSpeed();
	}

	public Vector3 GetCameraTargetPoint()
	{
		Vector3 zero = Vector3.zero;
		int nbWheels = m_pVehiclePhysic.GetNbWheels();
		RcPhysicWheel[] wheels = m_pVehiclePhysic.GetWheels();
		for (int i = 0; i < nbWheels; i++)
		{
			zero += wheels[i].GetWorldPos();
		}
		return zero / nbWheels;
	}

	public int GetNbWheels()
	{
		if ((bool)m_pVehiclePhysic)
		{
			return m_pVehiclePhysic.GetNbWheels();
		}
		return 0;
	}

	public GroundCharac GetGroundCharac(int _wheelIndex)
	{
		RcPhysicWheel[] wheels = m_pVehiclePhysic.GetWheels();
		return wheels[_wheelIndex].OGroundCharac;
	}

	public bool IsOnGround(int _wheelIndex)
	{
		RcPhysicWheel[] wheels = m_pVehiclePhysic.GetWheels();
		return wheels[_wheelIndex].BOnGround;
	}

	public bool IsLeftWheel(int _wheelIndex)
	{
		RcPhysicWheel[] wheels = m_pVehiclePhysic.GetWheels();
		return wheels[_wheelIndex].ESide == RcPhysicWheel.WheelSide.Left;
	}

	public bool IsOnGround()
	{
		for (int i = 0; i < GetNbWheels(); i++)
		{
			if (IsOnGround(i))
			{
				return true;
			}
		}
		return false;
	}

	public Vector3 GetCameraAt()
	{
		return GetVehiclePhysic().GetVehicleBodyTransform().rotation * Vector3.forward;
	}

	public Vector3 GetCameraUp()
	{
		return GetVehiclePhysic().GetVehicleBodyTransform().rotation * Vector3.up;
	}

	public float GetDriftRatio()
	{
		if ((bool)m_pVehiclePhysic && IsOnGround())
		{
			return m_pVehiclePhysic.GetDriftRatio();
		}
		return 0f;
	}

	public void SetDrift(float _brake)
	{
		m_fDriftFactor = _brake;
	}

	public void AddCollisionListener(RcCollisionListener pListener)
	{
		if ((bool)GetVehiclePhysic())
		{
			GetVehiclePhysic().AddCollisionListener(pListener);
		}
	}

	public void RemoveCollisionListener(RcCollisionListener pListener)
	{
		if ((bool)GetVehiclePhysic())
		{
			GetVehiclePhysic().RemoveCollisionListener(pListener);
		}
	}

	public void RegisterInputs()
	{
	}

	public Quaternion GetOrientation()
	{
		return m_pTransform.rotation;
	}

	public void SetControlType(ControlType eType)
	{
		if (m_eControlType != eType)
		{
			if (eType == ControlType.Human)
			{
				SetAutoPilot(false);
			}
			m_eControlType = eType;
		}
		if ((bool)m_pAudioListener)
		{
			m_pAudioListener.enabled = m_eControlType == ControlType.Human;
		}
	}

	public float GetTimeToStop()
	{
		if ((bool)m_pVehicleCarac)
		{
			return m_pVehicleCarac.GetTimeToStop(GetWheelSpeedMS());
		}
		return 0f;
	}

	public void SetRaceEnded(bool _raceEnded)
	{
		m_bRaceEnded = _raceEnded;
	}

	public void Kill(float respawnDelay)
	{
		m_fRespawnDelay = respawnDelay;
		SetState(eVehicleState.S_IS_FALLING, true);
		if (OnKilled != null)
		{
			OnKilled();
		}
	}

	public void Kill()
	{
		Kill(1.5f);
	}

	public void OnCollision(CollisionData collisionInfo)
	{
		if (collisionInfo.other == null || collisionInfo.other.GetComponent<RcVehicle>() != null)
		{
			m_bArcadeDriftLock = true;
		}
	}
}
