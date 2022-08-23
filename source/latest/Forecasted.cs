using UnityEngine;

public class Forecasted : MonoBehaviour
{
	public float Range;

	public float Weight;

	protected MonoBehaviour _entity;

	protected bool _forecastEnabled = true;

	protected Vector3 _forecastPosition = Vector3.zero;

	protected Vector3 _linearVelocity = Vector3.zero;

	protected Vector3 _prevPos = Vector3.zero;

	protected bool _hit;

	public MonoBehaviour Entity
	{
		get
		{
			return _entity;
		}
		set
		{
			_entity = value;
		}
	}

	public bool ForecastEnabled
	{
		get
		{
			return _forecastEnabled;
		}
		set
		{
			_forecastEnabled = value;
		}
	}

	public Vector3 CurrentPosition
	{
		get
		{
			return base.transform.position;
		}
	}

	public Vector3 ForecastPosition
	{
		get
		{
			return _forecastPosition;
		}
	}

	public Forecasted()
		: this(null)
	{
	}

	public Forecasted(MonoBehaviour pEntity)
	{
		_entity = pEntity;
		_prevPos = base.transform.position;
	}

	public virtual void Refresh(float pForecastTime)
	{
		if (Time.deltaTime != 0f)
		{
			_linearVelocity = (CurrentPosition - _prevPos) / Time.deltaTime;
		}
		_prevPos = CurrentPosition;
		_forecastPosition = Vector3.Cross(CurrentPosition, _linearVelocity) * pForecastTime;
		_hit = false;
	}

	public virtual float GetWeightFor(ForecastClient pClient)
	{
		return Weight;
	}

	public virtual bool IsAttractiveFor(ForecastClient pClient)
	{
		return false;
	}

	public virtual bool HasInfluenceFor(ForecastClient pClient)
	{
		return false;
	}

	public void ForecastCollision(ForecastClient pAI, float pForecastTime)
	{
		if (!HasInfluenceFor(pAI))
		{
			return;
		}
		float sqrMagnitude = (ForecastPosition - pAI.CurrentPosition).sqrMagnitude;
		float num = pAI.FrustrumEndWidth + pAI.SpeedMs * pForecastTime + Range;
		float num2 = 0f;
		if (sqrMagnitude < num * num)
		{
			if ((double)pAI.ForecastSegment.magnitude < 1E-12)
			{
				return;
			}
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 1f / pAI.ForecastSegment.magnitude;
			num3 = (ForecastPosition.x - pAI.Pos.x) * pAI.ForecastSegment.x + (ForecastPosition.z - pAI.Pos.y) * pAI.ForecastSegment.y;
			num4 = num3 * num7;
			if (num4 < 0f)
			{
				num5 = pAI.Pos.x - ForecastPosition.x;
				num6 = pAI.Pos.y - ForecastPosition.z;
			}
			else if (num4 > 1f)
			{
				num5 = pAI.ForecastPosition.x - ForecastPosition.x;
				num6 = pAI.ForecastPosition.z - ForecastPosition.z;
			}
			else
			{
				num5 = (1f - num4) * pAI.Pos.x + num4 * pAI.ForecastPosition.x - ForecastPosition.x;
				num6 = (1f - num4) * pAI.Pos.y + num4 * pAI.ForecastPosition.z - ForecastPosition.z;
			}
			sqrMagnitude = num5 * num5 + num6 * num6;
			float num8 = pAI.FrustrumStartWidth + Mathf.Min(num4, 1f) * (pAI.FrustrumEndWidth - pAI.FrustrumStartWidth);
			num = Range + num8;
			if (sqrMagnitude < num * num)
			{
				num2 += num - RcUtils.FastSqrtApprox(sqrMagnitude);
				_hit = true;
			}
		}
		if (!(num2 > 0f))
		{
			return;
		}
		float num9 = GetWeightFor(pAI) * num2;
		bool flag = IsAttractiveFor(pAI);
		if (pAI.IsObjectOnRight(this))
		{
			if (flag)
			{
				pAI.ForecastAttractive.Direction -= num9;
				pAI.ForecastAttractive.Weight += num9;
			}
			else
			{
				pAI.ForecastRepulsive.Direction -= num9;
				pAI.ForecastRepulsive.Weight += num9;
			}
		}
		else if (flag)
		{
			pAI.ForecastAttractive.Direction += num9;
			pAI.ForecastAttractive.Weight += num9;
		}
		else
		{
			pAI.ForecastRepulsive.Direction += num9;
			pAI.ForecastRepulsive.Weight += num9;
		}
	}
}
