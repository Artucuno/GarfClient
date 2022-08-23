using UnityEngine;

[ExecuteInEditMode]
public class RcFastPathComp : MonoBehaviour
{
	public bool m_bLoop;

	public bool m_bDebugDraw;

	public Color m_cDebugColor;

	public Transform m_pTransform;

	public RcFastPathComp()
	{
		m_bLoop = true;
		m_bDebugDraw = false;
		m_cDebugColor = Color.green;
	}

	public bool IsLooping()
	{
		return m_bLoop;
	}

	public void SetLoop(bool val)
	{
		m_bLoop = val;
	}

	public void Start()
	{
		m_pTransform = base.transform;
	}

	public void OnDrawGizmos()
	{
		if (!m_bDebugDraw || !Application.isEditor)
		{
			return;
		}
		int childCount = m_pTransform.GetChildCount();
		if (childCount > 1)
		{
			Vector3 vector = m_pTransform.GetChild(0).position;
			Vector3 vector2 = vector;
			for (int i = 1; i < childCount; i++)
			{
				vector2 = m_pTransform.GetChild(i).position;
				Debug.DrawLine(vector, vector2, m_cDebugColor, 0f, false);
				vector = vector2;
			}
			if (m_bLoop)
			{
				Debug.DrawLine(vector, m_pTransform.GetChild(0).position, m_cDebugColor, 0f, false);
			}
		}
	}
}
