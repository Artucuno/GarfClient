using System;
using System.IO;
using UnityEngine;

public static class Extensions
{
	public static void NullSafe<T>(this T target, Action<T> action)
	{
		if (target != null)
		{
			action(target);
		}
	}

	public static void NullSafeAct<T>(this T pTarget, Action pAction)
	{
		if (pTarget != null)
		{
			pAction();
		}
	}

	public static void NullSafeAct<T, T1>(this T pTarget, Action<T1> pAction, T1 pParam1)
	{
		if (pTarget != null)
		{
			pAction(pParam1);
		}
	}

	public static void NullSafeAct<T, T1, T2>(this T pTarget, Action<T1, T2> pAction, T1 pParam1, T2 pParam2)
	{
		if (pTarget != null)
		{
			pAction(pParam1, pParam2);
		}
	}

	public static void NullSafeAct<T, T1, T2, T3>(this T pTarget, Action<T1, T2, T3> pAction, T1 pParam1, T2 pParam2, T3 pParam3)
	{
		if (pTarget != null)
		{
			pAction(pParam1, pParam2, pParam3);
		}
	}

	public static void NullSafeAct<T, T1, T2, T3, T4>(this T pTarget, Action<T1, T2, T3, T4> pAction, T1 pParam1, T2 pParam2, T3 pParam3, T4 pParam4)
	{
		if (pTarget != null)
		{
			pAction(pParam1, pParam2, pParam3, pParam4);
		}
	}

	public static TResult NullSafeFunc<T, TResult>(this T pTarget, Func<TResult> pFunc)
	{
		if (pTarget != null)
		{
			return pFunc();
		}
		return default(TResult);
	}

	public static TResult NullSafeFunc<T, TResult, T1>(this T pTarget, Func<T1, TResult> pFunc, T1 pParam1)
	{
		if (pTarget != null)
		{
			return pFunc(pParam1);
		}
		return default(TResult);
	}

	public static TResult NullSafeFunc<T, TResult, T1, T2>(this T pTarget, Func<T1, T2, TResult> pFunc, T1 pParam1, T2 pParam2)
	{
		if (pTarget != null)
		{
			return pFunc(pParam1, pParam2);
		}
		return default(TResult);
	}

	public static TResult NullSafeFunc<T, TResult, T1, T2, T3>(this T pTarget, Func<T1, T2, T3, TResult> pFunc, T1 pParam1, T2 pParam2, T3 pParam3)
	{
		if (pTarget != null)
		{
			return pFunc(pParam1, pParam2, pParam3);
		}
		return default(TResult);
	}

	public static TResult NullSafeFunc<T, TResult, T1, T2, T3, T4>(this T pTarget, Func<T1, T2, T3, T4, TResult> pFunc, T1 pParam1, T2 pParam2, T3 pParam3, T4 pParam4)
	{
		if (pTarget != null)
		{
			return pFunc(pParam1, pParam2, pParam3, pParam4);
		}
		return default(TResult);
	}

	public static string Combine(params string[] pPaths)
	{
		string text = null;
		if (pPaths != null)
		{
			text = pPaths[0];
			for (int i = 1; i < pPaths.Length; i++)
			{
				text = Path.Combine(text, pPaths[i]);
			}
		}
		return text;
	}

	public static void FromAxes(ref Quaternion pQuaternion, Vector3 pXaxis, Vector3 pYaxis, Vector3 pZaxis)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity[0, 0] = pXaxis.x;
		identity[1, 0] = pXaxis.y;
		identity[2, 0] = pXaxis.z;
		identity[0, 1] = pYaxis.x;
		identity[1, 1] = pYaxis.y;
		identity[2, 1] = pYaxis.z;
		identity[0, 2] = pZaxis.x;
		identity[1, 2] = pZaxis.y;
		identity[2, 2] = pZaxis.z;
		FromRotationMatrix(ref pQuaternion, identity);
	}

	public static void FromRotationMatrix(ref Quaternion pQuaternion, Matrix4x4 pRotMatrix)
	{
		float num = pRotMatrix[0, 0] + pRotMatrix[1, 1] + pRotMatrix[2, 2];
		float num2;
		if ((double)num > 0.0)
		{
			num2 = Mathf.Sqrt(num + 1f);
			pQuaternion.w = 0.5f * num2;
			num2 = 0.5f / num2;
			pQuaternion.x = (pRotMatrix[2, 1] - pRotMatrix[1, 2]) * num2;
			pQuaternion.y = (pRotMatrix[0, 2] - pRotMatrix[2, 0]) * num2;
			pQuaternion.z = (pRotMatrix[1, 0] - pRotMatrix[0, 1]) * num2;
			return;
		}
		int[] array = new int[3] { 1, 2, 0 };
		int num3 = 0;
		if (pRotMatrix[1, 1] > pRotMatrix[0, 0])
		{
			num3 = 1;
		}
		if (pRotMatrix[2, 2] > pRotMatrix[num3, num3])
		{
			num3 = 2;
		}
		int num4 = array[num3];
		int num5 = array[num4];
		num2 = Mathf.Sqrt(pRotMatrix[num3, num3] - pRotMatrix[num4, num4] - pRotMatrix[num5, num5] + 1f);
		float[] array2 = new float[3] { pQuaternion.x, pQuaternion.y, pQuaternion.z };
		array2[num3] = 0.5f * num2;
		num2 = 0.5f / num2;
		pQuaternion.w = (pRotMatrix[num5, num4] - pRotMatrix[num4, num5]) * num2;
		array2[num4] = (pRotMatrix[num4, num3] + pRotMatrix[num3, num4]) * num2;
		array2[num5] = (pRotMatrix[num5, num3] + pRotMatrix[num3, num5]) * num2;
	}

	public static Vector3 Project(this Vector3 pFrom, Vector3 pTo)
	{
		Vector3 normalized = pTo.normalized;
		return Vector3.Dot(pFrom, normalized) * normalized;
	}
}
