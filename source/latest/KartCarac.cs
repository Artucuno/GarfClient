public class KartCarac : DrivingCarac
{
	public E_UnlockableItemSate State;

	public override void Start()
	{
		base.Start();
	}

	public static int Compare(KartCarac oItem1, KartCarac oItem2)
	{
		int num = CompareState(oItem1, oItem2);
		if (num == 0)
		{
			num = IconCarac.CompareName(oItem1, oItem2);
		}
		return num;
	}

	private static int CompareState(KartCarac oItem1, KartCarac oItem2)
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		return IconCarac.SuppressNewState(instance.GetKartState(oItem2.Owner)) - IconCarac.SuppressNewState(instance.GetKartState(oItem1.Owner));
	}
}
