using UnityEngine;
using System.Collections;
using System.Text;

[ExecuteInEditMode]
public class MapEditor : MonoBehaviour
{
	class MapItem
	{
		public ItemType   type;
		public GameObject item;

		public MapItem(ItemType type, GameObject item)
		{
			this.type = type;
			this.item = item;
		}
	}

	/// <summary>
	/// The item prefabs.
	/// </summary>
	public GameObject[] itemPrefabs = new GameObject[ItemType.Count.ToInt()];

	/// <summary>
	/// The number of rows.
	/// </summary>
	public int rows = 7;
	
	/// <summary>
	/// The number of columns.
	/// </summary>
	public int columns = 5;
	
	/// <summary>
	/// The size of cell.
	/// </summary>
	public float cellSize = 100.0f;
	
	/// <summary>
	/// The color.
	/// </summary>
	public Color color = Color.white;

	// The current item type
	private ItemType _itemType;

	// The current item prefab
	private GameObject _itemPrefab;

	// The array of items
	private MapItem[,] _items;

	// The start row
	private int _startRow = -1;

	// The start column
	private int _startColumn = -1;

	// The direction
	private Direction _direction = Direction.None;

	// The selected row
	private int _selectedRow = -1;

	// The selected column
	private int _selectedColumn = -1;

	// Is touch pressed?
	private bool _isPressed = false;
	
	void OnDrawGizmos()
	{
		// Set color
		Gizmos.color = color;
		
		// Bottom-left position
		float bottom = -rows    * cellSize * 0.5f;
		float left   = -columns * cellSize * 0.5f;
		
		// Top-right position
		float top   = -bottom;
		float right = -left;
		
		Vector3 from = Vector3.zero;
		Vector3 to   = Vector3.zero;
		
		//
		float x = left;
		float y = bottom;
		
		from.y = bottom;
		to.y   = top;
		
		// Draw vertical lines
		for (int i = 0; i <= columns; i++)
		{
			from.x = to.x = x;
			
			Gizmos.DrawLine(from, to);
			
			x += cellSize;
		}
		
		//
		from.x = left;
		to.x   = right;
		
		// Draw horizontal lines
		for (int i = 0; i <= rows; i++)
		{
			from.y = to.y = y;
			
			Gizmos.DrawLine(from, to);
			
			y += cellSize;
		}
	}

	void Update()
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
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				OnTouchReleased(GetWorldPosition(touch.position));
			}
		}
		// Mouse input
		else
		{
			if (Input.GetMouseButtonDown(0))
			{
				OnTouchPressed(GetWorldPosition(Input.mousePosition));
			}
			else if (Input.GetMouseButtonUp(0))
			{
				OnTouchReleased(GetWorldPosition(Input.mousePosition));
			}
		}
	}

	void OnTouchPressed(Vector3 position)
	{
		_isPressed = GetCell(position, ref _selectedRow, ref _selectedColumn);
	}
	
	void OnTouchReleased(Vector3 position)
	{
		if (_isPressed)
		{
			// Check if items is null
			if (_items == null)
			{
				_items = new MapItem[rows, columns];
			}

			int row    = -1;
			int column = -1;

			if (GetCell(position, ref row, ref column))
			{
				if (_selectedRow == row && _selectedColumn == column)
				{
					if (_itemType.IsFrog())
					{
						MapItem mapItem = _items[row, column];

						if (mapItem != null)
						{
							// Get direction
							Direction direction = _itemType.GetDirection();

							// Find a frog
							GameObject frog = mapItem.item.FindChild("Frog");

							if (frog != null)
							{
								Destroy(frog);

								if (direction == _direction)
								{
									// Reset start cell
									_startRow    = -1;
									_startColumn = -1;
								}
								else
								{
									// Replace frog
									frog = Instantiate(_itemPrefab) as GameObject;
									frog.name = "Frog";
									frog.transform.SetParent(mapItem.item.transform);
									frog.transform.localPosition = Vector3.zero;

									// Set direction
									_direction = direction;
								}
							}
							else
							{
								if (_startRow >= 0 && _startColumn >= 0)
								{
									GameObject foothold = _items[_startRow, _startColumn].item;

									// There is only one frog
									if (foothold != null)
									{
										foothold.DestroyChild("Frog");
									}
								}

								// Create frog
								frog = Instantiate(_itemPrefab) as GameObject;
								frog.name = "Frog";
								frog.transform.SetParent(mapItem.item.transform);
								frog.transform.localPosition = Vector3.zero;

								// Set start cell
								_startRow    = row;
								_startColumn = column;

								// Set direction
								_direction = direction;
							}
						}
					}
					else
					{
						// Add item
						if (_items[row, column] == null)
						{
							AddItem(_itemType, _itemPrefab, row, column);
						}
						else
						{
							Destroy(_items[row, column].item);

							// Remove item
							if (_items[row, column].type == _itemType)
							{
								_items[row, column] = null;
							}
							// Replace item
							else
							{
								AddItem(_itemType, _itemPrefab, row, column);
							}

							if (row == _startRow && column == _startColumn)
							{
								// Reset start cell
								_startRow    = -1;
								_startColumn = -1;
							}
						}
					}
				}
			}
		}
	}

	// Add item at the specified cell
	void AddItem(ItemType itemType, GameObject itemPrefab, int row, int column)
	{
		if (itemPrefab == null) return;

		// Create item
		GameObject item = Instantiate(itemPrefab, GetPosition(row, column), Quaternion.identity) as GameObject;
		item.transform.SetParent(transform);
		
		_items[row, column] = new MapItem(itemType, item);
	}

	// Destroy first item with the specified type
	void DestroyItemWithType(ItemType itemType)
	{
		if (_items == null) return;
		
		int rows    = _items.GetRow();
		int columns = _items.GetColumn();
		
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				MapItem mapItem = _items[i, j];
				
				if (mapItem != null && mapItem.type == itemType)
				{
					Destroy(mapItem.item);
					_items[i, j] = null;

					i = rows;
					break;
				}
			}
		}
	}

	// Destroy all items with the specified type
	void DestroyItemsWithType(ItemType itemType)
	{
		if (_items == null) return;

		int rows    = _items.GetRow();
		int columns = _items.GetColumn();

		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				MapItem mapItem = _items[i, j];

				if (mapItem != null && mapItem.type == itemType)
				{
					Destroy(mapItem.item);
					_items[i, j] = null;
				}
			}
		}
	}

	// Set type of item
	public void SetItemType(ItemType itemType)
	{
		// Set item type
		_itemType = itemType;
		
		// Set item prefab
		_itemPrefab = itemPrefabs[itemType.ToInt()];
	}

	// Shift left
	public void ShiftLeft()
	{
		if (_items == null) return;
		
		int rows    = _items.GetRow();
		int columns = _items.GetColumn();

		// Destroy
		for (int i = 0; i < rows; i++)
		{
			MapItem mapItem = _items[i, 0];
			
			if (mapItem != null)
			{
				Destroy(mapItem.item);
			}
		}

		// Shift
		for (int i = 0; i < rows; i++)
		{
			for (int j = 1; j < columns; j++)
			{
				MapItem mapItem = _items[i, j];
				
				if (mapItem != null)
				{
					mapItem.item.transform.TranslateX(-cellSize);
				}

				_items[i, j - 1] = mapItem;
			}
		}

		// Clear
		for (int i = 0; i < rows; i++)
		{
			_items[i, columns - 1] = null;
		}

		if (_startColumn >= 0)
		{
			_startColumn--;
		}
	}
	
	// Shift right
	public void ShiftRight()
	{
		if (_items == null) return;
		
		int rows    = _items.GetRow();
		int columns = _items.GetColumn();
		
		// Destroy
		for (int i = 0; i < rows; i++)
		{
			MapItem mapItem = _items[i, columns - 1];
			
			if (mapItem != null)
			{
				Destroy(mapItem.item);
			}
		}
		
		// Shift
		for (int i = 0; i < rows; i++)
		{
			for (int j = columns - 1; j > 0; j--)
			{
				MapItem mapItem = _items[i, j - 1];
				
				if (mapItem != null)
				{
					mapItem.item.transform.TranslateX(cellSize);
				}

				_items[i, j] = mapItem;
			}
		}
		
		// Clear
		for (int i = 0; i < rows; i++)
		{
			_items[i, 0] = null;
		}

		if (_startColumn >= 0)
		{
			if (_startColumn == columns - 1)
			{
				_startColumn = -1;
			}
			else
			{
				_startColumn++;
			}
		}
	}
	
	// Shift up
	public void ShiftUp()
	{
		if (_items == null) return;
		
		int rows    = _items.GetRow();
		int columns = _items.GetColumn();
		
		// Destroy
		for (int j = 0; j < columns; j++)
		{
			MapItem mapItem = _items[rows - 1, j];
			
			if (mapItem != null)
			{
				Destroy(mapItem.item);
			}
		}
		
		// Shift
		for (int j = 0; j < columns; j++)
		{
			for (int i = rows - 1; i > 0; i--)
			{
				MapItem mapItem = _items[i - 1, j];
				
				if (mapItem != null)
				{
					mapItem.item.transform.TranslateY(cellSize);
				}
				
				_items[i, j] = mapItem;
			}
		}
		
		// Clear
		for (int j = 0; j < columns; j++)
		{
			_items[0, j] = null;
		}

		if (_startRow >= 0)
		{
			if (_startRow == rows - 1)
			{
				_startRow = -1;
			}
			else
			{
				_startRow++;
			}
		}
	}

	// Shift down
	public void ShiftDown()
	{
		if (_items == null) return;
		
		int rows    = _items.GetRow();
		int columns = _items.GetColumn();
		
		// Destroy
		for (int j = 0; j < columns; j++)
		{
			MapItem mapItem = _items[0, j];
			
			if (mapItem != null)
			{
				Destroy(mapItem.item);
			}
		}
		
		// Shift
		for (int j = 0; j < columns; j++)
		{
			for (int i = 1; i < rows; i++)
			{
				MapItem mapItem = _items[i, j];
				
				if (mapItem != null)
				{
					mapItem.item.transform.TranslateY(-cellSize);
				}
				
				_items[i - 1, j] = mapItem;
			}
		}
		
		// Clear
		for (int j = 0; j < columns; j++)
		{
			_items[rows - 1, j] = null;
		}

		if (_startRow >= 0)
		{
			_startRow--;
		}
	}

	// Clear all items
	public void Clear()
	{
		if (_items == null) return;
		
		int rows    = _items.GetRow();
		int columns = _items.GetColumn();
		
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				MapItem mapItem = _items[i, j];
				
				if (mapItem != null)
				{
					Destroy(mapItem.item);
					_items[i, j] = null;
				}
			}
		}

		// Reset start cell
		_startRow    = -1;
		_startColumn = -1;
	}
	
	// Save to file
	public bool Save(string fileName)
	{
		// Get map data
		MapData mapData = GetMapData();

		if (mapData != null)
		{
			mapData.Log();

			return Helper.Save<MapData>(mapData, fileName);
		}
		
		return false;
	}
	
	// Load from file
	public bool Load(string fileName)
	{
		MapData mapData = null;
		Helper.Load<MapData>(fileName, ref mapData);
		
		if (mapData == null)
		{
			Debug.Log("Map not found!");
			return false;
		}
		
//		mapData.Log();

		// Clear items
		Clear();

		// Get footholds
		int[,] footholds = mapData.footholds;

		// Set rows
		rows = footholds.GetRow();

		// Set columns
		columns = footholds.GetColumn();

		// Create map items
		_items = new MapItem[rows, columns];

		for (int i = 0; i < rows; i++)
		{
			int row = rows - 1 - i;

			for (int j = 0; j < columns; j++)
			{
				FootholdType footholdType = footholds[i, j].ToFootholdType();

				if (footholdType != FootholdType.None)
				{
					ItemType itemType = footholdType.ToItemType();

					AddItem(itemType, GetItemPrefab(itemType), row, j);
				}
			}
		}

		// Set start cell
		_startRow    = rows - 1 - mapData.startRow;
		_startColumn = mapData.startColumn;

		// Set direction
		_direction = mapData.direction;

		// Set time foothold duration
		mapData.DeserializeTimeFootholds((row, column, duration) => {
			MapItem item = _items[rows - 1 - row, column];
			
			if (item != null)
			{
				TimeScript time = item.item.GetComponent<TimeScript>();

				if (time != null)
				{
					time.duration = duration;
				}
				else
				{
					//Log.Debug("TimeScript required!");
				}
			}
			else
			{
				//Log.Debug("TimeFoothold is null!");
			}
		});

		// Get foothold
		GameObject foothold = _items[_startRow, _startColumn].item;

		if (foothold != null)
		{
			// Create frog
			GameObject frog = Instantiate(GetItemPrefab(ItemTypeHelper.GetFrog(_direction))) as GameObject;
			frog.name = "Frog";
			frog.transform.SetParent(foothold.transform);
			frog.transform.localPosition = Vector3.zero;
		}

		return true;
	}

	public MapData GetMapData()
	{
		if (_items == null)
		{
			Debug.Log("Footholds not found!");
			return null;
		}

		if (_startRow < 0 || _startColumn < 0)
		{
			Debug.Log("Frog not found!");
			return null;
		}
		
		int rows    = _items.GetRow();
		int columns = _items.GetColumn();
		
		int[,] footholds = new int[rows, columns];
		StringBuilder durations = new StringBuilder();

		for (int i = 0; i < rows; i++)
		{
			int row = rows - 1 - i;
			
			for (int j = 0; j < columns; j++)
			{
				MapItem item = _items[row, j];
				
				if (item == null)
				{
					footholds[i, j] = FootholdType.None.ToInt();
				}
				else
				{
					footholds[i, j] = (int)item.type;

					if (item.type == ItemType.FootholdTime)
					{
						TimeScript time = item.item.GetComponent<TimeScript>();

						durations.AppendFormat("{0}:{1} ", i * columns + j, time.duration);
					}
				}
			}
		}
		
		MapData mapData = new MapData();
		mapData.footholds = footholds;
		mapData.startRow = rows - 1 - _startRow;
		mapData.startColumn = _startColumn;
		mapData.direction = _direction;

		if (durations.Length > 0)
		{
			mapData.timeFootholdDurations = durations.ToString().Substring(0, durations.Length - 1);
		}

		return mapData;
	}

	GameObject GetItemPrefab(ItemType itemType)
	{
		return itemPrefabs[itemType.ToInt()];
	}

	// Get world position of the specified screen position
	Vector3 GetWorldPosition(Vector3 position)
	{
		return Camera.main.ScreenToWorldPoint(position);
	}

	// Get cell at the specified world position
	bool GetCell(Vector3 position, ref int row, ref int column)
	{
		float width  = columns * cellSize;
		float height = rows    * cellSize;
		
		float x = position.x - transform.position.x + width  * 0.5f;
		float y = position.y - transform.position.y + height * 0.5f;
		
		if (x >= 0 && x < width && y >= 0 && y < height)
		{
			row    = Mathf.FloorToInt((y - Mathf.Epsilon) / cellSize);
			column = Mathf.FloorToInt((x - Mathf.Epsilon) / cellSize);
			
			return true;
		}
		
		return false;
	}
	
	// Get world position at the specified cell
	Vector3 GetPosition(int row, int column)
	{
		float width  = columns * cellSize;
		float height = rows    * cellSize;
		
		float x = transform.position.x - width  * 0.5f + (column + 0.5f) * cellSize;
		float y = transform.position.y - height * 0.5f + (row    + 0.5f) * cellSize;
		
		return new Vector3(x, y, 0);
	}
}
