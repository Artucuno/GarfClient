using UnityEngine;

public class GarfieldMenuBackGround : MonoBehaviour
{
	public MenuEntryPoint menu;

	public void selectBackgroundAnim()
	{
		menu.SelectRandomMenuAnim();
	}
}
