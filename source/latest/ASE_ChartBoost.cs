using System.Runtime.InteropServices;

public class ASE_ChartBoost
{
	public enum ChartBoostEvent
	{
		CBEVENT_INTERSTITIAL_DISMISS,
		CBEVENT_INTERSTITIAL_CLOSE,
		CBEVENT_INTERSTITIAL_CLICK,
		CBEVENT_INTERSTITIAL_LOADFAIL,
		CBEVENT_MOREAPPS_DISMISS,
		CBEVENT_MOREAPPS_CLOSE,
		CBEVENT_MOREAPPS_CLICK,
		CBEVENT_MOREAPPS_LOADFAIL
	}

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostInit(string sAppId, string sAppSignature);

	public static void Init(string sAppId, string sAppSignature)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostShowInterstitial(string sLocation);

	public static void ShowInterstitial(string sLocation)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostCacheInterstitial(string sLocation);

	public static void CacheInterstitial(string sLocation)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostSetInterstitialRules(string sLocation, bool bShouldRequest, bool bShouldDisplay);

	public static void SetInterstitialRules(string sLocation, bool bShouldRequest, bool bShouldDisplay)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostShowMoreApps();

	public static void ShowMoreApps()
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostCacheMoreApps();

	public static void CacheMoreApps()
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostSetMoreAppsRules(bool bShouldRequest, bool bShouldDisplay);

	public static void SetMoreAppsRules(bool bShouldRequest, bool bShouldDisplay)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostSetGameObjectName(string sGameObjectName);

	[DllImport("__Internal")]
	private static extern void ASE_ChartBoostSetMethodName(string sMethodName);

	public static void SetGameObjectName(string sGameObjectName)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	public static void SetMethodName(string sMethodName)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	public static void SetCallbackInformations(string sGameObjectName, string sMethodName)
	{
		if (!ASE_Tools.Available)
		{
		}
	}
}
