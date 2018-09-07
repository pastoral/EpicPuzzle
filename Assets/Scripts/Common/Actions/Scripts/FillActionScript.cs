using UnityEngine;

public class FillActionScript : MonoBehaviour
{
	/// <summary>
	/// The end amount.
	/// </summary>
	public float end = 1.0f;

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
		gameObject.Play(FillAction.Create(end, isRelative, duration, Ease.FromType(easeType), direction));
		
		// Self-destroy
		Destroy(this);
	}
}
