using System;
using System.Collections.Generic;
using UnityEngine;

public class SplineInterpolator : MonoBehaviour
{
	internal class SplineNode
	{
		internal Vector3 Point;

		internal Quaternion Rot;

		internal float Time;

		internal Vector2 EaseIO;

		internal SplineNode(Vector3 p, Quaternion q, float t, Vector2 io)
		{
			Point = p;
			Rot = q;
			Time = t;
			EaseIO = io;
		}

		internal SplineNode(SplineNode o)
		{
			Point = o.Point;
			Rot = o.Rot;
			Time = o.Time;
			EaseIO = o.EaseIO;
		}
	}

	private eEndPointsMode mEndPointsMode;

	private List<SplineNode> mNodes = new List<SplineNode>();

	private string mState = string.Empty;

	private bool mRotations;

	private OnEndCallback mOnEndCallback;

	private float mCurrentTime;

	private int mCurrentIdx = 1;

	private void Awake()
	{
		Reset();
	}

	public void StartInterpolation(OnEndCallback endCallback, bool bRotations, eWrapMode mode)
	{
		if (mState != "Reset")
		{
			throw new Exception("First reset, add points and then call here");
		}
		mState = ((mode != 0) ? "Loop" : "Once");
		mRotations = bRotations;
		mOnEndCallback = endCallback;
		SetInput();
	}

	public void Reset()
	{
		mNodes.Clear();
		mState = "Reset";
		mCurrentIdx = 1;
		mCurrentTime = 0f;
		mRotations = false;
		mEndPointsMode = eEndPointsMode.AUTO;
	}

	public void AddPoint(Vector3 pos, Quaternion quat, float timeInSeconds, Vector2 easeInOut)
	{
		if (mState != "Reset")
		{
			throw new Exception("Cannot add points after start");
		}
		mNodes.Add(new SplineNode(pos, quat, timeInSeconds, easeInOut));
	}

	private void SetInput()
	{
		if (mNodes.Count < 2)
		{
			throw new Exception("Invalid number of points");
		}
		if (mRotations)
		{
			for (int i = 1; i < mNodes.Count; i++)
			{
				SplineNode splineNode = mNodes[i];
				SplineNode splineNode2 = mNodes[i - 1];
				if (Quaternion.Dot(splineNode.Rot, splineNode2.Rot) < 0f)
				{
					splineNode.Rot.x = 0f - splineNode.Rot.x;
					splineNode.Rot.y = 0f - splineNode.Rot.y;
					splineNode.Rot.z = 0f - splineNode.Rot.z;
					splineNode.Rot.w = 0f - splineNode.Rot.w;
				}
			}
		}
		if (mEndPointsMode == eEndPointsMode.AUTO)
		{
			mNodes.Insert(0, mNodes[0]);
			mNodes.Add(mNodes[mNodes.Count - 1]);
		}
		else if (mEndPointsMode == eEndPointsMode.EXPLICIT && mNodes.Count < 4)
		{
			throw new Exception("Invalid number of points");
		}
	}

	private void SetExplicitMode()
	{
		if (mState != "Reset")
		{
			throw new Exception("Cannot change mode after start");
		}
		mEndPointsMode = eEndPointsMode.EXPLICIT;
	}

	public void SetAutoCloseMode(float joiningPointTime)
	{
		if (mState != "Reset")
		{
			throw new Exception("Cannot change mode after start");
		}
		mEndPointsMode = eEndPointsMode.AUTOCLOSED;
		mNodes.Add(new SplineNode(mNodes[0]));
		mNodes[mNodes.Count - 1].Time = joiningPointTime;
		Vector3 normalized = (mNodes[1].Point - mNodes[0].Point).normalized;
		Vector3 normalized2 = (mNodes[mNodes.Count - 2].Point - mNodes[mNodes.Count - 1].Point).normalized;
		float magnitude = (mNodes[1].Point - mNodes[0].Point).magnitude;
		float magnitude2 = (mNodes[mNodes.Count - 2].Point - mNodes[mNodes.Count - 1].Point).magnitude;
		SplineNode splineNode = new SplineNode(mNodes[0]);
		splineNode.Point = mNodes[0].Point + normalized2 * magnitude;
		SplineNode splineNode2 = new SplineNode(mNodes[mNodes.Count - 1]);
		splineNode2.Point = mNodes[0].Point + normalized * magnitude2;
		mNodes.Insert(0, splineNode);
		mNodes.Add(splineNode2);
	}

	private void Update()
	{
		if (mState == "Reset" || mState == "Stopped" || mNodes.Count < 4)
		{
			return;
		}
		mCurrentTime += Time.deltaTime;
		if (mCurrentTime >= mNodes[mCurrentIdx + 1].Time)
		{
			if (mCurrentIdx < mNodes.Count - 3)
			{
				mCurrentIdx++;
			}
			else if (mState != "Loop")
			{
				mState = "Stopped";
				base.transform.position = mNodes[mNodes.Count - 2].Point;
				if (mRotations)
				{
					base.transform.rotation = mNodes[mNodes.Count - 2].Rot;
				}
				if (mOnEndCallback != null)
				{
					mOnEndCallback();
				}
			}
			else
			{
				mCurrentIdx = 1;
				mCurrentTime = 0f;
			}
		}
		if (mState != "Stopped")
		{
			float t = (mCurrentTime - mNodes[mCurrentIdx].Time) / (mNodes[mCurrentIdx + 1].Time - mNodes[mCurrentIdx].Time);
			t = MathUtils.Ease(t, mNodes[mCurrentIdx].EaseIO.x, mNodes[mCurrentIdx].EaseIO.y);
			base.transform.position = GetHermiteInternal(mCurrentIdx, t);
			if (mRotations)
			{
				base.transform.rotation = GetSquad(mCurrentIdx, t);
			}
		}
	}

	private Quaternion GetSquad(int idxFirstPoint, float t)
	{
		Quaternion rot = mNodes[idxFirstPoint - 1].Rot;
		Quaternion rot2 = mNodes[idxFirstPoint].Rot;
		Quaternion rot3 = mNodes[idxFirstPoint + 1].Rot;
		Quaternion rot4 = mNodes[idxFirstPoint + 2].Rot;
		Quaternion squadIntermediate = MathUtils.GetSquadIntermediate(rot, rot2, rot3);
		Quaternion squadIntermediate2 = MathUtils.GetSquadIntermediate(rot2, rot3, rot4);
		return MathUtils.GetQuatSquad(t, rot2, rot3, squadIntermediate, squadIntermediate2);
	}

	public Vector3 GetHermiteInternal(int idxFirstPoint, float t)
	{
		float num = t * t;
		float num2 = num * t;
		Vector3 point = mNodes[idxFirstPoint - 1].Point;
		Vector3 point2 = mNodes[idxFirstPoint].Point;
		Vector3 point3 = mNodes[idxFirstPoint + 1].Point;
		Vector3 point4 = mNodes[idxFirstPoint + 2].Point;
		float num3 = 0.5f;
		Vector3 vector = num3 * (point3 - point);
		Vector3 vector2 = num3 * (point4 - point2);
		float num4 = 2f * num2 - 3f * num + 1f;
		float num5 = -2f * num2 + 3f * num;
		float num6 = num2 - 2f * num + t;
		float num7 = num2 - num;
		return num4 * point2 + num5 * point3 + num6 * vector + num7 * vector2;
	}

	public Vector3 GetHermiteAtTime(float timeParam)
	{
		if (timeParam >= mNodes[mNodes.Count - 2].Time)
		{
			return mNodes[mNodes.Count - 2].Point;
		}
		int i;
		for (i = 1; i < mNodes.Count - 2 && !(mNodes[i].Time > timeParam); i++)
		{
		}
		int num = i - 1;
		float t = (timeParam - mNodes[num].Time) / (mNodes[num + 1].Time - mNodes[num].Time);
		t = MathUtils.Ease(t, mNodes[num].EaseIO.x, mNodes[num].EaseIO.y);
		return GetHermiteInternal(num, t);
	}
}
