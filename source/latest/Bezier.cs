using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
	private const int NumberOfArray = 1000;

	private const float OneOverNum = 0.001f;

	private List<IBezierWaypoint> waypointList;

	private Vector3[] vectorArray;

	private float[] distanceArray = new float[1000];

	private float[] timeArray = new float[1000];

	public float MaxDistance;

	public Bezier(IBezierWaypoint[] waypointsArray)
	{
		waypointList = new List<IBezierWaypoint>();
		foreach (IBezierWaypoint item in waypointsArray)
		{
			waypointList.Add(item);
		}
	}

	public void SetUpDistanceLists(bool fullLoop)
	{
		distanceArray[0] = 0f;
		timeArray[0] = 0f;
		Vector3 vector = GetPointAtTime(0f, fullLoop);
		Vector3 zero = Vector3.zero;
		float num = 0f;
		for (int i = 1; i < 1000; i++)
		{
			zero = GetPointAtTime((float)i * 0.001f, fullLoop);
			num += (vector - zero).magnitude;
			distanceArray[i] = num;
			timeArray[i] = (float)i * 0.001f;
			vector = zero;
		}
		MaxDistance = num;
	}

	public Vector3 GetPointAtTime(float t, bool fullLoop)
	{
		t %= 1f;
		if (t < 0f)
		{
			t = 1f + t;
		}
		int num = ((!fullLoop) ? (waypointList.Count - 1) : waypointList.Count);
		int num2 = Mathf.FloorToInt(t * (float)num);
		float t2 = t * (float)num - (float)num2;
		Vector3 currentPosition = waypointList[CheckWithinArray(num2, waypointList.Count)].CurrentPosition;
		Vector3 currentPosition2 = waypointList[CheckWithinArray(num2, waypointList.Count)].RightPoint.CurrentPosition;
		Vector3 currentPosition3 = waypointList[CheckWithinArray(num2 + 1, waypointList.Count)].LeftPoint.CurrentPosition;
		Vector3 currentPosition4 = waypointList[CheckWithinArray(num2 + 1, waypointList.Count)].CurrentPosition;
		return DoBezierFor4Points(t2, currentPosition, currentPosition2, currentPosition3, currentPosition4);
	}

	private int CheckWithinArray(int x, int c)
	{
		if (x >= c)
		{
			return x % c;
		}
		return x;
	}

	private Vector3 DoBezierFor4Points(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 zero = Vector3.zero;
		float num = 1f - t;
		zero += p0 * num * num * num;
		zero += p1 * t * 3f * num * num;
		zero += p2 * 3f * t * t * num;
		return zero + p3 * t * t * t;
	}

	public float FindTimePointAlongeSplineAtDistance(float distance)
	{
		distance %= MaxDistance;
		if (distance < 0f)
		{
			distance = MaxDistance + distance;
		}
		float result = 0f;
		for (int i = 0; i < 1000; i++)
		{
			result = timeArray[i];
			if (distance < distanceArray[i])
			{
				break;
			}
		}
		return result;
	}

	public float LookupDistanceOfExistingTime(float time)
	{
		time %= 1f;
		float result = 0f;
		for (int i = 0; i < 1000; i++)
		{
			result = distanceArray[i];
			if (time < timeArray[i])
			{
				break;
			}
		}
		return result;
	}
}
