using System;
using System.Collections.Generic;
using UnityEngine;

public class InAppManager : Singleton<InAppManager>
{
	private AInAppService _inAppService;

	private bool pendingRequest;

	private string[] _productIds;

	public Action<List<InAppProductData>> OnProductDataReceived;

	public Action OnGotFocus;

	public Action OnLostFocus;

	public AInAppService.PurchaseDelegate OnPurchaseSucceed;

	public AInAppService.PurchaseDelegate OnPurchaseFailed;

	public AInAppService.PurchaseDelegate OnPurchaseCancelled;

	public bool InAppAvaible
	{
		get
		{
			return _inAppService.CanMakePurchase && Application.internetReachability != NetworkReachability.NotReachable;
		}
	}

	~InAppManager()
	{
		AInAppService inAppService = _inAppService;
		inAppService.OnProductListReceived = (Action<List<InAppProductData>>)Delegate.Remove(inAppService.OnProductListReceived, new Action<List<InAppProductData>>(ProductListReceived));
		AInAppService inAppService2 = _inAppService;
		inAppService2.OnPurchaseSucceed = (AInAppService.PurchaseDelegate)Delegate.Remove(inAppService2.OnPurchaseSucceed, new AInAppService.PurchaseDelegate(PurchaseSucceed));
		AInAppService inAppService3 = _inAppService;
		inAppService3.OnPurchaseFailed = (AInAppService.PurchaseDelegate)Delegate.Remove(inAppService3.OnPurchaseFailed, new AInAppService.PurchaseDelegate(PurchaseFailed));
		AInAppService inAppService4 = _inAppService;
		inAppService4.OnPurchaseCancelled = (AInAppService.PurchaseDelegate)Delegate.Remove(inAppService4.OnPurchaseCancelled, new AInAppService.PurchaseDelegate(PurchaseCancelled));
		AInAppService inAppService5 = _inAppService;
		inAppService5.OnRestoreSucceed = (AInAppService.RestoreSucceedDelegate)Delegate.Remove(inAppService5.OnRestoreSucceed, new AInAppService.RestoreSucceedDelegate(RestoreSucceed));
		AInAppService inAppService6 = _inAppService;
		inAppService6.OnRestoreFailed = (AInAppService.RestoreFailedDelegate)Delegate.Remove(inAppService6.OnRestoreFailed, new AInAppService.RestoreFailedDelegate(RestoreFailed));
	}

	public void CollectStoreInfo(string[] productIds)
	{
		if (!pendingRequest)
		{
			pendingRequest = true;
			if (_inAppService != null)
			{
				_inAppService.Dispose();
			}
			_inAppService = null;
			_productIds = productIds;
			_inAppService = new DummyInApp(_productIds);
			AInAppService inAppService = _inAppService;
			inAppService.OnProductListReceived = (Action<List<InAppProductData>>)Delegate.Combine(inAppService.OnProductListReceived, new Action<List<InAppProductData>>(ProductListReceived));
			AInAppService inAppService2 = _inAppService;
			inAppService2.OnPurchaseSucceed = (AInAppService.PurchaseDelegate)Delegate.Combine(inAppService2.OnPurchaseSucceed, new AInAppService.PurchaseDelegate(PurchaseSucceed));
			AInAppService inAppService3 = _inAppService;
			inAppService3.OnPurchaseFailed = (AInAppService.PurchaseDelegate)Delegate.Combine(inAppService3.OnPurchaseFailed, new AInAppService.PurchaseDelegate(PurchaseFailed));
			AInAppService inAppService4 = _inAppService;
			inAppService4.OnPurchaseCancelled = (AInAppService.PurchaseDelegate)Delegate.Combine(inAppService4.OnPurchaseCancelled, new AInAppService.PurchaseDelegate(PurchaseCancelled));
			AInAppService inAppService5 = _inAppService;
			inAppService5.OnRestoreSucceed = (AInAppService.RestoreSucceedDelegate)Delegate.Combine(inAppService5.OnRestoreSucceed, new AInAppService.RestoreSucceedDelegate(RestoreSucceed));
			AInAppService inAppService6 = _inAppService;
			inAppService6.OnRestoreFailed = (AInAppService.RestoreFailedDelegate)Delegate.Combine(inAppService6.OnRestoreFailed, new AInAppService.RestoreFailedDelegate(RestoreFailed));
			GetData();
		}
	}

	public void GetData()
	{
		_inAppService.GetData();
	}

	public void ProductListReceived(List<InAppProductData> pProductData)
	{
		pendingRequest = false;
		if (pProductData.Count != 0 && OnProductDataReceived != null)
		{
			OnProductDataReceived(pProductData);
		}
	}

	public void PurchaseProduct(string pProductId)
	{
		_inAppService.PurchaseProduct(pProductId);
		if (OnGotFocus != null)
		{
			OnGotFocus();
		}
	}

	private void PurchaseSucceed(string pProductId)
	{
		if (OnPurchaseSucceed != null)
		{
			OnPurchaseSucceed(pProductId);
		}
		if (OnLostFocus != null)
		{
			OnLostFocus();
		}
	}

	private void PurchaseFailed(string pError)
	{
		if (OnPurchaseFailed != null)
		{
			OnPurchaseFailed(pError);
		}
		if (OnLostFocus != null)
		{
			OnLostFocus();
		}
	}

	private void PurchaseCancelled(string pError)
	{
		if (OnPurchaseCancelled != null)
		{
			OnPurchaseCancelled(pError);
		}
		if (OnLostFocus != null)
		{
			OnLostFocus();
		}
	}

	private void RestoreSucceed()
	{
	}

	private void RestoreFailed(string pError)
	{
	}
}
