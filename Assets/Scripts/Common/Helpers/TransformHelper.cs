using UnityEngine;
using UnityEngine.UI;

public static class TransformHelper
{
	public static void SetPositionX(this Transform transform, float x)
	{
		Vector3 position = transform.position;
		position.x = x;
		transform.position = position;
	}

	public static void SetPositionY(this Transform transform, float y)
	{
		Vector3 position = transform.position;
		position.y = y;
		transform.position = position;
	}

	public static void SetLocalPositionX(this Transform transform, float x)
	{
		Vector3 position = transform.localPosition;
		position.x = x;
		transform.localPosition = position;
	}

	public static void SetLocalPositionY(this Transform transform, float y)
	{
		Vector3 position = transform.localPosition;
		position.y = y;
		transform.localPosition = position;
	}

	public static void TranslateX(this Transform transform, float deltaX)
	{
		Vector3 position = transform.position;
		position.x += deltaX;
		transform.position = position;
	}

	public static void TranslateY(this Transform transform, float deltaY)
	{
		Vector3 position = transform.position;
		position.y += deltaY;
		transform.position = position;
	}

	public static void SetScaleX(this Transform transform, float scaleX)
	{
		Vector3 scale = transform.localScale;
		scale.x = scaleX;
		transform.localScale = scale;
	}

	public static void SetScaleY(this Transform transform, float scaleY)
	{
		Vector3 scale = transform.localScale;
		scale.y = scaleY;
		transform.localScale = scale;
	}
	
	public static void SetScale(this Transform transform, float scale)
	{
		transform.localScale = new Vector3(scale, scale, scale);
	}
	
	public static void SetRotation(this Transform transform, float rotation)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
	}

	public static float GetWorldScaleXY(this Transform transform)
	{
		Vector3 worldScale = transform.lossyScale;
		return (worldScale.x + worldScale.y) * 0.5f;
	}
	
	public static void SetHorizontalFlip(this Transform transform, bool isFlip)
	{
		Vector3 scale = transform.localScale;

		if (isFlip)
		{
			if (scale.x > 0)
			{
				scale.x = -scale.x;
				transform.localScale = scale;
			}
		}
		else
		{
			if (scale.x < 0)
			{
				scale.x = -scale.x;
				transform.localScale = scale;
			}
		}
	}

	public static Transform FindInChildren(this Transform transform, string name)
	{
		// Get number of children
		int childCount = transform.childCount;
		
		// Breadth-first search
		for (int i = 0; i < childCount; i++)
		{
			// Get current child
			Transform child = transform.GetChild(i);
			
			if (child.name == name)
			{
				return child;
			}
		}
		
		// Depth-first search
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i).FindInChildren(name);
			
			if (child != null)
			{
				return child;
			}
		}
		
		return null;
	}
	
	public static T GetComponentInChildren<T>(this Transform transform, string name) where T : Component
	{
		Transform childTransform = transform.FindInChildren(name);
		
		if (childTransform != null)
		{
			return childTransform.GetComponent<T>();
		}
		
		return default(T);
	}

	public static void Show(this Transform transform)
	{
		transform.gameObject.SetActive(true);
	}
	
	public static void Hide(this Transform transform)
	{
		transform.gameObject.SetActive(false);
	}
	
	public static bool IsActive(this Transform transform)
	{
		return transform.gameObject.activeInHierarchy;
	}
	
	public static void DestroyChildren(this Transform transform)
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			GameObject.Destroy(transform.GetChild(i).gameObject);
		}
	}
	
	public static void DestroyImmediateChildren(this Transform transform)
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
		}
	}
	
	public static void SetTextInChildren(this Transform transform, string name, string text)
	{
		Text textComp = transform.GetComponentInChildren<Text>(name);
		
		if (textComp != null)
		{
			textComp.text = text;
		}
	}
	
	public static void SetButtonEnabledInChildren(this Transform transform, string name, bool enabled)
	{
		Button button = transform.GetComponentInChildren<Button>(name);
		
		if (button != null)
		{
			button.interactable = enabled;
		}
	}
}
