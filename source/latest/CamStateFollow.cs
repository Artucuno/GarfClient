using UnityEngine;

public class CamStateFollow : CamState
{
	public float Distance = 10f;

	public float Height = 5f;

	public float HeightDamping = 2f;

	public float RotationDamping = 3f;

	public float DistanceDamping = 10f;

	public float BoostDamping = 3f;

	public float SlowDamping = 30f;

	public float DampDamping = 10f;

	protected float CurrDamping;

	public override ECamState state
	{
		get
		{
			return ECamState.Follow;
		}
	}

	public bool bBoost { get; set; }

	public bool bSlow { get; set; }

	public override void Enter(Transform _Transform, Transform _Target)
	{
		base.Enter(_Transform, _Target);
		CurrDamping = DistanceDamping;
	}

	public override ECamState Manage(float dt)
	{
		if (base.m_Target == null)
		{
			GameMode gameMode = Singleton<GameManager>.Instance.GameMode;
			if (gameMode != null)
			{
				GameObject humanPlayer = gameMode.GetHumanPlayer();
				if (humanPlayer != null)
				{
					base.m_Target = humanPlayer.transform.Find("cible_camera").transform;
				}
			}
			if (base.m_Target == null)
			{
				return state;
			}
		}
		float y = base.m_Target.eulerAngles.y;
		float num = base.m_Target.position.y + Height;
		float y2 = base.m_Transform.eulerAngles.y;
		float y3 = base.m_Transform.position.y;
		Vector3 vector = base.m_Target.position - base.m_Transform.position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		y2 = ((!m_bDamping) ? y : Mathf.LerpAngle(y2, y, RotationDamping * dt));
		y3 = ((!m_bDamping) ? num : Mathf.Lerp(y3, num, HeightDamping * dt));
		Quaternion quaternion = Quaternion.Euler(0f, y2, 0f);
		base.m_Transform.position = base.m_Target.position;
		float to = DistanceDamping;
		float distance = Distance;
		if (bBoost != bSlow)
		{
			if (bBoost)
			{
				to = BoostDamping;
			}
			else if (bSlow)
			{
				to = SlowDamping;
			}
		}
		CurrDamping = Mathf.Lerp(CurrDamping, to, DampDamping * dt);
		distance = ((!m_bDamping) ? distance : Mathf.Lerp(magnitude, distance, CurrDamping * dt));
		base.m_Transform.position -= quaternion * Vector3.forward * distance;
		base.m_Transform.position = new Vector3(base.m_Transform.position.x, y3, base.m_Transform.position.z);
		base.m_Transform.LookAt(base.m_Target);
		return state;
	}
}
