using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
	public List<string> CharacterPrefab = new List<string>();

	public List<string> KartPrefab = new List<string>();

	private List<EAdvantage> m_pAdvantages = new List<EAdvantage>();

	public ECharacter m_eCharacter;

	public ECharacter m_eKart;

	public BonusCustom m_oHat;

	public KartCustom m_oKartCustom;

	private EAdvantage m_pSelectedAdvantage;

	public static readonly EAdvantage FirstAdvantageBonus = EAdvantage.LasagnaBonus;

	public static readonly EAdvantage LastAdvantageBonus = EAdvantage.PieBonus;

	public bool HasEasyChampionShipStar;

	public bool HasNormalChampionShipStar;

	public bool HasHardChampionShipStar;

	public bool HasTimeTrialStar;

	public bool HasEndStar;

	private Color playerColor;

	public Color PlayerColor
	{
		get
		{
			return playerColor;
		}
		set
		{
			playerColor = value;
		}
	}

	public int NbStars
	{
		get
		{
			int num = 0;
			if (HasEasyChampionShipStar)
			{
				num++;
			}
			if (HasNormalChampionShipStar)
			{
				num++;
			}
			if (HasHardChampionShipStar)
			{
				num++;
			}
			if (HasTimeTrialStar)
			{
				num++;
			}
			if (HasEndStar)
			{
				num++;
			}
			return num;
		}
	}

	public ECharacter Character
	{
		get
		{
			return m_eCharacter;
		}
		set
		{
			m_eCharacter = value;
		}
	}

	public ECharacter Kart
	{
		get
		{
			return m_eKart;
		}
		set
		{
			m_eKart = value;
		}
	}

	private void Awake()
	{
		m_pSelectedAdvantage = EAdvantage.None;
		playerColor = Color.yellow;
	}

	public List<EAdvantage> GetAdvantages()
	{
		return m_pAdvantages;
	}

	public EAdvantage GetAdvantage()
	{
		return m_pSelectedAdvantage;
	}

	public void ResetAdvantages()
	{
		m_pSelectedAdvantage = EAdvantage.None;
	}

	public void AddAdvantage(EAdvantage _Advantage)
	{
		m_pAdvantages.Add(_Advantage);
	}

	public void SelectAdvantage(EAdvantage _Advantage)
	{
		m_pSelectedAdvantage = _Advantage;
		m_pAdvantages.Remove(m_pSelectedAdvantage);
		Singleton<GameSaveManager>.Instance.UseAdvantage(m_pSelectedAdvantage, 1, true);
	}
}
