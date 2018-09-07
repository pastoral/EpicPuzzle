using UnityEngine;

public enum ActionType
{
	Move,
	Scale,
	Rotate,
	Fade,
	Tint,
	Delay,
	Sequence
}

[System.Serializable]
public class ActionData
{
	/// <summary>
	/// The type of action.
	/// </summary>
	public ActionType actionType = ActionType.Move;
	
	public float f1;
	public bool b1;
	public bool b2;
	public Vector3 v31;
	public Color c1 = Color.white;
	
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

	public BaseAction GetAction()
	{
		if (actionType == ActionType.Move)
		{
			return MoveAction.Create(v31, b1, b2, duration, Ease.FromType(easeType), direction);
		}

		if (actionType == ActionType.Scale)
		{
			return ScaleAction.Create(v31, b1, duration, Ease.FromType(easeType), direction);
		}

		if (actionType == ActionType.Rotate)
		{
			return RotateAction.Create(f1, b1, duration, Ease.FromType(easeType), direction);
		}

		if (actionType == ActionType.Fade)
		{
			return FadeAction.Create(f1, b1, b2, duration, Ease.FromType(easeType), direction);
		}

		if (actionType == ActionType.Tint)
		{
			return TintAction.Create(c1.RGB(), b1, b2, duration, Ease.FromType(easeType), direction);
		}

		if (actionType == ActionType.Delay)
		{
			return DelayAction.Create(duration);
		}

		return null;
	}
}

public class CustomAction : MonoBehaviour
{
	public ActionData[] actions = new ActionData[1];

	void Start()
	{
		gameObject.Play(actions[0].GetAction());

		Destroy(this);
	}
}
