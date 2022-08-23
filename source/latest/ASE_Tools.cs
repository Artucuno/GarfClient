using System.Collections.Generic;
using UnityEngine;

public class ASE_Tools
{
	public static bool Available
	{
		get
		{
			return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android;
		}
	}

	public static void DictionaryToArrays(Dictionary<string, string> dict, out string[] sKeys, out string[] sValues)
	{
		sKeys = null;
		sValues = null;
		if (dict == null)
		{
			return;
		}
		sKeys = new string[dict.Count];
		sValues = new string[dict.Count];
		int num = 0;
		foreach (KeyValuePair<string, string> item in dict)
		{
			sKeys[num] = item.Key;
			sValues[num] = item.Value;
			num++;
		}
	}

	public static int GetDataEvent(string sData)
	{
		if (string.IsNullOrEmpty(sData))
		{
			return -1;
		}
		return sData[0] - 65;
	}

	public static string GetDataMessage(string sData)
	{
		if (string.IsNullOrEmpty(sData) || sData.Length < 1)
		{
			return string.Empty;
		}
		return sData.Substring(1);
	}
}
