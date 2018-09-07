using UnityEngine;
using UnityEngine.UI;
using System;

public static class TextHelper
{
	public static void SetRGB(this Text text, Vector3 rgb)
	{
		Color color = text.color;
		color.r = rgb.x;
		color.g = rgb.y;
		color.b = rgb.z;

		text.color = color;
	}

	public static void SetAlpha(this Text text, float alpha)
	{
		Color color = text.color;
		color.a = alpha;

		text.color = color;
	}
}
