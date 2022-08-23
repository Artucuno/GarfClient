using UnityEngine;

public class ChangeCameWithKeyboard : MonoBehaviour
{
	public Camera camera1;

	public Camera camera2;

	public Camera camera3;

	public Camera camera4;

	public Camera camera5;

	public Camera camera6;

	public Camera camera7;

	public Camera camera8;

	public Camera camera9;

	public Camera camera10;

	public GameObject Kart1;

	public GameObject Kart2;

	public GameObject Kart3;

	public GameObject Kart4;

	public GameObject Kart5;

	public GameObject Kart6;

	public GameObject Kart7;

	public GameObject Kart8;

	public GameObject Kart9;

	public GameObject Kart10;

	private void Start()
	{
		camera1.camera.enabled = true;
		camera2.camera.enabled = false;
		camera3.camera.enabled = false;
		camera4.camera.enabled = false;
		camera5.camera.enabled = false;
		camera6.camera.enabled = false;
		camera7.camera.enabled = false;
		camera8.camera.enabled = false;
		camera9.camera.enabled = false;
		camera10.camera.enabled = false;
		Kart1.SetActive(true);
		Kart2.SetActive(false);
		Kart3.SetActive(false);
		Kart4.SetActive(false);
		Kart5.SetActive(false);
		Kart6.SetActive(false);
		Kart7.SetActive(false);
		Kart8.SetActive(false);
		Kart9.SetActive(false);
		Kart10.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Alpha1))
		{
			camera1.camera.enabled = true;
			camera2.camera.enabled = false;
			camera3.camera.enabled = false;
			camera4.camera.enabled = false;
			camera5.camera.enabled = false;
			camera6.camera.enabled = false;
			camera7.camera.enabled = false;
			camera8.camera.enabled = false;
			camera9.camera.enabled = false;
			camera10.camera.enabled = false;
			Kart1.SetActive(true);
			Kart2.SetActive(false);
			Kart3.SetActive(false);
			Kart4.SetActive(false);
			Kart5.SetActive(false);
			Kart6.SetActive(false);
			Kart7.SetActive(false);
			Kart8.SetActive(false);
			Kart9.SetActive(false);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha2))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = true;
			camera3.camera.enabled = false;
			camera4.camera.enabled = false;
			camera5.camera.enabled = false;
			camera6.camera.enabled = false;
			camera7.camera.enabled = false;
			camera8.camera.enabled = false;
			camera9.camera.enabled = false;
			camera10.camera.enabled = false;
			Kart1.SetActive(false);
			Kart2.SetActive(true);
			Kart3.SetActive(false);
			Kart4.SetActive(false);
			Kart5.SetActive(false);
			Kart6.SetActive(false);
			Kart7.SetActive(false);
			Kart8.SetActive(false);
			Kart9.SetActive(false);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha3))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = false;
			camera3.camera.enabled = true;
			camera4.camera.enabled = false;
			camera5.camera.enabled = false;
			camera6.camera.enabled = false;
			camera7.camera.enabled = false;
			camera8.camera.enabled = false;
			camera9.camera.enabled = false;
			camera10.camera.enabled = false;
			Kart1.SetActive(false);
			Kart2.SetActive(false);
			Kart3.SetActive(true);
			Kart4.SetActive(false);
			Kart5.SetActive(false);
			Kart6.SetActive(false);
			Kart7.SetActive(false);
			Kart8.SetActive(false);
			Kart9.SetActive(false);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha4))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = false;
			camera3.camera.enabled = false;
			camera4.camera.enabled = true;
			camera5.camera.enabled = false;
			camera6.camera.enabled = false;
			camera7.camera.enabled = false;
			camera8.camera.enabled = false;
			camera9.camera.enabled = false;
			camera10.camera.enabled = false;
			Kart1.SetActive(false);
			Kart2.SetActive(false);
			Kart3.SetActive(false);
			Kart4.SetActive(true);
			Kart5.SetActive(false);
			Kart6.SetActive(false);
			Kart7.SetActive(false);
			Kart8.SetActive(false);
			Kart9.SetActive(false);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha5))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = false;
			camera3.camera.enabled = false;
			camera4.camera.enabled = false;
			camera5.camera.enabled = true;
			camera6.camera.enabled = false;
			camera7.camera.enabled = false;
			camera8.camera.enabled = false;
			camera9.camera.enabled = false;
			camera10.camera.enabled = false;
			Kart1.SetActive(false);
			Kart2.SetActive(false);
			Kart3.SetActive(false);
			Kart4.SetActive(false);
			Kart5.SetActive(true);
			Kart6.SetActive(false);
			Kart7.SetActive(false);
			Kart8.SetActive(false);
			Kart9.SetActive(false);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha6))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = false;
			camera3.camera.enabled = false;
			camera4.camera.enabled = false;
			camera5.camera.enabled = false;
			camera6.camera.enabled = true;
			camera7.camera.enabled = false;
			camera8.camera.enabled = false;
			camera9.camera.enabled = false;
			camera10.camera.enabled = false;
			Kart1.SetActive(false);
			Kart2.SetActive(false);
			Kart3.SetActive(false);
			Kart4.SetActive(false);
			Kart5.SetActive(false);
			Kart6.SetActive(true);
			Kart7.SetActive(false);
			Kart8.SetActive(false);
			Kart9.SetActive(false);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha7))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = false;
			camera3.camera.enabled = false;
			camera4.camera.enabled = false;
			camera5.camera.enabled = false;
			camera6.camera.enabled = false;
			camera7.camera.enabled = true;
			camera8.camera.enabled = false;
			camera9.camera.enabled = false;
			camera10.camera.enabled = false;
			Kart1.SetActive(false);
			Kart2.SetActive(false);
			Kart3.SetActive(false);
			Kart4.SetActive(false);
			Kart5.SetActive(false);
			Kart6.SetActive(false);
			Kart7.SetActive(true);
			Kart8.SetActive(false);
			Kart9.SetActive(false);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha8))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = false;
			camera3.camera.enabled = false;
			camera4.camera.enabled = false;
			camera5.camera.enabled = false;
			camera6.camera.enabled = false;
			camera7.camera.enabled = false;
			camera8.camera.enabled = true;
			camera9.camera.enabled = false;
			camera10.camera.enabled = false;
			Kart1.SetActive(false);
			Kart2.SetActive(false);
			Kart3.SetActive(false);
			Kart4.SetActive(false);
			Kart5.SetActive(false);
			Kart6.SetActive(false);
			Kart7.SetActive(false);
			Kart8.SetActive(true);
			Kart9.SetActive(false);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha9))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = false;
			camera3.camera.enabled = false;
			camera4.camera.enabled = false;
			camera5.camera.enabled = false;
			camera6.camera.enabled = false;
			camera7.camera.enabled = false;
			camera8.camera.enabled = false;
			camera9.camera.enabled = true;
			camera10.camera.enabled = false;
			Kart1.SetActive(false);
			Kart2.SetActive(false);
			Kart3.SetActive(false);
			Kart4.SetActive(false);
			Kart5.SetActive(false);
			Kart6.SetActive(false);
			Kart7.SetActive(false);
			Kart8.SetActive(false);
			Kart9.SetActive(true);
			Kart10.SetActive(false);
		}
		if (Input.GetKey(KeyCode.Alpha0))
		{
			camera1.camera.enabled = false;
			camera2.camera.enabled = false;
			camera3.camera.enabled = false;
			camera4.camera.enabled = false;
			camera5.camera.enabled = false;
			camera6.camera.enabled = false;
			camera7.camera.enabled = false;
			camera8.camera.enabled = false;
			camera9.camera.enabled = false;
			camera10.camera.enabled = true;
			Kart1.SetActive(false);
			Kart2.SetActive(false);
			Kart3.SetActive(false);
			Kart4.SetActive(false);
			Kart5.SetActive(false);
			Kart6.SetActive(false);
			Kart7.SetActive(false);
			Kart8.SetActive(false);
			Kart9.SetActive(false);
			Kart10.SetActive(true);
		}
	}
}
