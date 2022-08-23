using UnityEngine;

public class ChooseControlTutorialState : IGTutorialState
{
	public void Start()
	{
		DisablePanelOnTouch = false;
	}

	public void OnSelectGyro()
	{
		Singleton<GameOptionManager>.Instance.SetInputType(E_InputType.Gyroscopic, true);
		GameMode.OnSuccess();
		base.gameObject.SetActive(false);
	}

	public void OnSelectTouch()
	{
		Singleton<GameOptionManager>.Instance.SetInputType(E_InputType.Touched, true);
		GameMode.OnSuccess();
		base.gameObject.SetActive(false);
	}

	public new void OnDisable()
	{
		base.OnDisable();
		Time.timeScale = 1f;
		Singleton<GameManager>.Instance.GameMode.Hud.HUDControls.ShowExceptPause(true);
	}
}
