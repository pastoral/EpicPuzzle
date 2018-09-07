using UnityEngine;

public class DoubleFoothold : Foothold
{
	/// <summary>
	/// The icon.
	/// </summary>
	public GameObject icon;

	/// <summary>
	/// The explosion prefab.
	/// </summary>
	public GameObject explosionPrefab;

	private int _enterCount;
	private bool _isDouble = true;

	public override void Construct(int row, int column, bool undo)
	{
		// Set type
		_type = FootholdType.Double;

		if (undo)
		{
			// Set entered
			_enterCount = 2;
			_isDouble = false;

			// Hide icon
			icon.SetAlpha(0);
			icon.Hide();
		}

		base.Construct(row, column, undo);
	}
	
	public override void OnAnimalEnter(Animal animal)
	{
		_enterCount++;

		base.OnAnimalEnter(animal);
	}
	
	public override void OnAnimalUnenter(Animal animal)
	{
		_enterCount--;

		base.OnAnimalUnenter(animal);
	}
	
	public override void OnAnimalExit(Animal animal)
	{
		if (_enterCount == 1)
		{
			// Set animal to null
			_animal = null;

			GameManager.Instance.OnAnimalExitFoothold(this);

			_isDouble = false;

			var delay = DelayAction.Create(0.5f);
			var playExplosion = CallFuncAction.Create(() => {
				// Play sound
				SoundManager.Instance.PlaySound(SoundID.Explose);

				if (explosionPrefab != null)
				{
					GameObject explosion = explosionPrefab.Create(transform, icon.transform.position);
					explosion.AddSortingOrder(icon.GetSortingOrder() + 1);
				}
			});
			var fadeOut = FadeAction.FadeOut(0.25f);
			var hide = HideAction.Create();

			// Hide icon
			icon.Play(SequenceAction.Create(delay, playExplosion, fadeOut, hide));
		}
		else
		{
			base.OnAnimalExit(animal);
		}
	}
	
	public override void OnAnimalStartUnexit(Animal animal)
	{
		if (_enterCount == 1)
		{
			_isDouble = true;

			icon.StopAction(true);
			icon.Show();
			
			// Show icon
			icon.Play(FadeAction.FadeIn(0.25f));
		}

		base.OnAnimalStartUnexit(animal);
	}
	
	public override bool IsAtomic()
	{
		return _enterCount > 1;
	}

	public override int GetCount()
	{
		return _isDouble ? 2 : 1;
	}
}
