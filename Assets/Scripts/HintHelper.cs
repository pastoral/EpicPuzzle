using UnityEngine;

public class HintHelper
{
	private static readonly Direction[] Directions = { Direction.Left, Direction.Up, Direction.Right, Direction.Down };

	private int _row;
	private int _column;
	private FootholdType[,] _types;

	private int _curRow;
	private int _curColumn;
	private Direction _curDirection;

	private int _total;
	private int _count;

	private Direction _direction;
	private bool _isDone;

	public HintHelper(int row, int column)
	{
		// Set number of rows
		_row = row;

		// Set number of columns
		_column = column;

		// Create array of foothold types
		_types = new FootholdType[_row, _column];
	}

	public Direction Hint(Foothold[,] footholds, int startRow, int startColumn, Direction startDirection)
	{
		// Reset total
		_total = 0;

		// Set footholds
		for (int i = 0; i < _row; i++)
		{
			for (int j = 0; j < _column; j++)
			{
				Foothold foothold = footholds[i, j];

				if (foothold != null)
				{
					// Set type
					_types[i, j] = foothold.NormalizeType;

					// Increase total
					_total += foothold.GetCount();
				}
				else
				{
					_types[i, j] = FootholdType.None;
				}
			}
		}

		_curRow       = startRow;
		_curColumn    = startColumn;
		_curDirection = startDirection;

		// Reset count
		_count = 0;

		//
		_isDone = false;

		Try(true);

		return _isDone ? _direction : Direction.None;
	}

	void Try(bool isFirst)
	{
		int nextRow    = -1;
		int nextColumn = -1;

		for (int i = 0; i < 4; i++)
		{
			if (_isDone) return;

			Direction dir = Directions[i];

			if (isFirst)
			{
				_direction = dir;
			}

			if (!dir.IsOpposite(_curDirection))
			{
				if (NextCell(dir, ref nextRow, ref nextColumn))
				{
					int row = _curRow;
					int column = _curColumn;
					Direction direction = _curDirection;

					FootholdType type1 = _types[_curRow, _curColumn];
					FootholdType type2 = _types[nextRow, nextColumn];

					// Set direction
					_curDirection = dir;

					// Update current cell
					if (type1.IsDouble())
					{
						_types[_curRow, _curColumn] = FootholdType.Normal;
						
						// Increase counter
						_count++;
					}
					else if (!type1.IsNone())
					{
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
						_isDone = true;
						return;
					}
					else
					{
						Try(false);
					}

					// Restore cell
					_curRow    = row;
					_curColumn = column;

					// Restore type
					if (!type1.IsNone())
					{
						_types[row, column] = type1;
						
						// Decrease counter
						_count--;
					}

					// Restore direction
					_curDirection = direction;
				}
			}
		}
	}

	bool NextCell(Direction direction, ref int nextRow, ref int nextColumn)
	{
		// Left
		if (direction.IsLeft())
		{
			for (int column = _curColumn - 1; column >= 0; column--)
			{
				if (!_types[_curRow, column].IsNone())
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
				if (!_types[_curRow, column].IsNone())
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
				if (!_types[row, _curColumn].IsNone())
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
				if (!_types[row, _curColumn].IsNone())
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
}
