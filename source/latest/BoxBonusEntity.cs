using System;
using System.Collections.Generic;
using UnityEngine;

public class BoxBonusEntity : BonusEntity
{
	public float LostDistance = 200f;

	public List<EITEM> DoNotGiveToLostPlayers = new List<EITEM>();

	public List<EITEM> MoreToLostPlayers = new List<EITEM>();

	[SerializeField]
	[HideInInspector]
	public int[] TwoBonusChance = new int[6];

	[SerializeField]
	[HideInInspector]
	public int[] ThreeBonusChance = new int[6];

	[SerializeField]
	[HideInInspector]
	public int[] BonusRatio = new int[6 * Enum.GetValues(typeof(EBonusCategory)).Length];

	[SerializeField]
	[HideInInspector]
	public int[] BonusRatioAI = new int[6 * Enum.GetValues(typeof(EBonusCategory)).Length];

	public LayerMask layer;

	public GameObject ParticleEffect;

	private GameObject _particleEffect;

	protected RcRace m_pRace;

	public BoxBonusEntity()
	{
		m_pRace = null;
	}

	public override void Awake()
	{
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TIME_TRIAL)
		{
			UnityEngine.Object.Destroy(base.gameObject.transform.parent.gameObject);
			return;
		}
		base.Awake();
		base.renderer.enabled = true;
		m_pCollider.enabled = true;
		m_bActive = true;
		m_pRace = UnityEngine.Object.FindObjectOfType(typeof(RcRace)) as RcRace;
		_particleEffect = (GameObject)UnityEngine.Object.Instantiate(ParticleEffect);
		_particleEffect.transform.parent = base.transform;
		_particleEffect.transform.localPosition = Vector3.zero;
	}

	public override void Start()
	{
		base.animation["BoxBonusEntityStand"].normalizedTime = (float)Singleton<RandomManager>.Instance.NextDouble();
	}

	public override void Update()
	{
		base.Update();
	}

	public override void OnTriggerEnter(Collider other)
	{
		int num = 1 << other.gameObject.layer;
		if ((num & (int)layer) != 0)
		{
			Kart componentInChildren = other.gameObject.GetComponentInChildren<Kart>();
			if ((bool)componentInChildren)
			{
				_particleEffect.particleSystem.Play();
				base.OnTriggerEnter(other);
			}
		}
	}

	public override void DoOnTriggerEnter(GameObject other, int otherlayer)
	{
		if (other != null)
		{
			Kart componentInChildren = other.GetComponentInChildren<Kart>();
			if ((bool)componentInChildren)
			{
				componentInChildren.KartSound.PlaySound(10);
				if (m_bActive && componentInChildren.GetBonusMgr().CanGetItem())
				{
					GiveItem(componentInChildren);
				}
			}
		}
		if (ReactivationDelay > 0f)
		{
			SetActive(false);
		}
	}

	public void GiveItem(Kart pKart)
	{
		if (pKart == null || pKart.GetBonusMgr() == null)
		{
			return;
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
		{
			TutorialGameMode tutorialGameMode = (TutorialGameMode)Singleton<GameManager>.Instance.GameMode;
			if (tutorialGameMode.WannaItem)
			{
				pKart.GetBonusMgr().SetItem(tutorialGameMode.DesiredItem, 0);
				tutorialGameMode.GotItem(tutorialGameMode.DesiredItem);
			}
		}
		RcVehicleRaceStats raceStats = pKart.RaceStats;
		int rank = raceStats.GetRank();
		if (rank >= 6)
		{
		}
		bool flag = pKart.GetControlType() == RcVehicle.ControlType.AI;
		int iQuantity = 1;
		if (!flag)
		{
			int num = UnityEngine.Random.Range(0, 101);
			if (num < ThreeBonusChance[rank])
			{
				iQuantity = 3;
			}
			else if (num < ThreeBonusChance[rank] + TwoBonusChance[rank])
			{
				iQuantity = 2;
			}
		}
		EAdvantage selectedAdvantage = pKart.SelectedAdvantage;
		if (selectedAdvantage >= PlayerConfig.FirstAdvantageBonus && selectedAdvantage <= PlayerConfig.LastAdvantageBonus && UnityEngine.Random.Range(0, 3) == 0)
		{
			switch (selectedAdvantage)
			{
			case EAdvantage.LasagnaBonus:
				pKart.GetBonusMgr().SetItem(EITEM.ITEM_LASAGNA, iQuantity);
				break;
			case EAdvantage.SpringBonus:
				pKart.GetBonusMgr().SetItem(EITEM.ITEM_SPRING, 0);
				break;
			case EAdvantage.PieBonus:
				pKart.GetBonusMgr().SetItem(EITEM.ITEM_PIE, iQuantity);
				break;
			}
			return;
		}
		int[] array = ((!flag) ? BonusRatio : BonusRatioAI);
		int num2 = rank * Enum.GetValues(typeof(EBonusCategory)).Length;
		if (!flag && raceStats.GetDistToEndOfRace() - raceStats.GetPreceding().RaceStats.GetDistToEndOfRace() > LostDistance)
		{
			int[] array2 = new int[Enum.GetValues(typeof(EBonusCategory)).Length];
			int num3 = 0;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[num2 + i];
			}
			foreach (EITEM doNotGiveToLostPlayer in DoNotGiveToLostPlayers)
			{
				num3 += array2[(int)(doNotGiveToLostPlayer - 1)];
				array2[(int)(doNotGiveToLostPlayer - 1)] = 0;
			}
			num3 = num3 / MoreToLostPlayers.Count + 1;
			foreach (EITEM moreToLostPlayer in MoreToLostPlayers)
			{
				array2[(int)(moreToLostPlayer - 1)] += num3;
			}
			array = array2;
			num2 = 0;
		}
		int num4 = UnityEngine.Random.Range(1, 101);
		int num5 = 0;
		for (int j = 0; j < Enum.GetValues(typeof(EBonusCategory)).Length; j++)
		{
			num5 += array[num2 + j];
			if (num4 > num5)
			{
				continue;
			}
			switch (j)
			{
				case 0:
					pKart.GetBonusMgr().SetItem(EITEM.ITEM_PIE, iQuantity);
					return;
				case 1:
					pKart.GetBonusMgr().SetItem(EITEM.ITEM_AUTOLOCK_PIE, iQuantity);
					return;
				case 2:
					pKart.GetBonusMgr().SetItem(EITEM.ITEM_SPRING, 0);
					return;
				case 3:
					pKart.GetBonusMgr().SetItem(EITEM.ITEM_LASAGNA, iQuantity);
					return;
				case 4:
					pKart.GetBonusMgr().SetItem(EITEM.ITEM_DIAMOND, 0);
					return;
				case 5:
					if (!Singleton<BonusMgr>.Instance.UfoLaunched)
					{
						pKart.GetBonusMgr().SetItem(EITEM.ITEM_UFO, 0);
					}
					else
					{
						pKart.GetBonusMgr().SetItem(EITEM.ITEM_PARFUME, 0);
					}
					return;
				case 7:
					pKart.GetBonusMgr().SetItem(EITEM.ITEM_PARFUME, 0);
					return;
				case 6:
					pKart.GetBonusMgr().SetItem(EITEM.ITEM_NAP, 0);
					return;
				case 8:
					pKart.GetBonusMgr().SetItem(EITEM.ITEM_MAGIC, iQuantity);
					return;
			}
		}
	}
}
