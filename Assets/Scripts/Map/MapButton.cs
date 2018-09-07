using UnityEngine;

public class MapButton : MonoBehaviour
{
	/// <summary>
	/// The level number.
	/// </summary>
	[SerializeField]
	private AtlasNumber _number;

	/// <summary>
	/// The lock.
	/// </summary>
	[SerializeField]
	private GameObject _lock;

	// The sprite lock
	private Sprite _spriteLock;

	// The sprite unlock.
	private Sprite _spriteUnlock;

	// The map ID
	private int _map;

	// Is map unlocked?
	private bool _isUnlocked;

	public int Map
	{
		get
		{
			return _map;
		}
	}

	public bool Unlocked
	{
		get
		{
			return _isUnlocked;
		}
		set
		{
			_isUnlocked = value;

			// Set sprite
			SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
			spriteRenderer.sprite = _isUnlocked ? _spriteUnlock : _spriteLock;

			// Set map
			if (_isUnlocked)
			{
				_number.Number = _map;
			}

			// Show/Hide number
			_number.gameObject.SetActive(_isUnlocked);

			// Show/Hide lock
			_lock.SetActive(!_isUnlocked);
		}
	}

	public void Construct(Sprite spriteLock, Sprite spriteUnlock, int map, bool isUnlocked)
	{
		// Set sprite lock
		_spriteLock = spriteLock;

		// Set sprite unlock
		_spriteUnlock = spriteUnlock;

		// Set map ID
		_map = map;

		// Set unlocked
		_isUnlocked = isUnlocked;

		// Set sprite
		SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = isUnlocked ? spriteUnlock : spriteLock;

		// Show/Hide number
		_number.gameObject.SetActive(isUnlocked);
		
		// Show/Hide lock
		_lock.SetActive(!isUnlocked);

		// Check if unlocked
		if (isUnlocked)
		{
			// Set map
			_number.Number = map;
		}
	}

	public void OnSelected()
	{
		if (!_isUnlocked)
		{
			_lock.StopAction();
			_lock.transform.SetRotation(0);
			
			_lock.Play(SequenceAction.Create(RotateAction.RotateBy(45.0f, 0.1f), RotateAction.RotateBy(-90.0f, 0.2f), RotateAction.RotateBy(45.0f, 0.1f)));
		}
	}
}
