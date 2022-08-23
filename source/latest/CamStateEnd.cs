using UnityEngine;

public class CamStateEnd : CamState
{
	public Vector3 Offset;

	public float Speed;

	public override ECamState state
	{
		get
		{
			return ECamState.End;
		}
	}

	public override void Enter(Transform _Transform, Transform _Target)
	{
		base.Enter(_Transform, _Target);
		if (base.m_Target != null)
		{
			base.m_Transform.position = base.m_Target.position + Offset;
		}
	}

	public override ECamState Manage(float dt)
	{
		if (base.m_Target == null)
		{
			GameMode gameMode = Singleton<GameManager>.Instance.GameMode;
			if (gameMode != null)
			{
				GameObject player = gameMode.GetPlayer(0);
				if (player != null)
				{
					base.m_Target = player.transform.Find("cible_camera").transform;
				}
			}
			if (base.m_Target == null)
			{
				return state;
			}
			base.m_Transform.position = base.m_Target.position + Offset;
		}
		base.m_Transform.RotateAround(base.m_Target.localPosition, Vector3.up, Speed * dt);
		Vector3 forward = -base.m_Transform.forward;
		forward.y = 0f;
		base.m_Transform.position = base.m_Target.position + Quaternion.LookRotation(forward) * Offset;
		base.m_Transform.LookAt(base.m_Target);
		return state;
	}

	public override void Exit()
	{
	}
}
