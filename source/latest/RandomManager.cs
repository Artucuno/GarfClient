using System;

public class RandomManager : Singleton<RandomManager>
{
	private Random _random = new Random();

	public void Init()
	{
		_random = new Random();
	}

	public int Next()
	{
		return _random.Next();
	}

	public int Next(int pMaxValue)
	{
		return _random.Next(pMaxValue + 1);
	}

	public int Next(int pMinValue, int pMaxValue)
	{
		return _random.Next(pMinValue, pMaxValue + 1);
	}

	public double NextDouble()
	{
		return _random.NextDouble();
	}
}
