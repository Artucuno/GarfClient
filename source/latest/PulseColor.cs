using UnityEngine;

public class PulseColor : MonoBehaviour
{
	public Color colorStart = Color.red;

	public Color colorEnd = Color.green;

	public float duration = 1f;

	private void Update()
	{
		float t = Mathf.PingPong(Time.time, duration) / duration;
		base.renderer.material.color = Color.Lerp(colorStart, colorEnd, t);
	}
}
