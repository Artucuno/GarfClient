using UnityEngine;

public class GameOptionManager : Singleton<GameOptionManager>
{
	public enum ELangID
	{
		French,
		English,
		German,
		Spanish,
		Italian
	}

	private const string _LANGUAGE = "lang_";

	private const string _SFXVOL = "sfx_";

	private const string _MUSICVOL = "mus_";

	private const string _GYRO = "gyro_";

	private const string _INPUT = "inp_";

	private const string _SAVE = "options";

	private GameSave _gameSave;

	private ELangID _language;

	private float _sfxVolume;

	private float _musicVolume;

	private float _gyroSensibility;

	private E_InputType _inputType;

	private static string[] m_oLanguageList = new string[5] { "Lang_DB_FR", "Lang_DB_UK", "Lang_DB_GE", "Lang_DB_SP", "Lang_DB_IT" };

	public void Init()
	{
		Load(out _language, out _sfxVolume, out _musicVolume, out _gyroSensibility, out _inputType, out _gameSave);
		Save();
	}

	public void Load(out ELangID opLanguage, out float opSfxVolume, out float opMusicVolume, out float opGyroSensibility, out E_InputType opInputType, out GameSave opGameSave)
	{
		opGameSave = GameSave.Load("options");
		opLanguage = ELangID.English;
		opSfxVolume = 0.5f;
		opMusicVolume = 0.5f;
		opGyroSensibility = 0f;
		opInputType = E_InputType.Keyboard;
		E_InputType pDefaultValue = E_InputType.Keyboard;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			pDefaultValue = E_InputType.Gyroscopic;
		}
		int @int = opGameSave.GetInt("inp_", (int)pDefaultValue);
		opInputType = (E_InputType)@int;
		ELangID eLangId;
		switch (Application.systemLanguage)
		{
		case SystemLanguage.French:
			eLangId = ELangID.French;
			break;
		case SystemLanguage.Spanish:
			eLangId = ELangID.Spanish;
			break;
		case SystemLanguage.German:
			eLangId = ELangID.German;
			break;
		case SystemLanguage.Italian:
			eLangId = ELangID.Italian;
			break;
		default:
			eLangId = ELangID.English;
			break;
		}
		string @string = opGameSave.GetString("lang_", ConvertLangIdToString(eLangId));
		SetLanguage(@string, false);
		opLanguage = GetCurrentLangId();
		opSfxVolume = opGameSave.GetFloat("sfx_", 0.5f);
		SetSfxVolume(opSfxVolume, false);
		opMusicVolume = opGameSave.GetFloat("mus_", 0.5f);
		SetMusicVolume(opMusicVolume, false);
		opGyroSensibility = opGameSave.GetFloat("gyro_", 0.7f);
	}

	public float GetSfxVolume()
	{
		return _sfxVolume;
	}

	public float GetMusicVolume()
	{
		return _musicVolume;
	}

	public float GetGyroSensibility()
	{
		return _gyroSensibility;
	}

	public E_InputType GetInputType()
	{
		return _inputType;
	}

	public void SetSfxVolume(float pVolume, bool pSave)
	{
		_sfxVolume = pVolume;
		AudioListener.volume = _sfxVolume;
		_gameSave.SetFloat("sfx_", _sfxVolume);
		if (pSave)
		{
			Save();
		}
	}

	public void SetMusicVolume(float pVolume, bool pSave)
	{
		_musicVolume = pVolume;
		AudioListener.volume = _musicVolume;
		_gameSave.SetFloat("mus_", _musicVolume);
		if (pSave)
		{
			Save();
		}
	}

	public void SetGyroSensibility(float fSensibility, bool pSave)
	{
		_gyroSensibility = fSensibility;
		_gameSave.SetFloat("gyro_", _gyroSensibility);
		if (pSave)
		{
			Save();
		}
	}

	public void SetInputType(E_InputType pInputType, bool pSave)
	{
		_inputType = pInputType;
		_gameSave.SetInt("inp_", (int)_inputType);
		if (pSave)
		{
			Save();
		}
	}

	public void SetLanguage(string pLanguage, bool pSave)
	{
		SetLanguage(ConvertLangStringToId(pLanguage), pSave);
	}

	public void SetLanguage(ELangID eLanguage, bool pSave)
	{
		_language = eLanguage;
		Localization.instance.currentLanguage = ConvertLangIdToString(eLanguage);
		_gameSave.SetString("lang_", ConvertLangIdToString(eLanguage));
		if (pSave)
		{
			Save();
		}
	}

	public ELangID GetCurrentLangId()
	{
		return _language;
	}

	public void Save()
	{
		_gameSave.Save();
	}

	public string ConvertLangIdToString(ELangID eLangId)
	{
		uint num = (uint)eLangId;
		if (num >= m_oLanguageList.Length)
		{
			num = 1u;
		}
		return m_oLanguageList[num];
	}

	public ELangID ConvertLangStringToId(string sLang)
	{
		int num = 0;
		string[] oLanguageList = m_oLanguageList;
		foreach (string text in oLanguageList)
		{
			if (text == sLang)
			{
				return (ELangID)num;
			}
			num++;
		}
		return ELangID.English;
	}
}
