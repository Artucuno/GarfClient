using UnityEngine;

public class ForecastedCar : Forecasted
{
	public ForecastedCar()
	{
	}

	public ForecastedCar(MonoBehaviour pEntity)
		: base(pEntity)
	{
	}

	public override bool IsAttractiveFor(ForecastClient pClient)
	{
		return false;
	}
}
