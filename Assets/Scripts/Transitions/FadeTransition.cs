using UnityEngine;
using System.Collections;

public class FadeTransition : TransitionDelegate
{
	/// <summary>
	/// The fade color.
	/// </summary>
	public Color color = Color.black;

	/// <summary>
	/// The fade duration.
	/// </summary>
	public float duration = 0.5f;

	public Shader GetShader()
	{
		return TransitionManager.Instance.GetShader("Transitions/Fade");
	}

	public Mesh GetMesh()
	{
		return null;
	}

	public Texture2D GetTexture()
	{
		return null;
	}

	public IEnumerator Play()
	{
		TransitionManager transitionManager = TransitionManager.Instance;

		// Set color
		transitionManager.Material.color = color;

		// Load level
		transitionManager.LoadLevelAsync();

		// Fade out
		yield return transitionManager.TickMaterialProgress(duration);

		//
		transitionManager.Material.mainTexture = TextureHelper.GetTransparentPixel();

		if (!transitionManager.IsLevelLoaded())
		{
			// Wait for level loaded
			yield return transitionManager.WaitForLevelToLoad();
		}

		// Fade in
		yield return transitionManager.TickMaterialProgress(duration, true);
	}
}
