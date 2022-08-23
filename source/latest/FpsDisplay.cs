using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	public void Start()
	{
		if (!base.guiText)
		{
			base.enabled = false;
		}
		else
		{
			timeleft = updateInterval;
		}
	}

	public void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			string text = string.Format("{0:F2} FPS", num);
			base.guiText.text = text;
			if (num < 30f)
			{
				base.guiText.material.color = Color.yellow;
			}
			else if (num < 10f)
			{
				base.guiText.material.color = Color.red;
			}
			else
			{
				base.guiText.material.color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
