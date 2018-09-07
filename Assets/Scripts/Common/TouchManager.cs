using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TouchManager : Singleton<TouchManager>
{
	class TouchEventListener
	{
		public ITouchEventListener listener;
		public int priority;

		public TouchEventListener(ITouchEventListener listener, int priority)
		{
			this.listener = listener;
			this.priority = priority;
		}
	}

	// The listeners
	private List<TouchEventListener> _listeners = new List<TouchEventListener>(10);

	// The current listener
	private ITouchEventListener _listener;

	// Is enabled?
	private bool _isEnabled = true;

	public bool Enabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			_isEnabled = value;
		}
	}

	public void AddEventListener(ITouchEventListener listener, int priority = -1)
	{
//		Log.Debug("AddEventListener: " + listener.ToString());

		int count = _listeners.Count;

		if (count < 1 || priority < _listeners[count - 1].priority)
		{
			_listeners.Add(new TouchEventListener(listener, priority));
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				if (priority >= _listeners[i].priority)
				{
					_listeners.Insert(i, new TouchEventListener(listener, priority));
					break;
				}
			}
		}
	}

	public void RemoveEventListener(ITouchEventListener listener)
	{
//		Log.Debug("RemoveEventListener: " + listener.ToString());

		int count = _listeners.Count;

		for (int i = 0; i < count; i++)
		{
			if (listener == _listeners[i].listener)
			{
				_listeners.RemoveAt(i);
				return;
			}
		}
	}

	public static void SafeRemoveEventListener(ITouchEventListener listener)
	{
		if (_instance != null)
		{
			_instance.RemoveEventListener(listener);
		}
	}

	void Update()
	{
		if (!_isEnabled) return;

		if (_listeners.Count < 1) return;

		// Touch input
		if (Input.touchCount > 0)
		{
			// Get touch
			Touch touch = Input.GetTouch(0);
			
			if (touch.phase == TouchPhase.Began)
			{
				_listener = null;

				if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
				{
					Vector3 position = ScreenToWorldPoint(touch.position);

					int count = _listeners.Count;

					for (int i = 0; i < count; i++)
					{
						ITouchEventListener listener = _listeners[i].listener;

						if (listener.OnTouchPressed(position))
						{
							_listener = listener;
							break;
						}
					}
				}
			}
			else
			{
				if (_listener != null)
				{
					if (touch.phase == TouchPhase.Moved)
					{
						if (!_listener.OnTouchMoved(ScreenToWorldPoint(touch.position)))
						{
							_listener = null;
						}
					}
					else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						_listener.OnTouchReleased(ScreenToWorldPoint(touch.position));
					}
				}
			}
		}
		// Mouse input
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				_listener = null;
				
				if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
				{
					Vector3 position = ScreenToWorldPoint(Input.mousePosition);
					
					int count = _listeners.Count;
					
					for (int i = 0; i < count; i++)
					{
						ITouchEventListener listener = _listeners[i].listener;
						
						if (listener.OnTouchPressed(position))
						{
							_listener = listener;
							break;
						}
					}
				}
			}
			else
			{
				if (_listener != null)
				{
					if (Input.GetMouseButton(0))
					{
						if (!_listener.OnTouchMoved(ScreenToWorldPoint(Input.mousePosition)))
						{
							_listener = null;
						}
					}
					else if (Input.GetMouseButtonUp(0))
					{
						_listener.OnTouchReleased(ScreenToWorldPoint(Input.mousePosition));
					}
				}
			}
		}
	}

	Vector3 ScreenToWorldPoint(Vector3 screenPosition)
	{
		Vector3 position = Camera.main.ScreenToWorldPoint(screenPosition);
		position.z = 0;

		return position;
	}
}
