using System.Collections.Generic;

internal class RaceScoreComparer : IComparer<Tuple<int, int>>
{
	public int Compare(int pScore1, int pScore2)
	{
		return pScore2.CompareTo(pScore1);
	}

	public int Compare(Tuple<int, int> x, Tuple<int, int> y)
	{
		return Compare(x.Item2, y.Item2);
	}
}
