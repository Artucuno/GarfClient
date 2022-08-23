using UnityEngine;

public class ForecastedAttractive : Forecasted
{
	public ForecastedAttractive()
	{
	}

	public ForecastedAttractive(MonoBehaviour pEntity)
		: base(pEntity)
	{
	}

	public override bool IsAttractiveFor(ForecastClient pClient)
	{
		return true;
	}
}
