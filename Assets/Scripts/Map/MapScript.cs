using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class MapScript : MonoBehaviour
{
	/// <summary>
	/// The zones.
	/// </summary>
	public ZoneSetting[] zones = new ZoneSetting[3];

	/// <summary>
	/// The maps.
	/// </summary>
	public Sprite[] maps;

	/// <summary>
	/// The button prefab.
	/// </summary>
	public GameObject buttonPrefab;
	
	/// <summary>
	/// The unlock level prefab.
	/// </summary>
	public GameObject unlockLevelPrefab;

	/// <summary>
	/// The sprite lock.
	/// </summary>
	public Sprite spriteLock;

	/// <summary>
	/// The background.
	/// </summary>
	public Transform background;

	/// <summary>
	/// The points.
	/// </summary>
	public Transform points;
	
	/// <summary>
	/// The clouds.
	/// </summary>
	public Transform clouds;

	/// <summary>
	/// The animal.
	/// </summary>
	public Animal animal;

	/// <summary>
	/// The cloud.
	/// </summary>
	public GameObject cloud;

	public delegate void MapEventHandler(int map, bool unlocked);
	private event MapEventHandler _OnMapSelected;

	// Button radius
	private static readonly float ButtonRadius = 0.80f;

	// The minimum delta movement
	private static readonly float MinDeltaMove = 0.20f;

	// The number of maps
	private int _mapCount;

	// The list of map buttons
	private MapButton[] _buttons;

	// The button square radius
	private float _squareRadius;

	// The vertical scroll
	private VerticalScroll _verticalScroll;

	// The selected map
	private int _selectedMap = -1;

	// True if interactable
	private bool _isInteractable = true;

	// Determine if touch enabled
	private bool _isTouchEnabled;

	// The touch position
	private Vector3 _touchPosition;

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

	public void AddEventHandler(MapEventHandler handler)
	{
		_OnMapSelected += handler;
	}
	
	public void RemoveEventHandler(MapEventHandler handler)
	{
		_OnMapSelected -= handler;
	}

	void Start()
	{
		// Get current level
		int level = UserData.Instance.Level;

		int mapCount = maps.Length;
		float mapWidth = maps[0].GetWidth();

		int buttonCount = -1;
		float cloudY = -1;

		for (int zone = 0; zone < 2; zone++)
		{
			ZoneType zoneType = zone.ToZoneType();

			if (level <= zoneType.MapCount())
			{
				buttonCount = zoneType.MapCount() + 1;
				cloudY = clouds.GetChild(zone).position.y;
				break;
			}
		}

		Vector3 position = Vector3.zero;

		for (int i = 0; i < mapCount; i++)
		{
			GameObject bg = new GameObject(string.Format("Background {0}", i + 1));
			bg.transform.SetParent(background);
			bg.transform.localPosition = position;

			SpriteRenderer renderer = bg.AddComponent<SpriteRenderer>();
			renderer.sprite = maps[i];
			renderer.sortingOrder = -3;

			float height = maps[i].GetHeight();

			position.y += height;

			if (cloudY > 0 && position.y >= cloudY)
			{
				break;
			}
		}

		if (cloudY < 0)
		{
			cloudY = position.y;
		}

		// Set cloud position
		cloud.transform.SetPositionY(cloudY);

		// Set number of maps
		_mapCount = points.childCount;

		if (buttonCount < 0)
		{
			buttonCount = _mapCount;
		}

		// Create buttons
		_buttons = new MapButton[buttonCount];

		bool isNewLevel = UserData.Instance.NewLevel;

		for (int i = 0; i < buttonCount; i++)
		{
			position = points.GetChild(i).position;
			
			int mapID = i + 1;
			
			GameObject map = buttonPrefab.Create(background, position);
			map.name = string.Format("Map {0}", mapID);

			bool unlocked = mapID < level || (mapID == level && !isNewLevel);

			MapButton mapButton = map.GetComponent<MapButton>();
			mapButton.Construct(spriteLock, zones[ZoneTypeHelper.GetZoneType(mapID).ToInt()].mapIcon, mapID, unlocked);
			
			_buttons[i] = mapButton;
		}
		
		// Destroy points
		Destroy(points.gameObject);

		// Destroy clouds
		Destroy(clouds.gameObject);

		// Fixed width
		float scale = Camera.main.GetWidth() / mapWidth;
		background.SetScale(scale);

		float mapHeight = cloudY * scale;

		//
		_squareRadius = ButtonRadius * ButtonRadius * scale * scale;

		// Create vertical scroll
		_verticalScroll = new VerticalScroll();
		_verticalScroll.Construct(-Camera.main.orthographicSize, mapHeight, Camera.main.GetHeight());

		// Set map position
//		transform.SetPositionY(_verticalScroll.Position);

		// Get current map
		int mapIndex = Mathf.Clamp(UserData.Instance.Map - 1, 0, _buttons.Length - 1);

		if (animal != null)
		{
			// Set animal position
			animal.transform.position = _buttons[mapIndex].transform.position;

			// Set skin
			animal.idleDuration = 0;
			
			ZoneType zoneType = ZoneTypeHelper.GetZoneType(mapIndex + 1);
			
			if (!zoneType.IsDefault())
			{
				ZoneSetting zone = zones[zoneType.ToInt()];
				animal.SetClothes(zone.clothesLeft, zone.clothesUp, zone.clothesDown);
				animal.SetCap(zone.capLeft, zone.capUp, zone.capDown);
			}
		}

		MoveObjectToCenter(_buttons[mapIndex].gameObject);
	}

	public void UnlockMap(int mapID, Action callback)
	{
		// Play sound
		SoundManager.Instance.PlaySound(SoundID.UnlockMap);

		MapButton button = _buttons[mapID - 1];

		// Play effect
		if (unlockLevelPrefab != null)
		{
			unlockLevelPrefab.Create(button.transform, button.transform.position);
		}

		// Unlock button
		button.Unlocked = true;

		if (animal != null)
		{
			animal.JumpToMap(0.5f, button.transform.position, 0.0f, callback);
		}
		else
		{
			callback();
		}
	}
	
	public void NextMap(int mapID, Action callback)
	{
		MapButton button = _buttons[mapID - 1];

		if (animal != null)
		{
			animal.JumpToMap(0.5f, button.transform.position, 0.0f, callback);
		}
		else
		{
			callback();
		}
	}

	/// <summary>
	/// Jumps to the specified map (one-based).
	/// </summary>
	public void JumpToMap(int mapID, Action callback)
	{
		MapButton button = _buttons[mapID - 1];
		Assert.IsTrue(button.Unlocked);

		if (animal != null)
		{
			animal.JumpToMap(0.0f, button.transform.position, 0.1f, callback);
		}
		else
		{
			callback();
		}
	}

	public void UnlockMap(int mapID)
	{
		if (mapID > _buttons.Length) return;

		_buttons[mapID - 1].Unlocked = true;
	}

	void Update()
	{
		if (_isInteractable)
		{
			// Touch input
			if (Input.touchCount > 0)
			{
				// Get touch
				Touch touch = Input.GetTouch(0);
				
				if (touch.phase == TouchPhase.Began)
				{
					OnTouchPressed(GetWorldPosition(touch.position));
				}
				else
				{
					if (_isTouchEnabled)
					{
						if (touch.phase == TouchPhase.Moved)
						{
							OnTouchMoved(GetWorldPosition(touch.position));
						}
						else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
						{
							OnTouchReleased(GetWorldPosition(touch.position));
						}
					}
				}
			}
			// Mouse input
			else
			{
				if (Input.GetMouseButtonDown(0))
				{
					OnTouchPressed(GetWorldPosition(Input.mousePosition));
				}
				else
				{
					if (_isTouchEnabled)
					{
						if (Input.GetMouseButton(0))
						{
							OnTouchMoved(GetWorldPosition(Input.mousePosition));
						}
						else if (Input.GetMouseButtonUp(0))
						{
							OnTouchReleased(GetWorldPosition(Input.mousePosition));
						}
					}
				}
			}
		}

		// Update scroll
		if (!_verticalScroll.OnUpdate(Time.deltaTime))
		{
			// Update position
			transform.SetPositionY(_verticalScroll.Position);
		}
	}

	void OnTouchPressed(Vector3 position)
	{
		#if UNITY_EDITOR
		_isTouchEnabled = (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject());
		#else
		_isTouchEnabled = (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch (0).fingerId));
		#endif

		if (!_isTouchEnabled) return;

		// Save touch position
		_touchPosition = position;

		_selectedMap = -1;

		Vector3 buttonPosition;
		float deltaX, deltaY;

		int buttonCount = _buttons.Length; 

		for (int i = 0; i < buttonCount; i++)
		{
			buttonPosition = _buttons[i].transform.position;

			deltaX = position.x - buttonPosition.x;
			deltaY = position.y - buttonPosition.y;

			if (deltaX * deltaX + deltaY * deltaY <= _squareRadius)
			{
				_selectedMap = i;
//				Debug.Log("Select map " + (_selectedMap + 1));

				break;
			}
		}

		_verticalScroll.OnTouchPressed(position);
	}
	
	void OnTouchMoved(Vector3 position)
	{
//		if (!_isTouchEnabled) return;

		if (_selectedMap >= 0)
		{
			float deltaX = position.x - _touchPosition.x;
			float deltaY = position.y - _touchPosition.y;

			if (deltaX * deltaX + deltaY * deltaY >= MinDeltaMove * MinDeltaMove)
			{
				_selectedMap = -1;
			}
		}

		_verticalScroll.OnTouchMoved(position);
	}

	void OnTouchReleased(Vector3 position)
	{
//		if (!_isTouchEnabled) return;

		_verticalScroll.OnTouchReleased(position);

		if (_selectedMap >= 0)
		{
//			if (_buttons[_selectedMap].Unlocked)
//			{
////				Debug.Log("Go to map " + (_selectedMap + 1));
//
//				if (UserData.Instance.Mana > 0)
//				{
//					// Set map ID
//					UserData.Instance.Map = _selectedMap + 1;
//					
//					// Decrease mana
//					UserData.Instance.Mana--;
//					
//					// Go to main game
//					TransitionManager.Instance.FadeTransitionScene("MainGame");
//				}
//				else
//				{
//					Debug.Log("Mana required!");
//				}
//			}

			_buttons[_selectedMap].OnSelected();

			if (_OnMapSelected != null)
			{
				_OnMapSelected(_selectedMap + 1, _buttons[_selectedMap].Unlocked);
			}

			_selectedMap = -1;
		}
	}
	
	// Get world position of the specified screen position
	Vector3 GetWorldPosition(Vector3 position)
	{
		return Camera.main.ScreenToWorldPoint(position);
	}

	void MoveObjectToCenter(GameObject go)
	{
		_verticalScroll.Position = transform.position.y - go.transform.position.y;
		transform.SetPositionY(_verticalScroll.Position);
	}

#if UNITY_EDITOR
	void OnLayout()
	{
		// Remove background
		Transform bgTransform = transform.FindInChildren("Bg");

		if (bgTransform != null)
		{
			bgTransform.DestroyImmediateChildren();
		}
		else
		{
			GameObject map = new GameObject("Bg");
			map.transform.SetParent(transform);

			bgTransform = map.transform;
		}

		if (maps == null) return;

		int mapCount = maps.Length;
		Vector3 position = Vector3.zero;

		for (int i = 0; i < mapCount; i++)
		{
			GameObject map = new GameObject(string.Format("Background {0}", i + 1));
			map.transform.SetParent(bgTransform);
			map.transform.localPosition = position;
			
			SpriteRenderer renderer = map.AddComponent<SpriteRenderer>();
			renderer.sprite = maps[i];
			renderer.sortingOrder = -3;

			float height = maps[i].GetHeight();
			
			position.y += height;
		}
	}

	void OnUnlayout()
	{
		// Remove background
		Transform bgTransform = transform.FindInChildren("Bg");
		
		if (bgTransform != null)
		{
			Destroy(bgTransform.gameObject);
		}
	}

//	void OnDrawGizmos()
//	{
//		if (_buttons == null) return;
//
//		float radius = Mathf.Sqrt(_squareRadius);
//
//		for (int i = 0; i < _buttons.Length; i++)
//		{
//			GizmosHelper.DrawCircle(_buttons[i].transform.position, radius);
//		}
//	}
#endif
}
