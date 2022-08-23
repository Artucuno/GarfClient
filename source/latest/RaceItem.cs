using UnityEngine;

public abstract class RaceItem : MonoBehaviour
{
	public GameObject Effect;

	public LayerMask Layer;

	private GameObject _effect;

	protected virtual void Awake()
	{
		if (base.networkView != null)
		{
			base.networkView.observed = this;
		}
		_effect = (GameObject)Object.Instantiate(Effect);
		_effect.transform.parent = base.transform;
		_effect.transform.localPosition = Vector3.zero;
	}

	public virtual void DoOnTriggerEnter(GameObject other, int otherlayer)
	{
		if (((int)Layer & (1 << otherlayer)) != 0 && other != null)
		{
			Kart componentInChildren = other.GetComponentInChildren<Kart>();
			if (componentInChildren.GetControlType() == RcVehicle.ControlType.Human)
			{
				DoTrigger(componentInChildren);
			}
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (Network.isServer && base.networkView != null)
		{
			if (other.gameObject.networkView != null)
			{
				NetworkViewID viewID = other.gameObject.networkView.viewID;
				base.networkView.RPC("OnNetworkViewTriggerEnter", RPCMode.All, viewID);
			}
			else
			{
				base.networkView.RPC("OnLayerTriggerEnter", RPCMode.All, other.gameObject.layer);
			}
		}
		else if (Network.peerType == NetworkPeerType.Disconnected || base.networkView == null)
		{
			DoOnTriggerEnter(other.gameObject, other.gameObject.layer);
		}
	}

	[RPC]
	public void OnLayerTriggerEnter(int layer)
	{
		DoOnTriggerEnter(null, layer);
	}

	[RPC]
	public void OnNetworkViewTriggerEnter(NetworkViewID viewId)
	{
		NetworkView networkView = NetworkView.Find(viewId);
		if (networkView != null)
		{
			DoOnTriggerEnter(networkView.gameObject, networkView.gameObject.layer);
		}
	}

	protected virtual void DoTrigger(RcVehicle pVehicle)
	{
		_effect.particleSystem.Play();
		base.collider.enabled = false;
		base.renderer.enabled = false;
	}
}
