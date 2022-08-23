using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

[XmlType("Ge")]
public class GameSave
{
	[XmlIgnore]
	public string FilePath;

	private static UTF8Encoding _encoder = new UTF8Encoding();

	[XmlElement("A")]
	public EncodedDictionary<byte[]> SaveData { get; set; }

	public GameSave()
	{
		SaveData = new EncodedDictionary<byte[]>();
	}

	private static string GetFilePath(string pGameSaveName)
	{
		string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
		byte[] bytes = _encoder.GetBytes(pGameSaveName);
		byte[] bytes2 = _encoder.GetBytes(deviceUniqueIdentifier);
		string path = Convert.ToBase64String(bytes2) + Convert.ToBase64String(bytes);
		return Path.Combine(Application.persistentDataPath, path);
	}

	public static bool Exists(string pGameSaveName)
	{
		string filePath = GetFilePath(pGameSaveName);
		return File.Exists(filePath);
	}

	public static GameSave Load(string pGameSaveName)
	{
		string filePath = GetFilePath(pGameSaveName);
		GameSave gameSave = null;
		if (File.Exists(filePath))
		{
			try
			{
				gameSave = Deserialize(filePath);
			}
			catch (Exception)
			{
				File.Delete(filePath);
			}
		}
		if (gameSave == null)
		{
			gameSave = new GameSave();
		}
		gameSave.FilePath = filePath;
		return gameSave;
	}

	private static GameSave Deserialize(string pFilePath)
	{
		GameSave gameSave = new GameSave();
		FileStream fileStream = File.Open(pFilePath, FileMode.Open);
		StreamReader streamReader = new StreamReader(fileStream);
		string text = null;
		while ((text = streamReader.ReadLine()) != null)
		{
			string[] array = text.Split(';');
			byte[] pKey = Convert.FromBase64String(array[0]);
			byte[] pValue = Convert.FromBase64String(array[1]);
			gameSave.SaveData.Add(pKey, pValue);
		}
		streamReader.Close();
		fileStream.Close();
		return gameSave;
	}

	private void Serialize(string pFilePath)
	{
		FileStream fileStream = File.Open(pFilePath, FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(fileStream);
		foreach (SerializablePair<byte[], byte[]> saveDatum in SaveData)
		{
			string text = Convert.ToBase64String(saveDatum.Key);
			string text2 = Convert.ToBase64String(saveDatum.Value);
			streamWriter.WriteLine(text + ";" + text2);
		}
		streamWriter.Close();
		fileStream.Close();
	}

	public void Save()
	{
		Serialize(FilePath);
	}

	private void SetValue(string pKey, byte[] pValue)
	{
		byte[] bytes = _encoder.GetBytes(pKey);
		if (SaveData.ContainsKey(bytes))
		{
			SaveData[bytes] = pValue;
		}
		else
		{
			SaveData.Add(bytes, pValue);
		}
	}

	public void SetString(string pKey, string pValue)
	{
		byte[] bytes = _encoder.GetBytes(pValue);
		SetValue(pKey, bytes);
	}

	public void SetInt(string pKey, int pValue)
	{
		string pValue2 = Convert.ToString(pValue);
		SetString(pKey, pValue2);
	}

	public void SetFloat(string pKey, float pValue)
	{
		string pValue2 = Convert.ToString(pValue);
		SetString(pKey, pValue2);
	}

	public void SetBool(string pKey, bool pValue)
	{
		string pValue2 = Convert.ToString(pValue);
		SetString(pKey, pValue2);
	}

	public string GetString(string pKey)
	{
		byte[] bytes = _encoder.GetBytes(pKey);
		if (SaveData.ContainsKey(bytes))
		{
			byte[] bytes2 = SaveData[bytes];
			return _encoder.GetString(bytes2);
		}
		return null;
	}

	public float GetFloat(string pKey)
	{
		string @string = GetString(pKey);
		if (@string != null)
		{
			return (float)Convert.ToDouble(@string);
		}
		return float.MinValue;
	}

	public int GetInt(string pKey)
	{
		string @string = GetString(pKey);
		if (@string != null)
		{
			return Convert.ToInt32(@string);
		}
		return int.MinValue;
	}

	public bool GetBool(string pKey)
	{
		string @string = GetString(pKey);
		if (@string != null)
		{
			return Convert.ToBoolean(@string);
		}
		return false;
	}

	public string GetString(string pKey, string pDefaultValue)
	{
		byte[] bytes = _encoder.GetBytes(pKey);
		if (SaveData.ContainsKey(bytes))
		{
			byte[] bytes2 = SaveData[bytes];
			return _encoder.GetString(bytes2);
		}
		SetString(pKey, pDefaultValue);
		return pDefaultValue;
	}

	public float GetFloat(string pKey, float pDefaultValue)
	{
		string @string = GetString(pKey);
		if (@string != null)
		{
			return (float)Convert.ToDouble(@string);
		}
		SetFloat(pKey, pDefaultValue);
		return pDefaultValue;
	}

	public int GetInt(string pKey, int pDefaultValue)
	{
		string @string = GetString(pKey);
		if (@string != null)
		{
			return Convert.ToInt32(@string);
		}
		SetInt(pKey, pDefaultValue);
		return pDefaultValue;
	}

	public bool GetBool(string pKey, bool pDefaultValue)
	{
		string @string = GetString(pKey);
		if (@string != null)
		{
			return Convert.ToBoolean(@string);
		}
		SetBool(pKey, pDefaultValue);
		return pDefaultValue;
	}
}
