using UnityEngine;

public class NetworkDiamondBonusEntity : NetworkMovableBonusEntity
{
	[RPC]
	public void OnLoseTarget()
	{
		((DiamondBonusEntity)m_pBonusEntity).DoLoseTarget();
	}

	[RPC]
	public void OnCollideWall()
	{
		((DiamondBonusEntity)m_pBonusEntity).DoCollideWall();
	}

	[RPC]
	public void OnCollideRoad(Vector3 hit)
	{
		((DiamondBonusEntity)m_pBonusEntity).DoCollideRoad(hit);
	}

	[RPC]
	public void OnAttachToTarget()
	{
		((DiamondBonusEntity)m_pBonusEntity).DoAttachToTarget();
	}

	[RPC]
	public void OnAcquireTarget(NetworkViewID id)
	{
		NetworkView networkView = NetworkView.Find(id);
		if (networkView != null)
		{
			((DiamondBonusEntity)m_pBonusEntity).DoAcquireTarget(networkView.gameObject);
		}
	}

	[RPC]
	public void Explode()
	{
		((DiamondBonusEntity)m_pBonusEntity).Explode();
	}

	[RPC]
	public override void OnNetworkDestroy()
	{
		((DiamondBonusEntity)m_pBonusEntity).Explode();
	}

	[RPC]
	public void Launch(NetworkViewID launcherViewID, bool pBehind)
	{
		NetworkInitialize(launcherViewID);
		((DiamondBonusEntity)m_pBonusEntity).Launch(pBehind);
	}
}
