public class BonusTutorialState : IGTutorialState
{
	public ETutorialState ENextState_Behind;

	public EITEM DesiredItem = EITEM.ITEM_LASAGNA;

	public LaunchType Type;

	private bool m_bLaunchedBehind;

	public bool LaunchedBehind
	{
		get
		{
			return m_bLaunchedBehind;
		}
		set
		{
			m_bLaunchedBehind = value;
		}
	}

	public override ETutorialState NextState
	{
		get
		{
			if (LaunchedBehind)
			{
				return ENextState_Behind;
			}
			return ENextState;
		}
	}

	public override void OnEnter()
	{
		base.OnEnter();
		GameMode.ReactivateBBEs();
		GameMode.ShowHUDBonus = true;
		GameMode.WannaItem = true;
		GameMode.DesiredItem = DesiredItem;
	}

	public override void OnExit()
	{
		GameMode.WannaItem = false;
		base.OnExit();
	}
}
