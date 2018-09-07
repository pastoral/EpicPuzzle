using UnityEngine;

public class TypeActionScript : MonoBehaviour
{
	/// <summary>
	/// The text.
	/// </summary>
	public string text;

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
		gameObject.Play(TypeAction.Create(text, duration, Ease.FromType(easeType), direction));
		
		// Self-destroy
		Destroy(this);
	}
}
