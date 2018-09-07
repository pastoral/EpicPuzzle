using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ConfirmScript : DialogBehaviour
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
	private Action<bool> _callback;

	public void Construct(string title, string message, Action<bool> callback = null)
	{
		// Set title
		titleText.text = title;

		// Set message
		messageText.text = message;

		// Set callback
		_callback = callback;

//		RectTransform popupRectTransform = popup.GetComponent<RectTransform>();
//		Vector2 popupSize = popupRectTransform.sizeDelta;
//		popupSize.y = messageText.preferredHeight + 420;
//		popupRectTransform.sizeDelta = popupSize;
	}


	public void Yes()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		if (_callback != null)
		{
			_callback(true);
		}

		// Self-destroy
		Destroy(gameObject);
	}
	
	public void No()
	{
		// Play sound
		SoundManager.PlayButtonClick();
		
		if (_callback != null)
		{
			_callback(false);
		}
		
		// Self-destroy
		Destroy(gameObject);
	}

	public override void Close()
	{
		No();
	}

//#if UNITY_EDITOR
//	void Update()
//	{
//		if (Input.GetKeyDown(KeyCode.Return))
//		{
//			Yes();
//		}
//		else if (Input.GetKeyDown(KeyCode.Escape))
//		{
//			No();
//		}
//	}
//#endif
}
