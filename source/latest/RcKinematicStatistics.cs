using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RcKinematicStatistics : MonoBehaviour
{
	public float fDebugScale = 0.05f;

	public int iStorageLimit = 1000;

	public bool Record;

	public bool Save;

	protected RcKinematicPhysic m_pComp;

	protected List<RcPhysStats> m_pRecord;

	protected float m_fCurTime;

	protected bool m_bRecording;

	protected static int s_iSaveIndex;

	public void Awake()
	{
		m_pComp = base.gameObject.GetComponent<RcKinematicPhysic>();
	}

	public void Start()
	{
	}

	public void StartRecording()
	{
		m_bRecording = true;
		m_fCurTime = 0f;
		m_pRecord = new List<RcPhysStats>();
	}

	public void StopRecording()
	{
		m_bRecording = false;
	}

	public void FixedUpdate()
	{
		if (m_bRecording && m_pComp != null)
		{
			if (iStorageLimit > 0 && m_pRecord.Count >= iStorageLimit)
			{
				m_pRecord.RemoveAt(0);
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			RcPhysStats item = default(RcPhysStats);
			item.fTime = m_fCurTime;
			item.vPos = m_pComp.GetVehicleBodyTransform().position;
			item.vCombineMv = m_pComp.CombineTrickMv;
			item.vOrient = m_pComp.GetVehicleBodyTransform().rotation;
			item.bFlying = m_pComp.Flying;
			item.bInertiaMode = m_pComp.BInertiaMode;
			item.fWheelSpeed = m_pComp.GetWheelSpeed();
			item.fWheelSpeedMs = m_pComp.GetVehicle().GetWheelSpeedMS();
			float fTurningRadius = 0f;
			float steeringAngle = m_pComp.GetVehicle().GetSteeringAngle();
			if (Mathf.Abs(steeringAngle) > 0.0001f)
			{
				float num = Mathf.Tan(steeringAngle);
				fTurningRadius = -1f * m_pComp.GetFrontToRearWheelLength() / num;
			}
			item.fTurningRadius = fTurningRadius;
			int num2 = 0;
			item.Wheel = new RcWheelStats[m_pComp.GetWheels().Length];
			RcPhysicWheel[] wheels = m_pComp.GetWheels();
			foreach (RcPhysicWheel rcPhysicWheel in wheels)
			{
				RcKinematicWheel rcKinematicWheel = rcPhysicWheel as RcKinematicWheel;
				item.Wheel[num2].bOnGround = rcKinematicWheel.BOnGround;
				item.Wheel[num2].fRestHeight = rcKinematicWheel.VRestContactPoint.y;
				item.Wheel[num2].fGroundHeight = rcKinematicWheel.OGroundCharac.point.y;
				item.Wheel[num2].fAnchorSpeed = rcKinematicWheel.AnchorSpeed;
				item.Wheel[num2].vGroundNormal = rcKinematicWheel.OGroundCharac.normal;
				item.Wheel[num2].iSurface = rcKinematicWheel.OGroundCharac.surface;
				item.Wheel[num2].fCompression = rcKinematicWheel.FStretch;
				num2++;
			}
			m_pRecord.Add(item);
			m_fCurTime += fixedDeltaTime;
		}
	}

	public void Update()
	{
		if (m_pComp.GetVehicle().GetControlType() == RcVehicle.ControlType.Human)
		{
			if (Input.GetKeyDown(KeyCode.T))
			{
				if (Time.fixedDeltaTime < 0.01f)
				{
					Time.fixedDeltaTime = 1f / 60f;
				}
				else
				{
					Time.fixedDeltaTime = 1f / 120f;
				}
			}
			if (Input.GetKeyDown(KeyCode.Y))
			{
				Record = !m_bRecording;
			}
			if (Input.GetKeyDown(KeyCode.U))
			{
				Save = true;
			}
		}
		if (Record != m_bRecording)
		{
			if (m_bRecording)
			{
				StopRecording();
			}
			else
			{
				StartRecording();
			}
		}
		if (Save)
		{
			Save = false;
			AutoSaveNew();
		}
	}

	private void OnDrawGizmos()
	{
		if (m_pRecord == null)
		{
			return;
		}
		foreach (RcPhysStats item in m_pRecord)
		{
			Gizmos.color = ((!item.bInertiaMode) ? ((!item.bFlying) ? Color.gray : Color.cyan) : ((!item.bFlying) ? Color.red : Color.magenta));
			Gizmos.DrawSphere(item.vPos, fDebugScale);
		}
	}

	public void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 200f, 50f), "Sim at : " + ((!(Time.fixedDeltaTime < 0.01f)) ? "60 fps" : "120 fps"));
	}

	public void AutoSaveNew()
	{
		string text;
		do
		{
			text = "CarPhysStats" + ++s_iSaveIndex + ".csv";
		}
		while (File.Exists(text));
		SaveAs(text);
	}

	public void SaveAs(string fileName)
	{
		using (StreamWriter streamWriter = new StreamWriter(fileName))
		{
			streamWriter.Write("Time; PosX; PosY; PosZ; TrickMvY; Ground; Inertia; VX; VY; VZ; WheelSpeed; SteeringRadius");
			RcPhysicWheel[] wheels = m_pComp.GetWheels();
			foreach (RcPhysicWheel rcPhysicWheel in wheels)
			{
				string text = "W";
				text += ((rcPhysicWheel.m_eAxle != 0) ? ((rcPhysicWheel.m_eAxle != RcPhysicWheel.WheelAxle.Rear) ? "C" : "R") : "F");
				text += ((rcPhysicWheel.m_eSide != 0) ? ((rcPhysicWheel.m_eSide != RcPhysicWheel.WheelSide.Right) ? "C" : "R") : "L");
				streamWriter.Write("; {0}_Height; {0}_GroundHeight; {0}_Compression; {0}_AnchorSpeed; {0}_NX; {0}_NY; {0}_NZ; {0}_Surface; {0}_Ground; {0}_VH; {0}_VGH", text);
			}
			streamWriter.WriteLine();
			bool flag = true;
			RcPhysStats rcPhysStats = default(RcPhysStats);
			foreach (RcPhysStats item in m_pRecord)
			{
				float num = 1f / Time.fixedDeltaTime;
				if (flag)
				{
					rcPhysStats = item;
				}
				else
				{
					num = 1f / (item.fTime - rcPhysStats.fTime);
				}
				streamWriter.Write("{0}; {1}; {2}; {3}; {4}; {5}; {6}", item.fTime, item.vPos.x, item.vPos.y, item.vPos.z, item.vCombineMv.y, (!item.bFlying) ? 1 : 0, item.bInertiaMode ? 1 : 0);
				float num2 = (item.vPos.x - rcPhysStats.vPos.x) * num;
				float num3 = (item.vPos.y - rcPhysStats.vPos.y) * num;
				float num4 = (item.vPos.z - rcPhysStats.vPos.z) * num;
				streamWriter.Write("; {0}; {1}; {2}; {3}; {4}", num2, num3, num4, item.fWheelSpeedMs, item.fTurningRadius);
				for (int j = 0; j < item.Wheel.Length; j++)
				{
					RcWheelStats rcWheelStats = item.Wheel[j];
					RcWheelStats rcWheelStats2 = rcPhysStats.Wheel[j];
					streamWriter.Write("; {0}; {1}; {2}; {3}", rcWheelStats.fRestHeight, rcWheelStats.fGroundHeight, rcWheelStats.fCompression, rcWheelStats.fAnchorSpeed);
					streamWriter.Write("; {0}; {1}; {2}; {3}; {4}", rcWheelStats.vGroundNormal.x, rcWheelStats.vGroundNormal.y, rcWheelStats.vGroundNormal.z, rcWheelStats.iSurface, rcWheelStats.bOnGround ? 1 : 0);
					float num5 = (rcWheelStats.fRestHeight - rcWheelStats2.fRestHeight) * num;
					float num6 = (rcWheelStats.fGroundHeight - rcWheelStats2.fGroundHeight) * num;
					if (!rcWheelStats.bOnGround || !rcWheelStats2.bOnGround)
					{
						num6 = 0f;
					}
					streamWriter.Write("; {0}; {1}", num5, num6);
				}
				streamWriter.WriteLine();
				flag = false;
				rcPhysStats = item;
			}
		}
	}
}
