using UnityEngine;

public class AtlasNumber : MonoBehaviour
{
	private static readonly Vector3 defaultScale = Vector3.one;

	/// <summary>
	/// The atlas.
	/// </summary>
	public DigitAtlas atlas;
	
	/// <summary>
	/// The current number.
	/// </summary>
	[SerializeField]
	private int _number;

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
	/// The gap.
	/// </summary>
	[SerializeField]
	private float _gap = 0.0f;

	/// <summary>
	/// The sorting order.
	/// </summary>
	[SerializeField]
	private int _orderInLayer;

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

	public int Number
	{
		get
		{
			return _number;
		}
		set
		{
			_number = value;

			OnValidate();
		}
	}

	void OnValidate()
	{
		int number = _number > 0 ? _number : 0;
		
		// Calculate number of digits
		int digitCount = 0;
		
		while (number >= 10)
		{
			digitCount++;
			
			number = (int)(number / 10);
		}
		
		digitCount++;
		
		// Get number of children
		int childCount = transform.childCount;
		
		if (digitCount > childCount)
		{
			// Enable all children
			for (int i = 0; i < childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(true);
			}
			
			// Add new children
			for (int i = digitCount - childCount; i > 0; i--)
			{
				GameObject digit = new GameObject((digitCount - i + 1).ToString());
				digit.transform.SetParent(transform);
				digit.transform.localScale = defaultScale;

				SpriteRenderer spriteRenderer = digit.AddComponent<SpriteRenderer>();
				spriteRenderer.color = _color;
				spriteRenderer.sortingOrder = _orderInLayer;
			}
		}
		else
		{
			for (int i = 0; i < digitCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(true);
			}
			
			for (int i = digitCount; i < childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		
		number = _number > 0 ? _number : 0;
		
		// Set sprite
		for (int i = digitCount - 1; i >= 0; i--)
		{
			transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = atlas.GetSprite(number % 10);
			
			number = (int)(number / 10);
		}
		
		// Update position
		UpdatePosition();
	}

	void UpdatePosition()
	{
		int childCount = transform.childCount;

		// Calculate size
		float width  = 0;
		float height = 0;
		
		Vector2 size;

		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);

			if (child.gameObject.activeSelf)
			{
				size = child.GetComponent<SpriteRenderer>().sprite.rect.size * 0.01f;

				width += size.x + _gap;

				if (size.y > height)
				{
					height = size.y;
				}
			}
			else
			{
				break;
			}
		}

		width -= _gap;

		Vector3 position = _anchor.GetPosition(width, height, _offsetX, _offsetY);

		Sprite sprite;
		float leftExtent;
		float bottomExtent;

		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			
			if (child.gameObject.activeSelf)
			{
				sprite = child.GetComponent<SpriteRenderer>().sprite;
				leftExtent = sprite.pivot.x / sprite.pixelsPerUnit;
				bottomExtent = sprite.pivot.y / sprite.pixelsPerUnit;

				position.x += leftExtent;
				position.y += bottomExtent;

				child.transform.localPosition = position;

				position.x += sprite.bounds.size.x - leftExtent + _gap;
				position.y -= bottomExtent;
			}
			else
			{
				break;
			}
		}
	}
}
