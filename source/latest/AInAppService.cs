using System;
using System.Collections.Generic;

public abstract class AInAppService
{
	public delegate void PurchaseDelegate(string pProductId);

	public delegate void RestoreSucceedDelegate();

	public delegate void RestoreFailedDelegate(string pError);

	protected bool _hasBeenDisposed;

	protected bool _isServiceAvailable;

	protected bool _canMakePurchase;

	protected string[] _productIds;

	public Action<List<InAppProductData>> OnProductListReceived;

	public PurchaseDelegate OnPurchaseSucceed;

	public PurchaseDelegate OnPurchaseFailed;

	public PurchaseDelegate OnPurchaseCancelled;

	public RestoreSucceedDelegate OnRestoreSucceed;

	public RestoreFailedDelegate OnRestoreFailed;

	public bool CanMakePurchase
	{
		get
		{
			return _canMakePurchase;
		}
	}

	public AInAppService(string[] pProductIds)
	{
		_hasBeenDisposed = false;
		_isServiceAvailable = false;
		_canMakePurchase = false;
		_productIds = pProductIds;
	}

	public abstract void PurchaseProduct(string pProductId);

	public abstract void GetData();

	public virtual void Dispose()
	{
		_hasBeenDisposed = true;
	}
}
