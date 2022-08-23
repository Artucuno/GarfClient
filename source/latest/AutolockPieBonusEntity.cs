using System;
using UnityEngine;

public class AutolockPieBonusEntity : PieBonusEntity
{
	public float QuitRailDist;

	private RcVehicle m_pTarget;

	private bool m_bOffRail;

	private MultiPathPosition m_NextPathPosition;

	private float FirstDistanceToPath;

	private KartArcadeGearBox m_pTargetGearBox;

	public float AddedSpeed;

	public float m_fCatchUpOnBoost = 0.25f;

	public AudioSource SoundLocked;

	public AutolockPieBonusEntity()
	{
		m_pTarget = null;
		QuitRailDist = 50f;
		m_NextPathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_bOffRail = false;
		m_pTargetGearBox = null;
		m_eItem = EITEM.ITEM_AUTOLOCK_PIE;
	}

	public override void Awake()
	{
		base.Awake();
		m_pTransform = base.transform;
	}

	public override void Update()
	{
		base.Update();
	}

	public void Launch(Kart _Launcher, bool _Behind)
	{
		Launch(_Behind);
		if (_Behind)
		{
			return;
		}
		if (_Launcher != null)
		{
			RcVehicleRaceStats raceStats = _Launcher.RaceStats;
			if (raceStats != null)
			{
				if (raceStats.GetRank() == 0)
				{
					m_pTarget = null;
				}
				else
				{
					m_pTarget = raceStats.GetPreceding();
					Kart kart = (Kart)m_pTarget;
					if (kart != null)
					{
						KartBonusMgr bonusMgr = kart.GetBonusMgr();
						bonusMgr.OnLaunchBonus = (Action<EITEM, bool>)Delegate.Combine(bonusMgr.OnLaunchBonus, new Action<EITEM, bool>(TargetLaunchBonus));
						kart.OnBeSwaped = (Action<Kart>)Delegate.Combine(kart.OnBeSwaped, new Action<Kart>(ChangeTarget));
						m_pTargetGearBox = m_pTarget.gameObject.transform.parent.GetComponentInChildren<KartArcadeGearBox>();
					}
				}
			}
		}
		m_bOffRail = false;
	}

	protected override void DisableHudRadar()
	{
		if (Singleton<GameManager>.Instance.GameMode != null && (bool)Singleton<GameManager>.Instance.GameMode.Hud && (bool)Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp)
		{
			Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.AutolockPies[_index].enabled = false;
		}
	}

	protected override void UpdateHudRadar(float pHorizontalDist, float pDistance)
	{
		if (Singleton<GameManager>.Instance.GameMode != null)
		{
			Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.UpdateAutolockPie(_index, pHorizontalDist, pDistance);
		}
	}

	protected override bool CheckHUDRadar()
	{
		bool flag = base.CheckHUDRadar();
		bool flag2 = !(m_pTarget == null) && m_pTarget == m_pHumanPlayer;
		return flag && flag2;
	}

	public void ChangeTarget(Kart pKart)
	{
		Kart kart = (Kart)m_pTarget;
		if (kart != null)
		{
			Kart kart2 = kart;
			kart2.OnBeSwaped = (Action<Kart>)Delegate.Remove(kart2.OnBeSwaped, new Action<Kart>(ChangeTarget));
			KartBonusMgr bonusMgr = kart.GetBonusMgr();
			bonusMgr.OnLaunchBonus = (Action<EITEM, bool>)Delegate.Remove(bonusMgr.OnLaunchBonus, new Action<EITEM, bool>(TargetLaunchBonus));
		}
		m_pTarget = pKart;
		kart = pKart;
		pKart.OnBeSwaped = (Action<Kart>)Delegate.Combine(pKart.OnBeSwaped, new Action<Kart>(ChangeTarget));
		KartBonusMgr bonusMgr2 = pKart.GetBonusMgr();
		bonusMgr2.OnLaunchBonus = (Action<EITEM, bool>)Delegate.Combine(bonusMgr2.OnLaunchBonus, new Action<EITEM, bool>(TargetLaunchBonus));
	}

	public override void SetActive(bool _Active)
	{
		base.SetActive(_Active);
		if (_Active)
		{
			if (!m_bBehind && IdealPath != null)
			{
				IdealPath.UpdateMPPosition(ref m_PathPosition, m_pLauncher.Transform.position, 0, 0, false);
			}
		}
		else
		{
			SetDefaultValues();
			DisableHudRadar();
		}
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if ((Network.peerType != 0 && !Network.isServer) || !m_bActive || !(m_pTarget != null))
		{
			return;
		}
		Vector3 vector = m_pTarget.GetPosition();
		if (!m_bOffRail && IdealPath != null)
		{
			float distToEndLine = IdealPath.GetDistToEndLine(m_PathPosition);
			float distToEndLine2 = IdealPath.GetDistToEndLine(m_pTarget.RaceStats.GetGuidePosition());
			if (Mathf.Abs(distToEndLine2 - distToEndLine) > QuitRailDist || distToEndLine2 > distToEndLine)
			{
				IdealPath.UpdateMPPosition(ref m_PathPosition, m_pTransform.position, 3, 1, false);
				m_NextPathPosition = m_PathPosition;
				if (FirstDistanceToPath == 0f)
				{
					FirstDistanceToPath = 5f + Mathf.Clamp(Mathf.Abs(m_PathPosition.section.GetSimplePath().GetSignedDistToSegment2D(m_pTransform.position, m_PathPosition.pathPosition.index, Vector3.up)), 0f, 50f);
				}
				else if (FirstDistanceToPath > 5f && Mathf.Abs(m_PathPosition.section.GetSimplePath().GetSignedDistToSegment2D(m_pTransform.position, m_PathPosition.pathPosition.index, Vector3.up)) < 1f)
				{
					FirstDistanceToPath = 5f;
				}
				float _distance = FirstDistanceToPath;
				vector = m_PathPosition.section.GetSimplePath().MoveOnPath(ref m_NextPathPosition.pathPosition, ref _distance, true);
				if (m_NextPathPosition.pathPosition.index == m_PathPosition.section.GetSimplePath().GetNbPoints() - 1 && _distance > 0f)
				{
					RcMultiPathSection rcMultiPathSection = null;
					float num = 1E+38f;
					RcMultiPathSection[] pAfterBranches = m_PathPosition.section.m_pAfterBranches;
					foreach (RcMultiPathSection rcMultiPathSection2 in pAfterBranches)
					{
						if (m_pTarget.RaceStats.GetGuidePosition().section == rcMultiPathSection2)
						{
							rcMultiPathSection = rcMultiPathSection2;
							break;
						}
						if ((bool)rcMultiPathSection2 && rcMultiPathSection2.GetDistToEndLine() < num)
						{
							rcMultiPathSection = rcMultiPathSection2;
							num = rcMultiPathSection2.GetDistToEndLine();
						}
					}
					if (rcMultiPathSection != null)
					{
						m_NextPathPosition.pathPosition = PathPosition.UNDEFINED_POSITION;
						vector = rcMultiPathSection.GetSimplePath().MoveOnPath(ref m_NextPathPosition.pathPosition, ref _distance, true);
					}
				}
			}
			else
			{
				SynchronizeOffRail();
				if (m_pTargetGearBox != null)
				{
					m_fCurrentSpeed = m_pTargetGearBox.GetBaseMaxSpeed() + AddedSpeed;
					m_fCurrentSpeed += m_pLauncher.GetBonusMgr().GetBonusValue(EITEM.ITEM_PIE, EBonusCustomEffect.SPEED) * m_fCurrentSpeed / 100f;
				}
			}
		}
		m_Direction = (vector - m_pTransform.position).normalized;
		if (m_Direction != Vector3.zero && !m_bBehind && m_pRigidBody.gameObject.activeInHierarchy)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion = Quaternion.LookRotation(m_Direction);
			if (m_pTransform.rotation != quaternion)
			{
				m_pTransform.rotation = quaternion;
			}
			if (m_pTarget.IsBoosting())
			{
				m_pRigidBody.velocity = m_Direction * (m_fCurrentSpeed * (1f - m_fCatchUpOnBoost) + m_pTargetGearBox.GetMaxSpeed() * m_fCatchUpOnBoost);
			}
			else
			{
				m_pRigidBody.velocity = m_Direction * m_fCurrentSpeed;
			}
			m_pRigidBody.angularVelocity = m_Direction * 3f;
		}
	}

	public override void DoDestroy()
	{
		base.DoDestroy();
		if ((bool)SoundLocked)
		{
			SoundLocked.Stop();
		}
		SetDefaultValues();
	}

	public void SetDefaultValues()
	{
		m_PathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_NextPathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_bOffRail = false;
		if (m_pTarget != null)
		{
			KartBonusMgr bonusMgr = ((Kart)m_pTarget).GetBonusMgr();
			bonusMgr.OnLaunchBonus = (Action<EITEM, bool>)Delegate.Remove(bonusMgr.OnLaunchBonus, new Action<EITEM, bool>(TargetLaunchBonus));
			Kart obj = (Kart)m_pTarget;
			obj.OnBeSwaped = (Action<Kart>)Delegate.Remove(obj.OnBeSwaped, new Action<Kart>(ChangeTarget));
			m_pTarget = null;
		}
		FirstDistanceToPath = 0f;
		m_pTargetGearBox = null;
	}

	private void TargetLaunchBonus(EITEM _Item, bool _Behind)
	{
		if (_Item != EITEM.ITEM_SPRING || _Behind || !m_bOffRail)
		{
			return;
		}
		Kart kart = (Kart)m_pTarget;
		if (kart != null)
		{
			KartBonusMgr bonusMgr = kart.GetBonusMgr();
			bonusMgr.OnLaunchBonus = (Action<EITEM, bool>)Delegate.Remove(bonusMgr.OnLaunchBonus, new Action<EITEM, bool>(TargetLaunchBonus));
			kart.OnBeSwaped = (Action<Kart>)Delegate.Remove(kart.OnBeSwaped, new Action<Kart>(ChangeTarget));
		}
		m_pTarget = m_pTarget.RaceStats.GetPreceding();
		if (m_pTarget.RaceStats.GetRank() == 5)
		{
			m_pTarget = null;
			return;
		}
		ComputeBaseSpeed();
		m_bOffRail = false;
		if ((bool)SoundLocked && (bool)SoundTravel)
		{
			SoundTravel.Play();
			SoundLocked.Stop();
		}
	}

	public override void DoStickOnGround(Vector3 normal)
	{
		base.DoStickOnGround(normal);
		if ((bool)SoundLocked)
		{
			SoundLocked.Stop();
		}
	}

	public void SynchronizeOffRail()
	{
		if (m_bActive)
		{
			if (Network.peerType != 0 && m_pNetworkView != null)
			{
				m_pNetworkView.RPC("OnSynchronizeOffRail", RPCMode.All);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
			{
				DoSynchronizeOffRail();
			}
		}
	}

	public void DoSynchronizeOffRail()
	{
		m_bOffRail = true;
		if ((bool)SoundLocked && (bool)SoundTravel)
		{
			SoundTravel.Stop();
			SoundLocked.Play();
		}
	}
}
