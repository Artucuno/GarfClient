using UnityEngine;

public class BezierControlPoint : MonoBehaviour, IBezierControlPoint
{
	public BezierControlPointSide side;

	public BezierControlPointSide Side
	{
		get
		{
			return side;
		}
	}

	public Vector3 CurrentPosition
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			base.transform.position = value;
		}
	}

	private void OnDrawGizmos()
	{
		if (base.transform.parent.parent != null)
		{
			BezierCurveManager bezierCurveManager = base.transform.parent.parent.GetComponent(typeof(BezierCurveManager)) as BezierCurveManager;
			if (bezierCurveManager.DrawControlPoints)
			{
				Gizmos.DrawIcon(base.transform.position, "/BezierControlPoint.png");
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (base.transform.parent != null)
		{
			Component component = base.transform.parent.GetComponent(typeof(BezierWaypoint));
			if (component != null && component is BezierWaypoint)
			{
				BezierWaypoint bezierWaypoint = component as BezierWaypoint;
				Vector3 vectorToFootPoint = bezierWaypoint.CurrentPosition - base.transform.position;
				bezierWaypoint.SetPositionOfOther(this, vectorToFootPoint);
			}
		}
	}
}
