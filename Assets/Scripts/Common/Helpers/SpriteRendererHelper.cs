using UnityEngine;

public static class SpriteRendererHelper
{
	public static float GetWidth(this SpriteRenderer renderer)
	{
		return renderer.sprite.GetWidth();
	}
	
	public static float GetHeight(this SpriteRenderer renderer)
	{
		return renderer.sprite.GetHeight();
	}

	public static float GetWidthInPixels(this SpriteRenderer renderer)
	{
		return renderer.sprite.GetWidthInPixels();
	}
	
	public static float GetHeightInPixels(this SpriteRenderer renderer)
	{
		return renderer.sprite.GetHeightInPixels();
	}

	public static void SetRGB(this SpriteRenderer renderer, Vector3 rgb)
	{
		Color color = renderer.color;
		color.r = rgb.x;
		color.g = rgb.y;
		color.b = rgb.z;
		
		renderer.color = color;
	}

	public static void SetAlpha(this SpriteRenderer renderer, float alpha)
	{
		Color color = renderer.color;
		color.a = alpha;

		renderer.color = color;
	}

	public static bool IsShowing(this SpriteRenderer renderer)
	{
		return renderer.gameObject.activeSelf;
	}
}
