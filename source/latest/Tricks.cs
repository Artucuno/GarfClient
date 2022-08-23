using System;
using UnityEngine;

public class Tricks
{
	public static double ComputeInertia(double v0, double v1, double Ine, double dt)
	{
		if (dt == 0.0)
		{
			return v1;
		}
		double num = Math.Max(Ine, 0.5 * dt) / dt;
		return (v0 * (num - 0.5) + v1) / (num + 0.5);
	}

	public static float ComputeInertia(float v0, float v1, float Ine, float dt)
	{
		float num = Math.Max(Ine, 0.5f * dt) / dt;
		return (v0 * (num - 0.5f) + v1) / (num + 0.5f);
	}

	public static int LogBase2(int iVal)
	{
		int num = -1;
		while (iVal > 0)
		{
			iVal >>= 1;
			num++;
		}
		return num;
	}

	public static bool isTablet()
	{
		return (float)Screen.width / Screen.dpi >= 6f;
	}
}
