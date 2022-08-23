public struct MultiPathPosition
{
	public PathPosition pathPosition;

	public RcMultiPathSection section;

	public static MultiPathPosition UNDEFINED_MP_POS
	{
		get
		{
			return new MultiPathPosition(PathPosition.UNDEFINED_POSITION, null);
		}
	}

	public MultiPathPosition(PathPosition p, RcMultiPathSection s)
	{
		pathPosition = p;
		section = s;
	}
}
