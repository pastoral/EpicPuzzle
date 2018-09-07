using UnityEngine;

public class FadeActionScript : MonoBehaviour
{
	/// <summary>
	/// The end alpha.
	/// </summary>
	public float end = 1.0f;

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
	
	/// <summary>
	/// The delay time.
	/// </summary>
	public float delay = 0;

	void Start()
	{
		var fade = FadeAction.Create(end, isRelative, isRecursive, duration, Ease.FromType(easeType), direction);

		if (delay > 0)
		{
			gameObject.Play(SequenceAction.Create(DelayAction.Create(delay), fade));
		}
		else
		{
			gameObject.Play(fade);
		}

		// Self-destroy
		Destroy(this);
	}
}
