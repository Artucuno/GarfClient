using System.Collections.Generic;
using UnityEngine;

public static class GkUtils
{
	public static List<string> GetHats(ERarity pRarity, bool pIncludeDefault)
	{
		List<string> list = new List<string>();
		Object[] array = Resources.LoadAll("Hat", typeof(BonusCustom));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (!(@object is BonusCustom))
			{
				continue;
			}
			BonusCustom bonusCustom = (BonusCustom)@object;
			if (pIncludeDefault || !@object.name.Contains("Default"))
			{
				ERarity rarity = bonusCustom.Rarity;
				if ((pRarity & rarity) != 0)
				{
					list.Add(@object.name);
				}
			}
		}
		return list;
	}

	public static string[] GetTracks()
	{
		Object @object = Resources.Load("Tracks", typeof(TrackList));
		return ((TrackList)@object).Tracks;
	}

	public static List<string> GetHats(bool pIncludeDefault)
	{
		List<string> list = new List<string>();
		Object[] array = Resources.LoadAll("Hat", typeof(BonusCustom));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (@object is BonusCustom && (pIncludeDefault || !@object.name.Contains("Default")))
			{
				list.Add(@object.name);
			}
		}
		return list;
	}

	public static List<string> GetCustoms(ERarity pRarity, bool pIncludeDefault)
	{
		List<string> list = new List<string>();
		Object[] array = Resources.LoadAll("Kart", typeof(KartCustom));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (!(@object is KartCustom))
			{
				continue;
			}
			KartCustom kartCustom = (KartCustom)@object;
			if (pIncludeDefault || !@object.name.Contains("Default"))
			{
				ERarity rarity = kartCustom.Rarity;
				if ((pRarity & rarity) != 0)
				{
					list.Add(@object.name);
				}
			}
		}
		return list;
	}

	public static List<string> GetChampionShips()
	{
		List<string> list = new List<string>();
		Object[] array = Resources.LoadAll("ChampionShip", typeof(ChampionShipData));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (@object is ChampionShipData)
			{
				list.Add(@object.name);
			}
		}
		return list;
	}

	public static List<string> GetCustoms(bool pIncludeDefault)
	{
		List<string> list = new List<string>();
		Object[] array = Resources.LoadAll("Kart", typeof(KartCustom));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (@object is KartCustom && (pIncludeDefault || !@object.name.Contains("Default")))
			{
				list.Add(@object.name);
			}
		}
		return list;
	}

	public static List<ECharacter> GetKarts()
	{
		List<ECharacter> list = new List<ECharacter>();
		Object[] array = Resources.LoadAll("Kart", typeof(KartCarac));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (@object is KartCarac)
			{
				list.Add(((KartCarac)@object).Owner);
			}
		}
		return list;
	}

	public static List<EAdvantage> GetAdvantages()
	{
		List<EAdvantage> list = new List<EAdvantage>();
		Object[] array = Resources.LoadAll("Advantages", typeof(AdvantageData));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (@object is AdvantageData)
			{
				AdvantageData advantageData = (AdvantageData)@object;
				list.Add(advantageData.AdvantageType);
			}
		}
		return list;
	}

	public static List<ECharacter> GetCharacters()
	{
		List<ECharacter> list = new List<ECharacter>();
		Object[] array = Resources.LoadAll("Character", typeof(CharacterCarac));
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			if (@object is CharacterCarac)
			{
				CharacterCarac characterCarac = (CharacterCarac)@object;
				list.Add(characterCarac.Owner);
			}
		}
		return list;
	}

	public static void UnlockAll()
	{
		List<string> hats = GetHats(true);
		foreach (string item in hats)
		{
			Singleton<GameSaveManager>.Instance.SetHatState(item, E_UnlockableItemSate.Unlocked, false);
		}
		List<string> customs = GetCustoms(true);
		foreach (string item2 in customs)
		{
			Singleton<GameSaveManager>.Instance.SetCustomState(item2, E_UnlockableItemSate.Unlocked, false);
		}
		List<ECharacter> karts = GetKarts();
		foreach (ECharacter item3 in karts)
		{
			Singleton<GameSaveManager>.Instance.SetKartState(item3, E_UnlockableItemSate.Unlocked, false);
		}
		List<string> championShips = GetChampionShips();
		foreach (string item4 in championShips)
		{
			Singleton<GameSaveManager>.Instance.SetChampionShipState(item4, EDifficulty.EASY, E_UnlockableItemSate.Unlocked, false);
			Singleton<GameSaveManager>.Instance.SetChampionShipState(item4, EDifficulty.NORMAL, E_UnlockableItemSate.Unlocked, false);
			Singleton<GameSaveManager>.Instance.SetChampionShipState(item4, EDifficulty.HARD, E_UnlockableItemSate.Unlocked, false);
		}
		List<EAdvantage> advantages = GetAdvantages();
		foreach (EAdvantage item5 in advantages)
		{
			Singleton<GameSaveManager>.Instance.SetAdvantageState(item5, E_UnlockableItemSate.Unlocked, false);
			Singleton<GameSaveManager>.Instance.SetAdvantageQuantity(item5, 99, false);
		}
		List<ECharacter> characters = GetCharacters();
		foreach (ECharacter item6 in characters)
		{
			Singleton<GameSaveManager>.Instance.SetCharacterState(item6, E_UnlockableItemSate.Unlocked, false);
		}
		Singleton<GameSaveManager>.Instance.Save();
	}
}
