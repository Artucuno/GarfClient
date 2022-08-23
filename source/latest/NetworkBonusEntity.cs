using UnityEngine;

public class NetworkBonusEntity : MonoBehaviour
{
	protected BonusEntity m_pBonusEntity;

	public virtual void Awake()
	{
		base.networkView.observed = this;
		m_pBonusEntity = null;
		for (int i = 0; i < base.transform.parent.childCount; i++)
		{
			if (!(m_pBonusEntity == null))
			{
				break;
			}
			m_pBonusEntity = base.transform.parent.GetChild(i).GetComponent<BonusEntity>();
		}
		if (!(m_pBonusEntity == null))
		{
		}
	}

	public virtual void Start()
	{
		base.networkView.observed = this;
	}

	[RPC]
	public void OnLayerTriggerEnter(int layer)
	{
		m_pBonusEntity.DoOnTriggerEnter(null, layer);
	}

	[RPC]
	public void OnNetworkViewTriggerEnter(NetworkViewID viewId)
	{
		NetworkView networkView = NetworkView.Find(viewId);
		if (!(networkView != null))
		{
			return;
		}
		Transform parent = networkView.gameObject.transform.parent;
		if (parent != null)
		{
			BonusEntity componentInChildren = parent.GetComponentInChildren<BonusEntity>();
			if (componentInChildren != null)
			{
				m_pBonusEntity.DoOnTriggerEnter(componentInChildren.gameObject, componentInChildren.gameObject.layer);
				return;
			}
		}
		m_pBonusEntity.DoOnTriggerEnter(networkView.gameObject, networkView.gameObject.layer);
	}

	[RPC]
	public void OnLayerTriggerExit(int layer)
	{
		m_pBonusEntity.DoOnTriggerExit(null, layer);
	}

	[RPC]
	public void OnNetworkViewTriggerExit(NetworkViewID viewId)
	{
		NetworkView networkView = NetworkView.Find(viewId);
		if (networkView != null)
		{
			m_pBonusEntity.DoOnTriggerExit(networkView.gameObject, networkView.gameObject.layer);
		}
	}

	[RPC]
	public virtual void OnNetworkDestroy()
	{
		m_pBonusEntity.DoDestroy();
	}

	[RPC]
	public virtual void NetworkInitialize(NetworkViewID LauncherViewID)
	{
		m_pBonusEntity.DoNetworkInitialize(LauncherViewID);
	}
}
