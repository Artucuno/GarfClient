using UnityEngine;

public class NetworkPieBonusEntity : NetworkMovableBonusEntity
{
	[RPC]
	public void OnStickOnGround(Vector3 normal)
	{
		((PieBonusEntity)m_pBonusEntity).DoStickOnGround(normal);
	}

	[RPC]
	public void Launch(NetworkViewID launcherViewID, bool _Behind)
	{
		NetworkInitialize(launcherViewID);
		((PieBonusEntity)m_pBonusEntity).Launch(_Behind);
	}
}
