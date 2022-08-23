using System.Collections.Generic;
using UnityEngine;

public class PathRecorder : MonoBehaviour
{
	public static List<Vector3> Path = new List<Vector3>();

	private float _pathTempo;

	public float Sampling;

	private bool _recording;

	private GameObject _target;

	private Vector3 _Position
	{
		get
		{
			return _target.transform.position;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if ((bool)_target)
		{
			if (Input.GetKeyDown(KeyCode.R) && Application.isPlaying && Application.isEditor)
			{
				_recording = !_recording;
			}
			if (!_recording || !Application.isPlaying || !Application.isEditor)
			{
				return;
			}
			float num = Time.deltaTime * 1000f;
			if (num != 0f)
			{
				_pathTempo += num;
				if (_pathTempo > Sampling)
				{
					_pathTempo -= Sampling;
					Path.Add(_Position);
				}
			}
		}
		else if (Singleton<GameManager>.Instance.GameMode != null)
		{
			_target = Singleton<GameManager>.Instance.GameMode.GetPlayer(0);
		}
	}

	private void OnGUI()
	{
		string text = "On";
		if (!_recording)
		{
			text = "Off";
		}
		GUI.contentColor = new Color(200f, 200f, 200f);
		GUI.Label(new Rect(20f, 50f, 200f, 20f), "Path Recorder : " + text);
	}
}
