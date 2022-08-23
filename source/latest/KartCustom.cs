public class KartCustom : DrivingCarac
{
	public ERarity Rarity;

	public ECharacter Character;

	public E_UnlockableItemSate State;

	public override void Start()
	{
		base.Start();
	}

	public static int Compare(KartCustom oItem1, KartCustom oItem2)
	{
		int num = IconCarac.CompareNameDefault(oItem1, oItem2);
		if (num == 0)
		{
			num = CompareStateHidden(oItem1, oItem2);
			if (num == 0)
			{
				num = CompareRarity(oItem1, oItem2);
				if (num == 0)
				{
					num = CompareState(oItem1, oItem2);
					if (num == 0)
					{
						num = IconCarac.CompareName(oItem1, oItem2);
					}
				}
			}
		}
		return num;
	}

	private static int CompareRarity(KartCustom oItem1, KartCustom oItem2)
	{
		return oItem1.Rarity - oItem2.Rarity;
	}

	private static int CompareState(KartCustom oItem1, KartCustom oItem2)
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		return IconCarac.SuppressNewState(instance.GetCustomState(oItem2.name)) - IconCarac.SuppressNewState(instance.GetCustomState(oItem1.name));
	}

	private static int CompareStateHidden(KartCustom oItem1, KartCustom oItem2)
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		bool flag = ((instance.GetCustomState(oItem2.name) == E_UnlockableItemSate.Hidden) ? true : false);
		if (instance.GetCustomState(oItem1.name) == E_UnlockableItemSate.Hidden)
		{
			if (flag)
			{
				return 0;
			}
			return 1;
		}
		if (flag)
		{
			return -1;
		}
		return 0;
	}
}
