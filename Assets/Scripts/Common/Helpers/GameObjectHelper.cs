using UnityEngine;
using UnityEngine.UI;

public static class GameObjectHelper
{
	private static readonly Vector3 defaultScale = Vector3.one;

	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();

		if (component == null)
		{
			component = gameObject.AddComponent<T>();
		}

		return component;
	}

	public static GameObject FindChild(this GameObject go, string name)
	{
		Transform transform = go.transform.Find(name);
		
		return (transform != null) ? transform.gameObject : null;
	}
	
	public static void DestroyChild(this GameObject go, string name)
	{
		Transform transform = go.transform.Find(name);
		
		if (transform != null)
		{
			GameObject.Destroy(transform.gameObject);
		}
	}

	public static GameObject FindInChildren(this GameObject go, string name)
	{
		Transform transform = go.transform.FindInChildren(name);
		
		return (transform != null) ? transform.gameObject : null;
	}
	
	public static T GetComponentInChildren<T>(this GameObject go, string name) where T : Component
	{
		return go.transform.GetComponentInChildren<T>(name);
	}

	public static void SetSprite(this GameObject go, Sprite sprite)
	{
		SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
		
		if (renderer != null)
		{
			renderer.sprite = sprite;
		}
	}

	public static void SetSpriteColor(this GameObject go, Color color)
	{
		SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
		
		if (renderer != null)
		{
			renderer.color = color;
		}
	}

	public static void SetText(this GameObject go, string text)
	{
		Text textComp = go.GetComponent<Text>();
		
		if (textComp != null)
		{
			textComp.text = text;
		}
	}

	public static void SetTextInChildren(this GameObject go, string name, string text)
	{
		GameObject child = go.FindInChildren(name);
		
		if (child != null)
		{
			Text textComp = child.GetComponent<Text>();
			
			if (textComp != null)
			{
				textComp.text = text;
			}
			else
			{
				//Log.Debug("{0} have not Text component!", name);
			}
		}
		else
		{
			//Log.Debug("Not found {0}!", name);
		}
	}
	
	public static void SetTextMeshInChildren(this GameObject go, string name, string text)
	{
		GameObject child = go.FindInChildren(name);
		
		if (child != null)
		{
			TextMesh textMesh = child.GetComponent<TextMesh>();
			
			if (textMesh != null)
			{
				textMesh.text = text;
			}
			else
			{
				//Log.Debug("{0} have not TextMesh component!", name);
			}
		}
		else
		{
			//Log.Debug("Not found {0}!", name);
		}
	}

	public static void Show(this GameObject go)
	{
		go.SetActive(true);
	}
	
	public static void Hide(this GameObject go)
	{
		go.SetActive(false);
	}

	public static void AddRectTransform(this GameObject go)
	{
		RectTransform rectTransform = go.AddComponent<RectTransform>();
		rectTransform.anchoredPosition = Vector2.zero;
		rectTransform.sizeDelta = Vector2.zero;
	}

	public static void SetColor(this GameObject go, Color color, bool isRecursive = false)
	{
		// Get sprite renderer
		SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
		
		if (spriteRenderer != null)
		{
			spriteRenderer.color = color;
		}
		else
		{
			// Get image
			Image image = go.GetComponent<Image>();
			
			if (image != null)
			{
				image.color = color;
			}
			else
			{
				// Get text
				Text text = go.GetComponent<Text>();
				
				if (text != null)
				{
					text.color = color;
				}
			}
		}
		
		if (isRecursive)
		{
			Transform transform = go.transform;
			
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetColor(color, true);
			}
		}
	}

	public static void SetRGB(this GameObject go, Vector3 rgb, bool isRecursive = false)
	{
		// Get sprite renderer
		SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
		
		if (spriteRenderer != null)
		{
			spriteRenderer.SetRGB(rgb);
		}
		else
		{
			// Get image
			Image image = go.GetComponent<Image>();
			
			if (image != null)
			{
				image.SetRGB(rgb);
			}
			else
			{
				// Get text
				Text text = go.GetComponent<Text>();
				
				if (text != null)
				{
					text.SetRGB(rgb);
				}
			}
		}
		
		if (isRecursive)
		{
			Transform transform = go.transform;
			
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetRGB(rgb, true);
			}
		}
	}
	
	public static void SetAlpha(this GameObject go, float a, bool isRecursive = false)
	{
		// Get sprite renderer
		SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
		
		if (spriteRenderer != null)
		{
			spriteRenderer.SetAlpha(a);
		}
		else
		{
			// Get image
			Image image = go.GetComponent<Image>();
			
			if (image != null)
			{
				image.SetAlpha(a);
			}
			else
			{
				// Get text
				Text text = go.GetComponent<Text>();
				
				if (text != null)
				{
					text.SetAlpha(a);
				}
			}
		}
		
		if (isRecursive)
		{
			Transform transform = go.transform;
			
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetAlpha(a, true);
			}
		}
	}
	
	public static Color GetColorInChildren(this GameObject go)
	{
		Transform transform = go.transform;
		int childCount = transform.childCount;

		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			
			if (!gameObject.activeSelf) continue;

			// Get sprite renderer
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			
			if (spriteRenderer != null)
			{
				return spriteRenderer.color;
			}

			// Get image
			Image image = gameObject.GetComponent<Image>();
			
			if (image != null)
			{
				return image.color;
			}

			// Get text
			Text text = gameObject.GetComponent<Text>();
			
			if (text != null)
			{
				return text.color;
			}
		}

		return default(Color);
	}

	public static void SetRGBInChildren(this GameObject go, Vector3 rgb)
	{
		Transform transform = go.transform;
		
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;

			if (!gameObject.activeSelf) continue;

			// Get sprite renderer
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			
			if (spriteRenderer != null)
			{
				spriteRenderer.SetRGB(rgb);
			}
			else
			{
				// Get image
				Image image = gameObject.GetComponent<Image>();
				
				if (image != null)
				{
					image.SetRGB(rgb);
				}
				else
				{
					// Get text
					Text text = gameObject.GetComponent<Text>();
					
					if (text != null)
					{
						text.SetRGB(rgb);
					}
				}
			}
			
			gameObject.SetRGBInChildren(rgb);
		}
	}
	
	public static void SetAlphaInChildren(this GameObject go, float a)
	{
		Transform transform = go.transform;
		
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			
			if (!gameObject.activeSelf) continue;

			// Get sprite renderer
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			
			if (spriteRenderer != null)
			{
				spriteRenderer.SetAlpha(a);
			}
			else
			{
				// Get image
				Image image = gameObject.GetComponent<Image>();
				
				if (image != null)
				{
					image.SetAlpha(a);
				}
				else
				{
					// Get text
					Text text = gameObject.GetComponent<Text>();
					
					if (text != null)
					{
						text.SetAlpha(a);
					}
				}
			}
			
			gameObject.SetAlphaInChildren(a);
		}
	}

	public static int GetSortingOrder(this GameObject go)
	{
		SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
		
		if (spriteRenderer != null)
		{
			return spriteRenderer.sortingOrder;
		}

		Transform transform = go.transform;
		
		for (int i = 0; i < transform.childCount; i++)
		{
			spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
			
			if (spriteRenderer != null)
			{
				return spriteRenderer.sortingOrder;
			}
		}

		return 0;
	}

	/// <summary>
	/// Adds the sorting order (recursively).
	/// </summary>
	public static void AddSortingOrder(this GameObject go, int sortingOrder)
	{
		SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
		
		if (spriteRenderer != null)
		{
			spriteRenderer.sortingOrder += sortingOrder;
		}

		Transform transform = go.transform;
		
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.AddSortingOrder(sortingOrder);
		}
	}

	public static void SetImageRGB(this GameObject go, Vector3 rgb, bool isRecursive = false)
	{
		// Get image
		Image image = go.GetComponent<Image>();
		
		if (image != null)
		{
			image.SetRGB(rgb);
		}
		
		if (isRecursive)
		{
			Transform transform = go.transform;
			
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetImageRGB(rgb, true);
			}
		}
	}
	
	public static void ResetButtonScale(this GameObject go, bool isRecursive = false)
	{
		// Get button
		Button button = go.GetComponent<Button>();
		
		if (button != null)
		{
			button.transform.localScale = defaultScale;
		}
		
		if (isRecursive)
		{
			Transform transform = go.transform;
			
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.ResetButtonScale(true);
			}
		}
	}

	public static GameObject Create(this GameObject prefab, Transform parent, Vector3 position)
	{
		GameObject gameObject = GameObject.Instantiate(prefab) as GameObject;
		gameObject.transform.SetParent(parent);
		gameObject.transform.localScale = defaultScale;
		gameObject.transform.position = position;

		return gameObject;
	}

	public static GameObject CreateUI(this GameObject prefab, Transform parent, Vector2? anchoredPosition = null, bool resetSize = true)
	{
		GameObject gameObject = GameObject.Instantiate(prefab) as GameObject;
		gameObject.transform.SetParent(parent);
		gameObject.transform.localScale = defaultScale;

		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

		if (rectTransform != null)
		{
			rectTransform.anchoredPosition = anchoredPosition ?? Vector2.zero;

			if (resetSize)
			{
				rectTransform.sizeDelta = Vector2.zero;
			}
		}
		
		return gameObject;
	}
}
