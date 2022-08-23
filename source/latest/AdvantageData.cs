public class AdvantageData : IconCarac
{
	public EAdvantage AdvantageType;

	public E_UnlockableItemSate State;

	public int Price;

	public bool bIsAvailableInSingle;

	public bool bIsAvailableInChampionship;

	public bool bIsAvailableInMulti;

	public static int Compare(AdvantageData oItem1, AdvantageData oItem2)
	{
		int num = CompareState(oItem1, oItem2);
		if (num == 0)
		{
			num = IconCarac.CompareName(oItem1, oItem2);
		}
		return num;
	}

	private static int CompareState(AdvantageData oItem1, AdvantageData oItem2)
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		return IconCarac.SuppressNewState(instance.GetAdvantageState(oItem2.AdvantageType)) - IconCarac.SuppressNewState(instance.GetAdvantageState(oItem1.AdvantageType));
	}
}
