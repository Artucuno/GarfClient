using UnityEngine;

public class MoodBoxManager : MonoBehaviour
{
	public Camera CurrentCamera;

	private Bloom CurrentBloom;

	public float IntensityBloom;

	public float IntensityThreshlod;

	public static MoodBox current;

	public MoodBoxData currentData;

	public MoodBox startMoodBox;

	public MoodBox currentMoodBox;

	private void Start()
	{
		current = startMoodBox;
		UpdateFromMoodBox();
		if ((bool)CurrentCamera)
		{
			CurrentBloom = CurrentCamera.GetComponent<Bloom>();
			CurrentBloom.bloomIntensity = IntensityBloom;
			CurrentBloom.bloomThreshhold = IntensityThreshlod;
		}
	}

	private void Update()
	{
		UpdateFromMoodBox();
	}

	public MoodBoxData GetData()
	{
		return currentData;
	}

	private void UpdateFromMoodBox()
	{
		currentMoodBox = current;
		if ((bool)current)
		{
			if (!Application.isPlaying)
			{
				currentData.FogStart = current.data.FogStart;
				currentData.FogEnd = current.data.FogEnd;
				currentData.FogColor = current.data.FogColor;
				currentData.outside = current.data.outside;
			}
			else
			{
				float deltaTime = Time.deltaTime;
				currentData.FogStart = Mathf.Lerp(currentData.FogStart, current.data.FogStart, deltaTime);
				currentData.FogEnd = Mathf.Lerp(currentData.FogEnd, current.data.FogEnd, deltaTime);
				currentData.FogColor = Color.Lerp(currentData.FogColor, current.data.FogColor, deltaTime);
				currentData.outside = current.data.outside;
			}
		}
		RenderSettings.fogColor = currentData.FogColor;
		RenderSettings.fogStartDistance = currentData.FogStart;
		RenderSettings.fogEndDistance = currentData.FogEnd;
	}
}
