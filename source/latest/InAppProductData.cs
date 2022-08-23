public class InAppProductData
{
	private string _productID;

	private string _name;

	private string _price;

	private string _description;

	private bool _isInPromo;

	public string ProductID
	{
		get
		{
			return _productID;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
	}

	public string Price
	{
		get
		{
			return _price;
		}
	}

	public string Description
	{
		get
		{
			return _description;
		}
	}

	public bool IsInPromo
	{
		get
		{
			return _isInPromo;
		}
	}

	public InAppProductData(string pProductID, string pName, string pPrice, string pDescription)
	{
		_productID = pProductID;
		_name = pName;
		_price = pPrice;
		_description = pDescription;
		_isInPromo = _description.StartsWith("#");
	}

	public override string ToString()
	{
		return string.Format("[InAppProductData: ProductID={0}, Name={1}, Price={2}, Description={3}, IsInPromo={4}]", ProductID, Name, Price, Description, IsInPromo);
	}
}
