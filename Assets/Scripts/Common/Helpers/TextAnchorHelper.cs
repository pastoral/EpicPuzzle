using UnityEngine;

public static class TextAnchorHelper
{
	public static Vector3 GetPosition(this TextAnchor anchor, float width, float height, float offsetX, float offsetY)
	{
		// Upper-Left
		if (anchor == TextAnchor.UpperLeft)
		{
			return new Vector3(offsetX, offsetY - height, 0);
		}

		// Upper-Center
		if (anchor == TextAnchor.UpperCenter)
		{
			return new Vector3(offsetX - width * 0.5f, offsetY - height, 0);
		}

		// Upper-Right
		if (anchor == TextAnchor.UpperRight)
		{
			return new Vector3(offsetX - width, offsetY - height, 0);
		}

		// Middle-Left
		if (anchor == TextAnchor.MiddleLeft)
		{
			return new Vector3(offsetX, offsetY - height * 0.5f, 0);
		}

		// Middle-Center
		if (anchor == TextAnchor.MiddleCenter)
		{
			return new Vector3(offsetX - width * 0.5f, offsetY - height * 0.5f, 0);
		}

		// Middle-Right
		if (anchor == TextAnchor.MiddleRight)
		{
			return new Vector3(offsetX - width, offsetY - height * 0.5f, 0);
		}

		// Lower-Left
		if (anchor == TextAnchor.LowerLeft)
		{
			return new Vector3(offsetX, offsetY, 0);
		}

		// Lower-Center
		if (anchor == TextAnchor.LowerCenter)
		{
			return new Vector3(offsetX - width * 0.5f, offsetY, 0);
		}

		// Lower-Right
		if (anchor == TextAnchor.LowerRight)
		{
			return new Vector3(offsetX - width, offsetY, 0);
		}

		return new Vector3(offsetX, offsetY, 0);
	}

	public static Vector2 GetAnchoredPosition(this TextAnchor anchor, float width, float height, float offsetX, float offsetY)
	{
		// Upper-Left
		if (anchor == TextAnchor.UpperLeft)
		{
			return new Vector2(offsetX, offsetY - height);
		}
		
		// Upper-Center
		if (anchor == TextAnchor.UpperCenter)
		{
			return new Vector2(offsetX - width * 0.5f, offsetY - height);
		}
		
		// Upper-Right
		if (anchor == TextAnchor.UpperRight)
		{
			return new Vector2(offsetX - width, offsetY - height);
		}
		
		// Middle-Left
		if (anchor == TextAnchor.MiddleLeft)
		{
			return new Vector2(offsetX, offsetY - height * 0.5f);
		}
		
		// Middle-Center
		if (anchor == TextAnchor.MiddleCenter)
		{
			return new Vector2(offsetX - width * 0.5f, offsetY - height * 0.5f);
		}
		
		// Middle-Right
		if (anchor == TextAnchor.MiddleRight)
		{
			return new Vector2(offsetX - width, offsetY - height * 0.5f);
		}
		
		// Lower-Left
		if (anchor == TextAnchor.LowerLeft)
		{
			return new Vector2(offsetX, offsetY);
		}
		
		// Lower-Center
		if (anchor == TextAnchor.LowerCenter)
		{
			return new Vector2(offsetX - width * 0.5f, offsetY);
		}
		
		// Lower-Right
		if (anchor == TextAnchor.LowerRight)
		{
			return new Vector2(offsetX - width, offsetY);
		}
		
		return new Vector2(offsetX, offsetY);
	}
}
