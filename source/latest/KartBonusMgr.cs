using System;
using UnityEngine;

public class KartBonusMgr : MonoBehaviour
{
	public const int MAX_NB_BONUS = 2;

	private KartBonus[] m_ItemTab = new KartBonus[2];

	private bool m_bActiveItem;

	private BonusEffectMgr m_pBonusEffectMgr;

	private Kart m_pParent;

	private PlayerCustom m_pCustomMgr;

	private HUDBonus m_pHudBonus;

	public Action<EITEM, bool> OnLaunchBonus;

	protected NetworkView netView;

	private bool m_bShowNapFadeIn;

	public HUDBonus HUDBonus
	{
		get
		{
			return m_pHudBonus;
		}
		set
		{
			if (m_pHudBonus != null)
			{
				HUDBonus pHudBonus = m_pHudBonus;
				pHudBonus.OnAnimationFinished = (Action<int>)Delegate.Remove(pHudBonus.OnAnimationFinished, new Action<int>(AnimationFinished));
			}
			m_pHudBonus = value;
			if (m_pHudBonus != null)
			{
				HUDBonus pHudBonus2 = m_pHudBonus;
				pHudBonus2.OnAnimationFinished = (Action<int>)Delegate.Combine(pHudBonus2.OnAnimationFinished, new Action<int>(AnimationFinished));
			}
		}
	}

	public PlayerCustom CustomMgr
	{
		get
		{
			return m_pCustomMgr;
		}
	}

	public void Awake()
	{
		m_bActiveItem = false;
		for (int i = 0; i < 2; i++)
		{
			m_ItemTab[i] = new KartBonus();
		}
		if (m_pHudBonus != null)
		{
			m_pHudBonus.ResetSlots();
		}
	}

	private void OnDestroy()
	{
		if (m_pHudBonus != null)
		{
			HUDBonus pHudBonus = m_pHudBonus;
			pHudBonus.OnAnimationFinished = (Action<int>)Delegate.Remove(pHudBonus.OnAnimationFinished, new Action<int>(AnimationFinished));
		}
		if (m_pBonusEffectMgr != null)
		{
			m_pBonusEffectMgr.Dispose();
			m_pBonusEffectMgr = null;
		}
	}

	public void Start()
	{
		m_pParent = base.gameObject.transform.parent.FindChild("Tunning").GetComponent<Kart>();
		m_pBonusEffectMgr = new BonusEffectMgr(m_pParent);
		m_pBonusEffectMgr.Start();
		m_pCustomMgr = base.transform.parent.FindChild("Base").GetComponent<PlayerCustom>();
		netView = base.networkView;
		m_bShowNapFadeIn = base.transform.parent.GetComponentInChildren<Kart>().GetControlType() == RcVehicle.ControlType.Human;
	}

	public void Update()
	{
		float deltaTime = Time.deltaTime;
		m_pBonusEffectMgr.Update();
		for (int i = 0; i < m_ItemTab.Length; i++)
		{
			if (m_ItemTab[i].m_bAnimated)
			{
				m_ItemTab[i].m_fSecureAnimationTimer += deltaTime;
				if (m_ItemTab[i].m_fSecureAnimationTimer > 5f)
				{
					m_ItemTab[i].m_bAnimated = false;
					m_ItemTab[i].m_fSecureAnimationTimer = 0f;
				}
			}
		}
	}

	public void ResetItems()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoResetItems();
		}
		else if (netView.isMine)
		{
			netView.RPC("DoResetItems", RPCMode.All);
		}
	}

	[RPC]
	public void DoResetItems()
	{
		for (int i = 0; i < 2; i++)
		{
			m_ItemTab[i].Reset();
		}
		m_bActiveItem = false;
		if (m_pHudBonus != null)
		{
			m_pHudBonus.ResetSlots();
		}
	}

	public void Respawn()
	{
		ResetItems();
		if (m_pBonusEffectMgr != null)
		{
			m_pBonusEffectMgr.Reset();
		}
	}

	public EITEM GetItem(int _Index)
	{
		if (_Index >= 0 && _Index < 2)
		{
			return m_ItemTab[_Index].m_eItem;
		}
		return EITEM.ITEM_NONE;
	}

	public int GetItemQuantity(int _Index)
	{
		if (_Index >= 0 && _Index < 2)
		{
			return m_ItemTab[_Index].m_iQuantity;
		}
		return 0;
	}

	public void SetItem(EITEM _item, int iQuantity)
	{
		if (Network.isServer)
		{
			netView.RPC("DoSetItem", RPCMode.All, (int)_item, m_pHudBonus != null || m_pParent.GetControlType() == RcVehicle.ControlType.Net, iQuantity);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoSetItem((int)_item, m_pHudBonus != null, iQuantity);
		}
	}

	[RPC]
	public void DoSetItem(int item, bool needToWaitAnim, int iQuantity)
	{
		if (item < 0 || item >= 10)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < 2; i++)
		{
			if (m_ItemTab[i].m_eItem == EITEM.ITEM_NONE && !flag)
			{
				if (item == 6)
				{
					Singleton<BonusMgr>.Instance.UfoLaunched = true;
				}
				else if (m_ItemTab[i].m_eItem == EITEM.ITEM_UFO)
				{
					Singleton<BonusMgr>.Instance.UfoLaunched = false;
				}
				m_ItemTab[i].m_eItem = (EITEM)item;
				EITEM item2 = (EITEM)item;
				if (item == 2)
				{
					item2 = EITEM.ITEM_PIE;
				}
				m_ItemTab[i].m_iQuantity = Math.Max(iQuantity, (int)GetBonusValue(item2, EBonusCustomEffect.QUANTITY));
				m_ItemTab[i].m_bAnimated = true;
				m_ItemTab[i].m_fSecureAnimationTimer = 0f;
				if (m_pHudBonus != null)
				{
					m_pHudBonus.StartAnimation(i, (EITEM)item);
				}
				else if (!needToWaitAnim)
				{
					m_ItemTab[i].m_bAnimated = false;
					m_ItemTab[i].m_fSecureAnimationTimer = 0f;
					m_bActiveItem = true;
				}
				break;
			}
			if (m_ItemTab[i].m_bAnimated)
			{
				flag = true;
			}
		}
	}

	public void AnimationFinished(int _SlotIndex)
	{
		if (Network.peerType != 0)
		{
			netView.RPC("DoAnimationFinished", RPCMode.All, _SlotIndex);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoAnimationFinished(_SlotIndex);
		}
	}

	[RPC]
	public void DoAnimationFinished(int _SlotIndex)
	{
		for (int i = 0; i < 2; i++)
		{
			if (m_ItemTab[_SlotIndex].m_bAnimated)
			{
				m_ItemTab[_SlotIndex].m_bAnimated = false;
				m_ItemTab[_SlotIndex].m_fSecureAnimationTimer = 0f;
				m_bActiveItem = true;
				if (m_pHudBonus != null && _SlotIndex == 0)
				{
					m_pHudBonus.SetQuantity(GetItemQuantity(_SlotIndex));
				}
			}
		}
	}

	public BonusEffectMgr GetBonusEffectMgr()
	{
		return m_pBonusEffectMgr;
	}

	public float GetBonusValue(EITEM _item, EBonusCustomEffect _effect)
	{
		if (m_pCustomMgr == null)
		{
			return 0f;
		}
		return m_pCustomMgr.GetBonusValue(_item, _effect);
	}

	public void ActivateBonus(bool _Behind)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoActivateBonus(_Behind);
		}
		else if (netView.isMine)
		{
			netView.RPC("DoActivateBonus", RPCMode.All, _Behind);
		}
	}

	[RPC]
	public void DoActivateBonus(bool _Behind)
	{
		if ((!m_pParent.IsOnGround() && !m_pParent.GetState(RcVehicle.eVehicleState.S_IS_RUNNING)) || !m_bActiveItem)
		{
			return;
		}
		EITEM l_Item = m_ItemTab[0].m_eItem;
		if (m_ItemTab[0].m_iQuantity < 2)
		{
			bool flag = false;
			for (int j = 1; j < 2; j++)
			{
				EITEM eItem = m_ItemTab[j].m_eItem;
				if (eItem != 0)
				{
					m_ItemTab[j - 1].Affect(m_ItemTab[j]);
					if (m_pHudBonus != null)
					{
						m_pHudBonus.AffectSlot(j - 1, j, _Behind);
					}
					m_ItemTab[j].Reset();
					if (eItem == EITEM.ITEM_UFO)
					{
						Singleton<BonusMgr>.Instance.UfoLaunched = true;
					}
					else if (l_Item == EITEM.ITEM_NAP && m_bShowNapFadeIn && NapBonusEffect.OnLaunched != null)
					{
						NapBonusEffect.OnLaunched();
					}
					flag = true;
				}
			}
			if (!flag)
			{
				m_ItemTab[0].Reset();
				if (l_Item == EITEM.ITEM_UFO)
				{
					Singleton<BonusMgr>.Instance.UfoLaunched = true;
				}
				else if (l_Item == EITEM.ITEM_NAP && m_bShowNapFadeIn && NapBonusEffect.OnLaunched != null)
				{
					NapBonusEffect.OnLaunched();
				}
				if (m_pHudBonus != null)
				{
					m_pHudBonus.Launch(0, _Behind);
				}
				m_bActiveItem = false;
			}
			else if (!flag && m_ItemTab[0].m_bAnimated)
			{
				m_bActiveItem = false;
			}
		}
		else
		{
			m_ItemTab[0].m_iQuantity--;
		}
		if (Singleton<ChallengeManager>.Instance.IsActive && m_pParent.GetControlType() == RcVehicle.ControlType.Human)
		{
			Singleton<ChallengeManager>.Instance.Notify(EChallengeSingleRaceObjective.NoBonus);
		}
		if (Singleton<GameConfigurator>.Instance.GameModeType == E_GameModeType.TUTORIAL)
		{
			((TutorialGameMode)Singleton<GameManager>.Instance.GameMode).UsedBonus(l_Item, _Behind);
		}
		Singleton<BonusMgr>.Instance.NullSafe(delegate(BonusMgr i)
		{
			i.RequestBonus(l_Item, m_pParent, _Behind);
		});
		if (OnLaunchBonus != null)
		{
			OnLaunchBonus(l_Item, _Behind);
		}
		if (m_pHudBonus != null)
		{
			m_pHudBonus.SetQuantity(GetItemQuantity(0));
		}
	}

	public bool CanGetItem()
	{
		for (int i = 0; i < 2; i++)
		{
			if (m_ItemTab[i].m_eItem == EITEM.ITEM_NONE)
			{
				return true;
			}
		}
		return false;
	}

	public void StartRace()
	{
		if (m_pParent.SelectedAdvantage == EAdvantage.BoostStart && m_pBonusEffectMgr != null)
		{
			m_pBonusEffectMgr.ActivateBonusEffect(EBonusEffect.BONUSEFFECT_BOOST);
		}
	}

	[RPC]
	public void OnStinkParfumeTriggerred(NetworkViewID vehicleId)
	{
		GameObject player = Singleton<GameManager>.Instance.GameMode.GetPlayer(vehicleId);
		if ((bool)player)
		{
			Kart componentInChildren = player.GetComponentInChildren<Kart>();
			DoStinkParfumeTriggerred(componentInChildren);
		}
	}

	public void DoStinkParfumeTriggerred(Kart pKart)
	{
		if ((bool)pKart)
		{
			pKart.GetBonusMgr().GetBonusEffectMgr().ActivateBonusEffect(EBonusEffect.BONUSEFFECT_SPIN);
			pKart.FxMgr.PlayKartFx(eKartFx.BadParfume);
			((ParfumeBonusEffect)pKart.BonusMgr.GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_ATTRACTED)).BadParfumeCollisionSound.Play();
		}
	}
}
