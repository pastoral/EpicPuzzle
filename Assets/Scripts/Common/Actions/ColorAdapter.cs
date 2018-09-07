using UnityEngine;
using UnityEngine.UI;

public abstract class ColorAdapter
{
	// Get RGB
	public abstract Vector3 GetRGB();
	
	// Set RGB
	public abstract void SetRGB(Vector3 rgb, bool isRecursive);
	
	// Get alpha
	public abstract float GetAlpha();
	
	// Set alpha
	public abstract void SetAlpha(float a, bool isRecursive);
	
	public static ColorAdapter Get(GameObject go)
	{
		// Get sprite renderer
		SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
		
		if (spriteRenderer != null)
		{
			return new SpriteRendererColor(spriteRenderer);
		}
		
		// Get image
		Image image = go.GetComponent<Image>();
		
		if (image != null)
		{
			return new ImageColor(image);
		}
		
		// Get text
		Text text = go.GetComponent<Text>();
		
		if (text != null)
		{
			return new TextColor(text);
		}
		
		return new DefaultColorAdapter(go);
	}
}

#region SpriteRenderer

public class SpriteRendererColor : ColorAdapter
{
	// The sprite renderer
	private SpriteRenderer _spriteRenderer;
	
	public SpriteRendererColor(SpriteRenderer spriteRenderer)
	{
		// Set sprite renderer
		_spriteRenderer = spriteRenderer;
	}
	
	public override Vector3 GetRGB()
	{
		return _spriteRenderer.color.RGB();
	}
	
	public override void SetRGB(Vector3 rgb, bool isRecursive)
	{
		_spriteRenderer.SetRGB(rgb);
		
		if (isRecursive)
		{
			_spriteRenderer.gameObject.SetRGBInChildren(rgb);
		}
	}
	
	public override float GetAlpha()
	{
		return _spriteRenderer.color.a;
	}
	
	public override void SetAlpha(float a, bool isRecursive)
	{
		_spriteRenderer.SetAlpha(a);
		
		if (isRecursive)
		{
			_spriteRenderer.gameObject.SetAlphaInChildren(a);
		}
	}
}

#endregion // SpriteRenderer

#region Image

public class ImageColor : ColorAdapter
{
	// The image
	private Image _image;
	
	public ImageColor(Image image)
	{
		// Set image
		_image = image;
	}
	
	public override Vector3 GetRGB()
	{
		return _image.color.RGB();
	}
	
	public override void SetRGB(Vector3 rgb, bool isRecursive)
	{
		_image.SetRGB(rgb);
		
		if (isRecursive)
		{
			_image.gameObject.SetRGBInChildren(rgb);
		}
	}
	
	public override float GetAlpha()
	{
		return _image.color.a;
	}
	
	public override void SetAlpha(float a, bool isRecursive)
	{
		_image.SetAlpha(a);
		
		if (isRecursive)
		{
			_image.gameObject.SetAlphaInChildren(a);
		}
	}
}

#endregion // Image

#region Text

public class TextColor : ColorAdapter
{
	// The text
	private Text _text;
	
	public TextColor(Text text)
	{
		// Set text
		_text = text;
	}
	
	public override Vector3 GetRGB()
	{
		return _text.color.RGB();
	}
	
	public override void SetRGB(Vector3 rgb, bool isRecursive)
	{
		_text.SetRGB(rgb);
	}
	
	public override float GetAlpha()
	{
		return _text.color.a;
	}
	
	public override void SetAlpha(float a, bool isRecursive)
	{
		_text.SetAlpha(a);
	}
}

#endregion // Text

#region Default

public class DefaultColorAdapter : ColorAdapter
{
	// The game object
	private GameObject _gameObject;

	// The rgb
	private Vector3 _rgb;

	// The alpha
	private float _a;

	public DefaultColorAdapter(GameObject gameObject)
	{
		// Set game object
		_gameObject = gameObject;

		Color color = Color.white;

		// Get sprite renderer
		SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

		if (spriteRenderer != null)
		{
			color = spriteRenderer.color;
		}
		else
		{
			// Get renderer
			Renderer renderer = gameObject.GetComponentInChildren<Renderer>();

			if (renderer != null)
			{
				if (renderer.material.HasProperty("_TintColor"))
				{
					color = renderer.material.GetColor("_TintColor");
				}
				else
				{
					color = renderer.material.color;
				}
			}
			else
			{
				// Get image
				Image image = gameObject.GetComponentInChildren<Image>();

				if (image != null)
				{
					color = image.color;
				}
				else
				{
					// Get text
					Text text = gameObject.GetComponentInChildren<Text>();

					if (text != null)
					{
						color = text.color;
					}
				}
			}
		}

		// Set rgb
		_rgb = color.RGB();

		// Set a
		_a = color.a;
	}

	public override Vector3 GetRGB()
	{
		return _rgb;
	}
	
	public override void SetRGB(Vector3 rgb, bool isRecursive)
	{
		_rgb = rgb;

		if (isRecursive)
		{
			_gameObject.SetRGBInChildren(rgb);
		}
	}
	
	public override float GetAlpha()
	{
		return _a;
	}
	
	public override void SetAlpha(float a, bool isRecursive)
	{
		_a = a;

		if (isRecursive)
		{
			_gameObject.SetAlphaInChildren(a);
		}
	}
}

#endregion // Default
