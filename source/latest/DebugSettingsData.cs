using System;
using UnityEngine;

public class DebugSettingsData : ScriptableObject
{
	public bool DisplayFPS;

	public bool DisplayAnnotation;

	public bool DisplaySpeed;

	public bool ShortcutLoadingScreen;

	public bool UseInAppService;

	public bool DisplayCustom;

	public bool LogTimeStamp;

	public bool LogCategory;

	public bool RandomPlayer;

	public ECharacter DefaultCharacter;

	public ECharacter DefaultKart;

	public GameObject DefaultKartCustom;

	public GameObject DefaultHat;

	public EAdvantage DefaultAdvOne;

	public EAdvantage DefaultAdvTwo;

	public EAdvantage DefaultAdvThree;

	public EAdvantage DefaultAdvFour;

	public bool[] Categories = new bool[Enum.GetValues(typeof(EDbgCategory)).Length];

	public string SceneToLaunch;

	public E_GameModeType GameMode;

	private DebugSettingsData()
	{
		DisplayFPS = true;
		ShortcutLoadingScreen = false;
		DisplayAnnotation = true;
		DisplaySpeed = false;
		DisplayCustom = false;
		RandomPlayer = false;
		LogTimeStamp = true;
		LogCategory = true;
		for (int i = 0; i < Categories.Length; i++)
		{
			Categories[i] = true;
		}
	}
}
