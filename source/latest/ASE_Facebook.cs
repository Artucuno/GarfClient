using System.Runtime.InteropServices;
using UnityEngine;

public class ASE_Facebook : MonoBehaviour
{
	public enum FacebookEvent
	{
		FBEVENT_PUBLICATION_DID_SHARED,
		FBEVENT_PUBLICATION_DID_CANCELED
	}

	[DllImport("__Internal")]
	private static extern void ASE_FacebookConnect(string appId);

	public static void Connect(string appId)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern bool ASE_FacebookIsConnected();

	public static bool IsConnected()
	{
		if (ASE_Tools.Available)
		{
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern void ASE_FacebookLogin();

	public static void Login()
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_FacebookLogout();

	public static void Logout()
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern int ASE_FacebookGetUserId();

	public static string GetUserId()
	{
		if (ASE_Tools.Available)
		{
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern int ASE_FacebookGetUserName();

	public static string GetUserName()
	{
		if (ASE_Tools.Available)
		{
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern void ASE_FacebookPublish(string name, string caption, string description, string link, string icon);

	public static void Publish(string name, string caption, string description, string link, string icon)
	{
		if (!ASE_Tools.Available)
		{
		}
	}

	[DllImport("__Internal")]
	private static extern void ASE_FacebookSetGameObjectName(string sGameObjectName);

	[DllImport("__Internal")]
	private static extern void ASE_FacebookSetMethodName(string sMethodName);

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
