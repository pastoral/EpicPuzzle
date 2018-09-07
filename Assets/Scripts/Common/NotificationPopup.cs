using UnityEngine;
using UnityEngine.UI;
using System;

public class NotificationPopup : MonoBehaviour
{
	private static readonly float OffsetY = 200.0f;

	/// <summary>
	/// The message text.
	/// </summary>
	[SerializeField]
	private Text _messageText;

	// True if showing
	private bool _isShowing;

	public bool Showing
	{
		get
		{
			return _isShowing;
		}
	}

	public void Show(string message, Action callback)
	{
		_isShowing = true;

		// Set message
		_messageText.text = message;

		var move1 = AnchoredMoveAction.Create(Vector2.zero, false, 0.5f, Ease.SineOut);
		var delay = DelayAction.Create(1.0f);
		var move2 = AnchoredMoveAction.Create(new Vector2(0, OffsetY), false, 0.5f, Ease.SineIn);
		var action = SequenceAction.Create(move1, delay, move2);

		gameObject.Play(action, () => {
			_isShowing = false;

			if (callback != null) callback();
		});
	}
}
