using UnityEngine;

public class TimeTrialUFO : MonoBehaviour
{
	private RcRace m_Race;

	private RcFastPath m_cPathToTake;

	private PathPosition m_PathPosition = PathPosition.UNDEFINED_POSITION;

	private PathPosition m_NextPathPosition = PathPosition.UNDEFINED_POSITION;

	private float FirstDistanceToPath;

	private float m_fCurrentSpeed;

	protected Vector3 m_Direction = Vector3.zero;

	public float UfoHeight = 3f;

	private bool _Active;

	private Transform m_pTransform;

	private int m_iTimeToBeatMs;

	private int m_iNbLap;

	private float m_fPathLength;

	private bool m_bFirstStartCross;

	private float m_fBaseSpeed;

	private float m_fCrossDistance;

	public float XOffset;

	private void Awake()
	{
		m_pTransform = base.transform;
		base.renderer.enabled = false;
	}

	private void FixedUpdate()
	{
		if (!_Active || m_cPathToTake == null)
		{
			return;
		}
		int num = (int)(Time.fixedDeltaTime * 1000f);
		float num2 = (float)num / 1000f;
		m_cPathToTake.UpdatePathPosition(ref m_PathPosition, m_pTransform.position, 3, 1, false, true);
		m_NextPathPosition = m_PathPosition;
		if (FirstDistanceToPath == 0f)
		{
			FirstDistanceToPath = 5f + Mathf.Clamp(Mathf.Abs(m_cPathToTake.GetSignedDistToSegment2D(m_pTransform.position, m_PathPosition.index, Vector3.up)), 0f, 50f);
		}
		else if (FirstDistanceToPath > 5f && Mathf.Abs(m_cPathToTake.GetSignedDistToSegment2D(m_pTransform.position, m_PathPosition.index, Vector3.up)) < 1f)
		{
			FirstDistanceToPath = 5f;
		}
		float _distance = FirstDistanceToPath;
		Vector3 vector = m_cPathToTake.MoveOnPath(ref m_NextPathPosition, ref _distance, true) + Vector3.up * UfoHeight;
		m_Direction = (vector - m_pTransform.position).normalized;
		if (m_Direction != Vector3.zero)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion = Quaternion.LookRotation(m_Direction);
			if (m_pTransform.rotation != quaternion)
			{
				m_pTransform.rotation = quaternion;
			}
			m_pTransform.position += num2 * (m_Direction * m_fCurrentSpeed);
		}
		if (m_iNbLap < 0)
		{
			m_fCurrentSpeed = m_fBaseSpeed;
			return;
		}
		m_iTimeToBeatMs -= num;
		float distToEndOfPath = m_cPathToTake.GetDistToEndOfPath(m_NextPathPosition.index, m_NextPathPosition.ratio);
		int num3 = m_iNbLap;
		if (!m_bFirstStartCross && m_NextPathPosition.index == 0)
		{
			num3--;
		}
		distToEndOfPath += m_fPathLength * (float)num3 - m_fCrossDistance;
		if (distToEndOfPath > 1f && m_iTimeToBeatMs > 200)
		{
			m_fCurrentSpeed = distToEndOfPath / ((float)m_iTimeToBeatMs / 1000f);
		}
	}

	public void Place()
	{
		GameObject gameObject = GameObject.Find("Race");
		m_Race = null;
		if (gameObject != null)
		{
			m_Race = gameObject.GetComponent<RcRace>();
		}
		GameObject gameObject2 = GameObject.Find("Paths" + Singleton<GameConfigurator>.Instance.StartScene + "(Clone)");
		if (gameObject2 != null)
		{
			AIPathHandler component = gameObject2.GetComponent<AIPathHandler>();
			if ((bool)component)
			{
				m_cPathToTake = component.GetFirstPath(E_PathType.GOOD);
			}
		}
		if (m_cPathToTake != null)
		{
			GameObject gameObject3 = GameObject.Find("Start");
			if (gameObject3 != null)
			{
				Transform child = gameObject3.transform.GetChild(gameObject3.transform.GetChildCount() - 1);
				m_pTransform.position = child.position + Vector3.up * UfoHeight + child.right * XOffset;
			}
			m_cPathToTake.UpdatePathPosition(ref m_PathPosition, m_pTransform.position, 0, 0, false, true);
			m_fPathLength = m_cPathToTake.GetTotalLength(true);
			float distToEndOfPath = m_cPathToTake.GetDistToEndOfPath(m_PathPosition.index, m_PathPosition.ratio);
			m_iNbLap = m_Race.GetRaceNbLap();
			distToEndOfPath += m_fPathLength * (float)m_iNbLap;
			string startScene = Singleton<GameConfigurator>.Instance.StartScene;
			TimeTrialConfig component2 = m_Race.GetComponent<TimeTrialConfig>();
			E_TimeTrialMedal medal = Singleton<GameSaveManager>.Instance.GetMedal(startScene, true);
			m_iTimeToBeatMs = 0;
			switch (medal)
			{
			case E_TimeTrialMedal.Platinium:
				Singleton<GameSaveManager>.Instance.GetTimeTrialRecord(startScene, ref m_iTimeToBeatMs);
				break;
			case E_TimeTrialMedal.Gold:
				m_iTimeToBeatMs = component2.Platinium;
				break;
			case E_TimeTrialMedal.Silver:
				m_iTimeToBeatMs = component2.Gold;
				break;
			case E_TimeTrialMedal.Bronze:
				m_iTimeToBeatMs = component2.Silver;
				break;
			case E_TimeTrialMedal.None:
				m_iTimeToBeatMs = component2.Bronze;
				break;
			default:
				m_iTimeToBeatMs = 1;
				break;
			}
			m_fCurrentSpeed = distToEndOfPath / ((float)m_iTimeToBeatMs / 1000f);
			m_fBaseSpeed = m_fCurrentSpeed;
			m_bFirstStartCross = false;
		}
		base.renderer.enabled = true;
	}

	public void Launch()
	{
		_Active = true;
	}

	public void CrossStartLine()
	{
		if (!m_bFirstStartCross)
		{
			m_bFirstStartCross = true;
		}
		if (m_iNbLap == 0)
		{
		}
		m_iNbLap--;
		m_fCrossDistance = m_cPathToTake.GetDistToEndOfPath(m_NextPathPosition.index, m_NextPathPosition.ratio);
		if (m_fCrossDistance > m_fPathLength * 0.5f)
		{
			m_fCrossDistance -= m_fPathLength;
		}
	}
}
