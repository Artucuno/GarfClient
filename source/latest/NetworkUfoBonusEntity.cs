using UnityEngine;

public class NetworkUfoBonusEntity : NetworkMovableBonusEntity
{
	[RPC]
	public void OnSynchronizeDestroy()
	{
		m_pBonusEntity.DoDestroy();
	}

	[RPC]
	public void OnChooseGoodRay(int RandomGood)
	{
		((UFOBonusEntity)m_pBonusEntity).DoChooseGoodRay(RandomGood);
	}

	[RPC]
	public void OnLeaveUfo()
	{
		((UFOBonusEntity)m_pBonusEntity).DoLeaveUfo();
	}

	[RPC]
	public void OnSynchronizePosition(float fPos)
	{
		((UFOBonusEntity)m_pBonusEntity).DoSynchronizePosition(fPos);
	}

	[RPC]
	public void OnRemoveUFO(bool bRemove)
	{
		((UFOBonusEntity)m_pBonusEntity).DoRemoveUFO(bRemove);
	}

	[RPC]
	public void OnSynchronizeUfoLaunch(float Dist, Vector3 LeftPos)
	{
		((UFOBonusEntity)m_pBonusEntity).DoSynchronizeUfoLaunch(Dist, LeftPos);
	}
}
