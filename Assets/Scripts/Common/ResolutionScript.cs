using UnityEngine;
using UnityEngine.UI;

public class ResolutionScript : MonoBehaviour
{
	public enum ResolutionPolicy
	{
		ExactFit,
		ShowAll,
		NoBorder,
		FixedWidth,
		FixedHeight,
	}

	/// <summary>
	/// The policy.
	/// </summary>
	public ResolutionPolicy policy;
	
	/// <summary>
	/// The desired width.
	/// </summary>
	public float width;
	
	/// <summary>
	/// The desired height.
	/// </summary>
	public float height;

	void OnEnable()
	{
		OnLayout();

		Destroy(this);
	}

	void OnLayout()
	{
		Sprite sprite = null;

		// Get sprite renderer
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

		if (spriteRenderer != null)
		{
			sprite = spriteRenderer.sprite;
		}
		else
		{
			// Get image
			Image image = GetComponent<Image>();

			if (image != null)
			{
				sprite = image.sprite;
			}
		}

		if (sprite == null) return;

		// Get sprite size
		Vector3 size = sprite.bounds.size;
		
		float scaleX = 1.0f;
		float scaleY = 1.0f;
		
		if (width < 0)
		{
			scaleX = Camera.main.GetWidth() / size.x;
		}
		else if (width > 0)
		{
			scaleX = width / size.x;
		}
		
		if (height < 0)
		{
			scaleY = Camera.main.GetHeight() / size.y;
		}
		else if (height > 0)
		{
			scaleY = height / size.y;
		}
		
		if (policy == ResolutionPolicy.ExactFit)
		{
			transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
		}
		else if (policy == ResolutionPolicy.ShowAll)
		{
			float scale = Mathf.Min(scaleX, scaleY);
			transform.localScale = new Vector3(scale, scale, 1.0f);
		}
		else if (policy == ResolutionPolicy.NoBorder)
		{
			float scale = Mathf.Max(scaleX, scaleY);
			transform.localScale = new Vector3(scale, scale, 1.0f);
		}
		else if (policy == ResolutionPolicy.FixedWidth)
		{
			transform.localScale = new Vector3(scaleX, scaleX, 1.0f);
		}
		else if (policy == ResolutionPolicy.FixedHeight)
		{
			transform.localScale = new Vector3(scaleY, scaleY, 1.0f);
		}
	}
	
	void OnUnlayout()
	{
		// Reset scale
		transform.localScale = Vector3.one;
	}
}
