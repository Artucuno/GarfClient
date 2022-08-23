using UnityEngine;

public class NamePlate : MonoBehaviour
{
	public GUIText m_cDisplay;

	public Vector3 m_vOffset = Vector3.up * 2f;

	public Transform m_oTargetTransform;

	private float m_fDisplayDistance = 20f;

	private float m_fSquaredDisplayDistance;

	private Camera m_oMainCam;

	private Transform m_oThisTransform;

	private Transform m_oCamTransform;

	private void Start()
	{
		m_oThisTransform = base.transform;
		m_oMainCam = Camera.main;
		m_oCamTransform = m_oMainCam.transform;
		m_fSquaredDisplayDistance = m_fDisplayDistance * m_fDisplayDistance;
	}

	public void setTarget(Transform T)
	{
		m_oTargetTransform = T;
	}

	public void setDisplayDistance(float D)
	{
		m_fDisplayDistance = D;
		m_fSquaredDisplayDistance = m_fDisplayDistance * m_fDisplayDistance;
	}

	private void Update()
	{
		float sqrMagnitude = (m_oCamTransform.position - m_oTargetTransform.position).sqrMagnitude;
		if (sqrMagnitude < m_fSquaredDisplayDistance)
		{
			Vector3 position = m_oMainCam.WorldToViewportPoint(m_oTargetTransform.position + m_vOffset);
			if (position.z > 0f)
			{
				m_cDisplay.material.color = new Color(1f, 1f, 1f, (m_fSquaredDisplayDistance - sqrMagnitude) / m_fSquaredDisplayDistance);
				m_cDisplay.enabled = true;
				m_oThisTransform.position = position;
			}
			else
			{
				m_cDisplay.enabled = false;
			}
		}
		else
		{
			m_cDisplay.enabled = false;
		}
	}
}
