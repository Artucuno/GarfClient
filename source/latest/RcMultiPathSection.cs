using System;
using UnityEngine;

public class RcMultiPathSection : MonoBehaviour
{
	private const int MAX_RECURSIVE_CALL = 8;

	public RcMultiPathSection[] m_pBeforeBranches;

	public RcMultiPathSection[] m_pAfterBranches;

	protected RcFastPath m_pSimplePath;

	protected float m_fShortestDistToEndLine;

	protected float m_fSegmentAverageLength;

	[NonSerialized]
	protected float m_fMultipathScale = 1f;

	public RcFastPath GetSimplePath()
	{
		return m_pSimplePath;
	}

	public void SetDistToEndLine(float _dist, RcMultiPathSection _firstSection)
	{
		if (_dist != m_fShortestDistToEndLine)
		{
			m_fShortestDistToEndLine = _dist;
			UpdateDistToEndLine(_firstSection);
		}
	}

	public float GetDistToEndLine()
	{
		return m_fShortestDistToEndLine;
	}

	public float GetRescaledLength()
	{
		return m_pSimplePath.GetTotalLength(false) * m_fMultipathScale;
	}

	public float GetLength()
	{
		return m_pSimplePath.GetTotalLength(false);
	}

	public float GetMultipathScale()
	{
		return m_fMultipathScale;
	}

	public void SetMultipathScale(float scale)
	{
		m_fMultipathScale = scale;
	}

	public RcMultiPathSection GetAfterSection(int _branch)
	{
		return m_pAfterBranches[_branch];
	}

	public RcMultiPathSection GetBeforeSection(int _branch)
	{
		return m_pBeforeBranches[_branch];
	}

	public int GetNbBranchesBefore()
	{
		if (m_pBeforeBranches != null)
		{
			return m_pBeforeBranches.Length;
		}
		return 0;
	}

	public int GetNbBranchesAfter()
	{
		if (m_pAfterBranches != null)
		{
			return m_pAfterBranches.Length;
		}
		return 0;
	}

	public void CreateFastPath(bool _Value)
	{
		if (!_Value)
		{
			m_pSimplePath = new RcFastPath(base.gameObject);
		}
		else
		{
			m_pSimplePath = new RcFastValuePath(base.gameObject);
		}
		m_fShortestDistToEndLine = 0f;
		m_fSegmentAverageLength = m_pSimplePath.GetTotalLength(false) / (float)m_pSimplePath.GetNbPoints();
	}

	public void DeleteFastPath()
	{
		if (m_pSimplePath != null)
		{
			m_pSimplePath = null;
		}
	}

	public void UpdateDistToEndLine(RcMultiPathSection _firstSection)
	{
		for (int i = 0; i < m_pBeforeBranches.Length; i++)
		{
			RcMultiPathSection rcMultiPathSection = m_pBeforeBranches[i];
			if (rcMultiPathSection != null && rcMultiPathSection != _firstSection)
			{
				Vector3 firstPoint = GetSimplePath().GetFirstPoint();
				Vector3 lastPoint = rcMultiPathSection.GetSimplePath().GetLastPoint();
				float magnitude = (firstPoint - lastPoint).magnitude;
				float num = m_fShortestDistToEndLine + magnitude + GetSimplePath().GetTotalLength(false);
				if (rcMultiPathSection.GetDistToEndLine() < num)
				{
					rcMultiPathSection.SetDistToEndLine(num, _firstSection);
				}
			}
		}
	}

	public void UpdateMPPosition(ref MultiPathPosition _mpPosition, Vector3 position, int _segmentForward, int _segmentBackward, bool _2D)
	{
		RcMultiPathSection[] _pSkipTable = new RcMultiPathSection[8];
		for (int i = 0; i < 8; i++)
		{
			_pSkipTable[i] = null;
		}
		int _skipTableSize = 0;
		UpdateMPPosition(ref _mpPosition, position, _segmentForward, _segmentBackward, _2D, ref _pSkipTable, ref _skipTableSize, false);
	}

	public void UpdateMPPosition(ref MultiPathPosition _mpPosition, Vector3 position, int _segmentForward, int _segmentBackward, bool _2D, ref RcMultiPathSection[] _pSkipTable, ref int _skipTableSize, bool _loopSection)
	{
		if (!_loopSection)
		{
			for (int i = 0; i < _skipTableSize; i++)
			{
				if (_pSkipTable[i] == this)
				{
					return;
				}
			}
		}
		if (m_pSimplePath == null)
		{
			return;
		}
		int num = _segmentForward;
		int num2 = _segmentBackward;
		int index = _mpPosition.pathPosition.index;
		bool flag = false;
		bool flag2 = false;
		if (num2 > index)
		{
			num2 = index;
			flag = true;
		}
		if (index + num >= m_pSimplePath.GetNbPoints())
		{
			num = m_pSimplePath.GetNbPoints() - 1 - index;
			flag2 = true;
		}
		_mpPosition.pathPosition.sqrDist = 1E+38f;
		m_pSimplePath.UpdatePathPosition(ref _mpPosition.pathPosition, position, num, num2, _2D, false);
		_pSkipTable[_skipTableSize] = this;
		_skipTableSize++;
		if (flag2 && _skipTableSize < 8)
		{
			int num3 = m_pSimplePath.GetNbPoints() - index;
			int segmentBackward = _segmentBackward + num3;
			int num4 = _segmentForward - num3;
			MultiPathPosition _mpPosition2 = MultiPathPosition.UNDEFINED_MP_POS;
			for (int j = 0; j < m_pAfterBranches.Length; j++)
			{
				RcMultiPathSection rcMultiPathSection = m_pAfterBranches[j];
				if ((bool)rcMultiPathSection)
				{
					bool flag3 = rcMultiPathSection == this;
					_mpPosition2.section = rcMultiPathSection;
					_mpPosition2.pathPosition.index = 0;
					_mpPosition2.pathPosition.sqrDist = 1E+38f;
					rcMultiPathSection.UpdateMPPosition(ref _mpPosition2, position, (!flag3) ? num4 : 0, segmentBackward, _2D, ref _pSkipTable, ref _skipTableSize, flag3);
					if (_mpPosition2.pathPosition.sqrDist <= _mpPosition.pathPosition.sqrDist)
					{
						_mpPosition = _mpPosition2;
					}
				}
			}
		}
		if (!flag || _skipTableSize >= 8)
		{
			return;
		}
		int num5 = _segmentBackward - index;
		int segmentForward = _segmentForward + index;
		MultiPathPosition _mpPosition3 = MultiPathPosition.UNDEFINED_MP_POS;
		for (int k = 0; k < m_pBeforeBranches.Length; k++)
		{
			RcMultiPathSection rcMultiPathSection2 = m_pBeforeBranches[k];
			if ((bool)rcMultiPathSection2)
			{
				bool flag4 = rcMultiPathSection2 == this;
				_mpPosition3.section = rcMultiPathSection2;
				_mpPosition3.pathPosition.index = rcMultiPathSection2.GetSimplePath().GetNbPoints() - 2;
				_mpPosition3.pathPosition.sqrDist = 1E+38f;
				rcMultiPathSection2.UpdateMPPosition(ref _mpPosition3, position, segmentForward, (!flag4) ? num5 : 0, _2D, ref _pSkipTable, ref _skipTableSize, flag4);
				if (_mpPosition3.pathPosition.sqrDist <= _mpPosition.pathPosition.sqrDist)
				{
					_mpPosition = _mpPosition3;
				}
			}
		}
	}

	public float GetDistToEndLine(PathPosition _pos)
	{
		return GetSimplePath().GetDistToEndOfPath(_pos.index, _pos.ratio) * m_fMultipathScale + m_fShortestDistToEndLine;
	}

	public PathPosition GetPosAtDistToEndLine(float distToEnd)
	{
		float ratio;
		int segmentForEndOfPathDist = GetSimplePath().GetSegmentForEndOfPathDist((distToEnd - m_fShortestDistToEndLine) / m_fMultipathScale, out ratio);
		return new PathPosition(segmentForEndOfPathDist, ratio, 0f);
	}
}
