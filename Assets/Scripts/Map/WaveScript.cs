using UnityEngine;

public class WaveScript : MonoBehaviour
{
	/// <summary>
	/// The start scale.
	/// </summary>
	public Vector3 scale = new Vector3(0.8f, 0.56f, 1.0f);

	public float minScale = 1.9f;
	public float maxScale = 2.1f;

	public float scaleDuration = 1.0f;
	public float fadeDuration  = 1.0f;

	void Start()
	{
		// Set scale
		transform.localScale = scale;

		gameObject.Play(ParallelAction.ParallelAll(ScaleAction.ScaleTo(scale * Random.Range(minScale, maxScale), scaleDuration), FadeAction.FadeOut(fadeDuration)), () => {
			GameObject.Destroy(gameObject);
		});
	}
}
