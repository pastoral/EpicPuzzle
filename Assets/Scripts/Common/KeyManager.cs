using UnityEngine;
using System.Collections.Generic;

public class KeyManager : Singleton<KeyManager>
{
	public delegate void KeyEventHandler();

//	private static KeyEventHandler _back;

	private static List<KeyEventHandler> _backs = new List<KeyEventHandler>();

	public static void AddBackEventHandler(KeyEventHandler handler)
	{
//		_back += handler;

		_backs.Add(handler);
	}

	public static void RemoveBackEventHandler(KeyEventHandler handler)
	{
//		_back -= handler;

		_backs.Remove(handler);
	}

#if !UNITY_IOS
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
//			if (_back != null)
//			{
//				_back();
//			}

			int count = _backs.Count;

			if (count > 0)
			{
				KeyEventHandler handler = _backs[count - 1];
//				_backs.Remove(handler);

				if (handler != null)
				{
					handler();
				}
			}
		}
	}
#endif
}
