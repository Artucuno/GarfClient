public class DriftTutorialState : IGTutorialState
{
	public ETutorialState OnAttempt = ETutorialState.Drift_TryAgain;

	public ETutorialState OnBlue = ETutorialState.Drift_NotBad;

	public ETutorialState OnRed = ETutorialState.Drift_Perfect;

	public override ETutorialState NextState
	{
		get
		{
			switch (GameMode.DriftSuccessLevel)
			{
			case 3:
				return OnRed;
			case 2:
				return OnBlue;
			case 1:
				return OnAttempt;
			default:
				return NextState;
			}
		}
	}
}
