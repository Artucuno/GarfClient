public struct PathPosition
{
	public int index;

	public float ratio;

	public float sqrDist;

	public static PathPosition UNDEFINED_POSITION
	{
		get
		{
			return new PathPosition(-1, 0f, float.MaxValue);
		}
	}

	public PathPosition(int i, float r, float s)
	{
		index = i;
		ratio = r;
		sqrDist = s;
	}
}
