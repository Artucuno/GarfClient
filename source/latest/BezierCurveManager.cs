using System;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveManager : MonoBehaviour, IBezierCurveManager
{
	private List<IBezierWaypoint> waypointList = new List<IBezierWaypoint>();

	private Bezier bezier;

	public bool EnableDistanceCalculations;

	public bool DrawPoints;

	public bool DrawCurve = true;

	public bool DrawControlPoints;

	public bool IsFullLoop;

	public float SecondsForFullLoop = 1f;

	public Color DebugColor = Color.white;

	private void Awake()
	{
		Component[] componentsInChildren = base.gameObject.GetComponentsInChildren(typeof(IBezierWaypoint));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			IBezierWaypoint item = (IBezierWaypoint)componentsInChildren[i];
			waypointList.Add(item);
		}
		bezier = new Bezier(waypointList.ToArray());
	}

	private void Start()
	{
		if (EnableDistanceCalculations)
		{
			bezier.SetUpDistanceLists(IsFullLoop);
			SecondsForFullLoop = 100f - bezier.MaxDistance / 10f;
		}
	}

	public Vector3 GetPositionAtTime(float time)
	{
		time /= SecondsForFullLoop;
		return bezier.GetPointAtTime(time, IsFullLoop);
	}

	public Vector3 GetPositionAtDistance(float distance, float time)
	{
		if (EnableDistanceCalculations)
		{
			time /= SecondsForFullLoop;
			float num = bezier.LookupDistanceOfExistingTime(time);
			float t = bezier.FindTimePointAlongeSplineAtDistance(distance + num);
			return bezier.GetPointAtTime(t, IsFullLoop);
		}
		throw new Exception("In order to use GetPositionAtDistance the EnableDistanceCalculation variable must be set to true at start up so the distance points can be pre-calculated");
	}

	public void OnDrawGizmos()
	{
		List<BezierWaypoint> list = new List<BezierWaypoint>();
		Component[] componentsInChildren = base.gameObject.GetComponentsInChildren(typeof(BezierWaypoint));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			BezierWaypoint bezierWaypoint = (BezierWaypoint)componentsInChildren[i];
			bezierWaypoint.SetControlPoints();
			if (bezierWaypoint.IsValid)
			{
				list.Add(bezierWaypoint);
			}
		}
		if (!DrawCurve || list.Count == 0)
		{
			return;
		}
		Bezier bezier = new Bezier(list.ToArray());
		for (float num = 0f; num < 1f; num += 0.001f)
		{
			if (num < 0.999f)
			{
				Gizmos.color = DebugColor;
				Gizmos.DrawLine(bezier.GetPointAtTime(num, IsFullLoop), bezier.GetPointAtTime(num + 0.001f, IsFullLoop));
			}
		}
	}
}
