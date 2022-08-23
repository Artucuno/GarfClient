using UnityEngine;

public class AccelerationTutorialState : IGTutorialState
{
	public ETutorialState ENextState_Mobile = ETutorialState.Direction_Gyro;

	public override ETutorialState NextState
	{
		get
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				return ENextState_Mobile;
			}
			return ENextState;
		}
	}
}
