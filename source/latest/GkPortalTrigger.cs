using UnityEngine;

public class GkPortalTrigger : RcPortalTrigger
{
	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		if (base.enabled && m_eTriggerSide != 0)
		{
			TimeTrialUFO componentInChildren = other.gameObject.GetComponentInChildren<TimeTrialUFO>();
			if (!(componentInChildren == null) && m_eActionType == PortalAction.StartLine)
			{
				componentInChildren.CrossStartLine();
			}
		}
	}
}
