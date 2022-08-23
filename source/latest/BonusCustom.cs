using System.Collections.Generic;

public class BonusCustom : IconCarac
{
	public EITEM Category;

	public EBonusCustomEffect Effect;

	public float Value;

	public ECharacter Character;

	public float MegaValue;

	public ERarity Rarity;

	public ECharacter Owner;

	public List<Template> Transforms = new List<Template>();

	public E_UnlockableItemSate State;

	public int NbSlots;

	public Template GetTemplate(ECharacter pCharacter)
	{
		foreach (Template transform in Transforms)
		{
			if (transform.Character == pCharacter)
			{
				return transform;
			}
		}
		return null;
	}

	public static int Compare(BonusCustom oItem1, BonusCustom oItem2)
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
						num = CompareUnique(oItem1, oItem2);
						if (num == 0)
						{
							num = IconCarac.CompareName(oItem1, oItem2);
						}
					}
				}
			}
		}
		return num;
	}

	private static int CompareRarity(BonusCustom oItem1, BonusCustom oItem2)
	{
		return oItem1.Rarity - oItem2.Rarity;
	}

	private static int CompareUnique(BonusCustom oItem1, BonusCustom oItem2)
	{
		bool flag = oItem2.Owner != ECharacter.NONE;
		if (oItem1.Owner != ECharacter.NONE)
		{
			if (flag)
			{
				return 0;
			}
			return -1;
		}
		if (flag)
		{
			return 1;
		}
		return 0;
	}

	private static int CompareState(BonusCustom oItem1, BonusCustom oItem2)
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		return IconCarac.SuppressNewState(instance.GetHatState(oItem2.name)) - IconCarac.SuppressNewState(instance.GetHatState(oItem1.name));
	}

	private static int CompareStateHidden(BonusCustom oItem1, BonusCustom oItem2)
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		bool flag = ((instance.GetHatState(oItem2.name) == E_UnlockableItemSate.Hidden) ? true : false);
		if (instance.GetHatState(oItem1.name) == E_UnlockableItemSate.Hidden)
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
