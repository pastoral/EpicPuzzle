using UnityEngine;

public class MapSolution
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

	private int _counter;

	public int Resolve(MapData mapData)
	{
		int[,] footholds = mapData.footholds;

		_row    = footholds.GetRow();
		_column = footholds.GetColumn();

		// Create array of foothold types
		_types = new FootholdType[_row, _column];
		
		// Reset total
		_total = 0;

		// Set footholds
		for (int i = 0; i < _row; i++)
		{
			int row = _row - 1 - i;

			for (int j = 0; j < _column; j++)
			{
				FootholdType type = footholds[i, j].ToFootholdType();

				_types[row, j] = type;

				if (type != FootholdType.None)
				{
					if (type == FootholdType.Double)
					{
						_total += 2;
					}
					else
					{
						_total++;
					}
				}
			}
		}

		_curRow       = _row - 1 - mapData.startRow;
		_curColumn    = mapData.startColumn;
		_curDirection = mapData.direction;

		// Reset count
		_count = 0;

		// Reset counter
		_counter = 0;

		Try();

		return _counter;
	}

	void Try()
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
					int row = _curRow;
					int column = _curColumn;
					Direction direction = _curDirection;

					FootholdType type1 = _types[_curRow, _curColumn];
					FootholdType type2 = _types[nextRow, nextColumn];

					// Set direction
					_curDirection = dir;

					// Update current cell
					if (type1 == FootholdType.Double)
					{
						_types[_curRow, _curColumn] = FootholdType.Normal;
						
						// Increase counter
						_count++;
					}
					else if (type1 != FootholdType.None)
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
						_counter++;
					}
					else
					{
						Try();
					}

					// Restore cell
					_curRow    = row;
					_curColumn = column;

					// Restore type
					if (type1 != FootholdType.None)
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
}
