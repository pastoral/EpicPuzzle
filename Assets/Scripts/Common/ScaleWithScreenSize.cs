using UnityEngine;

public class ScaleWithScreenSize : MonoBehaviour
{
	public Vector2 referenceResolution = new Vector2(1280, 1920);

	[Range(0.0f, 1.0f)]
	public float match = 0;

	void Start()
	{
//		Vector3 scale = transform.localScale;
//		Debug.Log("Scale ...");
//		float scaleX = Camera.main.GetWidth()  / referenceResolution.x * 100;
//		float scaleY = Camera.main.GetHeight() / referenceResolution.y * 100;
//
//		scale.x = scale.y = (1.0f - match) * scaleX + match * scaleY;
//
//		transform.localScale = scale;
//
//
//		// Self-destroy
//		Destroy(this);
	}
}
