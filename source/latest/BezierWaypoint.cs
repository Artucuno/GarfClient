using UnityEngine;

public class BezierWaypoint : MonoBehaviour, IBezierWaypoint
{
	private IBezierControlPoint leftPoint;

	private IBezierControlPoint rightPoint;

	public bool IsValid
	{
		get
		{
			return LeftPoint != null && RightPoint != null;
		}
	}

	public IBezierControlPoint LeftPoint
	{
		get
		{
			return leftPoint;
		}
		set
		{
			leftPoint = value;
		}
	}

	public IBezierControlPoint RightPoint
	{
		get
		{
			return rightPoint;
		}
		set
		{
			rightPoint = value;
		}
	}

	public Vector3 CurrentPosition
	{
		get
		{
			return base.transform.position;
		}
	}

	private void Awake()
	{
		SetControlPoints();
	}

	public void SetControlPoints()
	{
		Component[] componentsInChildren = GetComponentsInChildren(typeof(IBezierControlPoint));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			IBezierControlPoint bezierControlPoint = (IBezierControlPoint)componentsInChildren[i];
			if (bezierControlPoint.Side == BezierControlPointSide.Left)
			{
				LeftPoint = bezierControlPoint;
			}
			else if (bezierControlPoint.Side == BezierControlPointSide.Right)
			{
				RightPoint = bezierControlPoint;
			}
			else
			{
				Debug.LogError("Bezier Curve control points must be set either left or right in the Editor");
			}
		}
	}

	public void SetPositionOfOther(IBezierControlPoint controlPoint, Vector3 vectorToFootPoint)
	{
		if (RightPoint != null && LeftPoint != null)
		{
			vectorToFootPoint.Normalize();
			if (controlPoint.Side == BezierControlPointSide.Left)
			{
				float magnitude = (CurrentPosition - RightPoint.CurrentPosition).magnitude;
				RightPoint.CurrentPosition = CurrentPosition + vectorToFootPoint * magnitude;
			}
			else
			{
				float magnitude2 = (CurrentPosition - LeftPoint.CurrentPosition).magnitude;
				LeftPoint.CurrentPosition = CurrentPosition + vectorToFootPoint * magnitude2;
			}
		}
	}

	private void OnDrawGizmos()
	{
		BezierCurveManager bezierCurveManager = base.transform.parent.GetComponent(typeof(BezierCurveManager)) as BezierCurveManager;
		if (!IsValid)
		{
			return;
		}
		if (bezierCurveManager.DrawPoints)
		{
			Gizmos.DrawIcon(base.transform.position, "/BezierWaypoint.png");
		}
		if (bezierCurveManager.DrawControlPoints)
		{
			SetControlPoints();
			if (RightPoint != null && LeftPoint != null)
			{
				Gizmos.DrawLine(RightPoint.CurrentPosition, LeftPoint.CurrentPosition);
			}
		}
	}
}
