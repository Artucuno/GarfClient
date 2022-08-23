using UnityEngine;

public class MoodBox : MonoBehaviour
{
	public MoodBoxData data;

	private MoodBoxManager manager;

	private void Start()
	{
		manager = base.transform.parent.GetComponent<MoodBoxManager>();
		if (!manager)
		{
			base.enabled = false;
		}
	}

	private void OnDrawGizmos()
	{
		if ((bool)base.transform.parent)
		{
			Gizmos.color = new Color(0.5f, 0.9f, 1f, 0.15f);
			Gizmos.DrawCube(base.collider.bounds.center, base.collider.bounds.size);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if ((bool)base.transform.parent)
		{
			Gizmos.color = new Color(0.5f, 0.9f, 1f, 0.75f);
			Gizmos.DrawCube(base.collider.bounds.center, base.collider.bounds.size);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			ApplyMoodBox();
		}
	}

	private void ApplyMoodBox()
	{
		MoodBoxManager.current = this;
	}
}
