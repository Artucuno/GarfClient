using UnityEngine;

public class CreditsScrollingLabel : MonoBehaviour
{
	public delegate void ScrollingLabelEndHandler(CreditsScrollingLabel Label);

	public float Speed = 5f;

	private UILabel Label;

	public float EndOffset = 200f;

	private bool Updatable;

	public bool Updating
	{
		get
		{
			return Updatable;
		}
		private set
		{
		}
	}

	public event ScrollingLabelEndHandler OnLabelEnd;

	private void Awake()
	{
		Label = GetComponent<UILabel>();
	}

	private void Update()
	{
		if (!Updatable)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y += Time.deltaTime * Speed;
		base.transform.localPosition = localPosition;
		if (localPosition.y >= EndOffset)
		{
			Updatable = false;
			if (this.OnLabelEnd != null)
			{
				this.OnLabelEnd(this);
			}
		}
	}

	public void SetItem(string Text, CreditsItemDesc Desc, GameObject pCreditFont)
	{
		if ((bool)Label)
		{
			Label.text = Text;
			Label.transform.localScale = new Vector3(Desc.FontSize, Desc.FontSize, 1f);
			Label.font = pCreditFont.GetComponent<UIFont>();
			Label.color = Desc.FontColor;
			Label.MarkAsChanged();
			Updatable = true;
		}
	}

	public void Reset()
	{
		Updatable = false;
	}
}
