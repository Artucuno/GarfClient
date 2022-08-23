public class CharacterCarac : DrivingCarac
{
	public EWeight Weight;

	public E_UnlockableItemSate State;

	public override void Start()
	{
		base.Start();
	}

	public static int Compare(CharacterCarac oItem1, CharacterCarac oItem2)
	{
		int num = CompareState(oItem1, oItem2);
		if (num == 0)
		{
			num = IconCarac.CompareName(oItem1, oItem2);
		}
		return num;
	}

	private static int CompareState(CharacterCarac oItem1, CharacterCarac oItem2)
	{
		GameSaveManager instance = Singleton<GameSaveManager>.Instance;
		return IconCarac.SuppressNewState(instance.GetCharacterState(oItem2.Owner)) - IconCarac.SuppressNewState(instance.GetCharacterState(oItem1.Owner));
	}
}
