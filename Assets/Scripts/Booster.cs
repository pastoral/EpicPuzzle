using UnityEngine;
using System.Collections;

public class Booster : MonoBehaviour, ITouchEventListener
{
	private static readonly float touchExtra = 0.1f;

	/// <summary>
	/// The type of booster.
	/// </summary>
	[SerializeField]
	private BoosterType _type;

	/// <summary>
	/// The icon.
	/// </summary>
	[SerializeField]
	private Sprite _icon;

	/// <summary>
	/// The frame.
	/// </summary>
	[SerializeField]
	private GameObject _frame;
	
	/// <summary>
	/// The number.
	/// </summary>
	[SerializeField]
	private AtlasNumber _number;

	/// <summary>
	/// The lock.
	/// </summary>
	[SerializeField]
	private GameObject _lock;

	[SerializeField]
	private GameObject _buyMore;

	// The quantity
	private int _quantity = -1;

	// The listener
	private IBoosterEventListener _listener = NullBoosterEventListener.Instance;

	// The touch rect
	private Rect _touchRect;
	
	// Interactable or not
	private bool _isInteractable = true;

	// The zoom helper
	private LerpFloatHelper _zoomHelper = new LerpFloatHelper(1.0f);

	// Is touch inside?
	private bool _isTouchInside;

	// Get type
	public BoosterType Type
	{
		get
		{
			return _type;
		}
	}

	// Get/Set quantity
	public int Quantity
	{
		get
		{
			return _quantity;
		}
		set
		{
			if (_quantity != value)
			{
				// Check if locked
				if (value < 0)
				{
					if (_quantity >= 0)
					{
						// Hide frame
						_frame.Hide();

						// Show lock
						_lock.Show();

						_buyMore.Show();
					}
				}
				else
				{
					if (_quantity < 0)
					{
						// Hide lock
						_lock.Hide();

						// Set icon
						gameObject.SetSprite(_icon);

						// Show frame
						_frame.Show();

						_buyMore.Hide();
					}

					// Set number
					_number.Number = value;
					_buyMore.Show();
				}

				_quantity = value;
			}
		}
	}
	
	// Enable/Disable interaction
	public bool Interactable
	{
		get
		{
			return _isInteractable;
		}
		set
		{
			_isInteractable = value;
		}
	}

	public bool Locked
	{
		get
		{
			return _quantity < 0;
		}
	}

	public bool Empty
	{
		get
		{
			return _quantity < 1;
		}
	}

	// Get/Set event listener
	public IBoosterEventListener EventListener
	{
		get
		{
			return (_listener != NullBoosterEventListener.Instance) ? _listener : null;
		}
		set
		{
			_listener = (value != null) ? value : NullBoosterEventListener.Instance;
		}
	}

	void Start()
	{
		TouchManager.Instance.AddEventListener(this, 1);

		_buyMore.GetComponent<NonCanvasButton>().touchPressEvent += OnBuyMore;
	}

	void OnDestroy()
	{
		TouchManager.SafeRemoveEventListener(this);
	}

	public void UpdateBoundary()
	{
		float scale = transform.GetWorldScaleXY();
		
		// Get position
		Vector3 position = transform.position;
		
		// Get sprite renderer
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		
		// Get width
		float width = spriteRenderer.GetWidth();
		
		// Get height
		float height = spriteRenderer.GetHeight();
		
		// Get pivot
		Vector2 pivot = spriteRenderer.sprite.pivot / spriteRenderer.sprite.pixelsPerUnit;
		
		float left   = position.x - (pivot.x + touchExtra) * scale;
		float bottom = position.y - (pivot.y + touchExtra) * scale;
		
		_touchRect = new Rect(left, bottom, (width + touchExtra * 2) * scale, (height + touchExtra * 2) * scale);
	}

	public void OnSelected()
	{
		bool isFinished = _zoomHelper.IsFinished();

		_zoomHelper.Construct(transform.localScale.x, 1.2f, 0.1f);

		if (isFinished)
		{
			StartCoroutine(UpdateZoom());
		}
	}

	public void OnUnselected()
	{
		bool isFinished = _zoomHelper.IsFinished();

		_zoomHelper.Construct(transform.localScale.x, 1.0f, 0.1f);

		if (isFinished)
		{
			StartCoroutine(UpdateZoom());
		}
	}

	void OnBuyMore()
	{
		_listener.OnBoosterBuyMore(this);
		Debug.Log("Buy more ...");
	}

//	void OnValidate()
//	{
//		if (_number != null)
//		{
//			_number.Number = _quantity;
//		}
//	}
	
	bool Contains(Vector3 point)
	{
		return _touchRect.Contains(point);
	}

	public bool OnTouchPressed(Vector3 position)
	{
		if (!_isInteractable) return false;

		_isTouchInside = Contains(position);

		if (_isTouchInside)
		{
			if (_quantity > 0)
			{
				OnSelected();
			}
			else if (_quantity < 0)
			{
				_lock.StopAction();
				_lock.transform.SetRotation(0);

				_lock.Play(SequenceAction.Create(RotateAction.RotateBy(45.0f, 0.1f), RotateAction.RotateBy(-90.0f, 0.2f), RotateAction.RotateBy(45.0f, 0.1f)));
			}

			_listener.OnBoosterPressed(this);

			return true;
		}

		return false;
	}
	
	public bool OnTouchMoved(Vector3 position)
	{
		if (_quantity < 1) return false;

		if (_isTouchInside)
		{
			if (!Contains(position))
			{
				_isTouchInside = false;

				OnUnselected();

				if (_type.IsInstant())
				{
					return false;
				}

				_listener.OnBoosterTouchBegan(this, position);
			}
		}
		else
		{
			_listener.OnBoosterTouchMoved(position);
		}

		return true;
	}
	
	public void OnTouchReleased(Vector3 position)
	{
		if (_isTouchInside)
		{
			_isTouchInside = false;

			OnUnselected();

			_listener.OnBoosterReleased(this);
		}
		else
		{
			_listener.OnBoosterTouchEnded(this, position);
		}
	}

	IEnumerator UpdateZoom()
	{
		while (!_zoomHelper.IsFinished())
		{
			transform.SetScale(_zoomHelper.Update(Time.deltaTime));
			yield return null;
		}
	}

	void OnDrawGizmos()
	{
		GizmosHelper.DrawRect(_touchRect, Color.red);
	}
}
