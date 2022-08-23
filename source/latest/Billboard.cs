using UnityEngine;

public class Billboard : MonoBehaviour
{
	public Camera m_Camera;

	private Transform m_pTransform;

	private Transform m_pCamTransform;

	private void Start()
	{
		if (m_Camera == null)
		{
			SetCamera(GameObject.FindGameObjectWithTag("MainCamera").camera);
		}
		m_pTransform = base.transform;
	}

	public void SetCamera(Camera C)
	{
		m_Camera = C;
		m_pCamTransform = C.transform;
	}

	private void Update()
	{
		m_pTransform.LookAt(m_pTransform.position - m_pCamTransform.rotation * Vector3.back, m_pCamTransform.rotation * Vector3.up);
	}
}
