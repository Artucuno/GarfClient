using UnityEngine;

public interface IBezierCurveManager
{
	Vector3 GetPositionAtTime(float time);

	Vector3 GetPositionAtDistance(float distance, float time);
}
