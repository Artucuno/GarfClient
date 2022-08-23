using UnityEngine;

public class ManageRenderSettings : MonoBehaviour
{
	public float fogDensite = 0.1f;

	public Color ambientLight = new Color(0.2f, 0.3f, 0.4f, 0.5f);

	public Color fogColor = new Color(0.2f, 0.3f, 0.4f, 0.5f);

	public float FStartDistance = 1f;

	public float FEndDistance = 100f;

	private void Start()
	{
	}

	private void Update()
	{
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensite;
		RenderSettings.ambientLight = ambientLight;
		RenderSettings.fogStartDistance = FStartDistance;
		RenderSettings.fogEndDistance = FEndDistance;
	}
}
