using UnityEngine;

public class LayerCullDistance : MonoBehaviour
{
	[HideInInspector]
	public int distanceToCullNear;

	[HideInInspector]
	public int distanceToCullMed;

	[HideInInspector]
	public int distanceToCullFar;

	public void ChangeDistancePopMesh()
	{
		float[] array = new float[32];
		array[24] = distanceToCullNear;
		array[25] = distanceToCullMed;
		array[26] = distanceToCullFar;
		base.camera.layerCullDistances = array;
	}
}
