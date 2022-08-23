using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float vitesse = 1f;

	public bool _x = true;

	public bool _y;

	public bool _z;

	public Transform m_oTransform;

	private void Awake()
	{
		m_oTransform = base.gameObject.transform;
	}

	private void Update()
	{
		Vector3 vector = new Vector3((!_x) ? 0f : 1f, (!_y) ? 0f : 1f, (!_z) ? 0f : 1f);
		m_oTransform.Rotate(vector * Time.deltaTime * vitesse);
	}
}
