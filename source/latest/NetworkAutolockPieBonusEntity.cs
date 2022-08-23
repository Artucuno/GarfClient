using UnityEngine;

public class NetworkAutolockPieBonusEntity : NetworkPieBonusEntity
{
	[RPC]
	public void OnSynchronizeOffRail()
	{
		((AutolockPieBonusEntity)m_pBonusEntity).DoSynchronizeOffRail();
	}

	[RPC]
	public new void Launch(NetworkViewID launcherViewID, bool _Behind)
	{
		NetworkInitialize(launcherViewID);
		((AutolockPieBonusEntity)m_pBonusEntity).Launch(m_pBonusEntity.Launcher, _Behind);
	}
}
