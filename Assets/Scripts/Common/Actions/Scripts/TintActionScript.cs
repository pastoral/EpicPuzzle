using UnityEngine;

public class TintActionScript : MonoBehaviour
{
	/// <summary>
	/// The end RGB.
	/// </summary>
	public Color end = Color.white;

	/// <summary>
	/// Is relative.
	/// </summary>
	public bool isRelative = false;
	
	/// <summary>
	/// Is recursive?
	/// </summary>
	public bool isRecursive = false;

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
		gameObject.Play(TintAction.Create(end.RGB(), isRelative, isRecursive, duration, Ease.FromType(easeType), direction));
		
		// Self-destroy
		Destroy(this);
	}
}
