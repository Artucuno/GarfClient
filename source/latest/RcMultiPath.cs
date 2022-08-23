using UnityEngine;

public class RcMultiPath : MonoBehaviour
{
	private const float RESET_MULTIPATH_SQR_DIST = 2500f;

	protected RcMultiPathSection[] m_pSections;

	protected int m_iNbSection;

	protected float m_fTotalLength;

	public GameObject m_pFirstSection;

	public bool m_bDebugDraw;

	protected bool m_bValuePaths;

	public bool BValuePaths
	{
		get
		{
			return m_bValuePaths;
		}
	}

	public int NbSections
	{
		get
		{
			return m_iNbSection;
		}
	}

	public float TotalLength
	{
		get
		{
			return m_fTotalLength;
		}
	}

	public RcMultiPath()
	{
		m_iNbSection = 0;
		m_fTotalLength = 0f;
		m_pFirstSection = null;
		m_bDebugDraw = false;
		m_bValuePaths = true;
	}

	public RcMultiPathSection GetMultiPathSection(int _Index)
	{
		return m_pSections[_Index];
	}

	public void Start()
	{
		Rebuild();
	}

	public void Rebuild()
	{
		m_iNbSection = 0;
		m_fTotalLength = 0f;
		RcMultiPathSection[] componentsInChildren = GetComponentsInChildren<RcMultiPathSection>();
		m_pSections = new RcMultiPathSection[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].CreateFastPath(m_bValuePaths);
		}
		RcMultiPathSection component = m_pFirstSection.GetComponent<RcMultiPathSection>();
		if ((bool)component)
		{
			AddSection(component);
			for (int j = 0; j < component.GetNbBranchesBefore() && (bool)component.GetBeforeSection(j); j++)
			{
				Vector3 firstPoint = component.GetSimplePath().GetFirstPoint();
				Vector3 lastPoint = component.GetBeforeSection(j).GetSimplePath().GetLastPoint();
				float magnitude = (firstPoint - lastPoint).magnitude;
				component.GetBeforeSection(j).SetDistToEndLine(magnitude, component.GetBeforeSection(j));
			}
			m_fTotalLength = component.GetDistToEndLine() + component.GetLength();
		}
		NormalizeSectionLengthes();
	}

	public void Stop()
	{
		RcMultiPathSection[] componentsInChildren = GetComponentsInChildren<RcMultiPathSection>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DeleteFastPath();
		}
		for (int j = 0; j < m_pSections.Length; j++)
		{
			m_pSections[j] = null;
		}
		m_iNbSection = 0;
	}

	public float GetDistToEndLine(MultiPathPosition _mpPosition)
	{
		if (m_iNbSection > 0 && _mpPosition.section == m_pSections[0] && _mpPosition.pathPosition.index == 0 && _mpPosition.pathPosition.ratio < 0f)
		{
			RcFastPath simplePath = _mpPosition.section.GetSimplePath();
			return -1f * simplePath.GetSegmentLength(_mpPosition.pathPosition.index) * _mpPosition.pathPosition.ratio;
		}
		if (_mpPosition.section != null)
		{
			return _mpPosition.section.GetDistToEndLine(_mpPosition.pathPosition);
		}
		return 0f;
	}

	public void AddSection(RcMultiPathSection pSection)
	{
		m_pSections[m_iNbSection] = pSection;
		m_iNbSection++;
		AttachExtremities(pSection);
	}

	public void AttachExtremities(RcMultiPathSection pSection)
	{
		AttachExtremity(pSection);
		for (int i = 0; i < pSection.GetNbBranchesAfter() && (bool)pSection.GetAfterSection(i); i++)
		{
			AttachExtremity(pSection.GetAfterSection(i));
		}
	}

	public void AttachExtremity(RcMultiPathSection pSection)
	{
		if (pSection == null)
		{
			return;
		}
		RcMultiPathSection rcMultiPathSection = null;
		for (int i = 0; i < m_iNbSection; i++)
		{
			if (m_pSections[i] == pSection)
			{
				rcMultiPathSection = m_pSections[i];
				break;
			}
		}
		if (!rcMultiPathSection)
		{
			AddSection(pSection);
		}
	}

	public void ResetPosition(ref MultiPathPosition _mpPosition, Vector3 _position3D)
	{
		PathPosition uNDEFINED_POSITION = PathPosition.UNDEFINED_POSITION;
		float num = 1E+38f;
		for (int i = 0; i < m_iNbSection; i++)
		{
			RcMultiPathSection rcMultiPathSection = m_pSections[i];
			uNDEFINED_POSITION = PathPosition.UNDEFINED_POSITION;
			rcMultiPathSection.GetSimplePath().UpdatePathPosition(ref uNDEFINED_POSITION, _position3D, 3, 1, true, true);
			if (uNDEFINED_POSITION.sqrDist < num || _mpPosition.section == null)
			{
				_mpPosition.section = rcMultiPathSection;
				_mpPosition.pathPosition = uNDEFINED_POSITION;
				num = uNDEFINED_POSITION.sqrDist;
			}
		}
	}

	public void UpdateMPPosition(ref MultiPathPosition _mpPosition, Vector3 _position3D, int segmentForward, int segmentBackward, bool _2D)
	{
		if (_mpPosition.pathPosition.index != -1 && _mpPosition.pathPosition.sqrDist < 2500f)
		{
			_mpPosition.section.UpdateMPPosition(ref _mpPosition, _position3D, segmentForward, segmentBackward, _2D);
		}
		else
		{
			ResetPosition(ref _mpPosition, _position3D);
		}
	}

	public void RefreshRespawn(RcVehicleRaceStats _Stats)
	{
		UpdateMPPosition(ref _Stats.m_GuidePosition, _Stats.GetVehicle().GetPosition(), 2, 1, false);
		RcFastPath simplePath = _Stats.GetGuidePosition().section.GetSimplePath();
		int index = _Stats.GetGuidePosition().pathPosition.index;
		float prc = Mathf.Clamp(_Stats.GetGuidePosition().pathPosition.ratio, 0f, 1f);
		bool flag = true;
		if (m_bValuePaths)
		{
			RcFastValuePath rcFastValuePath = (RcFastValuePath)simplePath;
			flag = _Stats.GetVehicle().GetGroundSurface() != -1 && rcFastValuePath.GetPointValue(index) != -1f;
		}
		if (flag && _Stats.GetVehicle().IsOnGround())
		{
			Vector3 outPos;
			simplePath.GetRawPos(index, prc, out outPos);
			outPos += Vector3.up;
			Vector3 segment = simplePath.GetSegment(index);
			segment -= Vector3.Dot(segment, Vector3.up) * Vector3.up;
			segment.Normalize();
			Quaternion respawnOrientation = default(Quaternion);
			respawnOrientation.SetLookRotation(segment, Vector3.up);
			_Stats.GetVehicle().SetRespawnPos(outPos);
			_Stats.GetVehicle().SetRespawnOrientation(respawnOrientation);
		}
		float num = Mathf.Max(m_fTotalLength * 0.25f, 10f);
		float num2 = GetDistToEndLine(_Stats.GetGuidePosition());
		if (_Stats.GetNbCheckPointValidated() == 0 && num2 < num)
		{
			num2 = m_fTotalLength;
		}
		if (_Stats.GetNbCheckPointValidated() > 0 && num2 > m_fTotalLength - num)
		{
			num2 = 0f;
		}
		_Stats.SetDistToEndOfLap(num2);
		float num3 = num2;
		num3 += (float)(_Stats.GetRaceNbLap() - _Stats.GetLogicNbLap()) * m_fTotalLength;
		_Stats.SetDistToEndOfRace(num3);
	}

	protected void NormalizeSectionLengthes()
	{
		RcMultiPathSection[] pSections = m_pSections;
		foreach (RcMultiPathSection rcMultiPathSection in pSections)
		{
			if (rcMultiPathSection != null && rcMultiPathSection != m_pFirstSection)
			{
				float length = rcMultiPathSection.GetLength();
				float distToEndLine = rcMultiPathSection.GetDistToEndLine();
				float distToEndLine2 = rcMultiPathSection.m_pBeforeBranches[0].GetDistToEndLine();
				float num = distToEndLine2 - distToEndLine;
				if (length > 0f && num > length)
				{
					rcMultiPathSection.SetMultipathScale(num / length);
				}
			}
		}
	}
}
