using UnityEngine;
using UnityEngine.UI;
using System;

public class LeaderboardScript : MonoBehaviour
{
	/// <summary>
	/// The row prefab.
	/// </summary>
	public GameObject rowPrefab;

	/// <summary>
	/// The content transform.
	/// </summary>
	public RectTransform contentTransform;

	/// <summary>
	/// The loading.
	/// </summary>
	public GameObject loading;

	/// <summary>
	/// The row count.
	/// </summary>
	public int rowCount = 4;

	/// <summary>
	/// The space.
	/// </summary>
	public float space = 20.0f;
	
	// The callback
	private Action _callback;

	public void Show(Action callback = null)
	{
		// Set callback
		_callback = callback;

		// Disable interaction
		SetInteractable(false);

		// Show loading
		if (loading != null)
		{
			loading.Show();
		}

		// Clear content
		contentTransform.DestroyChildren();

		// Get highscores
		FBHelper.GetHighscores((ids, names, scores, error) => {
			// Hide loading
			if (loading != null)
			{
				loading.Hide();
			}

			if (string.IsNullOrEmpty(error))
			{
				// Show popup
				UIHelper.ShowPopup(gameObject, () => {
					Vector2 position = new Vector2(0, -space * 0.5f);
					float step = rowPrefab.GetComponent<RectTransform>().sizeDelta.y + space;
					int count = ids.Length;

					// Add rows
					for (int i = 0; i < count; i++)
					{
						GameObject row = rowPrefab.CreateUI(contentTransform, position, false);
						LeaderboardRowScript script = row.GetComponent<LeaderboardRowScript>();

						if (script != null)
						{
							script.Construct(ids[i], names[i], scores[i]);
						}

						position.y -= step;
					}

					// Set content size
					Vector2 size = contentTransform.sizeDelta;
					size.y = count * step;
					contentTransform.sizeDelta = size;

					// Enable interaction
					SetInteractable(true);
				});
			}
			else
			{
				// Enable interaction
				SetInteractable(true);

				Manager.Instance.ShowMessage(Settings.LeaderboardFailed);
			}
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
