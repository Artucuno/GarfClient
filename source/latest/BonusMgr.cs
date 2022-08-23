using System.Collections.Generic;
using UnityEngine;

public class BonusMgr : Singleton<BonusMgr>
{
	public const int MAX_BONUS_ENTITY = 20;

	public const int MAX_BONUS_DIAMOND = 8;

	public const int MAX_BONUS_MAGIC = 10;

	public PieBonusEntity[] m_pPieEntities = new PieBonusEntity[20];

	public AutolockPieBonusEntity[] m_pAutolockPieEntities = new AutolockPieBonusEntity[20];

	public SpringBonusEntity[] m_pSpringEntities = new SpringBonusEntity[20];

	public MagicBonusEntity[] m_pMagicEntities = new MagicBonusEntity[10];

	public DiamondBonusEntity[] m_pDiamondEntities = new DiamondBonusEntity[8];

	private UFOBonusEntity m_pUFOEntity;

	public bool UfoLaunched;

	private int nbPie;

	private int nbAutolockPie;

	private int nbSpring;

	private int nbMagic;

	private int nbDiamond;

	private Dictionary<NetworkViewID, Kart> m_cKarts = new Dictionary<NetworkViewID, Kart>();

	public Dictionary<NetworkViewID, Kart> Karts
	{
		get
		{
			if (Network.peerType != 0 && m_cKarts.Count == 0)
			{
				Kart[] array = (Kart[])Object.FindObjectsOfType(typeof(Kart));
				Kart[] array2 = array;
				foreach (Kart kart in array2)
				{
					m_cKarts[kart.networkViewID] = kart;
				}
			}
			return m_cKarts;
		}
	}

	public void Init()
	{
		UfoLaunched = false;
		nbPie = 0;
		nbAutolockPie = 0;
		nbSpring = 0;
		nbMagic = 0;
		nbDiamond = 0;
		if (!Network.isClient)
		{
			for (int i = 0; i < 20; i++)
			{
				GenerateItem(EITEM.ITEM_AUTOLOCK_PIE);
				GenerateItem(EITEM.ITEM_PIE);
				GenerateItem(EITEM.ITEM_SPRING);
			}
			for (int j = 0; j < 8; j++)
			{
				GenerateItem(EITEM.ITEM_DIAMOND);
			}
			for (int k = 0; k < 10; k++)
			{
				GenerateItem(EITEM.ITEM_MAGIC);
			}
			GenerateItem(EITEM.ITEM_UFO);
		}
	}

	public void Reset()
	{
		if (m_pUFOEntity != null)
		{
			DestroyEntities(m_pPieEntities);
			DestroyEntities(m_pAutolockPieEntities);
			DestroyEntities(m_pSpringEntities);
			DestroyEntities(m_pMagicEntities);
			DestroyEntities(m_pDiamondEntities);
			Object.Destroy(m_pUFOEntity.transform.parent.gameObject);
		}
		nbPie = 0;
		nbAutolockPie = 0;
		nbSpring = 0;
		nbMagic = 0;
		nbDiamond = 0;
		m_cKarts.Clear();
	}

	private void DestroyEntities(BonusEntity[] pEntities)
	{
		for (int i = 0; i < pEntities.Length; i++)
		{
			if (pEntities[i] != null)
			{
				Object.Destroy(pEntities[i].transform.parent.gameObject);
			}
		}
	}

	private void GenerateItem(EITEM _Item)
	{
		string path = string.Empty;
		switch (_Item)
		{
		case EITEM.ITEM_AUTOLOCK_PIE:
			path = "Bonus/AutolockPieBonusEntity";
			break;
		case EITEM.ITEM_PIE:
			path = "Bonus/PieBonusEntity";
			break;
		case EITEM.ITEM_SPRING:
			path = "Bonus/SpringBonusEntity";
			break;
		case EITEM.ITEM_UFO:
			path = "Bonus/UfoBonusEntity";
			break;
		case EITEM.ITEM_MAGIC:
			path = "Bonus/MagicBonusEntity";
			break;
		case EITEM.ITEM_DIAMOND:
			path = "Bonus/DiamondBonusEntity";
			break;
		}
		Object @object = Resources.Load(path);
		if (Network.isServer)
		{
			Network.Instantiate(@object, Vector3.zero, Quaternion.identity, 0);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected)
		{
			Object.Instantiate(@object);
		}
	}

	public void AddBonus(GameObject _Bonus, EITEM _Item)
	{
		string empty = string.Empty;
		switch (_Item)
		{
		default:
			return;
		case EITEM.ITEM_AUTOLOCK_PIE:
			empty = "AutolockPie" + nbAutolockPie;
			break;
		case EITEM.ITEM_PIE:
			empty = "Pie" + nbPie;
			break;
		case EITEM.ITEM_SPRING:
			empty = "Spring" + nbSpring;
			break;
		case EITEM.ITEM_UFO:
			empty = "UFO0";
			break;
		case EITEM.ITEM_MAGIC:
			empty = "Magic" + nbMagic;
			break;
		case EITEM.ITEM_DIAMOND:
			empty = "Diamond" + nbDiamond;
			break;
		case EITEM.ITEM_LASAGNA:
		case EITEM.ITEM_NAP:
		case EITEM.ITEM_PARFUME:
			return;
		}
		GameObject gameObject = new GameObject(empty);
		_Bonus.transform.parent = gameObject.transform;
		BonusEntity bonusEntity = null;
		switch (_Item)
		{
		case EITEM.ITEM_AUTOLOCK_PIE:
			m_pAutolockPieEntities[nbAutolockPie] = _Bonus.GetComponentInChildren<AutolockPieBonusEntity>();
			m_pAutolockPieEntities[nbAutolockPie].Index = nbAutolockPie;
			nbAutolockPie++;
			break;
		case EITEM.ITEM_PIE:
			m_pPieEntities[nbPie] = _Bonus.GetComponentInChildren<PieBonusEntity>();
			m_pPieEntities[nbPie].Index = nbPie;
			nbPie++;
			break;
		case EITEM.ITEM_SPRING:
			m_pSpringEntities[nbSpring++] = _Bonus.GetComponentInChildren<SpringBonusEntity>();
			break;
		case EITEM.ITEM_UFO:
			m_pUFOEntity = _Bonus.GetComponentInChildren<UFOBonusEntity>();
			break;
		case EITEM.ITEM_MAGIC:
		{
			bonusEntity = null;
			for (int i = 0; i < _Bonus.transform.childCount; i++)
			{
				if (!(bonusEntity == null))
				{
					break;
				}
				bonusEntity = _Bonus.transform.GetChild(i).GetComponent<MagicBonusEntity>();
			}
			m_pMagicEntities[nbMagic++] = (MagicBonusEntity)bonusEntity;
			break;
		}
		case EITEM.ITEM_DIAMOND:
			m_pDiamondEntities[nbDiamond++] = _Bonus.GetComponentInChildren<DiamondBonusEntity>();
			break;
		case EITEM.ITEM_LASAGNA:
		case EITEM.ITEM_NAP:
		case EITEM.ITEM_PARFUME:
			break;
		}
	}

	public void RequestBonus(EITEM _Item, Kart _Kart, bool _Behind)
	{
		switch (_Item)
		{
		case EITEM.ITEM_LASAGNA:
			_Kart.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_BOOST);
			_Kart.KartSound.PlaySound(17);
			break;
		case EITEM.ITEM_PIE:
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				BonusEntity bonusEntity3 = ActiveLastUsed(_Item, _Kart);
				(bonusEntity3 as PieBonusEntity).Launch(_Behind);
			}
			else if (Network.isServer)
			{
				BonusEntity bonusEntity4 = ActiveLastUsed(_Item, _Kart);
				(bonusEntity4 as PieBonusEntity).NetLaunch(_Kart.networkViewID, _Behind);
			}
			break;
		case EITEM.ITEM_AUTOLOCK_PIE:
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				BonusEntity bonusEntity7 = ActiveLastUsed(_Item, _Kart);
				(bonusEntity7 as AutolockPieBonusEntity).Launch(_Kart, _Behind);
			}
			else if (Network.isServer)
			{
				BonusEntity bonusEntity8 = ActiveLastUsed(_Item, _Kart);
				(bonusEntity8 as AutolockPieBonusEntity).NetLaunch(_Kart.networkViewID, _Behind);
			}
			break;
		case EITEM.ITEM_SPRING:
			if (_Behind)
			{
				if (Network.peerType == NetworkPeerType.Disconnected)
				{
					BonusEntity bonusEntity9 = ActiveLastUsed(_Item, _Kart);
					(bonusEntity9 as SpringBonusEntity).Launch();
				}
				else if (Network.isServer)
				{
					BonusEntity bonusEntity10 = ActiveLastUsed(_Item, _Kart);
					(bonusEntity10 as SpringBonusEntity).NetLaunch(_Kart.networkViewID);
				}
			}
			else
			{
				_Kart.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_JUMP);
			}
			break;
		case EITEM.ITEM_NAP:
		{
			int rank = _Kart.RaceStats.GetRank();
			Singleton<GameManager>.Instance.SoundManager.PlaySound(ERaceSounds.NapUsed);
			_Kart.Anim.LaunchSuccessAnim(true);
			_Kart.KartSound.PlayVoice((!(Random.value > 0.5f)) ? KartSound.EVoices.Good : KartSound.EVoices.Good2);
			for (int i = 0; i < Singleton<GameManager>.Instance.GameMode.PlayerCount; i++)
			{
				Kart kart = Singleton<GameManager>.Instance.GameMode.GetKart(i);
				if (!(kart != null) || kart.Index == _Kart.Index)
				{
					continue;
				}
				int rank2 = kart.RaceStats.GetRank();
				if (rank2 < rank)
				{
					ParfumeBonusEffect parfumeBonusEffect = (ParfumeBonusEffect)kart.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED);
					if (!parfumeBonusEffect.Activated || parfumeBonusEffect.StinkParfume)
					{
						((NapBonusEffect)kart.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_SLEPT)).Launcher = _Kart;
						kart.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_SLEPT);
					}
				}
			}
			break;
		}
		case EITEM.ITEM_MAGIC:
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				BonusEntity bonusEntity5 = ActiveLastUsed(_Item, _Kart);
				(bonusEntity5 as MagicBonusEntity).Launch();
			}
			else if (Network.isServer)
			{
				BonusEntity bonusEntity6 = ActiveLastUsed(_Item, _Kart);
				(bonusEntity6 as MagicBonusEntity).NetLaunch(_Kart.networkViewID);
			}
			break;
		case EITEM.ITEM_PARFUME:
			_Kart.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED);
			break;
		case EITEM.ITEM_UFO:
			m_pUFOEntity.Launcher = _Kart;
			m_pUFOEntity.Launch();
			break;
		case EITEM.ITEM_DIAMOND:
			if (Network.peerType == NetworkPeerType.Disconnected)
			{
				BonusEntity bonusEntity = ActiveLastUsed(_Item, _Kart);
				(bonusEntity as DiamondBonusEntity).Launch(_Behind);
			}
			else if (Network.isServer)
			{
				BonusEntity bonusEntity2 = ActiveLastUsed(_Item, _Kart);
				(bonusEntity2 as DiamondBonusEntity).NetLaunch(_Kart.networkViewID, _Behind);
			}
			break;
		}
	}

	public BonusEntity ActiveLastUsed(EITEM _Item, Kart _Kart)
	{
		BonusEntity[] _Tab = null;
		int lastUsed = GetLastUsed(_Item, ref _Tab);
		if (_Tab != null && _Tab[lastUsed] != null)
		{
			if (_Tab[lastUsed].Activate)
			{
				_Tab[lastUsed].SetActive(false);
			}
			_Tab[lastUsed].Launcher = _Kart;
			return _Tab[lastUsed];
		}
		return null;
	}

	public int GetLastUsed(EITEM _Item, ref BonusEntity[] _Tab)
	{
		float num = 0f;
		int result = 0;
		switch (_Item)
		{
		case EITEM.ITEM_PIE:
			_Tab = m_pPieEntities;
			break;
		case EITEM.ITEM_AUTOLOCK_PIE:
			_Tab = m_pAutolockPieEntities;
			break;
		case EITEM.ITEM_SPRING:
			_Tab = m_pSpringEntities;
			break;
		case EITEM.ITEM_MAGIC:
			_Tab = m_pMagicEntities;
			break;
		case EITEM.ITEM_DIAMOND:
			_Tab = m_pDiamondEntities;
			break;
		}
		for (int i = 0; i < _Tab.Length; i++)
		{
			if (_Tab[i].gameObject.activeSelf)
			{
				if (num == 0f || _Tab[i].Timer < num)
				{
					result = i;
					num = _Tab[i].Timer;
				}
				continue;
			}
			return i;
		}
		return result;
	}

	public void StartScene()
	{
		UfoLaunched = false;
		GameObject gameObject = GameObject.Find("SplineRespawn");
		RcMultiPath idealPath = null;
		if (gameObject != null)
		{
			idealPath = gameObject.GetComponent<RcMultiPath>();
		}
		for (int i = 0; i < 20; i++)
		{
			if (m_pAutolockPieEntities[i] != null)
			{
				m_pAutolockPieEntities[i].IdealPath = idealPath;
			}
		}
		for (int j = 0; j < 20; j++)
		{
			if (m_pPieEntities[j] != null)
			{
				m_pPieEntities[j].IdealPath = idealPath;
			}
		}
		m_pUFOEntity.IdealPath = idealPath;
		GameObject gameObject2 = GameObject.Find("Race");
		if (gameObject2 != null)
		{
			RcRace component = gameObject2.GetComponent<RcRace>();
			m_pUFOEntity.Race = component;
		}
	}
}
