using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class DebugMgr
{
	private static DebugMgr _instance;

	private DebugSettingsData _dbgData;

	public static DebugMgr Instance
	{
		get
		{
			return _instance;
		}
	}

	public DebugSettingsData dbgData
	{
		get
		{
			if (_dbgData == null)
			{
				_dbgData = Resources.Load("DebugSettings") as DebugSettingsData;
			}
			if (_dbgData == null)
			{
				UnityEngine.Debug.LogWarning("Resources.Load('DebugSettings.asset') returns null");
			}
			return _dbgData;
		}
		protected set
		{
		}
	}

	[Conditional("DEBUG_MGR")]
	public void Start()
	{
		if (dbgData.DisplayFPS)
		{
			UnityEngine.Object target = UnityEngine.Object.Instantiate(Resources.Load("HudFpsDisplay 1"));
			UnityEngine.Object.DontDestroyOnLoad(target);
		}
	}

	protected static string GetHeader(int frame, EDbgCategory cat)
	{
		StackTrace stackTrace = new StackTrace(true);
		string text = Time.time.ToString("0.00");
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(stackTrace.GetFrame(frame).GetFileName());
		string text2 = stackTrace.GetFrame(frame).GetFileLineNumber().ToString();
		string name = stackTrace.GetFrame(frame).GetMethod().Name;
		string text3 = fileNameWithoutExtension + ":" + name + "(" + text2 + ")\n";
		if (Instance.dbgData.LogCategory)
		{
			text3 = "[" + cat.ToString() + "] " + text3;
		}
		if (Instance.dbgData.LogTimeStamp)
		{
			text3 = "[" + text + "] " + text3;
		}
		return text3;
	}

	[Conditional("DEBUG_MGR")]
	public static void Log(object message)
	{
	}

	[Conditional("DEBUG_MGR")]
	public static void Log(object message, EDbgCategory cat)
	{
	}

	[Conditional("DEBUG_MGR")]
	private static void Log(object message, EDbgCategory cat, int frame)
	{
		if (Instance.dbgData.Categories[(int)cat])
		{
			UnityEngine.Debug.Log(string.Concat(GetHeader(frame, cat), message, "\n\n--------------------\n"));
		}
	}

	[Conditional("DEBUG_MGR")]
	public static void LogWarning(object message)
	{
	}

	[Conditional("DEBUG_MGR")]
	public static void LogWarning(object message, EDbgCategory cat)
	{
	}

	[Conditional("DEBUG_MGR")]
	public static void LogWarning(object message, EDbgCategory cat, int frame)
	{
		if (Instance.dbgData.Categories[(int)cat])
		{
			UnityEngine.Debug.LogWarning(string.Concat(GetHeader(frame, cat), message, "\n\n--------------------\n"));
		}
	}

	[Conditional("DEBUG_MGR")]
	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(string.Concat(GetHeader(2, EDbgCategory.ERROR), message, "\n\n--------------------\n"));
	}

	[Conditional("DEBUG_MGR")]
	public static void LogError(Exception pException)
	{
		while (pException.InnerException != null)
		{
			pException = pException.InnerException;
		}
	}

	[Conditional("DEBUG_MGR")]
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			dbgData.DisplayAnnotation = !dbgData.DisplayAnnotation;
		}
	}

	public bool IsAnnotationDisplayed()
	{
		return dbgData.DisplayAnnotation;
	}

	public bool IsLoadingScreenShortcut()
	{
		return dbgData.ShortcutLoadingScreen;
	}

	public void LoadDefaultPlayer(int pIndex, GameMode pGameMode)
	{
		LoadDefaultPlayer(pIndex, pGameMode, false, false);
	}

	public void LoadDefaultPlayer(int pIndex, GameMode pGameMode, bool pLock, bool isAI)
	{
		if (pGameMode != null)
		{
			pGameMode.CreatePlayer(dbgData.DefaultCharacter, dbgData.DefaultKart, dbgData.DefaultKartCustom.name, dbgData.DefaultHat.name, 0, pIndex, pLock, isAI);
		}
	}

	public void ApplyAdvantages()
	{
		if (dbgData.DefaultAdvOne != EAdvantage.None)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.AddAdvantage(dbgData.DefaultAdvOne);
		}
		if (dbgData.DefaultAdvTwo != EAdvantage.None)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.AddAdvantage(dbgData.DefaultAdvTwo);
		}
		if (dbgData.DefaultAdvThree != EAdvantage.None)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.AddAdvantage(dbgData.DefaultAdvThree);
		}
		if (dbgData.DefaultAdvFour != EAdvantage.None)
		{
			Singleton<GameConfigurator>.Instance.PlayerConfig.AddAdvantage(dbgData.DefaultAdvFour);
		}
	}
}
