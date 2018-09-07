using UnityEngine;
using UnityEngine.UI;

public class SetAnchoredPositionAction : BaseAction
{
	// The position
	private Vector2 _position;

	public SetAnchoredPositionAction(Vector2 position)
	{
		// Set position
		_position = position;
	}
	
	public static SetAnchoredPositionAction Create(Vector2 position)
	{
		return new SetAnchoredPositionAction(position);
	}

	public override void Play(GameObject target)
	{
		// Get rect transform
		RectTransform rectTransform = target.GetComponent<RectTransform>();

		if (rectTransform != null)
		{
			rectTransform.anchoredPosition = _position;
		}
		else
		{
			//Debug.LogWarning("Rect Transform required!");
		}
	}
}
