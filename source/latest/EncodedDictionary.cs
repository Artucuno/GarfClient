using System.Collections.Generic;

public class EncodedDictionary<T> : List<SerializablePair<byte[], T>>
{
	public T this[byte[] pKey]
	{
		get
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SerializablePair<byte[], T> current = enumerator.Current;
					if (current.Key != null && ByteArrayEqual(current.Key, pKey))
					{
						return current.Value;
					}
				}
			}
			return default(T);
		}
		set
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SerializablePair<byte[], T> current = enumerator.Current;
					if (current.Key != null && ByteArrayEqual(current.Key, pKey))
					{
						current.Value = value;
						break;
					}
				}
			}
		}
	}

	public void Add(byte[] pKey, T pValue)
	{
		if (!ContainsKey(pKey))
		{
			Add(new SerializablePair<byte[], T>(pKey, pValue));
		}
	}

	public void TryGetValue(byte[] pKey, out T opValue)
	{
		opValue = this[pKey];
	}

	public bool ContainsKey(byte[] pKey)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SerializablePair<byte[], T> current = enumerator.Current;
				if (ByteArrayEqual(current.Key, pKey))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool ByteArrayEqual(byte[] pFirst, byte[] pSecond)
	{
		if (pFirst.Length != pSecond.Length)
		{
			return false;
		}
		for (int i = 0; i < pFirst.Length; i++)
		{
			if (pFirst[i] != pSecond[i])
			{
				return false;
			}
		}
		return true;
	}

	public bool ContainsKey(byte[] pKey, out SerializablePair<byte[], T> opItem)
	{
		opItem = null;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SerializablePair<byte[], T> current = enumerator.Current;
				if (ByteArrayEqual(current.Key, pKey))
				{
					opItem = current;
					return true;
				}
			}
		}
		return false;
	}

	public void Remove(byte[] pKey)
	{
		SerializablePair<byte[], T> opItem = null;
		if (ContainsKey(pKey, out opItem))
		{
			Remove(opItem);
		}
	}
}
