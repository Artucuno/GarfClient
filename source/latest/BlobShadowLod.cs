using UnityEngine;

public class BlobShadowLod : MonoBehaviour
{
	public Projector m_pBlobShadow;

	private void Start()
	{
		if ((bool)m_pBlobShadow && Shader.globalMaximumLOD > 500)
		{
			m_pBlobShadow.material.shader = Shader.Find("Garfield/ProjectorMultiply");
		}
	}

	private void Update()
	{
		m_pBlobShadow.enabled = false;
	}

	public void OnWillRenderObject()
	{
		if ((bool)m_pBlobShadow)
		{
			m_pBlobShadow.enabled = true;
		}
	}

	public void OnRenderObject()
	{
		if ((bool)m_pBlobShadow)
		{
			m_pBlobShadow.enabled = false;
		}
	}
}
