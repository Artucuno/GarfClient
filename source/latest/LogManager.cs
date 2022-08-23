using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class LogManager
{
	public string _keyValueSeparator = " : ";

	public string _separator = "\n";

	public string _safeStr = "_";

	private static StreamWriter _logFile;

	private static LogManager _instance;

	public static LogManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public LogManager()
	{
		_logFile = new StreamWriter(Application.persistentDataPath + string.Format("/{0:d-M-yyyy_HH-mm-ss}.txt", DateTime.Now));
		_logFile.AutoFlush = true;
		UnityEngine.Object.Instantiate(Resources.Load("LumosPrefab"));
	}

	[Conditional("LOGMANAGER")]
	public void Log(string Key, string Value, bool Safe = false)
	{
		if (Safe)
		{
			Key = SafeParam(Key);
			Value = SafeParam(Value);
		}
		else
		{
			CheckParam(Key, "Key");
			CheckParam(Value, "Value");
		}
		string text = Key + _keyValueSeparator + Value + _separator;
		_logFile.Write(text);
		Lumos.Event(text);
	}

	[Conditional("LOGMANAGER")]
	public void Log(string Key, int Value, bool Safe = false)
	{
		string text;
		if (Safe)
		{
			Key = SafeParam(Key);
			text = SafeParam(Value.ToString());
		}
		else
		{
			CheckParam(Key, "Key");
			CheckParam(Value.ToString(), "Value");
			text = Value.ToString();
		}
		string value = Key + _keyValueSeparator + text + _separator;
		_logFile.Write(value);
		Lumos.Event(Key, Value);
	}

	[Conditional("LOGMANAGER")]
	public void Log(string Key, string Value1, int Value2, bool Safe = false)
	{
		string text;
		if (Safe)
		{
			Key = SafeParam(Key);
			Value1 = SafeParam(Value1);
			text = SafeParam(Value2.ToString());
		}
		else
		{
			CheckParam(Key, "Key");
			CheckParam(Value1, "Value1");
			CheckParam(Value2.ToString(), "Value2");
			text = Value2.ToString();
		}
		string value = Key + _keyValueSeparator + Value1 + _keyValueSeparator + text + _separator;
		_logFile.Write(value);
		Lumos.Event(Key + _keyValueSeparator + Value1, Value2);
	}

	private void CheckParam(string Param, string ParamName)
	{
		if ((_keyValueSeparator != string.Empty && Param.IndexOf(_keyValueSeparator) != -1) || (_separator != string.Empty && Param.IndexOf(_separator) != -1))
		{
			throw new ArgumentException("Parameters shouldn't contain the separators (" + _keyValueSeparator + " or " + _separator + "), " + Param + " does.", ParamName);
		}
	}

	private string SafeParam(string Param)
	{
		string text = Param;
		if (_keyValueSeparator != string.Empty)
		{
			text = text.Replace(_keyValueSeparator, _safeStr);
		}
		if (_separator != string.Empty)
		{
			text = text.Replace(_separator, _safeStr);
		}
		return text;
	}
}
