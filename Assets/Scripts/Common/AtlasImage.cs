using UnityEngine;
using UnityEngine.UI;

public class AtlasImage : MonoBehaviour
{
	/// <summary>
	/// The atlas.
	/// </summary>
	public Atlas atlas;
	
	/// <summary>
	/// The text.
	/// </summary>
	[SerializeField]
	private string _text;

	/// <summary>
	/// The color.
	/// </summary>
	[SerializeField]
	private Color _color = Color.white;

	/// <summary>
	/// The text anchor.
	/// </summary>
	[SerializeField]
	private TextAnchor _anchor = TextAnchor.MiddleCenter;

	/// <summary>
	/// The space width.
	/// </summary>
	[SerializeField]
	private float spaceWidth = 5f;

	/// <summary>
	/// The gap.
	/// </summary>
	[SerializeField]
	private float _gap = 0f;

	/// <summary>
	/// The offset x.
	/// </summary>
	[SerializeField]
	private float _offsetX;

	/// <summary>
	/// The offset y.
	/// </summary>
	[SerializeField]
	private float _offsetY;

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;

			OnValidate();
		}
	}

	void OnValidate()
	{
		if (atlas == null) return;

		int length = _text.Length;

		// Calculate number of characters
		int characterCount = 0;

		for (int i = 0; i < length; i++)
		{
			if (_text[i] != ' ')
			{
				characterCount++;
			}
		}
		
		// Get number of children
		int childCount = transform.childCount;
		
		if (characterCount > childCount)
		{
			// Enable all children
			for (int i = 0; i < childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(true);
			}
			
			// Add new children
			for (int i = characterCount - childCount; i > 0; i--)
			{
				GameObject character = new GameObject((characterCount - i + 1).ToString());
				character.transform.SetParent(transform);
				character.transform.localScale = Vector3.one;
				character.transform.localPosition = Vector3.zero;

				character.AddRectTransform();

				Image image = character.AddComponent<Image>();
				image.color = _color;
				image.raycastTarget = false;
			}
		}
		else
		{
			for (int i = 0; i < characterCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(true);
			}
			
			for (int i = characterCount; i < childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(false);
			}
		}

		// Check if empty
		if (length == 0)
		{
			gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
			return;
		}

		int index = 0;

		float width  = 0;
		float height = 0;
		Vector2 size;

		// Set sprite
		for (int i = 0; i < length; i++)
		{
			char c = _text[i];

			if (c != ' ')
			{
				Transform child = transform.GetChild(index++);

				Image image = child.GetComponent<Image>();
				image.sprite = atlas.Get(c);
				image.SetNativeSize();

				size = child.GetComponent<RectTransform>().sizeDelta;
				
				width += size.x + _gap;
				
				if (size.y > height)
				{
					height = size.y;
				}
			}
			else
			{
				width += spaceWidth - _gap;
			}
		}

		if (_text[length - 1] != ' ')
		{
			width -= _gap;
		}

		// Set size
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

		// Get bottom-left position
		Vector2 position = _anchor.GetAnchoredPosition(width, height, _offsetX, _offsetY);
		
		RectTransform rectTransform;
		float leftExtent;
		float bottomExtent;

		index = 0;

		// Set position
		for (int i = 0; i < length; i++)
		{
			if (_text[i] != ' ')
			{
				Transform child = transform.GetChild(index++);

				rectTransform = child.GetComponent<RectTransform>();
				size = rectTransform.sizeDelta;
				
				leftExtent   = size.x * 0.5f;
				bottomExtent = size.y * 0.5f;
				
				position.x += leftExtent;
				position.y += bottomExtent;
				
				rectTransform.anchoredPosition = position;
				
				position.x += size.x - leftExtent + _gap;
				position.y -= bottomExtent;
			}
			else
			{
				position.x += spaceWidth - _gap;
			}
		}
	}
}
