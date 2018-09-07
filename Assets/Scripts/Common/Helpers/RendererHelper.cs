using UnityEngine;

public static class RendererHelper
{
	public static void SetTexture(this Renderer renderer, Texture texture)
	{
		Material material = Application.isPlaying ? renderer.material : renderer.sharedMaterial;

		if (material != null)
		{
			material.mainTexture = texture;
		}
	}

	public static void SetColor(this Renderer renderer, Color color)
	{
		Material material = Application.isPlaying ? renderer.material : renderer.sharedMaterial;
		
		if (material != null)
		{
			material.color = color;
		}
	}
}
