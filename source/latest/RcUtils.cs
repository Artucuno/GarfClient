using System;
using UnityEngine;

public static class RcUtils
{
	public const float RIGHT_HANDED = -1f;

	public static void COMPUTE_INERTIA(ref float v0, float v1, float Ine, float dt)
	{
		v0 += (1f - Ine) * (v1 - v0) * dt;
	}

	public static void COMPUTE_INERTIA(ref Vector3 v0, Vector3 v1, float Ine, float dt)
	{
		v0 += (1f - Ine) * (v1 - v0) * dt;
	}

	public static float LinearInterpolation(float x1, float y1, float x2, float y2, float x)
	{
		return LinearInterpolation(x1, y1, x2, y2, x, false);
	}

	public static float LinearInterpolation(float x1, float y1, float x2, float y2, float x, bool clamp)
	{
		float num = (x - x1) / (x2 - x1);
		float num2 = y1 + num * (y2 - y1);
		if (clamp)
		{
			num2 = ((!(y1 < y2)) ? Mathf.Clamp(num2, y2, y1) : Mathf.Clamp(num2, y1, y2));
		}
		return num2;
	}

	public static bool IsOnRight(Vector3 pSegBegin, Vector3 pSegEnd, Vector3 pPt, Vector3 pUp)
	{
		Vector3 lhs = pSegEnd - pSegBegin;
		Vector3 rhs = pPt - pSegBegin;
		Vector3 lhs2 = Vector3.Cross(lhs, rhs);
		return Vector3.Dot(lhs2, pUp) <= 0f;
	}

	public static bool IsOnRight(Vector2 pSegBegin, Vector2 pSegEnd, Vector2 pPt)
	{
		return (pSegEnd.x - pSegBegin.x) * (pPt.y - pSegBegin.y) - (pSegEnd.y - pSegBegin.y) * (pPt.x - pSegBegin.x) >= 0f;
	}

	public static bool Inside(Vector2 pP, Vector2 pSP0, Vector2 pSP1)
	{
		if (pSP0.x != pSP1.x)
		{
			if (pSP0.x <= pP.x && pP.x <= pSP1.x)
			{
				return true;
			}
			if (pSP0.x >= pP.x && pP.x >= pSP1.x)
			{
				return true;
			}
		}
		else
		{
			if (pSP0.y <= pP.y && pP.y <= pSP1.y)
			{
				return true;
			}
			if (pSP0.y >= pP.y && pP.y >= pSP1.y)
			{
				return true;
			}
		}
		return false;
	}

	public static int SegmentsIntersection(Vector2 pS1P0, Vector2 pS1P1, Vector2 pS2P0, Vector2 pS2P1, ref Vector2 rpOut1, ref Vector2 rpOut2)
	{
		Vector2 vector = pS1P1 - pS1P0;
		Vector2 vector2 = pS2P1 - pS2P0;
		Vector2 to = pS1P0 - pS2P0;
		float num = Vector2.Angle(vector, vector2);
		if (Mathf.Abs(num) < 1E-08f)
		{
			if (Vector2.Angle(vector, to) != 0f || Vector2.Angle(vector2, to) != 0f)
			{
				return 0;
			}
			float num2 = Vector2.Angle(vector, vector);
			float num3 = Vector2.Angle(vector2, vector2);
			if (num2 == 0f && num3 == 0f)
			{
				if (pS1P0 == pS2P0)
				{
					return 0;
				}
				rpOut1 = pS1P0;
				return 1;
			}
			if (num2 == 0f)
			{
				if (!Inside(pS1P0, pS2P0, pS2P1))
				{
					return 0;
				}
				rpOut1 = pS1P0;
				return 1;
			}
			if (num3 == 0f)
			{
				if (!Inside(pS2P0, pS1P0, pS1P1))
				{
					return 0;
				}
				rpOut1 = pS2P0;
				return 1;
			}
			Vector2 vector3 = pS1P1 - pS2P0;
			float num4;
			float num5;
			if (vector2.x != 0f)
			{
				num4 = to.x / vector2.x;
				num5 = vector3.x / vector2.x;
			}
			else
			{
				num4 = to.y / vector2.y;
				num5 = vector3.y / vector2.y;
			}
			if (num4 > num5)
			{
				float num6 = num4;
				num4 = num5;
				num5 = num6;
			}
			if (num4 > 1f || num5 < 0f)
			{
				return 0;
			}
			num4 = ((!(num4 < 0f)) ? num4 : 0f);
			num5 = ((!(num5 > 1f)) ? num5 : 1f);
			if (num4 == num5)
			{
				rpOut1 = pS2P0 + num4 * vector2;
				return 1;
			}
			rpOut1 = pS2P0 + num4 * vector2;
			rpOut2 = pS2P0 + num5 * vector2;
			return 2;
		}
		float num7 = Vector2.Angle(vector2, to) / num;
		if (num7 < 0f || num7 > 1f)
		{
			return 0;
		}
		float num8 = Vector2.Angle(vector, to) / num;
		if (num8 < 0f || num8 > 1f)
		{
			return 0;
		}
		rpOut1 = pS1P0 + num7 * vector;
		return 1;
	}

	public static float FastSqrtApprox(float pValue)
	{
		int num = (int)pValue;
		num -= 8388608;
		num >>= 1;
		num += 536870912;
		return num;
	}

	public static float PointToSegmentDistance(Vector3 point, Vector3 segmentP1, Vector3 segmentP2)
	{
		return Mathf.Sqrt(PointToSegmentSqrDistance(point, segmentP1, segmentP2));
	}

	public static float PointToSegmentRatio(Vector2 point, Vector2 segmentP1, Vector2 segmentP2)
	{
		if (segmentP1 == segmentP2)
		{
			return 0f;
		}
		Vector2 vector = segmentP2 - segmentP1;
		return Vector3.Dot(point - segmentP1, vector) / vector.SqrMagnitude();
	}

	public static float PointToSegmentRatio(Vector3 point, Vector3 segmentP1, Vector3 segmentP2)
	{
		if (segmentP1 == segmentP2)
		{
			return 0f;
		}
		Vector3 rhs = segmentP2 - segmentP1;
		return Vector3.Dot(point - segmentP1, rhs) / rhs.sqrMagnitude;
	}

	public static float PointToSegmentSqrDistance(Vector2 point, Vector2 segmentP1, Vector2 segmentP2)
	{
		float num;
		float num2;
		if (segmentP1 == segmentP2)
		{
			num = segmentP1.x - point.x;
			num2 = segmentP1.y - point.y;
		}
		else
		{
			float num3 = PointToSegmentRatio(point, segmentP1, segmentP2);
			if (num3 < 0f)
			{
				num = segmentP1.x - point.x;
				num2 = segmentP1.y - point.y;
			}
			else if (num3 > 1f)
			{
				num = segmentP2.x - point.x;
				num2 = segmentP2.y - point.y;
			}
			else
			{
				num = (1f - num3) * segmentP1.x + num3 * segmentP2.x - point.x;
				num2 = (1f - num3) * segmentP1.y + num3 * segmentP2.y - point.y;
			}
		}
		return num * num + num2 * num2;
	}

	public static float PointToSegmentSqrDistance(Vector3 point, Vector3 segmentP1, Vector3 segmentP2)
	{
		float num;
		float num2;
		float num3;
		if (segmentP1 == segmentP2)
		{
			num = segmentP1.x - point.x;
			num2 = segmentP1.y - point.y;
			num3 = segmentP1.z - point.z;
		}
		else
		{
			float num4 = PointToSegmentRatio(point, segmentP1, segmentP2);
			if (num4 < 0f)
			{
				num = segmentP1.x - point.x;
				num2 = segmentP1.y - point.y;
				num3 = segmentP1.z - point.z;
			}
			else if (num4 > 1f)
			{
				num = segmentP2.x - point.x;
				num2 = segmentP2.y - point.y;
				num3 = segmentP2.z - point.z;
			}
			else
			{
				num = (1f - num4) * segmentP1.x + num4 * segmentP2.x - point.x;
				num2 = (1f - num4) * segmentP1.y + num4 * segmentP2.y - point.y;
				num3 = (1f - num4) * segmentP1.z + num4 * segmentP2.z - point.z;
			}
		}
		return num * num + num2 * num2 + num3 * num3;
	}

	public static Vector3 GetSmoothPos(Vector3 _Point1, Vector3 _Point2, Vector3 _Point3, Vector3 _Point4, float _Percentage, float _SplineTension)
	{
		if (_Percentage == 0f)
		{
			return _Point2;
		}
		Vector3[] array = new Vector3[4]
		{
			_Point2,
			_Point2 + (_Point3 - _Point1) / _SplineTension,
			_Point3 + (_Point2 - _Point4) / _SplineTension,
			_Point3
		};
		float num = 1f - _Percentage;
		float num2 = _Percentage * _Percentage;
		float num3 = num * num;
		Vector3 result = default(Vector3);
		result.x = array[0].x * num * num3 + array[1].x * 3f * _Percentage * num3 + array[2].x * 3f * num2 * num + array[3].x * _Percentage * num2;
		result.y = array[0].y * num * num3 + array[1].y * 3f * _Percentage * num3 + array[2].y * 3f * num2 * num + array[3].y * _Percentage * num2;
		result.z = array[0].z * num * num3 + array[1].z * 3f * _Percentage * num3 + array[2].z * 3f * num2 * num + array[3].z * _Percentage * num2;
		return result;
	}

	public static float MsToKph(float pValue)
	{
		return pValue * 3.6f;
	}

	public static float FastNormaliseApprox(ref Vector3 rpVector)
	{
		float num = FastSqrtApprox(rpVector.x * rpVector.x + rpVector.y * rpVector.y + rpVector.z * rpVector.z);
		if ((double)num > 1E-08)
		{
			float num2 = 1f / num;
			rpVector.x *= num2;
			rpVector.y *= num2;
			rpVector.z *= num2;
		}
		return num;
	}

	public static float Perlin(float pX, float pY, float pPersistence, int pOctaves)
	{
		float num = 0f;
		float num2 = 1f;
		for (int i = 0; i < pOctaves; i++)
		{
			float num3 = 1 << i;
			num += InterpolatedNoise(pX * num3, pY * num3) * num2;
			num2 *= pPersistence;
		}
		return num;
	}

	public static float InterpolateLine(float pA, float pB, float pX)
	{
		float f = pX * (float)Math.PI;
		float num = (1f - Mathf.Cos(f)) * 0.5f;
		return pA * (1f - num) + pB * num;
	}

	public static float Noise(int pX, int pY)
	{
		int num = pX + pY * 57;
		num = (num << 13) ^ num;
		return 1f - (float)((num * (num * num * 15731 + 789221) + 1376312589) & 0x7FFFFFFF) / 1.0737418E+09f;
	}

	public static float SmoothNoise(int pX, int pY)
	{
		float num = (Noise(pX - 1, pY - 1) + Noise(pX + 1, pY - 1) + Noise(pX - 1, pY + 1) + Noise(pX + 1, pY + 1)) / 16f;
		float num2 = (Noise(pX - 1, pY) + Noise(pX + 1, pY) + Noise(pX, pY - 1) + Noise(pX, pY + 1)) / 8f;
		float num3 = Noise(pX, pY) / 4f;
		return num + num2 + num3;
	}

	public static float InterpolatedNoise(float pX, float pY)
	{
		int num = (int)pX;
		float pX2 = pX - (float)num;
		int num2 = (int)pY;
		float pX3 = pY - (float)num2;
		float pA = SmoothNoise(num, num2);
		float pB = SmoothNoise(num + 1, num2);
		float pA2 = SmoothNoise(num, num2 + 1);
		float pB2 = SmoothNoise(num + 1, num2 + 1);
		float pA3 = InterpolateLine(pA, pB, pX2);
		float pB3 = InterpolateLine(pA2, pB2, pX2);
		return InterpolateLine(pA3, pB3, pX3);
	}

	public static bool IsOnRight(Vector3 pSegBegin, Vector3 pSegEnd, Vector3 pPt)
	{
		Vector3 lhs = pSegEnd - pSegBegin;
		Vector3 rhs = pPt - pSegBegin;
		Vector3 lhs2 = Vector3.Cross(lhs, rhs);
		return -1f * Vector3.Dot(lhs2, Vector3.up) <= 0f;
	}

	public static void SerializeAndCompressVector(BitStream stream, Vector3 vec, Vector3 amplitude)
	{
		short value = CompressFloat(vec.x, 0f - amplitude.x, amplitude.x);
		short value2 = CompressFloat(vec.y, 0f - amplitude.y, amplitude.y);
		short value3 = CompressFloat(vec.z, 0f - amplitude.z, amplitude.z);
		stream.Serialize(ref value);
		stream.Serialize(ref value2);
		stream.Serialize(ref value3);
	}

	public static Vector3 UnserializeCompressedVector(BitStream stream, Vector3 amplitude)
	{
		short value = 0;
		short value2 = 0;
		short value3 = 0;
		stream.Serialize(ref value);
		stream.Serialize(ref value2);
		stream.Serialize(ref value3);
		float x = DecompressFloat(value, 0f - amplitude.x, amplitude.x);
		float y = DecompressFloat(value2, 0f - amplitude.y, amplitude.y);
		float z = DecompressFloat(value3, 0f - amplitude.z, amplitude.z);
		return new Vector3(x, y, z);
	}

	public static void SerializeAndCompressQuaternion(BitStream stream, Quaternion quat)
	{
		float f = quat.w * quat.w + quat.x * quat.x + quat.y * quat.y + quat.z * quat.z;
		float num = 1f / Mathf.Sqrt(f);
		short value = CompressFloat(quat.w * num, -1f, 1f);
		short value2 = CompressFloat(quat.x * num, -1f, 1f);
		short value3 = CompressFloat(quat.y * num, -1f, 1f);
		short value4 = CompressFloat(quat.z * num, -1f, 1f);
		stream.Serialize(ref value);
		stream.Serialize(ref value2);
		stream.Serialize(ref value3);
		stream.Serialize(ref value4);
	}

	public static Quaternion UnserializeCompressedQuaternion(BitStream stream)
	{
		short value = 0;
		short value2 = 0;
		short value3 = 0;
		short value4 = 0;
		stream.Serialize(ref value);
		stream.Serialize(ref value2);
		stream.Serialize(ref value3);
		stream.Serialize(ref value4);
		float x = DecompressFloat(value, -1f, 1f);
		float y = DecompressFloat(value2, -1f, 1f);
		float z = DecompressFloat(value3, -1f, 1f);
		float w = DecompressFloat(value4, -1f, 1f);
		return new Quaternion(x, y, z, w);
	}

	public static short CompressFloat(float value_in, float minValue, float maxValue)
	{
		float num = value_in;
		if (num > maxValue)
		{
			num = maxValue;
		}
		if (num < minValue)
		{
			num = minValue;
		}
		float num2 = 65535f * (num - minValue) / (maxValue - minValue) - 32768f;
		if (num2 < -32768f)
		{
			num2 = -32768f;
		}
		if (num2 > 32767f)
		{
			num2 = 32767f;
		}
		return (short)num2;
	}

	public static float DecompressFloat(short value_in, float minValue, float maxValue)
	{
		short num = value_in;
		float num2 = minValue + ((float)num + 32768f) / 65535f * (maxValue - minValue);
		if (num2 < minValue)
		{
			num2 = minValue;
		}
		else if (num2 > maxValue)
		{
			num2 = maxValue;
		}
		return num2;
	}
}
