using UnityEngine;

public class RedirectFoothold : Foothold
{
	/// <summary>
	/// The effect prefab.
	/// </summary>
	public GameObject effectPrefab;

	/// <summary>
	/// The direction.
	/// </summary>
	public Direction direction = Direction.Left;

	// The animal old direction
	private Direction _oldDirection;

	public override Direction Direction
	{
		get
		{
			return direction;
		}
	}

	public override void Construct(int row, int column, bool undo)
	{
		base.Construct(row, column, undo);

		// Set type
		if (direction.IsLeft())
		{
			_type = FootholdType.RedirectLeft;
		}
		else if (direction.IsUp())
		{
			_type = FootholdType.RedirectUp;
		}
		else if (direction.IsRight())
		{
			_type = FootholdType.RedirectRight;
		}
		else if (direction.IsDown())
		{
			_type = FootholdType.RedirectDown;
		}

		if (undo)
		{
			_oldDirection = GameManager.Instance.PopDirection(row, column);
		}
	}
	
	public override void OnAnimalEnter(Animal animal)
	{
		base.OnAnimalEnter(animal);

		// Save direction
		_oldDirection = animal.Direction;
		// Change direction
		animal.Direction = direction;

		gameObject.Play(DelayAction.Create(0.5f), () => {
			if (effectPrefab != null)
			{
				// Play sound
				SoundManager.Instance.PlaySound(SoundID.ChangeDirection, SoundType.New);

				// Create effect
				Vector3 position = transform.position;
				position.y += 0.25f;

				effectPrefab.Create(transform.parent, position);
			}



			GameManager.Instance.OnAnimalDidFinishJump(animal);
		});
	}
	
	public override void OnAnimalUnenter(Animal animal)
	{
		base.OnAnimalUnenter(animal);

		// Restore direction
		animal.Direction = _oldDirection;
	}

	public override void OnAnimalExit(Animal animal)
	{
		base.OnAnimalExit(animal);

		GameManager.Instance.PushDirection(_row, _column, _oldDirection);
	}
}
