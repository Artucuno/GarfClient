using System;

[Serializable]
public class Tuple<T1>
{
	public T1 Item1;

	public Tuple()
		: this(default(T1))
	{
	}

	public Tuple(T1 pItem1)
	{
		Item1 = pItem1;
	}
}
[Serializable]
public class Tuple<T1, T2> : Tuple<T1>
{
	public T2 Item2;

	public Tuple()
		: this(default(T1), default(T2))
	{
	}

	public Tuple(T1 pItem1, T2 pItem2)
		: base(pItem1)
	{
		Item2 = pItem2;
	}
}
[Serializable]
public class Tuple<T1, T2, T3> : Tuple<T1, T2>
{
	public T3 Item3;

	public Tuple()
		: this(default(T1), default(T2), default(T3))
	{
	}

	public Tuple(T1 pItem1, T2 pItem2, T3 pItem3)
		: base(pItem1, pItem2)
	{
		Item3 = pItem3;
	}
}
[Serializable]
public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
{
	public T4 Item4;

	public Tuple()
		: this(default(T1), default(T2), default(T3), default(T4))
	{
	}

	public Tuple(T1 pItem1, T2 pItem2, T3 pItem3, T4 pItem4)
		: base(pItem1, pItem2, pItem3)
	{
		Item4 = pItem4;
	}
}
[Serializable]
public class Tuple<T1, T2, T3, T4, T5> : Tuple<T1, T2, T3, T4>
{
	public T5 Item5;

	public Tuple()
		: this(default(T1), default(T2), default(T3), default(T4), default(T5))
	{
	}

	public Tuple(T1 pItem1, T2 pItem2, T3 pItem3, T4 pItem4, T5 pItem5)
		: base(pItem1, pItem2, pItem3, pItem4)
	{
		Item5 = pItem5;
	}
}
[Serializable]
public class Tuple<T1, T2, T3, T4, T5, T6> : Tuple<T1, T2, T3, T4, T5>
{
	public T6 Item6;

	public Tuple()
		: this(default(T1), default(T2), default(T3), default(T4), default(T5), default(T6))
	{
	}

	public Tuple(T1 pItem1, T2 pItem2, T3 pItem3, T4 pItem4, T5 pItem5, T6 pItem6)
		: base(pItem1, pItem2, pItem3, pItem4, pItem5)
	{
		Item6 = pItem6;
	}
}
[Serializable]
public class Tuple<T1, T2, T3, T4, T5, T6, T7> : Tuple<T1, T2, T3, T4, T5, T6>
{
	public T7 Item7;

	public Tuple()
		: this(default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), default(T7))
	{
	}

	public Tuple(T1 pItem1, T2 pItem2, T3 pItem3, T4 pItem4, T5 pItem5, T6 pItem6, T7 pItem7)
		: base(pItem1, pItem2, pItem3, pItem4, pItem5, pItem6)
	{
		Item7 = pItem7;
	}
}
[Serializable]
public class Tuple<T1, T2, T3, T4, T5, T6, T7, T8> : Tuple<T1, T2, T3, T4, T5, T6, T7>
{
	public T8 Item8;

	public Tuple()
		: this(default(T1), default(T2), default(T3), default(T4), default(T5), default(T6), default(T7), default(T8))
	{
	}

	public Tuple(T1 pItem1, T2 pItem2, T3 pItem3, T4 pItem4, T5 pItem5, T6 pItem6, T7 pItem7, T8 pItem8)
		: base(pItem1, pItem2, pItem3, pItem4, pItem5, pItem6, pItem7)
	{
		Item8 = pItem8;
	}
}
