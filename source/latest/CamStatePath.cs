using UnityEngine;

public class CamStatePath : CamState
{
	private RcMultiPath m_Path;

	private MultiPathPosition m_PathPosition;

	private MultiPathPosition m_NextPathPosition;

	public float LookDistance = 5f;

	public float Speed = 10f;

	public float TargetHeight = 2f;

	public Vector3 Offset = new Vector3(0f, 0.5f, -3f);

	public float TargetDamping = 1f;

	public float PosDamping = 1f;

	public override ECamState state
	{
		get
		{
			return ECamState.Path;
		}
	}

	public void Setup(RcMultiPath _pathToFollow)
	{
		m_Path = _pathToFollow;
	}

	public override void Enter(Transform _Transform, Transform _Target)
	{
		base.Enter(_Transform, _Target);
		GameObject gameObject = GameObject.Find("Start");
		base.m_Target = new GameObject().transform;
		Transform obj = base.m_Target.transform;
		Vector3 position = gameObject.transform.position;
		base.m_Transform.position = position;
		obj.position = position;
		Transform obj2 = base.m_Target.transform;
		Quaternion rotation = gameObject.transform.rotation;
		base.m_Transform.rotation = rotation;
		obj2.rotation = rotation;
		m_PathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_NextPathPosition = MultiPathPosition.UNDEFINED_MP_POS;
		m_Path.UpdateMPPosition(ref m_PathPosition, base.m_Target.position, 0, 0, false);
	}

	public override ECamState Manage(float dt)
	{
		m_Path.UpdateMPPosition(ref m_PathPosition, base.m_Target.position, 3, 1, false);
		m_NextPathPosition = m_PathPosition;
		Vector3 zero = Vector3.zero;
		RcMultiPathSection rcMultiPathSection = m_PathPosition.section;
		zero = rcMultiPathSection.GetSimplePath().MoveOnPath(ref m_NextPathPosition.pathPosition, ref LookDistance, false);
		if (m_NextPathPosition.pathPosition.index == m_PathPosition.section.GetSimplePath().GetNbPoints() - 1)
		{
			float num = 1E+38f;
			RcMultiPathSection[] pAfterBranches = m_PathPosition.section.m_pAfterBranches;
			foreach (RcMultiPathSection rcMultiPathSection2 in pAfterBranches)
			{
				if ((bool)rcMultiPathSection2 && rcMultiPathSection2.GetDistToEndLine() < num)
				{
					rcMultiPathSection = rcMultiPathSection2;
					num = rcMultiPathSection2.GetDistToEndLine();
				}
			}
			if (rcMultiPathSection != null)
			{
				m_NextPathPosition.pathPosition = PathPosition.UNDEFINED_POSITION;
			}
			zero = rcMultiPathSection.GetSimplePath().MoveOnPath(ref m_NextPathPosition.pathPosition, ref LookDistance, false);
		}
		zero += Vector3.up * TargetHeight;
		Vector3 normalized = (zero - base.m_Target.position).normalized;
		Quaternion quaternion = default(Quaternion);
		if (normalized != Vector3.zero)
		{
			base.m_Target.position += dt * (normalized * Speed);
			quaternion = Quaternion.Lerp(base.m_Target.rotation, Quaternion.LookRotation(normalized), dt * TargetDamping);
			base.m_Target.rotation = quaternion;
		}
		base.m_Transform.position = Vector3.Lerp(base.m_Transform.position, base.m_Target.position + quaternion * Offset, dt * PosDamping);
		base.m_Transform.LookAt(base.m_Target);
		return state;
	}

	public override void Exit()
	{
		Object.DestroyImmediate(base.m_Target.gameObject);
		base.m_Target = null;
	}
}
