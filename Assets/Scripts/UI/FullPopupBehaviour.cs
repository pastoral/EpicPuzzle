using UnityEngine;
using UnityEngine.UI;
using System;

public class FullPopupBehaviour : MonoBehaviour
{
	private static readonly float fadeDuration = 0.2f;

	/// <summary>
	/// The background.
	/// </summary>
	public GameObject background;

	/// <summary>
	/// The no more mana popup prefab.
	/// </summary>
	public GameObject noMoreManaPopupPrefab;

	void OnEnable()
	{
		KeyManager.AddBackEventHandler(OnKeyBack);
	}

	void OnDisable()
	{
		KeyManager.RemoveBackEventHandler(OnKeyBack);
	}

	void OnKeyBack()
	{
		Home();
	}

//	void Start()
//	{
//		// Hide
//		gameObject.SetAlpha(0, true);
//		
//		// Hide touch and overlay
//		SetShowTouchAndOverlay(false);
//	}

	protected virtual float OverlayDuration
	{
		get
		{
			return 2.0f;
		}
	}

	public virtual void Show(Action callback = null)
	{
		// Hide
		gameObject.SetAlpha(0, true);
		
		// Hide touch and overlay
		SetShowTouchAndOverlay(false);

		// Disable UI
		SetUIEnabled(false);

		// Show
		gameObject.Show();

		// Overlay
		gameObject.Play(FadeAction.FadeTo(0.8f, this.OverlayDuration), () => {
			// Background
			background.Play(FadeAction.RecursiveFadeIn(fadeDuration), () => {
				OnShowFinished(callback);
			});
		});
	}

	protected virtual void OnShowFinished(Action callback)
	{
		// Show touch and overlay
		SetShowTouchAndOverlay(true);
		
		// Enable UI
		SetUIEnabled(true);
		
		if (callback != null)
		{
			callback();
		}
	}

	public virtual void Restart()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		Replay();
	}

	protected void Replay()
	{
		// Dont check mana
		if (true)
		{
			// Hide
			Hide(false, GameManager.Instance.Replay);
		}
		else
		{
			if (noMoreManaPopupPrefab != null)
			{
				// Create popup
				GameObject noMoreManaPopup = noMoreManaPopupPrefab.CreateUI(transform.parent);
				NoMoreManaPopupScript script = noMoreManaPopup.GetComponent<NoMoreManaPopupScript>();

				if (script != null)
				{
					NotificationManager.AddManaEventHandler(OnManaChanged);

					script.Show(null);
				}
			}
			else
			{
				//Log.Debug("No more mana!");
			}
		}
	}

	public virtual void Home()
	{
		// Play sound
		SoundManager.PlayButtonOpen();
		
		// Hide
		Hide(false, () => { TransitionManager.Instance.FadeTransitionScene(Settings.MapSelectionScene); });
	}
	
	public void Leaderboard()
	{
		GameManager.Instance.ShowLeaderboard();
	}

	protected virtual void SetUIEnabled(bool enabled)
	{
		Canvas canvas = GameObject.FindObjectOfType<Canvas>();
		canvas.SetInteractable(enabled);
	}

	protected virtual void SetChildrenEnabled(GameObject go, bool enabled)
	{
		Transform tf = go.transform;

		for (int i = 0; i < tf.childCount; i++)
		{
			tf.GetChild(i).gameObject.SetActive(enabled);
		}
	}

	protected virtual void SetShowTouchAndOverlay(bool show)
	{

	}

	protected virtual void Hide(bool fadeOut, Action callback)
	{
		// Disable UI
		SetUIEnabled(false);
		
		// Hide touch and overlay
		SetShowTouchAndOverlay(false);

		// Delay
		gameObject.Play(DelayAction.Create(0.1f), () => {
			if (fadeOut)
			{
				// Fade-out overlay
				gameObject.Play(FadeAction.FadeOut(fadeDuration));
				
				// Fade-out background
				background.Play(FadeAction.RecursiveFadeOut(fadeDuration), () => {
					// Hide
					gameObject.Hide();

					// Enable UI
					SetUIEnabled(true);
					
					callback();
				});
			}
			else
			{
				// Enable UI
				SetUIEnabled(true);

				callback();
			}
		});
	}

	void OnManaChanged(int mana)
	{
		NotificationManager.RemoveManaEventHandler(OnManaChanged);

		if (mana > 0)
		{
			NoMoreManaPopupScript script = FindObjectOfType<NoMoreManaPopupScript>();

			if (script != null)
			{
				script.ForceClose();
			}
		}
	}
}
