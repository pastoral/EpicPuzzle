using UnityEngine;
using UnityEngine.UI;
using System;

public static class UIHelper
{
	private static readonly Vector3 popupStartScale = new Vector3(0.5f, 0.5f, 0.5f);
	private static readonly Vector3 popupEndScale = new Vector3(1.1f, 1.1f, 1.1f);
	private static readonly float popupEndOpacity = 0.8f;
	private static readonly float popupDuration = 0.1f;

#region Button

	public static void SetSprite(this Button button, Sprite sprite)
	{
		// Get image
		Image image = button.GetComponent<Image>();
		
		if (image != null)
		{
			image.sprite = sprite;
		}
	}

#endregion

	public static void ShowPopup(GameObject popup, Action callback = null)
	{
		// Reset button scale
		popup.ResetButtonScale(true);
		
		// Show popup
		popup.Show();
		
		// Fade-in
		popup.Play(FadeAction.FadeTo(popupEndOpacity, popupDuration));
		
		// Content
		GameObject content = popup.FindInChildren("Popup");
		
		if (content != null)
		{
			content.transform.localScale = popupStartScale;
			
			var zoomOut = ScaleAction.ScaleTo(popupEndScale, popupDuration * 0.7f);
			var zoomIn = ScaleAction.ScaleTo(Vector3.one, popupDuration * 0.3f);
			var action = SequenceAction.Create(zoomOut, zoomIn);
			
			content.Play(action, callback);
		}
		else
		{
			if (callback != null)
			{
				callback();
			}
		}
	}

	public static void HidePopup(GameObject popup, Action callback)
	{
		// Content
		GameObject content = popup.FindInChildren("Popup");
		
		if (content != null)
		{
			// Hide content
			content.transform.localScale = Vector3.zero;
		}
		
		// Fade-out
		popup.Play(FadeAction.FadeOut(popupDuration), () => {
			// Hide popup
			popup.Hide();

			if (callback != null)
			{
				callback();
			}
		});
	}
}
