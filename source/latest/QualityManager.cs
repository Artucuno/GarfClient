using UnityEngine;

public class QualityManager : MonoBehaviour
{
	private GameObject FxPC;

	private GameObject FxMobile;

	public GameObject MoodManage;

	public Quality currentQuality;

	public DepthOfFieldScatter PCdepthOfField;

	public ScreenOverlay PCScreenOverlay;

	public Bloom PCbloom;

	public Vignetting PCVignetting;

	public static Quality quality;

	private void Start()
	{
		FxPC = GameObject.Find("FxPC");
		FxMobile = GameObject.Find("FxMobile");
		MoodManage = GameObject.Find("MoodManager");
		ApplyAndSetQuality(currentQuality);
	}

	private void Awake()
	{
		AutoDetectQuality();
	}

	private void AutoDetectQuality()
	{
		Shader.globalMaximumLOD = 1000;
		if (SystemInfo.graphicsPixelFillrate < 2800)
		{
			currentQuality = Quality.High;
		}
		else
		{
			currentQuality = Quality.Highest;
		}
	}

	private void ApplyAndSetQuality(Quality newQuality)
	{
		quality = newQuality;
		if (quality == Quality.Low)
		{
			RenderSettings.fog = true;
			EnableFx(PCdepthOfField, false);
			EnableFx(PCScreenOverlay, false);
			EnableFx(PCbloom, false);
			EnableFx(PCVignetting, false);
			if (MoodManage != null)
			{
				MoodManage.SetActive(false);
			}
			if (FxPC != null)
			{
				FxPC.SetActive(false);
			}
			if (FxMobile != null)
			{
				FxMobile.SetActive(false);
			}
		}
		else if (quality == Quality.Medium)
		{
			RenderSettings.fog = true;
			EnableFx(PCdepthOfField, false);
			EnableFx(PCScreenOverlay, false);
			EnableFx(PCbloom, false);
			EnableFx(PCVignetting, false);
			if (MoodManage != null)
			{
				MoodManage.SetActive(true);
			}
			if (FxPC != null)
			{
				FxPC.SetActive(false);
			}
			if (FxMobile != null)
			{
				FxMobile.SetActive(true);
			}
		}
		else if (quality == Quality.High)
		{
			RenderSettings.fog = true;
			EnableFx(PCdepthOfField, false);
			EnableFx(PCScreenOverlay, true);
			EnableFx(PCbloom, true);
			EnableFx(PCVignetting, false);
			if (MoodManage != null)
			{
				MoodManage.SetActive(true);
			}
			if (FxPC != null)
			{
				FxPC.SetActive(true);
			}
			if (FxMobile != null)
			{
				FxMobile.SetActive(false);
			}
		}
		else if (quality == Quality.Highest)
		{
			RenderSettings.fog = true;
			EnableFx(PCdepthOfField, true);
			EnableFx(PCScreenOverlay, true);
			EnableFx(PCbloom, true);
			EnableFx(PCVignetting, true);
			if (MoodManage != null)
			{
				MoodManage.SetActive(true);
			}
			if (FxPC != null)
			{
				FxPC.SetActive(true);
			}
			if (FxMobile != null)
			{
				FxMobile.SetActive(false);
			}
		}
	}

	private void DisableAllFx()
	{
		RenderSettings.fog = false;
		EnableFx(PCdepthOfField, false);
		EnableFx(PCScreenOverlay, false);
		EnableFx(PCbloom, false);
		EnableFx(PCVignetting, false);
		if (MoodManage != null)
		{
			MoodManage.SetActive(false);
		}
		if (FxPC != null)
		{
			FxPC.SetActive(false);
		}
		if (FxMobile != null)
		{
			FxMobile.SetActive(false);
		}
	}

	private void EnableFx(MonoBehaviour fx, bool enable)
	{
		if ((bool)fx)
		{
			fx.enabled = enable;
		}
	}
}
