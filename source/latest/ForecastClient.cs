using System;
using UnityEngine;

public class ForecastClient : ForecastedCar
{
	private RcFastPath _leftBorder;

	private RcFastPath _rightBorder;

	public PathPosition LeftBorderPosition;

	public PathPosition RightBorderPosition;

	public RcVehicle Vehicle;

	public float FrustrumStartWidth;

	public float FrustrumEndWidth;

	private bool _observerEnabled = true;

	private ForecastResult _forecastAttractive = new ForecastResult();

	private ForecastResult _forecastRepulsive = new ForecastResult();

	private Vector2 _pos = Vector2.zero;

	private Vector2 _forecastSegment = Vector2.zero;

	public bool ObserverEnabled
	{
		get
		{
			return _observerEnabled;
		}
		set
		{
			_observerEnabled = value;
			if (_observerEnabled)
			{
				ResetBorderPositions();
			}
		}
	}

	public ForecastResult ForecastAttractive
	{
		get
		{
			return _forecastAttractive;
		}
	}

	public ForecastResult ForecastRepulsive
	{
		get
		{
			return _forecastRepulsive;
		}
	}

	public RcFastPath LeftBorder
	{
		get
		{
			return _leftBorder;
		}
		set
		{
			_leftBorder = value;
		}
	}

	public RcFastPath RightBorder
	{
		get
		{
			return _rightBorder;
		}
		set
		{
			_rightBorder = value;
		}
	}

	public Vector2 Pos
	{
		get
		{
			return _pos;
		}
	}

	public Vector2 ForecastSegment
	{
		get
		{
			return _forecastSegment;
		}
	}

	public float SpeedMs
	{
		get
		{
			return ((RcVehicle)_entity).GetRealSpeedMs();
		}
	}

	public ForecastClient(RcVehicle pVehicle)
	{
		Vehicle = pVehicle;
	}

	private void Start()
	{
		RcVehicle vehicle = Vehicle;
		vehicle.OnTeleported = (Action)Delegate.Combine(vehicle.OnTeleported, new Action(NeedResetBorderPositions));
		RcVehicle vehicle2 = Vehicle;
		vehicle2.OnAutoPilotChanged = (Action)Delegate.Combine(vehicle2.OnAutoPilotChanged, new Action(NeedResetBorderPositions));
	}

	~ForecastClient()
	{
		RcVehicle vehicle = Vehicle;
		vehicle.OnTeleported = (Action)Delegate.Remove(vehicle.OnTeleported, new Action(NeedResetBorderPositions));
		RcVehicle vehicle2 = Vehicle;
		vehicle2.OnAutoPilotChanged = (Action)Delegate.Remove(vehicle2.OnAutoPilotChanged, new Action(NeedResetBorderPositions));
	}

	public override void Refresh(float pForecastTime)
	{
		base.Refresh(pForecastTime);
		_pos.x = _entity.transform.position.x;
		_pos.y = _entity.transform.position.z;
		_forecastSegment.x = _forecastPosition.x - _pos.x;
		_forecastSegment.y = _forecastPosition.z - _pos.y;
		_forecastAttractive.Refresh();
		_forecastRepulsive.Refresh();
	}

	public bool IsObjectInFront(Forecasted pOther)
	{
		return Vector3.Dot(_entity.transform.rotation * Vector3.forward, pOther.CurrentPosition - base.CurrentPosition) >= 0f;
	}

	public bool IsObjectOnRight(Forecasted pOther)
	{
		return RcUtils.IsOnRight(base.CurrentPosition, base.CurrentPosition + _entity.transform.rotation * Vector3.forward, pOther.ForecastPosition, _entity.transform.rotation * Vector3.up);
	}

	public bool IsSegmentOnRight(Vector2 pP1, Vector2 pP2)
	{
		Vector3 vector = _entity.transform.rotation * Vector3.forward;
		Vector2 vector2 = new Vector2(vector.x, vector.z);
		bool flag = RcUtils.IsOnRight(Pos, Pos + vector2, pP1);
		bool flag2 = RcUtils.IsOnRight(Pos, Pos + vector2, pP2);
		if (flag && flag2)
		{
			return true;
		}
		if (!flag && !flag2)
		{
			return false;
		}
		float num = Vector2.Dot(pP2 - pP1, vector2);
		Vector2 rpOut = Vector2.zero;
		Vector2 rpOut2 = Vector2.zero;
		int num2 = RcUtils.SegmentsIntersection(Pos, Pos + vector2 * 10000f, pP1, pP2, ref rpOut, ref rpOut2);
		bool flag3 = num2 == 0;
		if (flag)
		{
			if (num > 0f)
			{
				return (!flag3) ? true : false;
			}
			return flag3 ? true : false;
		}
		if (num < 0f)
		{
			return (!flag3) ? true : false;
		}
		return flag3 ? true : false;
	}

	public void ResetBorderPositions()
	{
		if (_leftBorder != null)
		{
			_leftBorder.ResetSidedPosition(ref LeftBorderPosition, _entity.transform.position, true, RcFastPath.PathSide.SIDE_RIGHT, _entity.transform.rotation * Vector3.up);
		}
		if (_rightBorder != null)
		{
			_rightBorder.ResetSidedPosition(ref RightBorderPosition, _entity.transform.position, true, RcFastPath.PathSide.SIDE_RIGHT, _entity.transform.rotation * Vector3.up);
		}
	}

	public void NeedResetBorderPositions()
	{
		ResetBorderPositions();
	}
}
