public class DrivingCarac : IconCarac
{
	public ECharacter Owner;

	public float Acceleration;

	public float Speed;

	public float Maniability;

	public float Bonus;

	public DrivingCaracteristics BonusCaracteristic;

	public int NbSlots;

	public virtual void Start()
	{
	}

	public float GetCarac(DrivingCaracteristics _Carac)
	{
		switch (_Carac)
		{
		case DrivingCaracteristics.ACCELERATION:
			return Acceleration;
		case DrivingCaracteristics.SPEED:
			return Speed;
		case DrivingCaracteristics.MANIABILITY:
			return Maniability;
		default:
			return 0f;
		}
	}
}
