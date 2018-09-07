using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class WatchVideoRewardScript : MonoBehaviour, IPointerClickHandler
{
	/// <summary>
	/// The popup.
	/// </summary>
	public GameObject popup;

	/// <summary>
	/// The title.
	/// </summary>
	public Text titleText;

	/// <summary>
	/// The message.
	/// </summary>
	public Text messageText;

	// The callback
	private Action _callback;
	
	public void Construct(string message, Action callback = null)
	{
		// Set message
		messageText.text = message;
		
		// Set callback
		_callback = callback;

//		RectTransform messageRectTransform = messageText.GetComponent<RectTransform>();
//		messageRectTransform.anchoredPosition = new Vector2(0, -120);
//
//		RectTransform popupRectTransform = popup.GetComponent<RectTransform>();
//		Vector2 popupSize = popupRectTransform.sizeDelta;
//		popupSize.y = messageText.preferredHeight + 360;
//		popupRectTransform.sizeDelta = popupSize;
	}

	public void OK()
	{
		// Play sound
		SoundManager.PlayButtonClick();
		MapSelectionScript script = FindObjectOfType<MapSelectionScript>();
		if (script != null){
			script.UpdateStatus();
		}

		
		if (_callback != null)
		{
			_callback();
		}


		// Self-destroy
		Destroy(gameObject);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OK();
	}
}
