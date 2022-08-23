using UnityEngine;

public class LodRemote : MonoBehaviour
{
	public LODGroup[] m_pLodGroupReceiver = new LODGroup[3];

	public void SetCurrentLod(int iLodId)
	{
		LODGroup[] pLodGroupReceiver = m_pLodGroupReceiver;
		foreach (LODGroup lODGroup in pLodGroupReceiver)
		{
			if ((bool)lODGroup)
			{
				lODGroup.ForceLOD(iLodId);
				Animator component = lODGroup.GetComponent<Animator>();
				if ((bool)component)
				{
					component.enabled = ((iLodId == 0) ? true : false);
				}
			}
		}
	}
}
