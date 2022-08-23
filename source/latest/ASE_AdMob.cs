using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ASE_AdMob
{
	[Obsolete("AdMobBannerType is deprecated, please use iOSAdMobBannerType instead.")]
	public enum AdMobBannerType
	{
		iPhone_320x50,
		iPad_728x90,
		iPad_468x60,
		iPad_320x250,
		SmartBannerPortrait,
		SmartBannerLandscape
	}

	public enum iOSAdMobBannerType
	{
		iPhone_320x50,
		iPad_728x90,
		iPad_468x60,
		iPad_320x250,
		SmartBannerPortrait,
		SmartBannerLandscape
	}

	public enum AndroidAdMobBannerType
	{
		phone_320x50,
		tablet_300x250,
		tablet_468x60,
		tablet_728x90,
		SmartBanner
	}

	public enum AdMobEvent
	{
		AD_DID_RECEIVED_AD,
		AD_FAILED_TO_RECEIVED_AD,
		INTERSTITIAL_DID_RECEIVED_AD,
		INTERSTITIAL_FAILED_TO_RECEIVED_AD
	}

	[DllImport("__Internal")]
	private static extern void ASE_AdMobInit(string publisherId, bool isTesting);

	public static void Init(string publisherId)
	{
		if (ASE_Tools.Available)
		{
			Init(publisherId, false);
		}
	}

	public static void Init(string publisherId, bool isTesting)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_AdMobCreateBanner(int bannerType, bool bannerOnBottom);

	[Obsolete("CreateBanner( AdMobBannerType bannerType, bool bannerOnBottom ) is deprecated, please use CreateBanner( iOSAdMobBannerType bannerType, bool bannerOnBottom ) instead.")]
	public static void CreateBanner(AdMobBannerType bannerType, bool bannerOnBottom)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			ASE_AdMobCreateBanner((int)bannerType, bannerOnBottom);
		}
		else
		{
			Debug.LogWarning(" Can't create the banner ! You're not on a iOS device ( Have you set the correct enum ? )");
		}
	}

	public static void CreateBanner(iOSAdMobBannerType bannerType, bool bannerOnBottom)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			ASE_AdMobCreateBanner((int)bannerType, bannerOnBottom);
		}
		else
		{
			Debug.LogWarning(" Can't create the banner ! You're not on a iOS device ( Have you set the correct enum ? )");
		}
	}

	public static void CreateBanner(AndroidAdMobBannerType bannerType, bool bannerOnBottom)
	{
	}

	[DllImport("__Internal")]
	private static extern void ASE_AdMobDestroyBanner();

	public static void DestroyBanner()
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_AdMobRequestInterstitalAd(string interstitialUnitId);

	public static void RequestInterstitalAd(string interstitialUnitId)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern bool ASE_AdMobIsInterstitialAdReady();

	public static bool IsInterstitialAdReady()
	{
		if (ASE_Tools.Available)
		{
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern void ASE_AdMobShowInterstitialAd();

	public static void ShowInterstitialAd()
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_AdMobSetGameObjectName(string sGameObject);

	[DllImport("__Internal")]
	private static extern void ASE_AdMobSetMethodName(string sMethodName);

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
