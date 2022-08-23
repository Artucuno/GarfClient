using UnityEngine;

public class DirecTactTutorialState : IGTutorialState
{
	public Animation ControlsAnim;

	public override void OnEnable()
	{
		Singleton<GameOptionManager>.Instance.SetInputType(E_InputType.Touched, false);
		Singleton<GameManager>.Instance.GameMode.Hud.HUDControls.ShowExceptPause(true);
		base.OnEnable();
		if ((bool)ControlsAnim)
		{
			ControlsAnim.Play();
		}
	}

	public override void OnDisable()
	{
		if ((bool)ControlsAnim)
		{
			ControlsAnim.Stop();
		}
		base.OnDisable();
	}
}
