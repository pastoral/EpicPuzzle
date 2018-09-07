using UnityEngine;

public class KeyDirection
{
	public int key;
	public Direction direction;
	
	public KeyDirection(int key, Direction direction)
	{
		this.key = key;
		this.direction = direction;
	}
	
	public override string ToString()
	{
		return string.Format("({0}, {1})", key.ToString(), direction.ToString());
	}
}
