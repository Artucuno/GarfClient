using UnityEngine;

public class DebugRemoveMaterial : MonoBehaviour
{
	private void Start()
	{
		RemoveMaterial(base.gameObject);
	}

	private void RemoveMaterial(GameObject pGameObject)
	{
		if (pGameObject.renderer != null)
		{
			pGameObject.renderer.material = null;
		}
		for (int i = 0; i < pGameObject.transform.childCount; i++)
		{
			RemoveMaterial(pGameObject.transform.GetChild(i).gameObject);
		}
	}
}
