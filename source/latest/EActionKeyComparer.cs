using System.Collections.Generic;

public class EActionKeyComparer : IEqualityComparer<EAction>
{
	public bool Equals(EAction x, EAction y)
	{
		return x == y;
	}

	public int GetHashCode(EAction x)
	{
		return (int)x;
	}
}
