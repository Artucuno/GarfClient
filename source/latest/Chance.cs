using System;

[Serializable]
public class Chance
{
	public int Good;

	public int Average;

	public int Bad;

	public Chance(int pGood, int pAverage, int pBad)
	{
		Good = pGood;
		Average = pAverage;
		Bad = pBad;
	}

	protected int SetValue(int pValue)
	{
		if (pValue < 0)
		{
			return 0;
		}
		if (pValue > 100)
		{
			return 100;
		}
		return pValue;
	}

	public int GetChance(E_AILevel pLevel)
	{
		switch (pLevel)
		{
		case E_AILevel.GOOD:
			return Good;
		case E_AILevel.AVERAGE:
			return Average;
		case E_AILevel.BAD:
			return Bad;
		default:
			return 0;
		}
	}
}
