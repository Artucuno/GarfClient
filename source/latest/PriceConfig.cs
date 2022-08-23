using System;
using UnityEngine;

public class PriceConfig : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	public int[] CustoPrices = new int[Enum.GetValues(typeof(ERarity)).Length];

	[HideInInspector]
	[SerializeField]
	public int[] HatPrices = new int[Enum.GetValues(typeof(ERarity)).Length];

	[SerializeField]
	[HideInInspector]
	public int[] UniqueHatPrices = new int[Enum.GetValues(typeof(ERarity)).Length];

	[SerializeField]
	[HideInInspector]
	public int KartPrices;

	public int GetCustoPrice(ERarity _Rarity)
	{
		return CustoPrices[Tricks.LogBase2((int)_Rarity)];
	}

	public int GetHatPrice(ERarity _Rarity, bool _Unique)
	{
		if (_Unique)
		{
			return UniqueHatPrices[Tricks.LogBase2((int)_Rarity)];
		}
		return HatPrices[Tricks.LogBase2((int)_Rarity)];
	}

	public int GetKartPrice()
	{
		return KartPrices;
	}
}
