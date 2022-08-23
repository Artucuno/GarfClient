using System.Collections.Generic;
using UnityEngine;

public class UFOBonusEntity : BonusEntity
{
	private Kart m_pTarget;

	public float SpeedForward;

	public RcRace Race;

	public RcMultiPath IdealPath;

	private MultiPathPosition m_PathPosition;

	private MultiPathPosition m_NextPathPosition;

	private float FirstDistanceToPath;

	public float EndDistance = 100f;

	private float m_fCurrentSpeed;

	protected Vector3 m_Direction;

	private bool CrossTarget;

	private float m_LastUfoDist;

	private float m_fSpecifiedTargetPos;

	private List<UFO> m_pUfo;

	private Ray m_pRay;

	private LayerMask m_IgnoreCollision;

	public float TimerToFinalDestination = 1f;

	private float m_fCurrentTimerFinalDestination;

	private int m_UfoToMove = -1;

	public float UfoHeight = 3f;

	private bool m_bCrossSection;

	private float m_fTimerToLeaveUfo = -1f;

	public float TimerToDestroyUFO = 20f;

	private float m_fCurrentTimerToDestroyUFO;

	private float m_fBonusDuration;

	public AudioSource SoundLaunched;

	public AudioSource SoundTravel;

	public AudioSource SoundDeploy;

	public float BonusDuration
	{
		get
		{
			return m_fBonusDuration;
		}
	}

	public UFOBonusEntity()
	{
		m_pTarget = null;
		IdealPath = null;
		m_PathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_NextPathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_Direction = Vector3.zero;
		CrossTarget = false;
		m_pUfo = new List<UFO>();
		m_pRay = default(Ray);
		m_eItem = EITEM.ITEM_UFO;
	}

	public override void Awake()
	{
		base.Awake();
		m_IgnoreCollision = LayerMask.NameToLayer("Everything") & ~LayerMask.NameToLayer("ColWallUFO");
		int num = 0;
		foreach (Transform item in base.transform)
		{
			if (item.GetComponent<UFO>() != null)
			{
				m_pUfo.Add(item.GetComponent<UFO>());
				num++;
			}
		}
		m_bSynchronizePosition = true;
		m_bSynchronizeRotation = true;
	}

	public override void Launch()
	{
		base.Launch();
		float num = SpeedForward + m_pLauncher.GetBonusMgr().GetBonusValue(EITEM.ITEM_UFO, EBonusCustomEffect.SPEED) * SpeedForward / 100f;
		m_fCurrentSpeed = num / 3.6f;
		if (Race != null)
		{
			m_fCurrentTimerToDestroyUFO = 0f;
			SetActive(true);
			UFOReset();
			m_fCurrentTimerFinalDestination = 0f;
			if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
			{
				int randomGood = Singleton<RandomManager>.Instance.Next(0, m_pUfo.Count - 1);
				ChooseGoodRay(randomGood);
			}
			m_pTarget = (Kart)Race.GetRankedVehicle(0).GetVehicle();
			if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
			{
				m_Direction = m_pLauncher.Transform.position + m_pLauncher.Transform.parent.rotation * new Vector3(0f, 0f, 3f);
				m_pTransform.rotation = m_pLauncher.Transform.rotation;
				m_pTransform.position = m_pLauncher.Transform.position + m_pLauncher.transform.up * 7f;
			}
			for (int i = 0; i < m_pUfo.Count; i++)
			{
				m_pUfo[i].Appear();
			}
			m_eState = BonusState.BONUS_LAUNCHED;
			if ((bool)SoundLaunched && (bool)SoundTravel)
			{
				SoundLaunched.Play();
				SoundTravel.Play();
			}
		}
	}

	public override void SetActive(bool _Active)
	{
		base.SetActive(_Active);
		ActivateGameObject(_Active);
		if (_Active)
		{
			if (IdealPath != null)
			{
				IdealPath.UpdateMPPosition(ref m_PathPosition, m_pLauncher.Transform.position, 0, 0, false);
				m_LastUfoDist = IdealPath.GetDistToEndLine(m_PathPosition);
			}
			if (Singleton<GameManager>.Instance.GameMode != null && Singleton<GameManager>.Instance.GameMode.Hud != null)
			{
				Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.SetUfoToNormal();
				Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.UpdateUFO(m_LastUfoDist);
			}
		}
		else
		{
			SetDefaultValues();
		}
		if (Singleton<GameManager>.Instance.GameMode != null && (bool)Singleton<GameManager>.Instance.GameMode.Hud && (bool)Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp)
		{
			Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.Ufo.enabled = _Active;
		}
	}

	public override void Update()
	{
		base.Update();
		float deltaTime = Time.deltaTime;
		if (IdealPath == null)
		{
			return;
		}
		if (m_bActive && m_pTarget != null)
		{
			if (m_eState == BonusState.BONUS_ANIMLAUNCHED)
			{
				m_fCurrentTimerFinalDestination += deltaTime;
				if (m_fCurrentTimerFinalDestination > TimerToFinalDestination)
				{
					UFOInPlace(m_UfoToMove);
				}
				else
				{
					MoveUFOToFinalDestination(deltaTime, m_UfoToMove);
				}
			}
			if (Network.peerType == NetworkPeerType.Disconnected || Network.isServer)
			{
				m_fCurrentTimerToDestroyUFO += deltaTime;
				if (m_fCurrentTimerToDestroyUFO > -1f && m_fCurrentTimerToDestroyUFO > TimerToDestroyUFO && m_eState < BonusState.BONUS_ANIMLAUNCHED)
				{
					RemoveUFO(true);
					m_fCurrentTimerToDestroyUFO = -1f;
				}
				Vector3 position = m_pTarget.GetPosition();
				if (m_eState == BonusState.BONUS_LAUNCHED)
				{
					m_pTarget = (Kart)Race.GetRankedVehicle(0).GetVehicle();
					bool flag = false;
					float distToEndLine = IdealPath.GetDistToEndLine(m_PathPosition);
					if (distToEndLine - m_LastUfoDist > 500f)
					{
						flag = true;
					}
					float distToEndLine2 = IdealPath.GetDistToEndLine(m_pTarget.RaceStats.GetGuidePosition());
					if (m_pTarget == m_pLauncher || (Mathf.Abs(distToEndLine2 - distToEndLine) < 15f && distToEndLine < distToEndLine2))
					{
						CrossTarget = true;
					}
					RcFastValuePath rcFastValuePath = null;
					if (IdealPath.BValuePaths)
					{
						rcFastValuePath = (RcFastValuePath)m_PathPosition.section.GetSimplePath();
					}
					if (CrossTarget && Mathf.Abs(((m_fSpecifiedTargetPos == 0f) ? distToEndLine2 : m_fSpecifiedTargetPos) - distToEndLine) > EndDistance && !flag && rcFastValuePath != null && (rcFastValuePath.GetIntPointValue(m_PathPosition.pathPosition.index) & 2) == 0 && !m_bCrossSection)
					{
						UFOArrived();
					}
					else
					{
						if (flag && Mathf.Abs(distToEndLine2 - m_LastUfoDist) < EndDistance)
						{
							m_fSpecifiedTargetPos = distToEndLine - EndDistance;
						}
						SynchronizePosition(distToEndLine);
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
						position = m_PathPosition.section.GetSimplePath().MoveOnPath(ref m_NextPathPosition.pathPosition, ref _distance, true) + Vector3.up * UfoHeight;
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
								if (CrossTarget && m_PathPosition.section.m_pAfterBranches.Length > 1)
								{
									m_bCrossSection = true;
								}
								else if (CrossTarget && m_bCrossSection && m_PathPosition.section.m_pAfterBranches.Length == 1)
								{
									m_bCrossSection = false;
								}
								m_NextPathPosition.pathPosition = PathPosition.UNDEFINED_POSITION;
								position = rcMultiPathSection.GetSimplePath().MoveOnPath(ref m_NextPathPosition.pathPosition, ref _distance, true) + Vector3.up * UfoHeight;
							}
						}
						m_Direction = (position - m_pTransform.position).normalized;
						if (m_Direction != Vector3.zero)
						{
							Quaternion quaternion = default(Quaternion);
							quaternion = Quaternion.LookRotation(m_Direction);
							if (m_pTransform.rotation != quaternion)
							{
								m_pTransform.rotation = quaternion;
							}
							m_pTransform.position += deltaTime * (m_Direction * m_fCurrentSpeed);
						}
					}
				}
				else if (m_eState == BonusState.BONUS_TRIGGERED)
				{
					bool flag2 = true;
					for (int j = 0; j < m_pUfo.Count; j++)
					{
						if (m_pUfo[j].AnimLeaveIsPlaying())
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						SynchronizeDestroy();
					}
				}
			}
			if (m_fTimerToLeaveUfo != -1f)
			{
				m_fTimerToLeaveUfo += deltaTime;
				if (m_fTimerToLeaveUfo > 0.8f)
				{
					LeaveUfo();
					m_fTimerToLeaveUfo = -1f;
				}
			}
		}
		if (Singleton<GameManager>.Instance.GameMode != null && Singleton<GameManager>.Instance.GameMode.Hud != null)
		{
			Singleton<GameManager>.Instance.GameMode.Hud.HudRadarComp.UpdateUFO(m_LastUfoDist);
		}
	}

	public override void DoDestroy()
	{
		base.DoDestroy();
		SetDefaultValues();
		SetActive(false);
		Singleton<BonusMgr>.Instance.UfoLaunched = false;
	}

	public void SetDefaultValues()
	{
		m_PathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_NextPathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_pTarget = null;
		CrossTarget = false;
		m_LastUfoDist = 0f;
		m_fSpecifiedTargetPos = 0f;
		m_UfoToMove = -1;
		m_bCrossSection = false;
		UFOReset();
		m_fTimerToLeaveUfo = -1f;
	}

	public void UFOArrived()
	{
		m_pRay.origin = m_pTransform.position;
		m_pRay.direction = m_pTransform.rotation * Vector3.left;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(m_pRay, out hitInfo, 25f, m_IgnoreCollision);
		m_pRay.direction = m_pTransform.rotation * Vector3.right;
		RaycastHit hitInfo2;
		bool flag2 = Physics.Raycast(m_pRay, out hitInfo2, 25f, m_IgnoreCollision);
		if (flag && flag2)
		{
			zero = hitInfo.point;
			zero2 = hitInfo2.point;
			float magnitude = (zero2 - zero).magnitude;
			magnitude /= 3f;
			SynchronizeUfoLaunch(magnitude, zero);
		}
	}

	public void UFOReset()
	{
		for (int i = 0; i < m_pUfo.Count; i++)
		{
			m_pUfo[i].Reset();
		}
	}

	public void UFOInPlace(int _UfoToMove)
	{
		m_eState = BonusState.BONUS_ONGROUND;
		if (_UfoToMove == -1)
		{
			for (int i = 0; i < m_pUfo.Count; i++)
			{
				m_pUfo[i].InPlace(true);
			}
		}
		else
		{
			m_pUfo[_UfoToMove].InPlace(false);
		}
		if ((bool)SoundTravel && (bool)SoundDeploy)
		{
			SoundDeploy.Play();
			SoundTravel.Stop();
		}
	}

	public void MoveUFOToFinalDestination(float _deltaTime, int _UfoToMove)
	{
		if (_UfoToMove == -1)
		{
			for (int i = 0; i < m_pUfo.Count; i++)
			{
				m_pUfo[i].Move(_deltaTime);
			}
		}
		else
		{
			m_pUfo[_UfoToMove].Move(_deltaTime);
		}
	}

	public void RemoveUFO(bool _All)
	{
		if (Network.peerType != 0 && m_pNetworkView != null)
		{
			m_pNetworkView.RPC("OnRemoveUFO", RPCMode.All, _All);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
		{
			DoRemoveUFO(_All);
		}
	}

	public void DoRemoveUFO(bool _All)
	{
		if (_All)
		{
			for (int i = 0; i < m_pUfo.Count; i++)
			{
				m_pUfo[i].DeactivateCollider();
			}
			m_fTimerToLeaveUfo = 0f;
		}
		else
		{
			m_pUfo[m_UfoToMove].PlayLeaveAnim();
			m_eState = BonusState.BONUS_TRIGGERED;
		}
		if ((bool)SoundDeploy)
		{
			SoundDeploy.Stop();
		}
	}

	public void MatchToTarget(UFO _Ufo, Kart _newTarget)
	{
		m_fBonusDuration = _newTarget.GetBonusMgr().GetBonusValue(EITEM.ITEM_UFO, EBonusCustomEffect.DURATION);
		m_eState = BonusState.BONUS_ANIMLAUNCHED;
		m_fCurrentTimerFinalDestination = 0f;
		m_pTarget = _newTarget;
		for (int i = 0; i < m_pUfo.Count; i++)
		{
			if (m_pUfo[i] != _Ufo)
			{
				m_pUfo[i].PlayLeaveAnim();
				continue;
			}
			m_UfoToMove = i;
			m_pUfo[i].MoveToTarget(m_pTarget, TimerToFinalDestination);
		}
	}

	public void ChooseGoodRay(int RandomGood)
	{
		if (Network.peerType != 0 && m_pNetworkView != null)
		{
			m_pNetworkView.RPC("OnChooseGoodRay", RPCMode.All, RandomGood);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
		{
			DoChooseGoodRay(RandomGood);
		}
	}

	public void DoChooseGoodRay(int RandomGood)
	{
		if (m_pUfo[RandomGood] != null)
		{
			m_pUfo[RandomGood].GoodRay = true;
		}
	}

	public void SynchronizeDestroy()
	{
		if (Network.isServer)
		{
			m_pNetworkView.RPC("OnSynchronizeDestroy", RPCMode.All);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected)
		{
			DoDestroy();
		}
	}

	public void LeaveUfo()
	{
		if (Network.peerType != 0 && m_pNetworkView != null)
		{
			m_pNetworkView.RPC("OnLeaveUfo", RPCMode.All);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
		{
			DoLeaveUfo();
		}
	}

	public void DoLeaveUfo()
	{
		for (int i = 0; i < m_pUfo.Count; i++)
		{
			m_pUfo[i].PlayLeaveAnim();
			m_eState = BonusState.BONUS_TRIGGERED;
		}
	}

	public void SynchronizePosition(float _UfoDist)
	{
		if (Network.peerType != 0 && m_pNetworkView != null)
		{
			m_pNetworkView.RPC("OnSynchronizePosition", RPCMode.All, _UfoDist);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
		{
			DoSynchronizePosition(_UfoDist);
		}
	}

	public void DoSynchronizePosition(float _UfoDist)
	{
		m_LastUfoDist = _UfoDist;
	}

	public void SynchronizeUfoLaunch(float Dist, Vector3 LeftPos)
	{
		if (Network.peerType != 0 && m_pNetworkView != null)
		{
			m_pNetworkView.RPC("OnSynchronizeUfoLaunch", RPCMode.All, Dist, LeftPos);
		}
		else if (Network.peerType == NetworkPeerType.Disconnected || m_pNetworkView == null)
		{
			DoSynchronizeUfoLaunch(Dist, LeftPos);
		}
	}

	public void DoSynchronizeUfoLaunch(float Dist, Vector3 LeftPos)
	{
		for (int i = 0; i < m_pUfo.Count; i++)
		{
			Vector3 finalPosition = LeftPos + m_pRay.direction * ((float)i * Dist + Dist / 2f);
			m_pUfo[i].Launch(finalPosition, TimerToFinalDestination, Dist);
		}
		if (m_eState == BonusState.BONUS_LAUNCHED)
		{
			m_eState = BonusState.BONUS_ANIMLAUNCHED;
		}
	}
}
