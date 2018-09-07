using UnityEngine;
using UnityEngine.UI;
using System;

public class TopFriendsScript : MonoBehaviour
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
	/// The deco.
	/// </summary>
	public GameObject deco;

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
	public float space = 0.0f;

	private static bool _isStopped;

	public void ShowTopFriends(int level)
	{
		// Show
		gameObject.Show();

		_isStopped = false;

		// Show loading
		if (loading != null)
		{
			loading.Show();
		}

		// Clear content
		contentTransform.DestroyChildren();

		// Hide background
		Image background = GetComponent<Image>();

		if (background != null)
		{
			background.enabled = false;
		}

		if (deco != null)
		{
			deco.SetActive(false);
		}

		// Get highscores
		FBHelper.GetHighscores((ids, names, scores, error) => {
			if (_isStopped)
			{
				return;
			}

			// Hide loading
			if (loading != null)
			{
				loading.Hide();
			}

			if (string.IsNullOrEmpty(error))
			{
				Vector2 position = new Vector2(space * 0.5f, 0);
				float step = rowPrefab.GetComponent<RectTransform>().sizeDelta.x + space;
				int length = ids.Length;
				int count = 0;

				// Add rows
				for (int i = 0; i < length; i++)
				{
					if (scores[i] >= level)
					{
						if (ids[i] == FBHelper.UserID) continue;

						GameObject row = rowPrefab.CreateUI(contentTransform, position, false);
						TopFriendsRowScript script = row.GetComponent<TopFriendsRowScript>();
						
						if (script != null)
						{
							script.Construct(ids[i], names[i]);
						}
						
						position.x += step;

						count++;
					}
					else
					{
						break;
					}
				}

				if (count > 0)
				{
					// Set content size
					Vector2 size = contentTransform.sizeDelta;
					size.x = count * step;
					contentTransform.sizeDelta = size;

					// Show background
					if (background != null)
					{
						background.enabled = true;
					}

					if (deco != null)
					{
						deco.SetActive(true);
					}
				}
				else
				{
					gameObject.Hide();
				}
			}
			else
			{
				//Log.Debug("Load top friends error: " + error);
			}
		});
	}

	public void Stop()
	{
		_isStopped = true;

		// Hide
		gameObject.Hide();
	}
}
