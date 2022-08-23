using UnityEngine;

public class CamStateTransMagic : CamStateTransition
{
	[HideInInspector]
	public float TranslationDistance;

	[HideInInspector]
	public Vector3 TranslationDirection = Vector3.zero;

	[HideInInspector]
	public float Step;

	public float TranslationTime = 2f;

	public override ECamState state
	{
		get
		{
			return ECamState.TransMagic;
		}
	}

	protected override bool Merge(float dt)
	{
		if (TranslationDistance > 0f)
		{
			TranslationDistance -= Step * dt;
			if (TranslationDistance < 0f)
			{
				TranslationDistance = 0f;
			}
		}
		else if (TranslationDistance < 0f)
		{
			TranslationDistance += Step * dt;
			if (TranslationDistance > 0f)
			{
				TranslationDistance = 0f;
			}
		}
		base.m_Transform.position = m_ToState.m_Transform.position;
		if (TranslationDistance != 0f)
		{
			base.m_Transform.position += TranslationDistance * TranslationDirection;
		}
		if (TranslationDistance >= 0f)
		{
			base.m_Transform.LookAt(base.m_Target);
		}
		return TranslationDistance == 0f;
	}
}
