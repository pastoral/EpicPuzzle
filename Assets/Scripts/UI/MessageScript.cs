using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MessageScript : DialogBehaviour
{
	private static readonly float extraHeight1 = 420.0f;
	private static readonly float extraHeight2 = 360.0f;
	private static readonly float space		   = 20.0f;

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

	/// <summary>
	/// The image.
	/// </summary>
	public Image image;

	// The callback
	private Action _callback;

	public void Construct(string title, string message, Action callback = null)
	{
		// Set title
		titleText.text = title;

		// Set message
		messageText.text = message;

		// Set callback
		_callback = callback;

//		RectTransform popupRectTransform = popup.GetComponent<RectTransform>();
//		Vector2 popupSize = popupRectTransform.sizeDelta;
//		popupSize.y = messageText.preferredHeight + extraHeight1;
//		popupRectTransform.sizeDelta = popupSize;
	}
	
	public void Construct(string message, Action callback = null)
	{
		// Set message
		messageText.text = message;
		
		// Set callback
		_callback = callback;
		
		// Hide title
		titleText.gameObject.Hide();

//		RectTransform messageRectTransform = messageText.GetComponent<RectTransform>();
//		messageRectTransform.anchoredPosition = new Vector2(0, -120);
//
//		RectTransform popupRectTransform = popup.GetComponent<RectTransform>();
//		Vector2 popupSize = popupRectTransform.sizeDelta;
//		popupSize.y = messageText.preferredHeight + extraHeight2;
//		popupRectTransform.sizeDelta = popupSize;
	}

	public void Construct(string title, string message, Sprite sprite, Action callback = null)
	{
		// Set title
		titleText.text = title;

		// Set message
		messageText.text = message;

		// Set callback
		_callback = callback;

		float messageHeight = messageText.preferredHeight;
		float imageHeight = -space;

		// Set image
		if (image != null)
		{
			image.sprite = sprite;
			image.SetNativeSize();
			image.gameObject.SetActive(true);

//			Vector2 imagePosition = image.rectTransform.anchoredPosition;
//			imagePosition.y = messageText.rectTransform.anchoredPosition.y - (messageHeight + space);
//			image.rectTransform.anchoredPosition = imagePosition;
//
//			imageHeight = image.rectTransform.sizeDelta.y;
		}

//		RectTransform popupRectTransform = popup.GetComponent<RectTransform>();
//		Vector2 popupSize = popupRectTransform.sizeDelta;
//		popupSize.y = messageHeight + space + imageHeight + extraHeight1;
//		popupRectTransform.sizeDelta = popupSize;
	}

	public void Construct(string message, Sprite sprite, Action callback = null)
	{
		// Set message
		messageText.text = message;

		// Set callback
		_callback = callback;

		// Hide title
		titleText.gameObject.Hide();

		RectTransform messageRectTransform = messageText.GetComponent<RectTransform>();
		messageRectTransform.anchoredPosition = new Vector2(0, -160);

		float messageHeight = messageText.preferredHeight;
		float imageHeight = -space;

		// Set image
		if (image != null)
		{
			image.sprite = sprite;
			image.SetNativeSize();
			image.gameObject.SetActive(true);

			Vector2 imagePosition = image.rectTransform.anchoredPosition;
			imagePosition.y = messageText.rectTransform.anchoredPosition.y - (messageHeight + space);
			image.rectTransform.anchoredPosition = imagePosition;

			imageHeight = image.rectTransform.sizeDelta.y;
		}

		RectTransform popupRectTransform = popup.GetComponent<RectTransform>();
		Vector2 popupSize = popupRectTransform.sizeDelta;
		popupSize.y = messageHeight + space + imageHeight + extraHeight2;
		popupRectTransform.sizeDelta = popupSize;
	}

	public void Ok()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		if (_callback != null)
		{
			_callback();
		}

		// Self-destroy
		Destroy(gameObject);
	}

	public override void Close()
	{
		Ok();
	}

//#if UNITY_EDITOR
//	void Update()
//	{
//		if (Input.GetKeyDown(KeyCode.Return))
//		{
//			Ok();
//		}
//	}
//#endif
}
