using System;

[Serializable]
public class ChanceSettings
{
	public Chance KartChance = new Chance(33, 20, 10);

	public ItemChance CustoChance = new ItemChance(33, 20, 5, 33);

	public ItemChance HatChance = new ItemChance(33, 20, 5, 33);
}
