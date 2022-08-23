using UnityEngine;

public class MagicBonusEffect : BonusEffect
{
	public LayerMask m_oLayerMask;

	[HideInInspector]
	public Kart FirstKart;

	[HideInInspector]
	public Kart SecondKart;

	public override void Start()
	{
		m_bStoppedByAnim = false;
		base.Start();
	}

	public override bool Activate()
	{
		if (FirstKart.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_LEVITATE)
			.Activated || SecondKart.GetBonusMgr().GetBonusEffectMgr().GetBonusEffect(EBonusEffect.BONUSEFFECT_LEVITATE)
			.Activated)
		{
			return true;
		}
		base.Activate();
		Transform transform = FirstKart.transform;
		Transform transform2 = SecondKart.transform;
		Vector3 position = transform.position;
		Vector3 position2 = transform2.position;
		Vector3 direction = position2 - position;
		RaycastHit[] array = Physics.RaycastAll(position, direction, direction.magnitude, m_oLayerMask);
		RaycastHit[] array2 = array;
		foreach (RaycastHit raycastHit in array2)
		{
			RcPortalTrigger component = raycastHit.collider.GetComponent<RcPortalTrigger>();
			if (component != null)
			{
				component.OnTriggerEnter(transform.parent.collider);
				component.OnTriggerEnter(transform2.parent.collider);
			}
		}
		Quaternion rotation = transform.rotation;
		Quaternion rotation2 = transform2.rotation;
		FirstKart.KartSound.PlaySound(14);
		SecondKart.KartSound.PlaySound(14);
		FirstKart.Teleport(position2, rotation2, SecondKart.GetVehiclePhysic().GetLinearVelocity());
		SecondKart.Teleport(position, rotation, FirstKart.GetVehiclePhysic().GetLinearVelocity());
		FirstKart.KartSound.PlayVoice(KartSound.EVoices.Good);
		SecondKart.KartSound.PlayVoice(KartSound.EVoices.Bad);
		CameraBase component2 = Camera.mainCamera.GetComponent<CameraBase>();
		CamStateTransMagic component3 = Camera.mainCamera.GetComponent<CamStateTransMagic>();
		if (component2.CurrentState == ECamState.Follow && (FirstKart.GetControlType() == RcVehicle.ControlType.Human || SecondKart.GetControlType() == RcVehicle.ControlType.Human))
		{
			float num = (component3.TranslationDistance = (position - position2).magnitude);
			Vector3 translationDirection = position - position2;
			translationDirection.Normalize();
			component3.TranslationDirection = translationDirection;
			component3.Step = num / component3.TranslationTime;
			component2.SwitchCamera(ECamState.Follow, ECamState.TransMagic);
		}
		if (FirstKart.OnBeSwaped != null)
		{
			FirstKart.OnBeSwaped(SecondKart);
		}
		if (SecondKart.OnBeSwaped != null)
		{
			SecondKart.OnBeSwaped(FirstKart);
		}
		FirstKart.RaceStats.ForceRefreshRespawn();
		SecondKart.RaceStats.ForceRefreshRespawn();
		Deactivate();
		return true;
	}
}
