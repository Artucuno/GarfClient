using UnityEngine;

public class AIForecaster : MonoBehaviour
{
	public const int MAX_NB_AI_FORECAST = 8;

	public const int MAX_FORECASTED_OBJECTS = 512;

	public const int MAX_FORECASTED_POLYGONS = 32;

	public float BordersRange;

	public float BordersWeight;

	public float ForecastTime;

	public Forecasted[] ForecastedObjects = new Forecasted[512];

	public ForecastClient[] ForecastClients = new ForecastClient[8];

	protected uint _nbForecastedObjects;

	public void UnableForecast(int pVehicleId, bool pEnable)
	{
		if (ForecastClients[pVehicleId] != null)
		{
			ForecastClients[pVehicleId].ForecastEnabled = pEnable;
		}
	}

	public void UnableObserver(int pVehicleId, bool pEnable)
	{
		if ((bool)ForecastClients[pVehicleId])
		{
			ForecastClients[pVehicleId].ObserverEnabled = pEnable;
		}
	}

	public virtual void Update()
	{
		for (int i = 0; i < 8; i++)
		{
			if ((bool)ForecastClients[i])
			{
				ForecastClients[i].Refresh(ForecastTime);
			}
		}
		for (uint num = 0u; num < _nbForecastedObjects; num++)
		{
			if (ForecastedObjects[num].ForecastEnabled)
			{
				ForecastedObjects[num].Refresh(ForecastTime);
			}
		}
		ForecastObjects();
		ForecastBorders();
		ForecastPolygons();
	}

	public ForecastResult GetRepulsiveForecast(int pVehicleId)
	{
		return ForecastClients[pVehicleId].ForecastRepulsive;
	}

	public ForecastResult GetAttractiveForecast(int pVehicleId)
	{
		return ForecastClients[pVehicleId].ForecastRepulsive;
	}

	public void Reset()
	{
		for (int i = 0; i < 8; i++)
		{
			if (ForecastClients[i] != null)
			{
				ForecastClients[i].ResetBorderPositions();
			}
		}
	}

	public void RegisterClient(RcVehicle pVehicle, RcFastPath pLeftBorder, RcFastPath pRightBorder)
	{
		ForecastClient forecastClient = new ForecastClient(pVehicle);
		forecastClient.LeftBorder = pLeftBorder;
		forecastClient.RightBorder = pRightBorder;
		ForecastClients[pVehicle.GetVehicleId()] = forecastClient;
	}

	public void UnRegisterClient(RcVehicle pVehicle)
	{
		ForecastClients[pVehicle.GetVehicleId()] = null;
	}

	public virtual void RegisterEntity(MonoBehaviour pEntity, int pType)
	{
		Forecasted forecasted = null;
		switch (pType)
		{
		default:
			return;
		case 3:
			forecasted = new Forecasted(pEntity);
			break;
		case 4:
			forecasted = new Forecasted(pEntity);
			break;
		}
		ForecastedObjects[_nbForecastedObjects] = forecasted;
		_nbForecastedObjects++;
	}

	public void UnRegisterEntity(MonoBehaviour pEntity)
	{
		for (uint num = 0u; num < _nbForecastedObjects; num++)
		{
			if (ForecastedObjects[num].Entity == pEntity)
			{
				ForecastedObjects[num] = null;
				ForecastedObjects[num] = ForecastedObjects[_nbForecastedObjects - 1];
				ForecastedObjects[_nbForecastedObjects - 1] = null;
				_nbForecastedObjects--;
				break;
			}
		}
	}

	protected virtual void ForecastObjects()
	{
		for (int i = 0; i < 8; i++)
		{
			ForecastClient forecastClient = ForecastClients[i];
			if (!(forecastClient != null) || !forecastClient.ObserverEnabled)
			{
				continue;
			}
			for (uint num = 0u; num < _nbForecastedObjects; num++)
			{
				if (ForecastedObjects[num].ForecastEnabled && ForecastedObjects[num].Entity != ForecastClients[i].Entity && forecastClient.IsObjectInFront(ForecastedObjects[num]))
				{
					ForecastedObjects[num].ForecastCollision(forecastClient, ForecastTime);
				}
			}
		}
	}

	protected virtual void ForecastBorders()
	{
		if (!(BordersWeight > 0f))
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			ForecastClient forecastClient = ForecastClients[i];
			if (!(forecastClient != null) || !forecastClient.ObserverEnabled)
			{
				continue;
			}
			Vector3 forecastPosition = forecastClient.ForecastPosition;
			Vector3 up = forecastClient.Entity.transform.rotation * Vector3.up;
			RcFastPath leftBorder = forecastClient.LeftBorder;
			RcFastPath rightBorder = forecastClient.RightBorder;
			if (leftBorder == null || rightBorder == null)
			{
				continue;
			}
			leftBorder.UpdatePathPosition(ref forecastClient.LeftBorderPosition, forecastPosition, 5, 1, false, true);
			rightBorder.UpdatePathPosition(ref forecastClient.RightBorderPosition, forecastPosition, 5, 1, false, true);
			float bordersRange = BordersRange;
			float num = bordersRange * bordersRange;
			float sqrDist = forecastClient.RightBorderPosition.sqrDist;
			if (rightBorder.IsOnRight(forecastPosition, forecastClient.RightBorderPosition, up))
			{
				float a = bordersRange + RcUtils.FastSqrtApprox(sqrDist);
				a = Mathf.Min(a, 2f * bordersRange);
				forecastClient.ForecastRepulsive.Direction -= BordersWeight * a;
				forecastClient.ForecastRepulsive.Weight += BordersWeight * a;
				continue;
			}
			if (sqrDist < num)
			{
				float num2 = bordersRange - RcUtils.FastSqrtApprox(sqrDist);
				forecastClient.ForecastRepulsive.Direction -= BordersWeight * num2;
				forecastClient.ForecastRepulsive.Weight += BordersWeight * num2;
				continue;
			}
			float sqrDist2 = forecastClient.LeftBorderPosition.sqrDist;
			if (!leftBorder.IsOnRight(forecastPosition, forecastClient.LeftBorderPosition, up))
			{
				float a2 = bordersRange + RcUtils.FastSqrtApprox(sqrDist2);
				a2 = Mathf.Min(a2, 2f * bordersRange);
				forecastClient.ForecastRepulsive.Direction += BordersWeight * a2;
				forecastClient.ForecastRepulsive.Weight += BordersWeight * a2;
			}
			else if (sqrDist2 < num)
			{
				float num3 = bordersRange - RcUtils.FastSqrtApprox(sqrDist2);
				forecastClient.ForecastRepulsive.Direction += BordersWeight * num3;
				forecastClient.ForecastRepulsive.Weight += BordersWeight * num3;
			}
		}
	}

	protected virtual void ForecastPolygons()
	{
		for (int i = 0; i < 8; i++)
		{
			ForecastClient forecastClient = ForecastClients[i];
			if (!(forecastClient != null) || forecastClient.ObserverEnabled)
			{
			}
		}
	}
}
