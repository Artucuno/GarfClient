using UnityEngine;

public class LookAtCameraYonly : MonoBehaviour
{
	private void OnWillRenderObject()
	{
		Quaternion quaternion = new Quaternion(0f, 1f, 0f, 0f);
		base.transform.rotation = Camera.main.transform.rotation * quaternion;
	}
}
