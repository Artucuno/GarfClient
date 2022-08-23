using System;
using UnityEngine;

[Serializable]
public class RacingAIManager
{
	public const int MAX_NB_AI = 6;

	public const float GRAPH_SCALE_1 = 1f;

	public const float GRAPH_SCALE_2 = 0.15f;

	public const float GRAPH_SCALE_3 = 0.01f;

	public const float GRAPH_V_STEP = -0.1f;

	public const float GRAPH_INIT_ORD = 3.5f;

	public const int NB_GRAPH_VALUES = 9;

	private AIForecaster _forecaster;

	protected AIPathHandler _pathModule;

	public RacingAI[] AIs = new RacingAI[6];

	private int _nbAI;

	private bool _forceMode;

	private int _forcedMode;

	private float _speedCtrlMinError;

	private float _speedCtrlMaxError;

	private float _speedCtrlMaxReduction;

	private float _minSpeed;

	private float _debugAttraction;

	private float _debugErrorAngle;

	private float _debugCollError;

	private float _debugPidOutput;

	private float _debugMaxAngle;

	private float _debugIdealSteer;

	private float _debugWantedSteer;

	private float m_fMinRatioIdealFromMaxSpeed
	{
		get
		{
			return Singleton<GameConfigurator>.Instance.AISettings.BehaviourSettings.m_fMinRatioIdealFromMaxSpeed;
		}
	}

	public virtual void Init(AIPathHandler pPathModule)
	{
		_pathModule = pPathModule;
		for (int i = 0; i < 6; i++)
		{
			AIs[i] = null;
		}
	}

	public void Reset()
	{
		if ((bool)_forecaster)
		{
			_forecaster.Reset();
		}
		for (int i = 0; i < _nbAI; i++)
		{
			AIs[i].Reset();
		}
	}

	public virtual RacingAI CreateAI()
	{
		return new RacingAI();
	}

	public void RegisterVirtualController(RcVirtualController pVirtualController, E_AILevel pLevel)
	{
		AIs[_nbAI] = CreateAI();
		AIs[_nbAI].Level = pLevel;
		AIs[_nbAI].VirtualController = pVirtualController;
		AIs[_nbAI].VirtualController.AIIndex = _nbAI;
		AIs[_nbAI].Init();
		if (_forecaster != null)
		{
			_forecaster.RegisterClient(pVirtualController.GetVehicle(), _pathModule.GetLeftBorder(_nbAI), _pathModule.GetRightBorder(_nbAI));
			_forecaster.UnableObserver(pVirtualController.GetVehicle().GetVehicleId(), true);
		}
		_pathModule.InitIdealPaths(AIs[_nbAI], _nbAI);
		_nbAI++;
	}

	public void RegisterHumanControllerForForecasterTests(RcController pController)
	{
		if (_forecaster != null)
		{
			_forecaster.RegisterClient(pController.GetVehicle(), _pathModule.GetLeftBorder(0), _pathModule.GetRightBorder(0));
			_forecaster.UnableObserver(pController.GetVehicle().GetVehicleId(), true);
		}
	}

	public void UnregisterVirtualController(RcVirtualController pVirtualController)
	{
		for (int i = 0; i < _nbAI; i++)
		{
			if (AIs[i].VirtualController == pVirtualController)
			{
				AIs[i].Term();
				AIs[i] = AIs[_nbAI - 1];
				AIs[_nbAI - 1] = null;
				_nbAI--;
				break;
			}
		}
		if ((bool)_forecaster)
		{
			_forecaster.UnRegisterClient(pVirtualController.GetVehicle());
		}
	}

	public RacingAI GetControllerAI(RcVirtualController pVirtualController)
	{
		for (int i = 0; i < _nbAI; i++)
		{
			if (AIs[i].VirtualController == pVirtualController)
			{
				return AIs[i];
			}
		}
		return null;
	}

	public void Update()
	{
		DispatchModes();
		if ((bool)_forecaster && (!_forceMode || _forcedMode != 0))
		{
			_forecaster.Update();
		}
		for (int i = 0; i < _nbAI; i++)
		{
			if (AIs[i].VirtualController.IsDrivingEnabled())
			{
				UpdateAI(AIs[i]);
				AIs[i].Update();
			}
		}
	}

	private void UpdateAI(RacingAI pAi)
	{
		float rpWantedSteer = pAi.Steer;
		float rpSpeedBehaviour = pAi.SpeedBehaviour;
		ForecastResult forecastResult = new ForecastResult();
		forecastResult.Direction = 0f;
		forecastResult.Weight = 0f;
		int vehicleId = pAi.Vehicle.GetVehicleId();
		switch (pAi.Mode)
		{
		case E_AIMode.IDLE_MODE:
			rpWantedSteer = 0f;
			rpSpeedBehaviour = ((!(pAi.Vehicle.GetSpeedKPH() > 0f)) ? 0f : (-1f));
			break;
		case E_AIMode.START_MODE:
		case E_AIMode.DRIVEN_MODE:
			if (pAi.PreviousMode != E_AIMode.DRIVEN_MODE && pAi.PreviousMode != E_AIMode.START_MODE && (bool)_forecaster)
			{
				_forecaster.UnableForecast(vehicleId, true);
				_forecaster.UnableObserver(vehicleId, true);
			}
			ComputeDriveParameters(pAi, ref rpWantedSteer, ref rpSpeedBehaviour);
			break;
		case E_AIMode.GO_BACK_MODE:
		case E_AIMode.HALF_TURN_MODE:
			rpSpeedBehaviour = -1f;
			rpWantedSteer = 0f;
			if (pAi.Vehicle.GetSpeedKPH() <= 10f)
			{
				_pathModule.UpdatePositionOnPath(pAi);
				RcFastPath currentPath = pAi.CurrentPath;
				int currentPathIndex = pAi.CurrentPathIndex;
				if (currentPathIndex != -1)
				{
					Vector3 lhs = Vector3.Cross(pAi.Vehicle.GetCameraUp(), currentPath.GetSegment(currentPathIndex));
					rpWantedSteer = ((!(Vector3.Dot(lhs, pAi.Vehicle.GetCameraAt()) < 0f)) ? (-1f) : 1f);
				}
			}
			break;
		default:
			rpWantedSteer = 0f;
			rpSpeedBehaviour = 0f;
			break;
		}
		pAi.SetDriveParameters(rpWantedSteer, rpSpeedBehaviour);
	}

	private void DispatchModes()
	{
		for (int i = 0; i < _nbAI; i++)
		{
			RacingAI racingAI = AIs[i];
			RcVehicle vehicle = racingAI.Vehicle;
			if (_forceMode)
			{
				racingAI.Mode = (E_AIMode)_forcedMode;
			}
			else if (vehicle.IsLocked())
			{
				racingAI.Mode = E_AIMode.IDLE_MODE;
			}
			else if (racingAI.Mode == E_AIMode.IDLE_MODE)
			{
				racingAI.Mode = E_AIMode.START_MODE;
				racingAI.StartStartMode();
			}
			else if (racingAI.Mode == E_AIMode.START_MODE && racingAI.StartModeTime > 0f)
			{
				racingAI.Mode = E_AIMode.START_MODE;
			}
			else if (racingAI.AntiReverseRemainingTime > 0f)
			{
				racingAI.Mode = E_AIMode.HALF_TURN_MODE;
			}
			else if (racingAI.AntiJamRemainingTime > 0f)
			{
				racingAI.Mode = E_AIMode.GO_BACK_MODE;
			}
			else
			{
				racingAI.Mode = E_AIMode.DRIVEN_MODE;
			}
		}
	}

	private bool IsOnRight(Vector3 pSegBegin, Vector3 pSegEnd, Vector3 pPt, Vector3 pUp)
	{
		Vector3 lhs = pSegEnd - pSegBegin;
		Vector3 rhs = pPt - pSegBegin;
		Vector3 lhs2 = Vector3.Cross(lhs, rhs);
		return Vector3.Dot(lhs2, pUp) <= 0f;
	}

	protected virtual void CheckPositionOnPath(RacingAI pAi)
	{
	}

	public void ComputeDriveParameters(RacingAI pAi, ref float rpWantedSteer, ref float rpSpeedBehaviour)
	{
		if (_pathModule.UpdatePositionOnPath(pAi))
		{
			CheckPositionOnPath(pAi);
			_pathModule.UpdatePositionOnPath(pAi);
		}
		if (pAi.Vehicle.BArcadeDriftLock)
		{
			pAi.Vehicle.SetArcadeDriftFactor(0f);
			pAi.Vehicle.BArcadeDriftLock = false;
		}
		int currentPathIndex = pAi.CurrentPathIndex;
		if (currentPathIndex == -1)
		{
			rpSpeedBehaviour = 0f;
			rpWantedSteer = 0f;
			return;
		}
		RcVehicle vehicle = pAi.Vehicle;
		int vehicleId = pAi.Vehicle.GetVehicleId();
		Vector3 position = vehicle.GetPosition();
		Vector3 cameraUp = vehicle.GetCameraUp();
		Vector3 cameraAt = vehicle.GetCameraAt();
		Vector3 vector = -1f * Vector3.Cross(cameraUp, cameraAt);
		ForecastResult rpCollisionForecast = new ForecastResult();
		ForecastResult forecastResult = new ForecastResult();
		if ((bool)_forecaster)
		{
			rpCollisionForecast = _forecaster.GetRepulsiveForecast(vehicleId);
			forecastResult = _forecaster.GetAttractiveForecast(vehicleId);
		}
		Vector3 targetPoint = pAi.GetTargetPoint();
		float direction = rpCollisionForecast.Direction;
		float num = 0f - forecastResult.Direction;
		Vector3 targetPoint2 = targetPoint - (num + direction) * vector;
		float deltaTime = Time.deltaTime;
		Vector3 lhs = targetPoint2 - position;
		lhs.Normalize();
		float value = Vector3.Dot(lhs, cameraAt);
		value = Mathf.Clamp(value, -1f, 1f);
		float num2 = Mathf.Acos(value) * ((float)Math.PI / 180f);
		if (!IsOnRight(position, position + cameraAt, targetPoint2, cameraUp))
		{
			num2 = 0f - num2;
		}
		float num3 = ComputeIdealRadius(vehicle, ref targetPoint2);
		float pIdealSpeedKPH = RcUtils.MsToKph(ComputeMaxSpeed(vehicle, num3));
		ComputeSpeedBehaviour(vehicle, rpCollisionForecast.Weight - Mathf.Abs(rpCollisionForecast.Direction), pIdealSpeedKPH, ref rpCollisionForecast, ref rpSpeedBehaviour);
		float num4 = 0f;
		float radius = vehicle.GetCarac().ComputeMinRadius(Mathf.Abs(vehicle.GetWheelSpeedMS()));
		float num5 = vehicle.SteeringRadiusToAngle(radius);
		if (num3 != 0f)
		{
			float num6 = vehicle.SteeringRadiusToAngle(num3);
			num4 = num6 / num5;
		}
		pAi.PidController.Record(num4, deltaTime);
		float debugPidOutput = (rpWantedSteer = pAi.PidController.GetOutput());
		if (pAi.Mode == E_AIMode.START_MODE)
		{
			rpWantedSteer *= RcUtils.LinearInterpolation(pAi.StartModeDelay, 0f, 0f, 1f, pAi.StartModeTime, true);
		}
		_debugAttraction = num;
		_debugErrorAngle = num2;
		_debugCollError = direction;
		_debugPidOutput = debugPidOutput;
		_debugMaxAngle = num5;
		_debugIdealSteer = num4;
		_debugWantedSteer = rpWantedSteer;
	}

	public void ComputeSpeedBehaviour(RcVehicle pVehicle, float pErrorDiff, float pIdealSpeedKPH, ref ForecastResult rpCollisionForecast, ref float rpSpeedBehaviour)
	{
		float num = pIdealSpeedKPH;
		if (pErrorDiff > _speedCtrlMinError && _speedCtrlMaxReduction > 0f)
		{
			float num2 = 0f;
			num2 = ((!(pErrorDiff < _speedCtrlMaxError)) ? _speedCtrlMaxReduction : RcUtils.LinearInterpolation(_speedCtrlMinError, 0f, _speedCtrlMaxError, _speedCtrlMaxReduction, pErrorDiff, true));
			num -= num2 * num;
		}
		if (num < _minSpeed)
		{
			num = _minSpeed;
		}
		float speedKPH = pVehicle.GetSpeedKPH();
		if (speedKPH < num)
		{
			rpSpeedBehaviour = 1f;
		}
		else if (speedKPH < num * 1.1f)
		{
			rpSpeedBehaviour = 0f;
		}
		else
		{
			rpSpeedBehaviour = -1f;
		}
	}

	public void RegisterVehicle(RcVehicle pVehicle)
	{
		RegisterEntity(pVehicle, 4);
	}

	public void RegisterEntity(MonoBehaviour pEntity, int pType)
	{
		_forecaster.RegisterEntity(pEntity, pType);
	}

	public void UnregisterVehicle(RcVehicle pVehicle)
	{
		_forecaster.UnRegisterEntity(pVehicle);
	}

	public float ComputeMaxSpeed(RcVehicle pVehicle, float pRadius)
	{
		float num = 0f;
		if (pRadius == 0f)
		{
			return pVehicle.GetMaxSpeed();
		}
		return Mathf.Max(pVehicle.GetMaxSpeed() * m_fMinRatioIdealFromMaxSpeed, pVehicle.GetCarac().ComputeMaxSpeedForRadius(Mathf.Abs(pRadius)));
	}

	public float ComputeIdealRadius(RcVehicle pVehicle, ref Vector3 targetPoint)
	{
		Vector3 lhs = targetPoint - pVehicle.GetPosition();
		Vector3 cameraUp = pVehicle.GetCameraUp();
		Vector3 cameraAt = pVehicle.GetCameraAt();
		Vector3 rhs = -1f * Vector3.Cross(cameraUp, cameraAt);
		float num = Vector3.Dot(lhs, rhs);
		if (Mathf.Abs(num) == 0f)
		{
			return 0f;
		}
		float num2 = Vector3.Dot(lhs, pVehicle.GetCameraAt());
		return (num2 * num2 + num * num) / (2f * num);
	}

	public void DebugDraw(bool pDebugDraw, int iAIDebugIndex)
	{
		if (pDebugDraw)
		{
			int num = iAIDebugIndex % AIs.Length;
			if (AIs[num] != null)
			{
				AIs[num].DebugDraw();
			}
			GUI.Label(new Rect(20f, 50f, 200f, 20f), "AI " + num + " of " + AIs.Length.ToString());
			string[] array = new string[9] { "Angle Error", "Col Error", "Attraction", "P", "I", "D", "PID", "Ideal Steer", "Wanted Steer" };
			for (num = 0; num < 9; num++)
			{
				GUI.contentColor = new Color(200f, 200f, 200f);
				GUI.Label(new Rect(400f, num * 19 + 12, 200f, 20f), array[num]);
			}
		}
		int num2 = 50;
		RacingAI[] aIs = AIs;
		foreach (RacingAI racingAI in aIs)
		{
			if (racingAI != null)
			{
				GUI.contentColor = new Color(200f, 200f, 200f);
				GUI.Label(new Rect(600f, num2, 300f, 20f), string.Format("AI : {0} ; PATH : {1} ; {2}", racingAI.Level.ToString(), ((RcFastValuePath)racingAI.CurrentPath).PathType.ToString(), _pathModule.GetPathIndex((RcFastValuePath)racingAI.CurrentPath).ToString()));
				num2 += 20;
			}
		}
	}

	public void DebugDrawGizmos(bool pDebugDraw, int iAIDebugIndex)
	{
		if (!pDebugDraw)
		{
			return;
		}
		RacingAI racingAI = AIs[iAIDebugIndex % AIs.Length];
		if (racingAI != null && racingAI.Vehicle != null)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			float[] array = new float[9]
			{
				_debugErrorAngle,
				0.15f * _debugCollError,
				0.15f * _debugAttraction,
				1f * racingAI.PidController.PCoefficient * racingAI.PidController.Error,
				1f * racingAI.PidController.ICoefficient * racingAI.PidController.ErrorIntegral,
				1f * racingAI.PidController.DCoefficient * racingAI.PidController.GetErrorDerivative(),
				_debugPidOutput / _debugMaxAngle,
				0f - _debugIdealSteer,
				0f - _debugWantedSteer
			};
			float num = -0.1f;
			float num2 = 3.5f;
			Vector3 position = racingAI.Vehicle.transform.position;
			Vector3 cameraUp = racingAI.Vehicle.GetCameraUp();
			Vector3 cameraAt = racingAI.Vehicle.GetCameraAt();
			Vector3 vector = -1f * Vector3.Cross(cameraUp, cameraAt);
			zero = position + num2 * cameraUp;
			zero2 = position + (num2 + 8f * num) * cameraUp;
			Color color = new Color(0f, 1f, 0f, 1f);
			for (int i = 0; i < 9; i++)
			{
				zero = position + num2 * cameraUp;
				zero2 = position + num2 * cameraUp - array[i] * vector;
				if (i > 5)
				{
					color = new Color(0f, 0f, 1f, 1f);
				}
				else if (i > 2)
				{
					color = new Color(1f, 0f, 1f, 1f);
				}
				else
				{
					switch (i)
					{
					case 2:
						color = new Color(0f, 1f, 0f, 1f);
						break;
					case 1:
						color = new Color(1f, 0f, 0f, 1f);
						break;
					case 0:
						color = new Color(1f, 1f, 0f, 1f);
						break;
					}
				}
				zero.y -= 1f;
				zero2.y -= 1f;
				Debug.DrawLine(zero, zero2, color);
				num2 += num;
			}
		}
		RacingAI[] aIs = AIs;
		foreach (RacingAI racingAI2 in aIs)
		{
			if (racingAI2 != null)
			{
				Vector3 position2 = racingAI2.Vehicle.transform.position;
				Debug.DrawLine(position2, racingAI2.GetTargetPoint(), Color.red);
				racingAI2.DebugDrawGizmos();
			}
		}
	}
}
