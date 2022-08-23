using UnityEngine;

public class TrackModeButton : MonoBehaviour
{
	public GameObject PanelToHide;

	public GameObject PanelToShow;

	private void OnClick()
	{
		if (PanelToHide != null)
		{
			PanelToHide.SetActive(false);
		}
		if (PanelToShow != null)
		{
			PanelToShow.SetActive(true);
		}
	}
}
