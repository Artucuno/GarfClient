using System;

public class Singleton<T>
{
	private static object _sInstance = Activator.CreateInstance(typeof(T));

	public static T Instance
	{
		get
		{
			if (_sInstance == null)
			{
				_sInstance = Activator.CreateInstance(typeof(T));
			}
			return (T)_sInstance;
		}
	}

	protected Singleton()
	{
		if (_sInstance != null)
		{
			throw new Exception("instance already exist");
		}
		_sInstance = this;
		if (_sInstance == null)
		{
			throw new Exception("instance creation failed");
		}
	}

	public static void DestroyInstance()
	{
		_sInstance = null;
		GC.Collect();
	}
}
