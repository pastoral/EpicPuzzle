using UnityEngine;

public class ResizeActionScript : MonoBehaviour
{
	/// <summary>
	/// The end size.
	/// </summary>
	public Vector2 end = Vector2.one;

	/// <summary>
	/// Is relative.
	/// </summary>
	public bool isRelative = false;

	/// <summary>
	/// The duration.
	/// </summary>
	public float duration = 1.0f;

	/// <summary>
	/// The type of ease.
	/// </summary>
	public EaseType easeType = EaseType.Linear;

	/// <summary>
	/// The direction.
	/// </summary>
	public LerpDirection direction = LerpDirection.Forward;

	void Start()
	{
		gameObject.Play(ResizeAction.Create(end, isRelative, duration, Ease.FromType(easeType), direction));
		
		// Self-destroy
		Destroy(this);
	}
}
