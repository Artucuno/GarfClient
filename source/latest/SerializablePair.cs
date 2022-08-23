using System.Xml.Serialization;

public class SerializablePair<T, U>
{
	private T _key;

	private U _value;

	[XmlElement("A")]
	public T Key
	{
		get
		{
			return _key;
		}
		set
		{
			_key = value;
		}
	}

	[XmlElement("B")]
	public U Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public SerializablePair()
	{
		_key = default(T);
		_value = default(U);
	}

	public SerializablePair(T pFirst, U pSecond)
	{
		_key = pFirst;
		_value = pSecond;
	}
}
