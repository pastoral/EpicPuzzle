using UnityEngine;

public class UndoData
{
	public int row;
	public int column;
	public FootholdType foothold;
	public Direction direction;
	
	public UndoData(int row, int column, FootholdType foothold, Direction direction)
	{
		this.row 	   = row;
		this.column    = column;
		this.foothold  = foothold;
		this.direction = direction;
	}
	
	public override string ToString()
	{
		return string.Format("[Undo: row={0}, column={1}, foothold={2}, direction={3}]", row, column, foothold, direction);
	}
}
