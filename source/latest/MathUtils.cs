using System;
using UnityEngine;

public class MathUtils
{
	public static float GetQuatLength(Quaternion q)
	{
		return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
	}

	public static Quaternion GetQuatConjugate(Quaternion q)
	{
		return new Quaternion(0f - q.x, 0f - q.y, 0f - q.z, q.w);
	}

	public static Quaternion GetQuatLog(Quaternion q)
	{
		Quaternion result = q;
		result.w = 0f;
		if (Mathf.Abs(q.w) < 1f)
		{
			float num = Mathf.Acos(q.w);
			float num2 = Mathf.Sin(num);
			if ((double)Mathf.Abs(num2) > 0.0001)
			{
				float num3 = num / num2;
				result.x = q.x * num3;
				result.y = q.y * num3;
				result.z = q.z * num3;
			}
		}
		return result;
	}

	public static Quaternion GetQuatExp(Quaternion q)
	{
		Quaternion result = q;
		float num = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
		float num2 = Mathf.Sin(num);
		result.w = Mathf.Cos(num);
		if ((double)Mathf.Abs(num2) > 0.0001)
		{
			float num3 = num2 / num;
			result.x = num3 * q.x;
			result.y = num3 * q.y;
			result.z = num3 * q.z;
		}
		return result;
	}

	public static Quaternion GetQuatSquad(float t, Quaternion q0, Quaternion q1, Quaternion a0, Quaternion a1)
	{
		float t2 = 2f * t * (1f - t);
		Quaternion p = Slerp(q0, q1, t);
		Quaternion q2 = Slerp(a0, a1, t);
		return Slerp(p, q2, t2);
	}

	public static Quaternion GetSquadIntermediate(Quaternion q0, Quaternion q1, Quaternion q2)
	{
		Quaternion quatConjugate = GetQuatConjugate(q1);
		Quaternion quatLog = GetQuatLog(quatConjugate * q0);
		Quaternion quatLog2 = GetQuatLog(quatConjugate * q2);
		Quaternion q3 = new Quaternion(-0.25f * (quatLog.x + quatLog2.x), -0.25f * (quatLog.y + quatLog2.y), -0.25f * (quatLog.z + quatLog2.z), -0.25f * (quatLog.w + quatLog2.w));
		return q1 * GetQuatExp(q3);
	}

	public static float Ease(float t, float k1, float k2)
	{
		float num = k1 * 2f / (float)Math.PI + k2 - k1 + (1f - k2) * 2f / (float)Math.PI;
		float num2 = ((t < k1) ? (k1 * (2f / (float)Math.PI) * (Mathf.Sin(t / k1 * (float)Math.PI / 2f - (float)Math.PI / 2f) + 1f)) : ((!(t < k2)) ? (2f * k1 / (float)Math.PI + k2 - k1 + (1f - k2) * (2f / (float)Math.PI) * Mathf.Sin((t - k2) / (1f - k2) * (float)Math.PI / 2f)) : (2f * k1 / (float)Math.PI + t - k1)));
		return num2 / num;
	}

	public static Quaternion Slerp(Quaternion p, Quaternion q, float t)
	{
		float num = Quaternion.Dot(p, q);
		Quaternion result = default(Quaternion);
		if ((double)(1f + num) > 1E-05)
		{
			float num4;
			float num5;
			if ((double)(1f - num) > 1E-05)
			{
				float num2 = Mathf.Acos(num);
				float num3 = 1f / Mathf.Sin(num2);
				num4 = Mathf.Sin((1f - t) * num2) * num3;
				num5 = Mathf.Sin(t * num2) * num3;
			}
			else
			{
				num4 = 1f - t;
				num5 = t;
			}
			result.x = num4 * p.x + num5 * q.x;
			result.y = num4 * p.y + num5 * q.y;
			result.z = num4 * p.z + num5 * q.z;
			result.w = num4 * p.w + num5 * q.w;
		}
		else
		{
			float num6 = Mathf.Sin((1f - t) * (float)Math.PI * 0.5f);
			float num7 = Mathf.Sin(t * (float)Math.PI * 0.5f);
			result.x = num6 * p.x - num7 * p.y;
			result.y = num6 * p.y + num7 * p.x;
			result.z = num6 * p.z - num7 * p.w;
			result.w = p.z;
		}
		return result;
	}
}
