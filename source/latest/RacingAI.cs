using System;
using UnityEngine;

[Serializable]
public class RacingAI
{
	public const int COLLISION_HISTORY_SIZE = 5;

	public const float RAIL_TRANSITION_SPEED_MS = 10f;

	private const float SAMPLING = 200f;

	private RcVirtualController _virtualController;

	protected Vector3 _previousRailPoint = Vector3.zero;

	private RcFastPath _idealPath = new RcFastPath(220);

	protected float _drivePriority;

	protected PathPosition _pathPosition = PathPosition.UNDEFINED_POSITION;

	protected bool _catchMode;

	protected bool _railTransition;

	public PidController _pidController;

	private E_AIMode _mode;

	protected E_AIMode _previousMode;

	protected float _jamTime;

	protected float _antiJamTime;

	protected float _bigJamTime;

	protected float _antiBigJamTime = 5f;

	protected float _reverseTime;

	protected float _antiReverseTime;

	protected Vector3 _capturedPos = Vector3.zero;

	protected float _captureTime;

	protected float _jamSpeed = 3f;

	protected float _collCheck = 0.5f;

	protected float _bigJamSpeed = 3f;

	protected float _bigJameCheck = 3f;

	protected float _antiJamStart = 0.5f;

	protected float _antiBigJamStart = 15f;

	protected float _antiReverseStart = 1.5f;

	protected float _antiJamDelay = 1.5f;

	protected float _antiJamReload = 2.2f;

	protected float _antiBigJamReload;

	protected float _reverseDot = 85f;

	protected float _stopAntiReverseDot = 75f;

	protected float _antiReverseDelay = 1.5f;

	protected float _antiReverseReload = 2.2f;

	protected float _startModeDelay = 2f;

	protected float _startModeTime;

	protected float _lastPathRecomputeTime;

	protected float _closeDistance;

	protected float _farDistance;

	protected float _closeDynamic;

	protected float _closeStatic;

	protected float _farDynamic;

	protected float _farStatic;

	public float _forecastTime = 0.6f;

	public float _minForecastDist = 5f;

	protected bool _debugInfoOn;

	protected bool _forcePathRecomputing;

	protected bool _mobilePathTarget;

	protected E_AILevel _level;

	public E_AILevel Level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
		}
	}

	public PidController PidController
	{
		get
		{
			return _pidController;
		}
	}

	public PathPosition PathPosition
	{
		get
		{
			return _pathPosition;
		}
		set
		{
			_pathPosition = value;
		}
	}

	public int CurrentPathIndex
	{
		get
		{
			return _pathPosition.index;
		}
	}

	public float ClosestSegmentRatio
	{
		get
		{
			return _pathPosition.ratio;
		}
	}

	public E_AIMode Mode
	{
		get
		{
			return _mode;
		}
		set
		{
			_mode = value;
		}
	}

	public E_AIMode PreviousMode
	{
		get
		{
			return _previousMode;
		}
	}

	public bool IsRailTransition
	{
		get
		{
			return _railTransition;
		}
		set
		{
			_railTransition = value;
		}
	}

	public float DrivePriority
	{
		get
		{
			return _drivePriority;
		}
		set
		{
			_drivePriority = value;
		}
	}

	public float AntiJamRemainingTime
	{
		get
		{
			return _antiJamTime;
		}
	}

	public float AntiReverseRemainingTime
	{
		get
		{
			return _antiReverseTime;
		}
	}

	public float AntiBigJamRemainingTime
	{
		get
		{
			return _antiBigJamTime;
		}
	}

	public float StartModeTime
	{
		get
		{
			return _startModeTime;
		}
	}

	public float StartModeDelay
	{
		get
		{
			return _startModeDelay;
		}
	}

	public RcVehicle Vehicle
	{
		get
		{
			return _virtualController.GetVehicle();
		}
	}

	public RcFastPath IdealPath
	{
		set
		{
			_idealPath = value;
		}
	}

	public RcVirtualController VirtualController
	{
		get
		{
			return _virtualController;
		}
		set
		{
			_virtualController = value;
		}
	}

	public float Steer
	{
		get
		{
			return _virtualController.GetSteer();
		}
	}

	public float SpeedBehaviour
	{
		get
		{
			return _virtualController.GetSpeedBehaviour();
		}
	}

	public RcFastPath CurrentPath
	{
		get
		{
			return _idealPath;
		}
	}

	~RacingAI()
	{
		Term();
	}

	public virtual void Update()
	{
		_lastPathRecomputeTime += Time.deltaTime;
		if (!Vehicle.IsLocked())
		{
			_captureTime += Time.deltaTime;
			if (_captureTime > _bigJameCheck)
			{
				Vector3 vector = Vehicle.GetPosition() - _capturedPos;
				float num = _bigJamSpeed * _bigJameCheck * _bigJamSpeed * _bigJameCheck;
				if (vector.sqrMagnitude < num && Vehicle.GetMaxSpeed() > _bigJamSpeed)
				{
					_bigJamTime += _bigJameCheck;
				}
				else
				{
					_bigJamTime = 0f;
				}
				_captureTime = 0f;
				_capturedPos = Vehicle.GetPosition();
			}
			if (_mode == E_AIMode.DRIVEN_MODE)
			{
				bool flag = true;
				if (_jamTime < 0f || (flag && Mathf.Abs(Vehicle.GetRealSpeedMs()) < _jamSpeed && Vehicle.GetMaxSpeed() > _jamSpeed))
				{
					_jamTime += Time.deltaTime;
				}
				else
				{
					_jamTime = 0f;
				}
			}
			if (_pathPosition.index != -1 && _idealPath != null)
			{
				Vector3 targetPoint = GetTargetPoint();
				PathPosition pathPosition = _pathPosition;
				_idealPath.UpdatePathPosition(ref pathPosition, targetPoint, 3, 0, false, _idealPath.IsLooping());
				float num2 = Vector3.Dot(_idealPath.GetSegment(pathPosition.index).normalized, Vehicle.GetCameraAt());
				num2 *= 57.29578f;
				if (num2 < _reverseDot * ((float)Math.PI / 180f))
				{
					_reverseTime += Time.deltaTime;
				}
				else
				{
					_reverseTime = 0f;
				}
				if (num2 > _stopAntiReverseDot * ((float)Math.PI / 180f))
				{
					_antiReverseTime = 0f;
				}
			}
		}
		if (_bigJamTime >= _antiBigJamStart)
		{
			StartAntiBigJam();
		}
		if (_jamTime > _antiJamStart)
		{
			StartAntiJam();
		}
		if (_reverseTime >= _antiReverseStart)
		{
			StartAntiReverse();
		}
		if (_antiJamTime > 0f)
		{
			_antiJamTime -= Time.deltaTime;
		}
		if (_antiBigJamTime > 0f)
		{
			_antiBigJamTime -= Time.deltaTime;
		}
		if (_antiReverseTime > 0f)
		{
			_antiReverseTime -= Time.deltaTime;
		}
		if (_mode == E_AIMode.START_MODE && _startModeTime > 0f)
		{
			_startModeTime -= Time.deltaTime;
		}
	}

	public virtual void Init()
	{
		_forcePathRecomputing = true;
		_pidController = new PidController();
		RcVehicle vehicle = Vehicle;
		vehicle.OnAutoPilotChanged = (Action)Delegate.Combine(vehicle.OnAutoPilotChanged, new Action(EventRaised));
		RcVehicle vehicle2 = Vehicle;
		vehicle2.OnTeleported = (Action)Delegate.Combine(vehicle2.OnTeleported, new Action(Reset));
	}

	private void EventRaised()
	{
		_pathPosition = PathPosition.UNDEFINED_POSITION;
		_railTransition = false;
		_catchMode = false;
		_mode = E_AIMode.IDLE_MODE;
		_previousMode = E_AIMode.IDLE_MODE;
		_previousRailPoint = Vector3.zero;
		_drivePriority = 0f;
		_jamTime = 0f;
		_antiJamTime = 0f;
		_forcePathRecomputing = true;
	}

	public bool MustRecomputePath()
	{
		return true;
	}

	public void StartStartMode()
	{
		_startModeTime = _startModeDelay;
	}

	public virtual void StartAntiJam()
	{
		_antiJamTime = _antiJamDelay;
		_jamTime = 0f - _antiJamReload;
	}

	public void StartAntiBigJam()
	{
		Vehicle.ForceRespawn();
		_bigJamTime = 0f - _antiBigJamReload;
	}

	public void StartAntiReverse()
	{
		_antiReverseTime = _antiReverseDelay;
		_reverseTime = 0f - _antiReverseReload;
	}

	public void SetForcePathRecomputing(bool pForce)
	{
		_forcePathRecomputing = pForce;
	}

	public void ForcePathRecomputing()
	{
		_forcePathRecomputing = true;
	}

	public void SetDriveParameters(float pWantedSteer, float pSpeedBehaviour)
	{
		_virtualController.setDriveParameters(pWantedSteer, pSpeedBehaviour);
	}

	public void Reset()
	{
		_pathPosition = PathPosition.UNDEFINED_POSITION;
		_railTransition = false;
		_catchMode = false;
		_forcePathRecomputing = true;
		_mode = E_AIMode.IDLE_MODE;
		_previousMode = E_AIMode.IDLE_MODE;
		_previousRailPoint = Vector3.zero;
		_drivePriority = 0f;
		_jamTime = 0f;
		_reverseTime = 0f;
		_antiJamTime = 0f;
	}

	public void Term()
	{
		RcVehicle vehicle = Vehicle;
		vehicle.OnStateChanged = (Action)Delegate.Remove(vehicle.OnStateChanged, new Action(EventRaised));
		RcVehicle vehicle2 = Vehicle;
		vehicle2.OnAutoPilotChanged = (Action)Delegate.Remove(vehicle2.OnAutoPilotChanged, new Action(EventRaised));
	}

	public Vector3 GetTargetPoint()
	{
		Vector3 position = Vector3.zero;
		float dist = Vehicle.GetWheelSpeedMS() * _forecastTime;
		if (dist < _minForecastDist)
		{
			dist = _minForecastDist;
		}
		float closestSegmentRatio = ClosestSegmentRatio;
		if (closestSegmentRatio > 0f)
		{
			float segmentLength = CurrentPath.GetSegmentLength(CurrentPathIndex);
			dist += segmentLength * closestSegmentRatio;
		}
		CurrentPath.GetPosAtDist(CurrentPathIndex, ref dist, out position);
		return position;
	}

	public virtual void DebugDrawGizmos()
	{
	}

	public virtual void DebugDraw()
	{
		GUI.contentColor = new Color(200f, 200f, 200f);
		GUI.Label(new Rect(20f, 75f, 200f, 20f), "Mode : " + _mode);
		GUI.Label(new Rect(20f, 100f, 200f, 20f), "Target Point : " + GetTargetPoint().ToString());
		GUI.Label(new Rect(20f, 125f, 200f, 20f), "Path Index : " + CurrentPathIndex);
		GUI.Label(new Rect(20f, 150f, 200f, 20f), "Seg Ratio : " + ClosestSegmentRatio);
		GUI.Label(new Rect(20f, 175f, 200f, 20f), "Pid Output : " + _pidController.GetOutput());
		GUI.Label(new Rect(20f, 200f, 200f, 20f), "Steer : " + _virtualController.GetSteer());
		GUI.Label(new Rect(20f, 225f, 200f, 20f), "Speed : " + _virtualController.GetSpeedBehaviour());
		if (GUI.Button(new Rect(20f, 250f, 40f, 20f), "Respawn"))
		{
			Vehicle.Respawn();
		}
	}
}
