using UnityEngine;

public class ResolutionHelper
{
	private static float designWidth  = 12.80f;
	private static float designHeight = 19.20f;

	private static float contentWidth  = 10.00f;
	private static float contentHeight = 15.00f;

	public static float GetScale()
	{
		float screenWidth  = Camera.main.GetWidth();
		float screenHeight = Camera.main.GetHeight();

		float scaleX = screenWidth  / designWidth;
		float scaleY = screenHeight / designHeight;
		
		// No border
		float scale  = Mathf.Max(scaleX, scaleY);
		
		float width  = designWidth  * scale;
		float height = designHeight * scale;
		
		/*
		 * +--------+---------------+--------+
		 * ||||||||||				||||||||||
		 * ||||||||||	  screen	||||||||||
		 * ||||||||||				||||||||||
		 * +--------+---------------+--------+
		 */
		if (width > screenWidth)
		{
			float minWidth = contentWidth * scale;
			
			if (screenWidth < minWidth)
			{
				scale *= screenWidth / minWidth;

				return scale;
			}
		}
		/*
		 * +----------------+
		 * ||||||||||||||||||
		 * ||||||||||||||||||
		 * +----------------+
		 * |				|
		 * |	 screen		|
		 * |				|
		 * |				|
		 * +----------------+
		 * ||||||||||||||||||
		 * ||||||||||||||||||
		 * +----------------+
		 */
		if (height > screenHeight)
		{
			float minHeight = contentHeight * scale;
			
			if (screenHeight < minHeight)
			{
				scale *= screenHeight / minHeight;

				return scale;
			}
		}

		return scale;
	}
}
