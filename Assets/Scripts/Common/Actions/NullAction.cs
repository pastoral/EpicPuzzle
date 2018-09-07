using UnityEngine;

public class NullAction : BaseAction
{
	private static NullAction _instance = new NullAction();

	public static NullAction Instance
	{
		get
		{
			return _instance;
		}
	}	
}
