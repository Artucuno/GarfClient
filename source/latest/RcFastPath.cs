using UnityEngine;

public class RcFastPath
{
	public enum PathSide
	{
		SIDE_ANY,
		SIDE_LEFT,
		SIDE_RIGHT
	}

	public const int INVALID_PATH_INDEX = -1;

	private const float FASTPATH_SPLINE_TENSION_INV = 0.166666f;

	protected int m_iNbPoints;

	protected int m_iMaxArraySize;

	protected Vector3[] m_pVectorPoints;

	protected float[] m_pSegmentLengths;

	protected float[] m_pDistToEndOfPath;

	protected float m_fTotalLength;

	protected bool m_bFlagLoop;

	public RcFastPath(int _arraySize)
	{
		m_iNbPoints = 0;
		m_iMaxArraySize = _arraySize;
		m_fTotalLength = 0f;
		m_bFlagLoop = false;
		m_pVectorPoints = new Vector3[m_iMaxArraySize];
		m_pSegmentLengths = new float[m_iMaxArraySize];
		m_pDistToEndOfPath = new float[m_iMaxArraySize];
		for (int i = 0; i < m_iMaxArraySize; i++)
		{
			m_pVectorPoints[i] = Vector3.zero;
			m_pSegmentLengths[i] = 0f;
			m_pDistToEndOfPath[i] = 0f;
		}
	}

	public RcFastPath(GameObject slowPath)
	{
		m_iNbPoints = 0;
		m_iMaxArraySize = 0;
		m_fTotalLength = 0f;
		m_bFlagLoop = false;
		m_bFlagLoop = slowPath.GetComponent<RcFastPathComp>().IsLooping();
		m_iNbPoints = slowPath.transform.GetChildCount();
		m_iMaxArraySize = m_iNbPoints;
		if (m_iNbPoints <= 0)
		{
			return;
		}
		m_pVectorPoints = new Vector3[m_iMaxArraySize];
		m_pSegmentLengths = new float[m_iMaxArraySize];
		m_pDistToEndOfPath = new float[m_iMaxArraySize];
		int num = m_iNbPoints - 1;
		for (int num2 = m_iNbPoints - 1; num2 >= 0; num2--)
		{
			m_pVectorPoints[num] = slowPath.transform.GetChild(num2).position;
			int num3 = num + 1;
			int num4 = num2;
			num4 = ((num3 < m_iNbPoints) ? (num4 + 1) : 0);
			Vector3 position = slowPath.transform.GetChild(num4).position;
			Vector3 vector = position - m_pVectorPoints[num];
			m_pSegmentLengths[num] = vector.magnitude;
			if (num < m_iNbPoints - 1)
			{
				m_fTotalLength += m_pSegmentLengths[num];
				m_pDistToEndOfPath[num] = m_pDistToEndOfPath[num + 1] + m_pSegmentLengths[num];
			}
			else if (m_bFlagLoop)
			{
				m_pDistToEndOfPath[num] = m_pSegmentLengths[num];
			}
			else
			{
				m_pDistToEndOfPath[num] = 0f;
			}
			num--;
		}
	}

	public float GetDistToEndOfPath(int pInd, float ratio)
	{
		if (m_iNbPoints == 0)
		{
			return 0f;
		}
		return m_pDistToEndOfPath[pInd] - m_pSegmentLengths[pInd] * ratio;
	}

	public int GetSegmentForEndOfPathDist(float dist, out float ratio)
	{
		if (m_iNbPoints <= 1 || dist >= m_fTotalLength)
		{
			ratio = 0f;
			return (m_iNbPoints == 0) ? (-1) : 0;
		}
		if (dist <= 0f)
		{
			ratio = 1f;
			return m_iNbPoints - 1;
		}
		int num = 0;
		int num2 = m_iNbPoints - 1;
		while (num + 1 < num2)
		{
			int num3 = (num + num2) / 2;
			if (dist <= m_pDistToEndOfPath[num3])
			{
				num = num3;
			}
			else
			{
				num2 = num3;
			}
		}
		ratio = (m_pDistToEndOfPath[num] - dist) / m_pSegmentLengths[num];
		return num;
	}

	public Vector3 GetPositionPoint(int pInd)
	{
		return m_pVectorPoints[pInd];
	}

	public Vector3 GetSegment(int segInd)
	{
		if (m_iNbPoints <= 0)
		{
			return Vector3.zero;
		}
		int num = segInd + 1;
		if (num >= m_iNbPoints)
		{
			num -= m_iNbPoints;
		}
		return m_pVectorPoints[num] - m_pVectorPoints[segInd];
	}

	public bool IsOnRight(Vector3 pt, int index, Vector3 up)
	{
		Vector3 segment = GetSegment(index);
		Vector3 rhs = pt - m_pVectorPoints[index];
		Vector3 lhs = Vector3.Cross(segment, rhs);
		return -1f * Vector3.Dot(lhs, up) <= 0f;
	}

	public bool IsOnRight(Vector3 pt, PathPosition _pathPosition, Vector3 up)
	{
		return IsOnRight(pt, _pathPosition.index, up);
	}

	public void GetRawPos(PathPosition _pathPosition, out Vector3 outPos)
	{
		GetRawPos(_pathPosition.index, _pathPosition.ratio, out outPos);
	}

	public void GetRawPos(int segmentIndex, float prc, out Vector3 outPos)
	{
		if (m_pVectorPoints != null && m_iNbPoints != 0)
		{
			if (segmentIndex < m_iNbPoints)
			{
				if (m_iNbPoints < 2 || prc == 0f)
				{
					outPos = m_pVectorPoints[segmentIndex];
				}
				else
				{
					outPos = m_pVectorPoints[segmentIndex] + GetSegment(segmentIndex) * prc;
				}
			}
			else
			{
				outPos = Vector3.zero;
			}
		}
		else
		{
			outPos = Vector3.zero;
		}
	}

	public Vector3 GetSegmentBegin(int segInd)
	{
		return m_pVectorPoints[segInd];
	}

	public Vector3 GetSegmentEnd(int segInd)
	{
		return m_pVectorPoints[(segInd + 1) % m_iNbPoints];
	}

	public virtual void SetSize(int _newSize, bool _updateLengthsAndDistances)
	{
		if (_newSize >= m_iMaxArraySize)
		{
			_newSize = m_iMaxArraySize;
		}
		for (int i = m_iNbPoints; i < _newSize; i++)
		{
			m_pVectorPoints[i] = Vector3.zero;
			m_pSegmentLengths[i] = 0f;
			m_pDistToEndOfPath[i] = 0f;
		}
		m_iNbPoints = _newSize;
		if (!_updateLengthsAndDistances)
		{
			return;
		}
		for (int num = m_iNbPoints - 1; num >= 0; num--)
		{
			if (num < m_iNbPoints - 1)
			{
				m_fTotalLength += m_pSegmentLengths[num];
				m_pDistToEndOfPath[num] = m_pDistToEndOfPath[num + 1] + m_pSegmentLengths[num];
			}
			else
			{
				m_pDistToEndOfPath[num] = 0f;
			}
		}
	}

	public void SetPointPos(int pointIndex, Vector3 _newPosition)
	{
		if (pointIndex < 0 || pointIndex >= m_iNbPoints)
		{
			return;
		}
		m_pVectorPoints[pointIndex] = _newPosition;
		int num = pointIndex + 1;
		if (num >= m_iNbPoints)
		{
			num -= m_iNbPoints;
		}
		int num2 = pointIndex - 1;
		if (num2 < 0)
		{
			num2 += m_iNbPoints;
		}
		m_fTotalLength -= m_pSegmentLengths[num2];
		m_fTotalLength -= m_pSegmentLengths[pointIndex];
		m_pSegmentLengths[num2] = (m_pVectorPoints[pointIndex] - m_pVectorPoints[num2]).magnitude;
		m_pSegmentLengths[pointIndex] = (m_pVectorPoints[num] - m_pVectorPoints[pointIndex]).magnitude;
		m_fTotalLength += m_pSegmentLengths[num2];
		m_fTotalLength += m_pSegmentLengths[pointIndex];
		for (int num3 = pointIndex; num3 >= 0; num3--)
		{
			if (num3 < m_iNbPoints - 1)
			{
				m_pDistToEndOfPath[num3] = m_pDistToEndOfPath[num3 + 1] + m_pSegmentLengths[num3];
			}
			else
			{
				m_pDistToEndOfPath[num3] = 0f;
			}
		}
	}

	public void ResetSidedPosition(ref PathPosition _pathPosition, Vector3 position, bool _loopSearch, PathSide _side, Vector3 _up)
	{
		if (m_iNbPoints <= 0)
		{
			_pathPosition = PathPosition.UNDEFINED_POSITION;
		}
		else if (m_iNbPoints > 1)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			int index = -1;
			float ratio = 0f;
			int num3 = m_iNbPoints - 1;
			if (!_loopSearch)
			{
				num3 = m_iNbPoints - 2;
			}
			for (int i = 0; i <= num3; i++)
			{
				Vector3 vector = m_pVectorPoints[i];
				int num4 = ((i >= m_iNbPoints - 1) ? (i - m_iNbPoints + 1) : (i + 1));
				Vector3 vector2 = m_pVectorPoints[num4];
				float num5 = 0f;
				if (m_pSegmentLengths[i] != 0f)
				{
					float num6 = Vector3.Dot(position - vector, vector2 - vector);
					num5 = num6 / (m_pSegmentLengths[i] * m_pSegmentLengths[i]);
				}
				else
				{
					num5 = -1f;
				}
				if (num5 < 0f)
				{
					num2 = (vector - position).sqrMagnitude;
				}
				else if (num5 > 1f)
				{
					num2 = (vector2 - position).sqrMagnitude;
				}
				else
				{
					float num7 = (1f - num5) * vector.x + num5 * vector2.x - position.x;
					float num8 = (1f - num5) * vector.y + num5 * vector2.y - position.y;
					float num9 = (1f - num5) * vector.z + num5 * vector2.z - position.z;
					num2 = num7 * num7 + num8 * num8 + num9 * num9;
				}
				if (i == 0 || num2 < num)
				{
					bool flag = _side == PathSide.SIDE_ANY;
					if (!flag)
					{
						bool flag2 = IsOnRight(position, i, _up);
						flag = (flag2 && _side == PathSide.SIDE_RIGHT) || (!flag2 && _side == PathSide.SIDE_LEFT);
					}
					if (flag)
					{
						num = num2;
						index = i;
						ratio = num5;
					}
				}
			}
			_pathPosition.index = index;
			_pathPosition.ratio = ratio;
			_pathPosition.sqrDist = num;
		}
		else
		{
			_pathPosition.index = 0;
			_pathPosition.ratio = 0f;
			_pathPosition.sqrDist = (m_pVectorPoints[0] - position).sqrMagnitude;
		}
	}

	public void UpdatePosition(ref PathPosition _pathPosition, Vector3 position, int startIndex, int endIndex, bool _loopSearch)
	{
		if (m_iNbPoints <= 0)
		{
			_pathPosition = PathPosition.UNDEFINED_POSITION;
		}
		else if (m_iNbPoints > 1)
		{
			float num = 1E+38f;
			float num2 = 1E+38f;
			int index = -1;
			float ratio = 0f;
			bool flag = startIndex > endIndex;
			_loopSearch = _loopSearch && m_bFlagLoop;
			if (!_loopSearch && endIndex >= m_iNbPoints - 1)
			{
				endIndex = m_iNbPoints - 2;
			}
			for (int i = startIndex; i <= endIndex || flag; i++)
			{
				Vector3 vector = m_pVectorPoints[i];
				int num3 = ((i >= m_iNbPoints - 1) ? (i - m_iNbPoints + 1) : (i + 1));
				Vector3 vector2 = m_pVectorPoints[num3];
				float num4 = 0f;
				if (m_pSegmentLengths[i] != 0f)
				{
					float num5 = Vector3.Dot(position - vector, vector2 - vector);
					num4 = num5 / (m_pSegmentLengths[i] * m_pSegmentLengths[i]);
				}
				else
				{
					num4 = -1f;
				}
				if (num4 < 0f)
				{
					num2 = (vector - position).sqrMagnitude;
				}
				else if (num4 > 1f)
				{
					num2 = (vector2 - position).sqrMagnitude;
				}
				else
				{
					float num6 = (1f - num4) * vector.x + num4 * vector2.x - position.x;
					float num7 = (1f - num4) * vector.y + num4 * vector2.y - position.y;
					float num8 = (1f - num4) * vector.z + num4 * vector2.z - position.z;
					num2 = num6 * num6 + num7 * num7 + num8 * num8;
				}
				if (i == startIndex || num2 < num)
				{
					num = num2;
					index = i;
					ratio = num4;
				}
				if (flag && i == m_iNbPoints - 1)
				{
					i = -1;
					flag = false;
				}
			}
			_pathPosition.index = index;
			_pathPosition.ratio = ratio;
			_pathPosition.sqrDist = num;
		}
		else
		{
			_pathPosition.index = 0;
			_pathPosition.ratio = 0f;
			_pathPosition.sqrDist = (m_pVectorPoints[0] - position).sqrMagnitude;
		}
	}

	public void UpdatePosition2D(ref PathPosition _pathPosition, Vector3 position, int startIndex, int endIndex, bool _loopSearch)
	{
		if (m_iNbPoints <= 0)
		{
			_pathPosition = PathPosition.UNDEFINED_POSITION;
		}
		else if (m_iNbPoints > 1)
		{
			if (!_loopSearch && endIndex >= m_iNbPoints - 1)
			{
				endIndex = m_iNbPoints - 2;
			}
			float num = 1E+38f;
			int index = -1;
			float ratio = 0f;
			bool flag = startIndex > endIndex;
			for (int i = startIndex; i <= endIndex || flag; i++)
			{
				float x = m_pVectorPoints[i].x;
				float z = m_pVectorPoints[i].z;
				int num2 = i + 1;
				if (num2 >= m_iNbPoints)
				{
					num2 -= m_iNbPoints;
				}
				float x2 = m_pVectorPoints[num2].x;
				float z2 = m_pVectorPoints[num2].z;
				float num3 = 0f;
				float num4 = 0f;
				if (m_pSegmentLengths[i] != 0f)
				{
					num3 = (position.x - x) * (x2 - x) + (position.z - z) * (z2 - z);
					num4 = num3 / (m_pSegmentLengths[i] * m_pSegmentLengths[i]);
				}
				else
				{
					num4 = -1f;
				}
				float num5;
				float num6;
				if (num4 < 0f)
				{
					num5 = x - position.x;
					num6 = z - position.z;
				}
				else if (num4 > 1f)
				{
					num5 = x2 - position.x;
					num6 = z2 - position.z;
				}
				else
				{
					num5 = (1f - num4) * x + num4 * x2 - position.x;
					num6 = (1f - num4) * z + num4 * z2 - position.z;
				}
				float num7 = num5 * num5 + num6 * num6;
				if (i == startIndex || num7 < num)
				{
					num = num7;
					index = i;
					ratio = num4;
				}
				if (flag && i == m_iNbPoints - 1)
				{
					i = -1;
					flag = false;
				}
			}
			_pathPosition.index = index;
			_pathPosition.ratio = ratio;
			_pathPosition.sqrDist = num;
		}
		else
		{
			float num5 = m_pVectorPoints[0].x - position.x;
			float num6 = m_pVectorPoints[0].z - position.z;
			_pathPosition.index = 0;
			_pathPosition.ratio = 0f;
			_pathPosition.sqrDist = num5 * num5 + num6 * num6;
		}
	}

	public float GetSignedDistToSegment2D(Vector3 position, int index, Vector3 up)
	{
		if (m_iNbPoints > 1)
		{
			int num = ((index >= m_iNbPoints) ? (index - m_iNbPoints) : index);
			int num2 = num + 1;
			if (num2 >= m_iNbPoints)
			{
				num2 -= m_iNbPoints;
			}
			Vector3 vector = position - Vector3.Dot(position, up) * up;
			Vector3 vector2 = m_pVectorPoints[num] - Vector3.Dot(m_pVectorPoints[num], up) * up;
			Vector3 vector3 = m_pVectorPoints[num2] - Vector3.Dot(m_pVectorPoints[num2], up) * up;
			float num3 = RcUtils.PointToSegmentDistance(vector, vector2, vector3);
			Vector3 lhs = Vector3.Cross(vector3 - vector2, vector - vector2);
			if (-1f * Vector3.Dot(lhs, up) <= 0f)
			{
				return 0f - num3;
			}
			return num3;
		}
		return -1E+37f;
	}

	public float GetSegmentLength(int segInd)
	{
		if (segInd < m_iNbPoints)
		{
			return m_pSegmentLengths[segInd];
		}
		return 0f;
	}

	public bool IsLooping()
	{
		return m_bFlagLoop;
	}

	public void SetLoop(bool bLoop)
	{
		m_bFlagLoop = bLoop;
	}

	public void ResetPosition(ref PathPosition _pathPosition, Vector3 position, bool _loopSearch)
	{
		if (m_iNbPoints != 0)
		{
			UpdatePosition(ref _pathPosition, position, 0, m_iNbPoints - 1, _loopSearch);
		}
		else
		{
			_pathPosition = PathPosition.UNDEFINED_POSITION;
		}
	}

	public int GetNbPoints()
	{
		return m_iNbPoints;
	}

	public void UpdatePathPosition(ref PathPosition _pathPosition, Vector3 _position3D, int segmentForward, int segmentBackward, bool _2D, bool _loopSearch)
	{
		if (m_iNbPoints <= 0)
		{
			_pathPosition = PathPosition.UNDEFINED_POSITION;
		}
		else if (_pathPosition.index != -1)
		{
			int i = _pathPosition.index - segmentBackward;
			int num = _pathPosition.index + segmentForward;
			for (; i < 0; i += GetNbPoints())
			{
			}
			while (num >= GetNbPoints())
			{
				num -= GetNbPoints();
			}
			if (_2D)
			{
				UpdatePosition2D(ref _pathPosition, _position3D, i, num, _loopSearch);
			}
			else
			{
				UpdatePosition(ref _pathPosition, _position3D, i, num, _loopSearch);
			}
		}
		else
		{
			ResetPosition(ref _pathPosition, _position3D, _loopSearch);
		}
	}

	public int GetPosAtDist(int startInd, ref float dist, out Vector3 position)
	{
		position = Vector3.zero;
		if (m_iNbPoints <= 0)
		{
			position = Vector3.zero;
			return -1;
		}
		if (m_iNbPoints == 1 || m_fTotalLength == 0f)
		{
			position = m_pVectorPoints[0];
			return 0;
		}
		bool flag = false;
		uint num = ((startInd != -1) ? ((uint)startInd) : 0u);
		float num2 = m_pSegmentLengths[num];
		while (!flag && (m_bFlagLoop || num <= m_iNbPoints - 2))
		{
			num2 = m_pSegmentLengths[num];
			if (dist < num2)
			{
				flag = true;
				continue;
			}
			dist -= num2;
			num++;
			if (num >= m_iNbPoints)
			{
				num -= (uint)m_iNbPoints;
			}
		}
		if (!flag)
		{
			position = m_pVectorPoints[m_iNbPoints - 1];
			return m_iNbPoints - 1;
		}
		if (num < m_iNbPoints)
		{
			float prc = 0f;
			if (m_pSegmentLengths[num] != 0f)
			{
				prc = dist / m_pSegmentLengths[num];
			}
			GetRawPos((int)num, prc, out position);
		}
		return (int)num;
	}

	public Vector3 GetFirstPoint()
	{
		return m_pVectorPoints[0];
	}

	public Vector3 GetLastPoint()
	{
		return m_pVectorPoints[m_iNbPoints - 1];
	}

	public float GetClosestSqrDistToPoint(Vector3 position)
	{
		if (m_iNbPoints > 0)
		{
			float num = float.MaxValue;
			for (int i = 0; (i < m_iNbPoints && m_bFlagLoop) || i < m_iNbPoints - 1; i++)
			{
				Vector3 segmentP = m_pVectorPoints[i];
				int num2 = i + 1;
				if (num2 >= m_iNbPoints)
				{
					num2 = 0;
				}
				Vector3 segmentP2 = m_pVectorPoints[num2];
				float num3 = RcUtils.PointToSegmentSqrDistance(position, segmentP, segmentP2);
				if (num3 < num)
				{
					num = num3;
				}
			}
			return num;
		}
		return 0f;
	}

	public Vector3 MoveOnPath(ref PathPosition _pathPosition, ref float _distance, bool _smooth)
	{
		if (m_iNbPoints == 0)
		{
			return Vector3.zero;
		}
		if (m_iNbPoints == 1)
		{
			return m_pVectorPoints[0];
		}
		int num = 0;
		float dist = _distance;
		if (_pathPosition.index == -1)
		{
			num = 0;
		}
		else
		{
			num = _pathPosition.index;
			dist += m_pSegmentLengths[num] * _pathPosition.ratio;
		}
		Vector3 position;
		int num2 = (_pathPosition.index = GetPosAtDist(num, ref dist, out position));
		_pathPosition.sqrDist = 0f;
		_pathPosition.ratio = RcUtils.PointToSegmentRatio(position, GetSegmentBegin(num2), GetSegmentEnd(num2));
		if (!_smooth)
		{
			return position;
		}
		if (!m_bFlagLoop && (num2 < 1 || num2 >= m_iNbPoints - 3))
		{
			return position;
		}
		Vector3[] array = new Vector3[4]
		{
			m_pVectorPoints[(num2 + m_iNbPoints - 1) % m_iNbPoints],
			m_pVectorPoints[num2],
			m_pVectorPoints[(num2 + 1) % m_iNbPoints],
			m_pVectorPoints[(num2 + 2) % m_iNbPoints]
		};
		return RcUtils.GetSmoothPos(array[0], array[1], array[2], array[3], _pathPosition.ratio, 6f);
	}

	public float GetTotalLength(bool _includeLoopSegment)
	{
		if (_includeLoopSegment)
		{
			return m_fTotalLength + m_pSegmentLengths[m_iNbPoints - 1];
		}
		return m_fTotalLength;
	}
}
