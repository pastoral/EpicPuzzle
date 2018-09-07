using UnityEngine;
using System;

/*
 * 		+-----+-----+-----+
 * 		|     |	    |	  |
 * 		|  6  |	 7  |  8  |
 * 		+-----+-----+-----+
 * 		|     |	    |	  |
 * y	|  3  |	 4  |  5  |
 * ^	+-----+-----+-----+
 * |	|     |	    |	  |
 * |	|  0  |	 1  |  2  |
 * |	+-----+-----+-----+
 * |
 * +--------> x
 */
public class Board : MonoBehaviour, ITouchEventListener
{
	// The row
	[SerializeField]
	private int _row = 7;
	
	// The column
	[SerializeField]
	private int _column = 5;
	
	// The cell size
	[SerializeField]
	private float _cellSize = 1.75f;

	// The line thick
	[SerializeField]
	private float _lineThick = 0.1f;

	// The minimum delta movement
	[SerializeField]
	private float _minDeltaMove = 0.3f;
	
	// The minimum delta dragging
	[SerializeField]
	private float _minDeltaDrag = 0.7f;

	// The board event listener
	private IBoardEventListener _listener = NullBoardEventListener.Instance;
	
	// The step
	private float _step;

	// The board width
	private float _width;

	// The board height
	private float _height;

	// The square minimum delta movement
	private float _squareMinDeltaMove;
	
	// The square minimum delta dragging
	private float _squareMinDeltaDrag;

	// Interactable or not
	private bool _isInteractable = true;

	// True if moved
	private bool _isMoved;

	// The touch position
	private Vector3 _touchPosition;

	// The enabled cell
	private int _enabledRow = -1;
	private int _enabledColumn = -1;

	// The enabled direction
	private Direction _enabledDirection = Direction.None;

	// The selected cell
	private int _selectedRow;
	private int _selectedColumn;

	// Get/Set event listener
	public IBoardEventListener EventListener
	{
		get
		{
			return (_listener == NullBoardEventListener.Instance) ? null : _listener;
		}
		set
		{
			_listener = (value != null) ? value : NullBoardEventListener.Instance;
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

	public void SetEnabledCell(int row, int column)
	{
		_enabledRow    = row;
		_enabledColumn = column;
	}

	public void ResetEnabledCell()
	{
		_enabledRow = _enabledColumn = -1;
	}
	
	public void SetEnabledDirection(Direction direction)
	{
		_enabledDirection = direction;
	}
	
	public void ResetEnabledDirection()
	{
		_enabledDirection = Direction.None;
	}

	void OnEnable()
	{
		TouchManager.Instance.AddEventListener(this);
	}

	void OnDisable()
	{
		TouchManager.SafeRemoveEventListener(this);
	}

	void Awake()
	{
		float scale = ResolutionHelper.GetScale();
		_cellSize  *= scale;
		_lineThick *= scale;

		// Set step
		_step = _cellSize + _lineThick;

		// Set board width
		_width = _column * _step + _lineThick;

		// Set board height
		_height = _row * _step + _lineThick;

		// Set square minimum delta movement
		_squareMinDeltaMove = _minDeltaMove * _minDeltaMove;
		
		// Set square minimum delta dragging
		_squareMinDeltaDrag = _minDeltaDrag * _minDeltaDrag;
	}

	// Get cell at the specified world position
	public bool GetCell(Vector3 position, ref int row, ref int column)
	{
		float x = position.x - transform.position.x + _width  * 0.5f;
		float y = position.y - transform.position.y + _height * 0.5f;
		
		if (x >= _lineThick && x < _width - _lineThick && y >= _lineThick && y < _height - _lineThick)
		{
			x -= _lineThick;
			y -= _lineThick;
			
			int r = Mathf.FloorToInt((y - Mathf.Epsilon) / _step);
			int c = Mathf.FloorToInt((x - Mathf.Epsilon) / _step);
			
			if (x <= c * _step + _cellSize && y <= r * _step + _cellSize)
			{
				row    = r;
				column = c;
				
				return true;
			}
		}
		
		return false;
	}

	public bool GetCell(float left, float top, float right, float bottom, Func<int, int, bool> accept, ref int row, ref int column)
	{
		float deltaX = transform.position.x - _width  * 0.5f;
		float deltaY = transform.position.y - _height * 0.5f;

		left   -= deltaX;
		right  -= deltaX;
		top    -= deltaY;
		bottom -= deltaY;

		float boardLeft   = _lineThick;
		float boardRight  = boardLeft + _width;
		float boardBottom = _lineThick;
		float boardTop    = boardBottom + _height;

		if (left   > boardRight)	return false;
		if (right  < boardLeft) 	return false;
		if (bottom > boardTop) 		return false;
		if (top    < boardBottom)	return false;

		float maxSurface = 0;
		int maxRow = -1;
		int maxColumn = -1;

		float surface;
		int r = -1;
		int c = -1;

		// Top-Left
		surface = GetBRIntersection(left, top, ref r, ref c);

		if (surface > maxSurface)
		{
			if (accept(r, c))
			{
				maxSurface = surface;
				maxRow = r;
				maxColumn = c;
			}
		}
		
		// Top-Right
		surface = GetBLIntersection(right, top, ref r, ref c);

		if (surface > maxSurface)
		{
			if (accept(r, c))
			{
				maxSurface = surface;
				maxRow = r;
				maxColumn = c;
			}
		}
		
		// Bottom-Left
		surface = GetTRIntersection(left, bottom, ref r, ref c);

		if (surface > maxSurface)
		{
			if (accept(r, c))
			{
				maxSurface = surface;
				maxRow = r;
				maxColumn = c;
			}
		}
		
		// Bottom-Right
		surface = GetTLIntersection(right, bottom, ref r, ref c);

		if (surface > maxSurface)
		{
			if (accept(r, c))
			{
				maxSurface = surface;
				maxRow = r;
				maxColumn = c;
			}
		}

		if (maxRow >= 0 && maxColumn >= 0)
		{
			row    = maxRow;
			column = maxColumn;

			return true;
		}

		return false;
	}

	float GetBLIntersection(float x, float y, ref int row, ref int column)
	{
		if (x >= _lineThick && x < _width - _lineThick && y >= _lineThick && y < _height - _lineThick)
		{
			x -= _lineThick;
			y -= _lineThick;
			
			int r = Mathf.FloorToInt((y - Mathf.Epsilon) / _step);
			int c = Mathf.FloorToInt((x - Mathf.Epsilon) / _step);
			
			if (x <= c * _step + _cellSize && y <= r * _step + _cellSize)
			{
				row    = r;
				column = c;
				
				return (x - c * _step) * (y - r * _step);
			}
		}

		return 0;
	}
	
	float GetBRIntersection(float x, float y, ref int row, ref int column)
	{
		if (x >= _lineThick && x < _width - _lineThick && y >= _lineThick && y < _height - _lineThick)
		{
			x -= _lineThick;
			y -= _lineThick;
			
			int r = Mathf.FloorToInt((y - Mathf.Epsilon) / _step);
			int c = Mathf.FloorToInt((x - Mathf.Epsilon) / _step);
			
			if (x <= c * _step + _cellSize && y <= r * _step + _cellSize)
			{
				row    = r;
				column = c;
				
				return (_cellSize - (x - c * _step)) * (y - r * _step);
			}
		}
		
		return 0;
	}
	
	float GetTLIntersection(float x, float y, ref int row, ref int column)
	{
		if (x >= _lineThick && x < _width - _lineThick && y >= _lineThick && y < _height - _lineThick)
		{
			x -= _lineThick;
			y -= _lineThick;
			
			int r = Mathf.FloorToInt((y - Mathf.Epsilon) / _step);
			int c = Mathf.FloorToInt((x - Mathf.Epsilon) / _step);
			
			if (x <= c * _step + _cellSize && y <= r * _step + _cellSize)
			{
				row    = r;
				column = c;
				
				return (x - c * _step) * (_cellSize - (y - r * _step));
			}
		}
		
		return 0;
	}
	
	float GetTRIntersection(float x, float y, ref int row, ref int column)
	{
		if (x >= _lineThick && x < _width - _lineThick && y >= _lineThick && y < _height - _lineThick)
		{
			x -= _lineThick;
			y -= _lineThick;
			
			int r = Mathf.FloorToInt((y - Mathf.Epsilon) / _step);
			int c = Mathf.FloorToInt((x - Mathf.Epsilon) / _step);
			
			if (x <= c * _step + _cellSize && y <= r * _step + _cellSize)
			{
				row    = r;
				column = c;
				
				return (_cellSize - (x - c * _step)) * (_cellSize - (y - r * _step));
			}
		}
		
		return 0;
	}

	// Get world position at the specified cell
	public Vector3 GetPosition(int row, int column)
	{
		float x = transform.position.x - _width  * 0.5f + _lineThick + column * _step + _cellSize * 0.5f;
		float y = transform.position.y - _height * 0.5f + _lineThick + row    * _step + _cellSize * 0.5f;
		
		return new Vector3(x, y, 0);
	}

#if UNITY_EDITOR || UNITY_STANDALONE
	void Update()
	{
		if (!_isInteractable) return;

		// Key input
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			if (_enabledDirection.IsNone() || _enabledDirection.IsLeft())
			{
				_listener.OnDraggedLeft();
			}
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			if (_enabledDirection.IsNone() || _enabledDirection.IsUp())
			{
				_listener.OnDraggedUp();
			}
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			if (_enabledDirection.IsNone() || _enabledDirection.IsRight())
			{
				_listener.OnDraggedRight();
			}
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if (_enabledDirection.IsNone() || _enabledDirection.IsDown())
			{
				_listener.OnDraggedDown();
			}
		}
	}
#endif

#region ITouchEventListener

	public bool OnTouchPressed(Vector3 position)
	{
		if (!_isInteractable) return false;

		// Set not moved
		_isMoved = false;

		// Save touch position
		_touchPosition = position;

		_selectedRow    = -1;
		_selectedColumn = -1;

		if (GetCell(position, ref _selectedRow, ref _selectedColumn))
		{
			if (_enabledRow >= 0 && _enabledColumn >= 0)
			{
				if (_selectedRow == _enabledRow && _selectedColumn == _enabledColumn)
				{
					_listener.OnCellPressed(_selectedRow, _selectedColumn);
				}
				else
				{
					_selectedRow = _selectedColumn = -1;
				}
			}
			else
			{
				_listener.OnCellPressed(_selectedRow, _selectedColumn);
			}
		}

		return true;
	}
	
	public bool OnTouchMoved(Vector3 position)
	{
		float deltaX = position.x - _touchPosition.x;
		float deltaY = position.y - _touchPosition.y;
		float delta  = deltaX * deltaX + deltaY * deltaY;

		if (!_isMoved)
		{
			// Check if moved
			if (delta >= _squareMinDeltaMove)
			{
				_isMoved = true;
			}
		}

		// Check if dragged
		if (delta >= _squareMinDeltaDrag)
		{
			// Calculate angle
			float angle = Helper.GetDegreeAngle(deltaX, deltaY);

			if (angle >= 45f)
			{
				// Top
				if (angle <= 135f)
				{
					if (_enabledDirection.IsNone() || _enabledDirection.IsUp())
					{
						_listener.OnDraggedUp();
					}
				}
				// Left
				else if (angle <= 225f)
				{
					if (_enabledDirection.IsNone() || _enabledDirection.IsLeft())
					{
						_listener.OnDraggedLeft();
					}
				}
				// Bottom
				else if (angle <= 315f)
				{
					if (_enabledDirection.IsNone() || _enabledDirection.IsDown())
					{
						_listener.OnDraggedDown();
					}
				}
				// Right
				else
				{
					if (_enabledDirection.IsNone() || _enabledDirection.IsRight())
					{
						_listener.OnDraggedRight();
					}
				}
			}
			else
			{
				// Right
				if (_enabledDirection.IsNone() || _enabledDirection.IsRight())
				{
					_listener.OnDraggedRight();
				}
			}

			return false;
		}

		return true;
	}
	
	public void OnTouchReleased(Vector3 position)
	{
		// Check if moved
		if (_isMoved)
		{
			_listener.OnCellCancelled();
		}
		else
		{
			int row    = -1;
			int column = -1;

			if (GetCell(position, ref row, ref column))
			{
				if (row == _selectedRow && column == _selectedColumn)
				{
					_listener.OnCellReleased(_selectedRow, _selectedColumn);
				}
				else
				{
					_listener.OnCellCancelled();
				}
			}
			else
			{
				_listener.OnCellCancelled();
			}
		}
	}

#endregion // ITouchEventListener
	
	void OnDrawGizmos()
	{
		float scale 	= Application.isPlaying ? 1.0f : ResolutionHelper.GetScale();
		float cellSize  = _cellSize * scale;
		float lineThick = _lineThick * scale;
		
		// Set color
		Gizmos.color = Color.blue;
		
		float step = cellSize + lineThick;
		
		// Bottom-left position
		float bottom = -(_row    * step + lineThick) * 0.5f;
		float left   = -(_column * step + lineThick) * 0.5f;
		
		// Top-right position
		float top   = -bottom;
		float right = -left;
		
		Vector3 from = Vector3.zero;
		Vector3 to   = Vector3.zero;
		
		// Border
		{
			// Vertical
			from.y = bottom;
			to.y   = top;
			
			from.x = to.x = left;
			Gizmos.DrawLine(from, to);
			
			from.x = to.x = right;
			Gizmos.DrawLine(from, to);
			
			// Horizontal
			from.x = left;
			to.x   = right;
			
			from.y = to.y = bottom;
			Gizmos.DrawLine(from, to);
			
			from.y = to.y = top;
			Gizmos.DrawLine(from, to);
		}
		
		//
		float x;
		float y = bottom + lineThick;
		
		// Draw vertical lines
		for (int i = 0; i < _row; i++)
		{
			x = left + lineThick;
			
			from.y = y;
			to.y   = from.y + cellSize;
			
			for (int j = 0; j < _column; j++)
			{
				from.x = to.x = x;
				Gizmos.DrawLine(from, to);
				
				from.x = to.x = x + cellSize;
				Gizmos.DrawLine(from, to);
				
				x += step;
			}
			
			y += step;
		}
		
		x = left + lineThick;
		
		// Draw horizontal lines
		for (int j = 0; j < _column; j++)
		{
			y = bottom + lineThick;
			
			from.x = x;
			to.x   = from.x + cellSize;
			
			for (int i = 0; i < _row; i++)
			{
				from.y = to.y = y;
				Gizmos.DrawLine(from, to);
				
				from.y = to.y = y + cellSize;
				Gizmos.DrawLine(from, to);
				
				y += step;
			}
			
			x += step;
		}
	}
}
