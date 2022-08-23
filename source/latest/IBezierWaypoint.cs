using UnityEngine;

public interface IBezierWaypoint
{
	IBezierControlPoint LeftPoint { get; }

	IBezierControlPoint RightPoint { get; }

	Vector3 CurrentPosition { get; }
}
