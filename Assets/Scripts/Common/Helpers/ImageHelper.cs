using UnityEngine;
using UnityEngine.UI;

public static class ImageHelper
{
	public static void SetRGB(this Image image, Vector3 rgb)
	{
		Color color = image.color;
		color.r = rgb.x;
		color.g = rgb.y;
		color.b = rgb.z;
		
		image.color = color;
	}
	
	public static void SetAlpha(this Image image, float alpha)
	{
		Color color = image.color;
		color.a = alpha;
		
		image.color = color;
	}
}
