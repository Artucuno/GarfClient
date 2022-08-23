using System;

public class DummyInApp : AInAppService, IDisposable
{
	public DummyInApp(string[] pProductIds)
		: base(pProductIds)
	{
		_canMakePurchase = true;
	}

	~DummyInApp()
	{
		if (!_hasBeenDisposed)
		{
			Dispose();
		}
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void GetData()
	{
	}

	public override void PurchaseProduct(string pProductId)
	{
		OnPurchaseSucceed(pProductId);
	}
}
