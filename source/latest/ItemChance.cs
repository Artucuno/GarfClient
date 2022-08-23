using System;

[Serializable]
public class ItemChance : Chance
{
	public int None;

	public ItemChance(int pGood, int pAverage, int pBad, int pNone)
		: base(pGood, pAverage, pBad)
	{
		None = pNone;
	}
}
