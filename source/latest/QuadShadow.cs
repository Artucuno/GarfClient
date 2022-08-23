using UnityEngine;

public class QuadShadow : MonoBehaviour
{
	private bool m_bUpdated;

	public float m_fMaxDistProj = 10f;

	public LayerMask m_oLayerProj;

	private Transform m_pBoneAttach;

	private void Start()
	{
		m_pBoneAttach = base.transform.parent;
	}

	private void Update()
	{
		m_bUpdated = false;
	}

	private void OnWillRenderObject()
	{
		if (!m_bUpdated)
		{
			ComputeQuadShadow();
		}
	}

	private void ComputeQuadShadow()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(m_pBoneAttach.position + Vector3.up, Vector3.down, out hitInfo, m_fMaxDistProj, m_oLayerProj.value))
		{
			base.transform.position = hitInfo.point;
			Quaternion identity = Quaternion.identity;
			Vector3 lhs = Vector3.Cross(m_pBoneAttach.up, hitInfo.normal);
			Vector3 vector = Vector3.Cross(lhs, hitInfo.normal);
			identity.SetLookRotation(hitInfo.normal, -vector);
			base.transform.rotation = identity;
			base.transform.localScale = Vector3.one;
		}
		else
		{
			base.transform.localScale = Vector3.zero;
		}
		m_bUpdated = true;
	}
}
