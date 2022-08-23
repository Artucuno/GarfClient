using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChanceDispatcher
{
	private ChanceSettings _chanceSettings;

	private List<ECharacter> _availableCharacters = new List<ECharacter>
	{
		ECharacter.GARFIELD,
		ECharacter.HARRY,
		ECharacter.JON,
		ECharacter.ODIE,
		ECharacter.ARLENE,
		ECharacter.NERMAL,
		ECharacter.LIZ,
		ECharacter.SQUEAK
	};

	private ECharacter[] _availableKarts = new ECharacter[8]
	{
		ECharacter.GARFIELD,
		ECharacter.HARRY,
		ECharacter.JON,
		ECharacter.ODIE,
		ECharacter.ARLENE,
		ECharacter.NERMAL,
		ECharacter.LIZ,
		ECharacter.SQUEAK
	};

	private Dictionary<ECharacter, List<GameObject>> _kartCustoms = new Dictionary<ECharacter, List<GameObject>>();

	private Dictionary<ECharacter, GameObject> _defaultKartCustoms = new Dictionary<ECharacter, GameObject>();

	private List<GameObject> _allKartCustoms = new List<GameObject>();

	private Dictionary<ECharacter, List<GameObject>> _availableUniqueHats = new Dictionary<ECharacter, List<GameObject>>();

	private Dictionary<ECharacter, List<GameObject>> _availableHats = new Dictionary<ECharacter, List<GameObject>>();

	private Dictionary<ECharacter, GameObject> _heads = new Dictionary<ECharacter, GameObject>();

	private List<GameObject> _allHats = new List<GameObject>();

	public Action<PlayerData, int> OnCreatePlayer;

	public ChanceDispatcher()
	{
		_chanceSettings = Singleton<GameConfigurator>.Instance.AISettings.ChanceSettings;
		UnityEngine.Object[] array = Resources.LoadAll("Hat");
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			if (!(@object is GameObject))
			{
				continue;
			}
			GameObject gameObject = (GameObject)@object;
			BonusCustom component = gameObject.GetComponent<BonusCustom>();
			if (!(component != null))
			{
				continue;
			}
			ECharacter character = component.Character;
			if (component.Category == EITEM.ITEM_NONE)
			{
				_heads.Add(character, gameObject);
			}
			else if (character == ECharacter.NONE)
			{
				if (!_availableHats.ContainsKey(character))
				{
					_availableHats.Add(character, new List<GameObject>());
				}
				_availableHats[character].Add(gameObject);
				_allHats.Add(gameObject);
			}
			else
			{
				if (!_availableUniqueHats.ContainsKey(character))
				{
					_availableUniqueHats.Add(character, new List<GameObject>());
				}
				_availableUniqueHats[character].Add(gameObject);
			}
		}
		UnityEngine.Object[] array3 = Resources.LoadAll("Kart");
		UnityEngine.Object[] array4 = array3;
		foreach (UnityEngine.Object object2 in array4)
		{
			if (!(object2 is GameObject))
			{
				continue;
			}
			GameObject gameObject2 = (GameObject)object2;
			KartCustom component2 = gameObject2.GetComponent<KartCustom>();
			if (!(component2 != null))
			{
				continue;
			}
			ECharacter character2 = component2.Character;
			if (character2 != ECharacter.NONE)
			{
				_defaultKartCustoms.Add(character2, gameObject2);
				continue;
			}
			if (!_kartCustoms.ContainsKey(component2.Owner))
			{
				_kartCustoms.Add(component2.Owner, new List<GameObject>());
			}
			_kartCustoms[component2.Owner].Add(gameObject2);
			_allKartCustoms.Add(gameObject2);
		}
	}

	public void DispatchAI(int pIndex, bool pLock, out ECharacter vPerso, out ECharacter vKart, out GameObject vHat, out GameObject vKartCusto)
	{
		E_AILevel pLevel = E_AILevel.GOOD;
		if (pIndex == 5)
		{
			pLevel = E_AILevel.BAD;
		}
		else if (pIndex > 2)
		{
			pLevel = E_AILevel.AVERAGE;
		}
		if (Singleton<ChallengeManager>.Instance.IsActive && Singleton<ChallengeManager>.Instance.GetSomeoneToBeat() && _availableCharacters.Contains(Singleton<ChallengeManager>.Instance.GetCharacterToBeat()))
		{
			vPerso = Singleton<ChallengeManager>.Instance.GetCharacterToBeat();
			_availableCharacters.Remove(vPerso);
		}
		else
		{
			vPerso = GetRandomCharacter(_availableCharacters);
		}
		vKart = GenerateKart(pLevel, vPerso);
		vHat = GenerateHat(pLevel, vPerso);
		vKartCusto = GenerateCustom(pLevel, vKart);
		if (OnCreatePlayer != null)
		{
			OnCreatePlayer(new PlayerData(vPerso, vKart, vKartCusto.name, vHat.name, 0, string.Empty, Color.white), pIndex);
		}
	}

	public void AddPlayerData(ECharacter pPerso, ECharacter pKart, GameObject pKartCusto, GameObject pHat, int iNbStars, int pIndex)
	{
		if (_availableCharacters.Contains(pPerso))
		{
			_availableCharacters.Remove(pPerso);
		}
		if (OnCreatePlayer != null)
		{
			OnCreatePlayer(new PlayerData(pPerso, pKart, pKartCusto.name, pHat.name, iNbStars, string.Empty, Singleton<GameConfigurator>.Instance.PlayerConfig.PlayerColor), pIndex);
		}
	}

	private ECharacter GetRandomCharacter(List<ECharacter> pCharacters)
	{
		ECharacter randomItem = GetRandomItem(pCharacters.ToArray());
		pCharacters.Remove(randomItem);
		return randomItem;
	}

	private ECharacter GetRandomItem(ECharacter[] pItems)
	{
		int num = Singleton<RandomManager>.Instance.Next(pItems.Length - 1);
		return pItems[num];
	}

	private GameObject GetRandomObject(List<GameObject> pGameObjects)
	{
		int index = Singleton<RandomManager>.Instance.Next(pGameObjects.Count - 1);
		return pGameObjects[index];
	}

	private ECharacter GenerateKart(E_AILevel pLevel, ECharacter pCharacter)
	{
		int chance = _chanceSettings.KartChance.GetChance(pLevel);
		if (ForceChoice(chance))
		{
			return pCharacter;
		}
		return GetRandomItem(_availableKarts);
	}

	private GameObject GenerateCustom(E_AILevel pLevel, ECharacter pCharacter)
	{
		int chance = _chanceSettings.HatChance.GetChance(pLevel);
		if (ForceChoice(chance))
		{
			List<GameObject> list = new List<GameObject>();
			if (_kartCustoms.ContainsKey(pCharacter))
			{
				list = _kartCustoms[pCharacter];
			}
			if (list.Count > 0)
			{
				return GetRandomObject(list);
			}
			if (_defaultKartCustoms.ContainsKey(pCharacter))
			{
				return _defaultKartCustoms[pCharacter];
			}
			return _defaultKartCustoms[ECharacter.GARFIELD];
		}
		if (IsNone(_chanceSettings.CustoChance.None))
		{
			if (_defaultKartCustoms.ContainsKey(pCharacter))
			{
				return _defaultKartCustoms[pCharacter];
			}
			return _defaultKartCustoms[ECharacter.GARFIELD];
		}
		return GetRandomObject(_allKartCustoms);
	}

	private List<GameObject> GetAll(List<GameObject> pLeftList, List<GameObject> pRightList)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject pLeft in pLeftList)
		{
			list.Add(pLeft);
		}
		foreach (GameObject pRight in pRightList)
		{
			list.Add(pRight);
		}
		return list;
	}

	private GameObject GenerateHat(E_AILevel pLevel, ECharacter pCharacter)
	{
		int chance = _chanceSettings.HatChance.GetChance(pLevel);
		if (ForceChoice(chance))
		{
			List<GameObject> pLeftList = new List<GameObject>();
			if (_availableHats.ContainsKey(pCharacter))
			{
				pLeftList = _availableHats[pCharacter];
			}
			List<GameObject> pRightList = new List<GameObject>();
			if (_availableUniqueHats.ContainsKey(pCharacter))
			{
				pRightList = _availableUniqueHats[pCharacter];
			}
			List<GameObject> all = GetAll(pLeftList, pRightList);
			if (all.Count > 0)
			{
				return GetRandomObject(all);
			}
			return _heads[pCharacter];
		}
		if (IsNone(_chanceSettings.HatChance.None))
		{
			return _heads[pCharacter];
		}
		List<GameObject> pRightList2 = new List<GameObject>();
		if (_availableUniqueHats.ContainsKey(pCharacter))
		{
			pRightList2 = _availableUniqueHats[pCharacter];
		}
		List<GameObject> all2 = GetAll(_allHats, pRightList2);
		if (all2.Count > 0)
		{
			return GetRandomObject(all2);
		}
		return _heads[pCharacter];
	}

	private bool ForceChoice(int pValue)
	{
		int num = Singleton<RandomManager>.Instance.Next(1, 100);
		return pValue >= num;
	}

	private bool IsNone(int pValue)
	{
		int num = Singleton<RandomManager>.Instance.Next(1, 100);
		return pValue >= num;
	}
}
