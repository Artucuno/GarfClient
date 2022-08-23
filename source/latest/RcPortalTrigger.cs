using UnityEngine;

public class RcPortalTrigger : MonoBehaviour
{
	public enum PortalType
	{
		Vertical,
		Horizontal
	}

	public enum PortalSide
	{
		None,
		Normal,
		Reverse,
		Both
	}

	public enum PortalAction
	{
		StartLine,
		EndLine,
		CheckPoint,
		RefreshRespawn
	}

	public PortalType m_eTriggerType;

	public PortalSide m_eTriggerSide;

	public PortalAction m_eActionType;

	public int m_iId;

	private PortalSide m_eSideTriggered;

	private Vector3 m_vBL;

	private Vector3 m_vTR;

	private Vector3 m_vBLtoTL;

	private Vector3 m_vBLtoBR;

	private Vector3 m_vNormal;

	private RcRace m_pRace;

	public RcPortalTrigger()
	{
		m_eTriggerType = PortalType.Vertical;
		m_eTriggerSide = PortalSide.None;
		m_eSideTriggered = PortalSide.None;
		m_iId = 0;
		m_eActionType = PortalAction.StartLine;
		m_pRace = null;
	}

	public void Awake()
	{
		if (base.networkView == null)
		{
		}
		base.networkView.stateSynchronization = NetworkStateSynchronization.ReliableDeltaCompressed;
		base.networkView.observed = null;
	}

	public void Start()
	{
		m_pRace = (RcRace)Object.FindObjectOfType(typeof(RcRace));
		BoxCollider boxCollider = (BoxCollider)base.collider;
		Vector3 size = boxCollider.size;
		m_vTR = base.transform.position + base.transform.rotation * new Vector3(size.x, size.y, 0f);
		m_vBL = base.transform.position + base.transform.rotation * new Vector3(0f - size.x, 0f - size.y, 0f);
		m_vBLtoTL = (m_vBLtoBR = m_vTR - m_vBL);
		if (m_eTriggerType == PortalType.Vertical)
		{
			m_vBLtoBR.y = 0f;
			m_vBLtoTL.x = 0f;
			m_vBLtoTL.z = 0f;
		}
		else if (m_eTriggerType == PortalType.Horizontal)
		{
			m_vBLtoBR.z = 0f;
			m_vBLtoTL.x = 0f;
			m_vBLtoTL.y = 0f;
		}
		m_vNormal = Vector3.Cross(m_vBLtoBR, m_vBLtoTL);
		m_vNormal.Normalize();
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if (!base.enabled || !IsTriggeredBy(other))
		{
			return;
		}
		RcVehicleRaceStats componentInChildren = other.gameObject.GetComponentInChildren<RcVehicleRaceStats>();
		if ((bool)componentInChildren && (bool)m_pRace)
		{
			if (Network.isServer)
			{
				NetworkViewID viewID = other.gameObject.networkView.viewID;
				base.networkView.RPC("VehicleTriggerred", RPCMode.All, viewID, (int)m_eSideTriggered);
			}
			else if (Network.peerType == NetworkPeerType.Disconnected)
			{
				DoVehicleTriggerred(componentInChildren, (int)m_eSideTriggered);
			}
		}
	}

	public void DoVehicleTriggerred(RcVehicleRaceStats pStats, int side)
	{
		if (!pStats)
		{
			return;
		}
		switch (side)
		{
		case 1:
			if (m_eActionType == PortalAction.StartLine)
			{
				m_pRace.CrossStartLine(pStats, false);
			}
			else if (m_eActionType == PortalAction.EndLine)
			{
				m_pRace.CrossEndLine(pStats);
			}
			else if (m_eActionType == PortalAction.CheckPoint)
			{
				m_pRace.CrossCheckPoint(pStats, m_iId);
			}
			else if (m_eActionType == PortalAction.RefreshRespawn)
			{
				m_pRace.ForceRefreshRespawn(pStats);
			}
			break;
		case 2:
			if (m_eActionType == PortalAction.StartLine)
			{
				m_pRace.CrossStartLine(pStats, true);
			}
			else if (m_eActionType == PortalAction.CheckPoint)
			{
				m_pRace.CrossCheckPoint(pStats, m_iId);
			}
			else if (m_eActionType == PortalAction.RefreshRespawn)
			{
				m_pRace.ForceRefreshRespawn(pStats);
			}
			break;
		}
	}

	[RPC]
	public void VehicleTriggerred(NetworkViewID vehicleId, int side)
	{
		if ((bool)m_pRace)
		{
			RcVehicleRaceStats vehicleStats = m_pRace.GetVehicleStats(vehicleId);
			DoVehicleTriggerred(vehicleStats, side);
		}
	}

	public bool IsTriggeredBy(Collider other)
	{
		if (m_eTriggerSide == PortalSide.None)
		{
			return false;
		}
		RcVehicle componentInChildren = other.gameObject.GetComponentInChildren<RcVehicle>();
		if (componentInChildren == null)
		{
			return false;
		}
		Vector3 position = componentInChildren.GetPosition();
		Vector3 lastFramePos = componentInChildren.GetLastFramePos();
		if ((m_eTriggerSide == PortalSide.Normal || m_eTriggerSide == PortalSide.Both) && IsCrossingPortal(lastFramePos, position))
		{
			m_eSideTriggered = PortalSide.Normal;
			return true;
		}
		if ((m_eTriggerSide == PortalSide.Reverse || m_eTriggerSide == PortalSide.Both) && IsCrossingPortal(position, lastFramePos))
		{
			m_eSideTriggered = PortalSide.Reverse;
			return true;
		}
		return false;
	}

	public bool IsCrossingPortal(Vector3 _Start, Vector3 _End)
	{
		Vector3 lhs = _End - _Start;
		float num = Vector3.Dot(lhs, m_vNormal);
		return num >= 0f;
	}
}
