using UnityEngine;

public interface IBezierControlPoint
{
	BezierControlPointSide Side { get; }

	Vector3 CurrentPosition { get; set; }
}
