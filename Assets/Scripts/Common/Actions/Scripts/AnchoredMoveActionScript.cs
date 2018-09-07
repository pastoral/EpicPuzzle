using UnityEngine;

public class AnchoredMoveActionScript : MonoBehaviour
{
	/// <summary>
	/// The end position.
	/// </summary>
	public Vector2 end = Vector2.zero;

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

	/// <summary>
	/// The delay time.
	/// </summary>
	public float delay = 0;

	public SoundID sound = SoundID.Woosh;

	void Start()
	{
		var move = AnchoredMoveAction.Create(end, isRelative, duration, Ease.FromType(easeType), direction);
		var callfunc = CallFuncAction.Create(() => {
			if (sound != SoundID.Count)
			{
				//SoundManager.Instance.PlaySound(sound);
			}
		});
		var action = SequenceAction.Create(DelayAction.Create(delay), move, callfunc);

		gameObject.Play(SequenceAction.Create(action));

		// Self-destroy
		Destroy(this);
	}
}
