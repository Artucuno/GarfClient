using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplineInterpolator))]
[AddComponentMenu("Splines/Spline Controller")]
public class SplineController : MonoBehaviour
{
	public GameObject SplineRoot;

	public float Duration = 10f;

	public eOrientationMode OrientationMode;

	public eWrapMode WrapMode;

	public bool AutoStart = true;

	public bool AutoClose = true;

	public bool HideOnExecute = true;

	private SplineInterpolator mSplineInterp;

	private Transform[] mTransforms;

	private void OnDrawGizmos()
	{
		Transform[] transforms = GetTransforms();
		if (transforms.Length >= 2)
		{
			SplineInterpolator splineInterpolator = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
			SetupSplineInterpolator(splineInterpolator, transforms);
			splineInterpolator.StartInterpolation(null, false, WrapMode);
			Vector3 vector = transforms[0].position;
			for (int i = 1; i <= 100; i++)
			{
				float timeParam = (float)i * Duration / 100f;
				Vector3 hermiteAtTime = splineInterpolator.GetHermiteAtTime(timeParam);
				float r = (hermiteAtTime - vector).magnitude * 2f;
				Gizmos.color = new Color(r, 0f, 0f, 1f);
				Gizmos.DrawLine(vector, hermiteAtTime);
				vector = hermiteAtTime;
			}
		}
	}

	private void Start()
	{
		mSplineInterp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
		mTransforms = GetTransforms();
		if (HideOnExecute)
		{
			DisableTransforms();
		}
		if (AutoStart)
		{
			FollowSpline();
		}
	}

	private void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
	{
		interp.Reset();
		float num = ((!AutoClose) ? (Duration / (float)(trans.Length - 1)) : (Duration / (float)trans.Length));
		int i;
		for (i = 0; i < trans.Length; i++)
		{
			if (OrientationMode == eOrientationMode.NODE)
			{
				interp.AddPoint(trans[i].position, trans[i].rotation, num * (float)i, new Vector2(0f, 1f));
			}
			else if (OrientationMode == eOrientationMode.TANGENT)
			{
				Quaternion quat = ((i != trans.Length - 1) ? Quaternion.LookRotation(trans[i + 1].position - trans[i].position, trans[i].up) : ((!AutoClose) ? trans[i].rotation : Quaternion.LookRotation(trans[0].position - trans[i].position, trans[i].up)));
				interp.AddPoint(trans[i].position, quat, num * (float)i, new Vector2(0f, 1f));
			}
		}
		if (AutoClose)
		{
			interp.SetAutoCloseMode(num * (float)i);
		}
	}

	private Transform[] GetTransforms()
	{
		if (SplineRoot != null)
		{
			List<Component> list = new List<Component>(SplineRoot.GetComponentsInChildren(typeof(Transform)));
			List<Transform> list2 = list.ConvertAll((Component c) => (Transform)c);
			list2.Remove(SplineRoot.transform);
			list2.Sort((Transform a, Transform b) => a.name.CompareTo(b.name));
			return list2.ToArray();
		}
		return null;
	}

	private void DisableTransforms()
	{
		if (SplineRoot != null)
		{
			SplineRoot.SetActive(false);
		}
	}

	private void FollowSpline()
	{
		if (mTransforms.Length > 0)
		{
			SetupSplineInterpolator(mSplineInterp, mTransforms);
			mSplineInterp.StartInterpolation(null, true, WrapMode);
		}
	}
}
