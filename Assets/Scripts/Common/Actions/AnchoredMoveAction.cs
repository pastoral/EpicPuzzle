using UnityEngine;
using UnityEngine.UI;

public class AnchoredMoveAction : BaseAction
{
	// The helper
	private LerpVector2Helper _helper = new LerpVector2Helper();

	// Relative or absolute
	private bool _isRelative;

	// The rect transform
	private RectTransform _rectTransform;

	public AnchoredMoveAction(Vector2 end, bool isRelative, float duration, Easer easer, LerpDirection direction)
	{
		_helper.Construct(end, duration, easer, direction);

		_isRelative = isRelative;
	}

	public static AnchoredMoveAction Create(Vector2 end, bool isRelative, float duration, Easer easer = null, LerpDirection direction = LerpDirection.Forward)
	{
		return new AnchoredMoveAction(end, isRelative, duration, easer, direction);
	}

	public static AnchoredMoveAction MoveTo(Vector2 end, float duration, Easer easer = null, LerpDirection direction = LerpDirection.Forward)
	{
		return Create(end, false, duration, easer, direction);
	}
	
	public static AnchoredMoveAction MoveBy(Vector2 end, float duration, Easer easer = null, LerpDirection direction = LerpDirection.Forward)
	{
		return Create(end, true, duration, easer, direction);
	}

	public override void Play(GameObject target)
	{
		// Get rect transform
		_rectTransform = target.GetComponent<RectTransform>();

		if (_rectTransform != null)
		{
			// Set start
			_helper.Start = _rectTransform.anchoredPosition;

			if (_isRelative)
			{
				_helper.AddEndByStart();
			}

			_helper.Play();
		}
		else
		{
			//Debug.LogWarning("Rect Transform required!");

			_helper.Stop();
		}
	}
	
	public override void Reset()
	{
		_rectTransform.anchoredPosition = _helper.Start;
	}
	
	public override void Stop(bool forceEnd = false)
	{
		if (!_helper.IsFinished())
		{
			_helper.Stop();

			if (forceEnd)
			{
				_rectTransform.anchoredPosition = _helper.End;
			}
		}
	}
	
	public override bool IsFinished()
	{
		return _helper.IsFinished();
	}
	
	public override bool Update(float deltaTime)
	{
		if (!_helper.IsFinished())
		{
			_rectTransform.anchoredPosition = _helper.Update(deltaTime);
		}

		return _helper.IsFinished();
	}
}
