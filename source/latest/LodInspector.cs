using UnityEngine;

public class LodInspector : MonoBehaviour
{
	public int m_iLodId;

	public LodRemote m_pLodRemote;

	public void OnWillRenderObject()
	{
		if ((bool)m_pLodRemote)
		{
			m_pLodRemote.SetCurrentLod(m_iLodId);
		}
	}
}
