using System.Collections.Generic;
using UnityEngine;

public class ChampionShipData : MonoBehaviour
{
	public string[] Tracks = new string[4];

	public string[] TracksNameId = new string[4];

	public string ChampionShipNameId;

	public E_UnlockableItemSate EasyState;

	public E_UnlockableItemSate NormalState;

	public E_UnlockableItemSate HardState;

	public int Index;

	private string m_sChampionShipName;

	private string[] m_sTracksName = new string[4];

	public List<RewardBase> Rewards = new List<RewardBase>();

	public string ChampionShipName
	{
		get
		{
			return m_sChampionShipName;
		}
	}

	public string[] TracksName
	{
		get
		{
			return m_sTracksName;
		}
	}

	public void Localize()
	{
		m_sChampionShipName = Localization.instance.Get(ChampionShipNameId);
		for (int i = 0; i < 4; i++)
		{
			m_sTracksName[i] = Localization.instance.Get(TracksNameId[i]);
		}
	}
}
