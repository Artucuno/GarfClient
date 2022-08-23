using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
	public string LanguageOverride = string.Empty;

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		if (!Debug.isDebugBuild || LanguageOverride == null || LanguageOverride == string.Empty)
		{
			SystemLanguage systemLanguage = Application.systemLanguage;
			if (systemLanguage == SystemLanguage.French)
			{
				Localization.instance.currentLanguage = "Lang_DB_FR";
			}
			else
			{
				Localization.instance.currentLanguage = "Lang_DB_UK";
			}
		}
		else
		{
			Localization.instance.currentLanguage = LanguageOverride;
		}
	}
}
