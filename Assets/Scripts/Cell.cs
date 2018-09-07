using UnityEngine;
using System.Collections;

public struct Cell
{
	public int row;
	public int column;
	
	public Cell(int row, int column)
	{
		this.row    = row;
		this.column = column;
	}

	public bool Is(int row, int column)
	{
		return this.row == row && this.column == column;
	}

	public override string ToString()
	{
		return string.Format("({0},{1})", row, column);
	}
}
