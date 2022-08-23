using UnityEngine;

public class DebugPathRecorder : MonoBehaviour
{
	public Color Color = Color.red;

	private Vector3 _previousPosition;

	public bool DebugDrawOn;

	public void OnDrawGizmos()
	{
		if (DebugDrawOn)
		{
			int childCount = base.transform.childCount;
			Material material = null;
			if (childCount > 0)
			{
				GameObject gameObject = base.transform.GetChild(0).gameObject;
				if (gameObject.GetComponent<MeshRenderer>().renderer.sharedMaterial != null)
				{
					material = new Material(gameObject.GetComponent<MeshRenderer>().renderer.sharedMaterial);
					material.color = Color;
				}
			}
			for (int i = 0; i < childCount; i++)
			{
				Vector3 position = base.transform.GetChild(i).position;
				GameObject gameObject2 = base.transform.GetChild(i).gameObject;
				gameObject2.GetComponent<MeshRenderer>().enabled = true;
				if (material != null)
				{
					gameObject2.GetComponent<MeshRenderer>().renderer.sharedMaterial = material;
				}
				if (i < childCount - 1)
				{
					Vector3 position2 = base.transform.GetChild(i + 1).position;
					Debug.DrawLine(position, position2, Color);
				}
			}
		}
		else
		{
			int childCount2 = base.transform.childCount;
			for (int j = 0; j < childCount2; j++)
			{
				GameObject gameObject3 = base.transform.GetChild(j).gameObject;
				gameObject3.GetComponent<MeshRenderer>().enabled = false;
			}
		}
	}
}
