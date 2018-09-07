using UnityEngine;
using UnityEngine.UI;
using System;

public class UnlockBoosterPopupScript : MonoBehaviour
{
	/// <summary>
	/// The booster icons.
	/// </summary>
	public Sprite[] icons = new Sprite[4];

	/// <summary>
	/// The message.
	/// </summary>
	public Text message;

	/// <summary>
	/// The icon.
	/// </summary>
	public Image icon;

	/// <summary>
	/// The description.
	/// </summary>
	public Text description;

	// The callback
	private Action _callback;

	public void Show(BoosterType type, float delay = 0.5f, Action callback = null)
	{
		// Set callback
		_callback = callback;

		// Set message
		//message.text = type.GetUnlockMessage();

		// Set icon
		icon.sprite = icons[type.ToInt()];

		// Set description
		description.text = type.GetDescription();

		// Disable interaction
		SetInteractable(false);

		if (delay > 0)
		{
			Invoke("ShowCallback", delay);
		}
		else
		{
			ShowCallback();
		}
	}

	void ShowCallback()
	{
		// Show popup
		UIHelper.ShowPopup(gameObject, () => {
			// Enable interaction
			SetInteractable(true);
		});
	}

	public void Close()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Hide popup
		HidePopup();
	}

	void SetInteractable(bool interactable)
	{
		Canvas canvas = FindObjectOfType<Canvas>();

		if (canvas != null)
		{
			canvas.SetInteractable(interactable);
		}
	}
	
	void HidePopup()
	{
		// Disable interaction
		SetInteractable(false);
		
		UIHelper.HidePopup(gameObject, () => {
			// Enable interaction
			SetInteractable(true);

			if (_callback != null)
			{
				_callback();
			}
		});
	}
}
