using System;

[Serializable]
public class BehaviourSettings
{
	public float EasyBonusDelay = 0.5f;

	public float NormalBonusDelay = 0.5f;

	public float HardBonusDelay = 0.25f;

	public float BonusDelayBeforeUseMin = 2f;

	public float BonusDelayBeforeUseMax = 5f;

	public float BonusDelayMax = 20f;

	public float PieBeforeDistance;

	public float PieBehindDistance;

	public float MagicDistance;

	public float DiamondBehindDistance;

	public float DiamondBeforeDistance;

	public float ParfumeDistance;

	public float DefenseZoneRadius = 5f;

	public Chance ShootChance = new Chance(90, 60, 30);

	public Chance BoostStart50ccChance = new Chance(75, 30, 0);

	public Chance BoostStart100ccChance = new Chance(75, 30, 0);

	public Chance BoostStart150ccChance = new Chance(95, 50, 0);

	public float KeepItemChance = 33f;

	public float RatioSphereCollider = 1.3f;

	public float PCoefficient = 1f;

	public float ICoefficient = 0.1f;

	public float DCoefficient = 0.1f;

	public float m_fMinRatioIdealFromMaxSpeed = 0.8f;
}
