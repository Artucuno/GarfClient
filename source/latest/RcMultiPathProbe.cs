using UnityEngine;

public class RcMultiPathProbe : MonoBehaviour
{
	public RcMultiPath Path;

	public int SegmentForward = 2;

	public int SegmentBackward = 1;

	public bool Project2D;

	public bool ForceHeight = true;

	public float Height = 2f;

	public bool DebugDraw = true;

	public float DebugScale = 1f;

	public bool DebugReset;

	public GUIStyle DebugGuiStyle;

	public bool DrawProgress;

	public float ProgressStep = 25f;
}
