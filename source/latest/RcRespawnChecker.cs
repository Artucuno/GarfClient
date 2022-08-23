using UnityEngine;

public class RcRespawnChecker : MonoBehaviour
{
	private const float COS_85 = 0.0871f;

	private RcVehicle[] m_pVehicles;

	public bool m_bKillingCollOn;

	public bool m_bUnderwaterKillOn;

	public bool m_bUpsideDownKillOn;

	public bool m_bHyperspaceKillOn;

	public float m_fHyperspaceHeight;

	public float m_fSeaLevel;

	public RcRespawnChecker()
	{
		m_bKillingCollOn = true;
		m_bUnderwaterKillOn = true;
		m_bUpsideDownKillOn = true;
		m_bHyperspaceKillOn = true;
		m_fHyperspaceHeight = 1000f;
		m_fSeaLevel = -100f;
	}

	public void Awake()
	{
	}

	public void Start()
	{
		m_pVehicles = (RcVehicle[])Object.FindObjectsOfType(typeof(RcVehicle));
	}

	public void Update()
	{
		for (int i = 0; i < m_pVehicles.Length; i++)
		{
			float respawnDelay;
			if (m_pVehicles[i] != null && m_pVehicles[i].GetState(RcVehicle.eVehicleState.S_IS_RUNNING) && !m_pVehicles[i].GetState(RcVehicle.eVehicleState.S_IS_TELEPORTING) && !m_pVehicles[i].IsLocked() && CheckRespawn(m_pVehicles[i], out respawnDelay))
			{
				m_pVehicles[i].Kill(respawnDelay);
			}
		}
	}

	public bool CheckRespawn(RcVehicle pVehicle, out float respawnDelay)
	{
		respawnDelay = 1.5f;
		if (m_bKillingCollOn && pVehicle.GetVehiclePhysic().BTouchedDeath)
		{
			respawnDelay = pVehicle.GetVehiclePhysic().FDeathDelay;
			if (pVehicle.GetControlType() == RcVehicle.ControlType.Human)
			{
				Camera.mainCamera.GetComponent<CameraBase>().SwitchCamera(ECamState.Kill, ECamState.TransCut);
			}
			return true;
		}
		if (pVehicle.GetVehiclePhysic().GetVehicleBody().isKinematic)
		{
			return false;
		}
		if (m_bUpsideDownKillOn && Vector3.Dot(pVehicle.GetOrientation() * Vector3.up, Vector3.up) < 0.0871f && !pVehicle.IsOnGround())
		{
			return true;
		}
		if (m_bUnderwaterKillOn && Vector3.Dot(pVehicle.GetPosition(), Vector3.up) < m_fSeaLevel)
		{
			return true;
		}
		if (m_bHyperspaceKillOn && Vector3.Dot(pVehicle.GetPosition(), Vector3.up) > m_fHyperspaceHeight)
		{
			return true;
		}
		return false;
	}
}
