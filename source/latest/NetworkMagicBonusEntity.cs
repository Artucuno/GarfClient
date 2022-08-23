using UnityEngine;

public class NetworkMagicBonusEntity : NetworkMovableBonusEntity
{
	[RPC]
	public new void OnNetworkDestroy()
	{
		((MagicBonusEntity)m_pBonusEntity).DoDestroyByWall();
	}

	[RPC]
	public override void Launch(NetworkViewID launcherViewID)
	{
		NetworkInitialize(launcherViewID);
		((MagicBonusEntity)m_pBonusEntity).Launch();
	}
}
