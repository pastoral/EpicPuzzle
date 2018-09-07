using UnityEngine;

public class MoveActionScript : MonoBehaviour
{
	/// <summary>
	/// The end position.
	/// </summary>
	public Vector3 end = Vector3.zero;

	/// <summary>
	/// Is relative.
	/// </summary>
	public bool isRelative = false;

	/// <summary>
	/// Is local.
	/// </summary>
	public bool isLocal = false;

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
		gameObject.Play(MoveAction.Create(end, isRelative, isLocal, duration, Ease.FromType(easeType), direction));
		
		// Self-destroy
		Destroy(this);
	}
}
