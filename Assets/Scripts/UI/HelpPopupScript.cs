using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class HelpPopupScript : DialogBehaviour
{
	// The close callback
	private Action _closeCallback;

	public Action CloseCallback
	{
		set
		{
			_closeCallback = value;
		}
	}

	void Start()
	{
		UIHelper.ShowPopup(gameObject);
	}

	public override void Close()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		UIHelper.HidePopup(gameObject, () => {
			if (_closeCallback != null)
			{
				_closeCallback();
			}

			// Self-destroy
			Destroy(gameObject);	
		});
	}
}
