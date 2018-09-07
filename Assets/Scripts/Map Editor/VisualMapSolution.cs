using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class VisualMapSolution : MonoBehaviour
{
	/// <summary>
	/// The normal foothold.
	/// </summary>
	public Sprite normalFoothold;

	/// <summary>
	/// The double foothold.
	/// </summary>
	public Sprite doubleFoothold;

	/// <summary>
	/// The time foothold.
	/// </summary>
	public Sprite timeFoothold;

	/// <summary>
	/// The redirect left foothold.
	/// </summary>
	public Sprite redirectLeftFoothold;

	/// <summary>
	/// The redirect up foothold.
	/// </summary>
	public Sprite redirectUpFoothold;

	/// <summary>
	/// The redirect right foothold.
	/// </summary>
	public Sprite redirectRightFoothold;

	/// <summary>
	/// The redirect down foothold.
	/// </summary>
	public Sprite redirectDownFoothold;

	/// <summary>
	/// The frogs (left, up, right, down).
	/// </summary>
	public Sprite[] frogs = new Sprite[4];
	
	/// <summary>
	/// The size of cell.
	/// </summary>
	public float cellSize = 1.0f;

	/// <summary>
	/// The jump duration.
	/// </summary>
	public float jumpDuration = 0.5f;
	
	/// <summary>
	/// The unjump duration.
	/// </summary>
	public float unjumpDuration = 0.25f;

	private static readonly Direction[] Directions = { Direction.Left, Direction.Up, Direction.Right, Direction.Down };

	private MapData _mapData;
	private int _row;
	private int _column;
	private FootholdType[,] _types;

	private int _curRow;
	private int _curColumn;
	private Direction _curDirection;
	
	private SpriteRenderer[,] _footholds;
	private SpriteRenderer _frog;

	private int _total;
	private int _count;

	private Stack<Cell> _cells = new Stack<Cell>(64);
	private Stack<Direction> _dirs = new Stack<Direction>(64);
	private Queue<Queue<Direction>> _solutions = new Queue<Queue<Direction>>(4);
	private Queue<Direction> _solution;

	#region Resolve

	public void Resolve(MapData mapData)
	{
		if (mapData == null) return;

		// Stop all coroutines
		StopAllCoroutines();

		// Set map data
		SetMapData(mapData);

		// Clear cells
		_cells.Clear();

		// Resolve
		Invoke("ResolveCallback", jumpDuration);
	}

	public void ReplayResolve()
	{
		Resolve(_mapData);
	}

	void ResolveCallback()
	{
		StartCoroutine(Try());
	}

	IEnumerator Try()
	{
		int nextRow    = -1;
		int nextColumn = -1;

		for (int i = 0; i < 4; i++)
		{
			Direction dir = Directions[i];
			
			if (!dir.IsOpposite(_curDirection))
			{
				if (NextCell(dir, ref nextRow, ref nextColumn))
				{
					// Push
					_cells.Push(new Cell(_curRow, _curColumn));

					// Save cell
					int row    = _curRow;
					int column = _curColumn;

					// Save direction
					Direction direction = _curDirection;

					FootholdType type1 = _types[_curRow, _curColumn];
					FootholdType type2 = _types[nextRow, nextColumn];
					
					// Set direction
					SetDirection(dir);

					// Update current cell
					if (type1 == FootholdType.Double)
					{
						// Set foothold
						SetFoothold(_curRow, _curColumn, FootholdType.Normal);
						
						// Increase counter
						_count++;
					}
					else if (type1 != FootholdType.None)
					{
						// Set foothold
						SetFoothold(_curRow, _curColumn, FootholdType.None);

						// Increase counter
						_count++;
					}

					// Jump
					var jump = MoveAction.MoveTo(GetPosition(nextRow, nextColumn), jumpDuration * 0.5f);

					if (type2.IsRedirect())
					{
						Direction newDirection = type2.GetDirection();

						_frog.gameObject.Play(SequenceAction.Create(jump, CallFuncAction.Create(() => {
							SetDirection(newDirection);
						})));
					}
					else
					{
						_frog.gameObject.Play(jump);
					}
					
					// Set current cell to next one
					_curRow    = nextRow;
					_curColumn = nextColumn;

					yield return new WaitForSeconds(jumpDuration);

					// Check if finished
					if (_count == _total - 1)
					{
						Show();
					}
					else
					{
						yield return StartCoroutine(Try());
					}

					// Restore cell
					_curRow    = row;
					_curColumn = column;

					// Restore position
					_frog.transform.position = GetPosition(_curRow, _curColumn);

					// Restore type
					if (type1 != FootholdType.None)
					{
						// Set foothold
						SetFoothold(_curRow, _curColumn, type1);
						
						// Decrease counter
						_count--;
					}

					// Restore direction
					SetDirection(direction);

					// Pop
					_cells.Pop();

					yield return new WaitForSeconds(unjumpDuration);
				}
			}
		}
	}

	void Show()
	{
		Cell[] cells = _cells.ToArray();
		
		int count = cells.Length;
		
		StringBuilder sb = new StringBuilder();
		
		sb.Append(cells[count - 1].ToString());
		
		for (int i = count - 2; i >= 0; i--)
		{
			sb.Append(string.Format(" => {0}", cells[i].ToString()));
		}
		
		sb.Append(string.Format(" => ({0},{1})", _curRow, _curColumn));
		
		//Debug.Log(sb.ToString());
	}

	#endregion

	#region Solution

	public void Solution(MapData mapData)
	{
		if (mapData == null) return;

		// Set map data
		SetMapData(mapData);
		
		// Clear directions
		_dirs.Clear();

		// Clear solutions
		_solutions.Clear();

		// Solution
		Invoke("SolutionCallback", jumpDuration);
	}
	
	public void ReplaySolution()
	{
		Solution(_mapData);
	}

	void SolutionCallback()
	{
		// Try
		Try2();

		if (_solutions.Count > 0)
		{
			Queue<Direction>[] solutions = _solutions.ToArray();

			for (int i = 0; i < solutions.Length; i++)
			{
				Direction[] dirs = solutions[i].ToArray();

				StringBuilder sb = new StringBuilder();

				sb.Append(string.Format("{0}: {1}", (i + 1).ToString(), dirs[0].ToString()));

				for (int j = 1; j < dirs.Length; j++)
				{
					sb.Append(string.Format(" => {0}", dirs[j].ToString()));
				}

				//Debug.Log(sb.ToString());
			}

			// Get first solution
			_solution = _solutions.Dequeue();
			
			// Start to jump
			Jump();
		}
	}

	void Try2()
	{
		int nextRow    = -1;
		int nextColumn = -1;
		
		for (int i = 0; i < 4; i++)
		{
			Direction dir = Directions[i];
			
			if (!dir.IsOpposite(_curDirection))
			{
				if (NextCell(dir, ref nextRow, ref nextColumn))
				{
					// Push
					_dirs.Push(dir);
					
					// Save cell
					int row    = _curRow;
					int column = _curColumn;
					
					// Save direction
					Direction direction = _curDirection;
					
					FootholdType type1 = _types[_curRow, _curColumn];
					FootholdType type2 = _types[nextRow, nextColumn];
					
					// Set direction
					_curDirection = dir;
					
					// Update current cell
					if (type1 == FootholdType.Double)
					{
						// Set foothold
						_types[_curRow, _curColumn] = FootholdType.Normal;
						
						// Increase counter
						_count++;
					}
					else if (type1 != FootholdType.None)
					{
						// Set foothold
						_types[_curRow, _curColumn] = FootholdType.None;

						// Increase counter
						_count++;
					}

					if (type2.IsRedirect())
					{
						_curDirection = type2.GetDirection();
					}
					
					// Set current cell to next one
					_curRow    = nextRow;
					_curColumn = nextColumn;

					// Check if finished
					if (_count == _total - 1)
					{
						Direction[] dirs = _dirs.ToArray();
						int count = dirs.Length;

						Queue<Direction> solution = new Queue<Direction>(count);
						
						for (int idx = 0; idx < count; idx++)
						{
							solution.Enqueue(dirs[count - 1 - idx]);
						}

						// Add to solutions list
						_solutions.Enqueue(solution);
					}
					else
					{
						Try2();
					}
					
					// Restore cell
					_curRow    = row;
					_curColumn = column;

					// Restore type
					if (type1 != FootholdType.None)
					{
						// Set foothold
						_types[_curRow, _curColumn] = type1;
						
						// Decrease counter
						_count--;
					}
					
					// Restore direction
					_curDirection = direction;
					
					// Pop
					_dirs.Pop();
				}
			}
		}
	}

	void Jump()
	{
		Direction direction = _solution.Dequeue();

		int nextRow    = -1;
		int nextColumn = -1;

		if (NextCell(direction, ref nextRow, ref nextColumn))
		{
			// Set direction
			SetDirection(direction);

			// Get foothold type
			FootholdType type = _types[_curRow, _curColumn];

			// Update foothold
			if (type == FootholdType.Double)
			{
				// Set foothold
				SetFoothold(_curRow, _curColumn, FootholdType.Normal);
			}
			else if (type != FootholdType.None)
			{
				// Set foothold
				SetFoothold(_curRow, _curColumn, FootholdType.None);
			}

			// Set next cell
			_curRow    = nextRow;
			_curColumn = nextColumn;

			// Jump
			var jump = MoveAction.MoveTo(GetPosition(nextRow, nextColumn), jumpDuration * 0.5f);
			var delay = DelayAction.Create(jumpDuration * 0.5f);
			var callFunc = CallFuncAction.Create(JumpCallback);
			var action = SequenceAction.Create(jump, delay, callFunc);

			_frog.gameObject.Play(action);
		}
	}

	void JumpCallback()
	{
		// Get foothold type
		FootholdType type = _types[_curRow, _curColumn];

		if (type.IsRedirect())
		{
			SetDirection(type.GetDirection().Reverse());
		}

		if (_solution.Count == 0)
		{
			ResetMap();

			if (_solutions.Count > 0)
			{
				// Next solution
				_solution = _solutions.Dequeue();

				// Start to jump
				Invoke("Jump", jumpDuration);
			}
		}
		else
		{
			// Continue to jump
			Jump();
		}
	}
	
	void ResetMap()
	{
		if (_mapData == null) return;
		
		// Get footholds
		int[,] footholds = _mapData.footholds;
		
		// Reset footholds
		for (int i = 0; i < _row; i++)
		{
			int row = _row - 1 - i;
			
			for (int j = 0; j < _column; j++)
			{
				FootholdType type = footholds[i, j].ToFootholdType();
				
				if (type != FootholdType.None)
				{
					SetFoothold(row, j, type);
				}
			}
		}
		
		// Reset start cell
		_curRow    = _row - 1 - _mapData.startRow;
		_curColumn = _mapData.startColumn;
		
		// Reset direction
		_curDirection = _mapData.direction;
		
		// Update frog
		UpdateFrog();
	}

	#endregion

	void SetMapData(MapData mapData)
	{
		// Set map data
		_mapData = mapData;

		// Remove all children
		transform.DestroyImmediateChildren();

		// Get footholds
		int[,] footholds = mapData.footholds;

		// Get number of rows
		_row = footholds.GetRow();

		// Get number of columns
		_column = footholds.GetColumn();
		
		// Create array of foothold types
		_types = new FootholdType[_row, _column];
		
		// Create array of footholds
		_footholds = new SpriteRenderer[_row, _column];
		
		// Reset total
		_total = 0;
		
		// Reset count
		_count = 0;

		// Set footholds
		for (int i = 0; i < _row; i++)
		{
			int row = _row - 1 - i;
			
			for (int j = 0; j < _column; j++)
			{
				FootholdType type = footholds[i, j].ToFootholdType();

				// Set type
				_types[row, j] = type;

				if (type != FootholdType.None)
				{
					GameObject foothold = new GameObject("Foothold");
					foothold.transform.position = GetPosition(row, j);
					foothold.transform.SetParent(transform);
					
					_footholds[row, j] = foothold.AddComponent<SpriteRenderer>();

					// Update foothold
					UpdateFoothold(row, j);
					
					if (type == FootholdType.Double)
					{
						_total += 2;
					}
					else if (type != FootholdType.None)
					{
						_total++;
					}
				}
			}
		}

		// Set start cell
		_curRow    = _row - 1 - mapData.startRow;
		_curColumn = mapData.startColumn;

		// Set start direction
		_curDirection = mapData.direction;

		// Frog
		GameObject frog = new GameObject("Frog");
		frog.transform.SetParent(transform);
		
		_frog = frog.AddComponent<SpriteRenderer>();
		_frog.sortingOrder = 1;

		// Update frog
		UpdateFrog();
	}

	void SetFoothold(int row, int column, FootholdType type)
	{
//		if (_types[row, column] != type)
		{
			_types[row, column] = type;
			
			UpdateFoothold(row, column);
		}
	}
	
	void UpdateFoothold(int row, int column)
	{
		FootholdType type = _types[row, column];
		
		if (type == FootholdType.None)
		{
			_footholds[row, column].enabled = false;
		}
		else
		{
			_footholds[row, column].enabled = true;
			
			if (type == FootholdType.Normal)
			{
				_footholds[row, column].sprite = normalFoothold;
			}
			else if (type == FootholdType.Double)
			{
				_footholds[row, column].sprite = doubleFoothold;
			}
			else if (type == FootholdType.Time)
			{
				_footholds[row, column].sprite = timeFoothold;
			}
			else if (type == FootholdType.RedirectLeft)
			{
				_footholds[row, column].sprite = redirectLeftFoothold;
			}
			else if (type == FootholdType.RedirectUp)
			{
				_footholds[row, column].sprite = redirectUpFoothold;
			}
			else if (type == FootholdType.RedirectRight)
			{
				_footholds[row, column].sprite = redirectRightFoothold;
			}
			else if (type == FootholdType.RedirectDown)
			{
				_footholds[row, column].sprite = redirectDownFoothold;
			}
		}
	}

	void SetDirection(Direction direction)
	{
//		if (_curDirection != direction)
		{
			_curDirection = direction;
			
			_frog.sprite = frogs[_curDirection.ToInt()];
		}
	}

	void UpdateFrog()
	{
		_frog.sprite = frogs[_curDirection.ToInt()];
		_frog.transform.position = GetPosition(_curRow, _curColumn);
	}

	bool NextCell(Direction direction, ref int nextRow, ref int nextColumn)
	{
		// Left
		if (direction.IsLeft())
		{
			for (int column = _curColumn - 1; column >= 0; column--)
			{
				if (_types[_curRow, column] != FootholdType.None)
				{
					nextRow    = _curRow;
					nextColumn = column;
					
					return true;
				}
			}
			
			return false;
		}
		
		// Right
		if (direction.IsRight())
		{
			for (int column = _curColumn + 1; column < _column; column++)
			{
				if (_types[_curRow, column] != FootholdType.None)
				{
					nextRow    = _curRow;
					nextColumn = column;
					
					return true;
				}
			}
			
			return false;
		}
		
		// Down
		if (direction.IsDown())
		{
			for (int row = _curRow - 1; row >= 0; row--)
			{
				if (_types[row, _curColumn] != FootholdType.None)
				{
					nextRow    = row;
					nextColumn = _curColumn;
					
					return true;
				}
			}
			
			return false;
		}
		
		// Up
		if (direction.IsUp())
		{
			for (int row = _curRow + 1; row < _row; row++)
			{
				if (_types[row, _curColumn] != FootholdType.None)
				{
					nextRow    = row;
					nextColumn = _curColumn;
					
					return true;
				}
			}
			
			return false;
		}
		
		return false;
	}

	Vector3 GetPosition(int row, int column)
	{
		float width  = _column * cellSize;
		float height = _row    * cellSize;
		
		float x = transform.position.x - width  * 0.5f + (column + 0.5f) * cellSize;
		float y = transform.position.y - height * 0.5f + (row    + 0.5f) * cellSize;
		
		return new Vector3(x, y, 0);
	}
	
	void OnDrawGizmos()
	{
		if (_row <= 0 || _column <= 0) return;

		// Set color
		Gizmos.color = Color.white;
		
		// Bottom-left position
		float bottom = -_row    * cellSize * 0.5f;
		float left   = -_column * cellSize * 0.5f;
		
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
		for (int i = 0; i <= _column; i++)
		{
			from.x = to.x = x;
			
			Gizmos.DrawLine(from, to);
			
			x += cellSize;
		}
		
		//
		from.x = left;
		to.x   = right;
		
		// Draw horizontal lines
		for (int i = 0; i <= _row; i++)
		{
			from.y = to.y = y;
			
			Gizmos.DrawLine(from, to);
			
			y += cellSize;
		}
	}
}
