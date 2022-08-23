using System;

[Serializable]
public class PathSettings
{
	public Chance EasyBadPathChance = new Chance(10, 20, 80);

	public Chance EasyAveragePathChance = new Chance(30, 70, 20);

	public Chance EasyGoodPathChance = new Chance(60, 10, 0);

	public Chance EasyShortcutPathChance = new Chance(60, 10, 0);

	public Chance NormalBadPathChance = new Chance(10, 20, 80);

	public Chance NormalAveragePathChance = new Chance(30, 70, 20);

	public Chance NormalGoodPathChance = new Chance(60, 10, 0);

	public Chance NormalShortcutPathChance = new Chance(60, 10, 0);

	public Chance HardBadPathChance = new Chance(0, 10, 60);

	public Chance HardAveragePathChance = new Chance(20, 60, 30);

	public Chance HardGoodPathChance = new Chance(80, 30, 10);

	public Chance HardShortcutPathChance = new Chance(80, 30, 10);
}
