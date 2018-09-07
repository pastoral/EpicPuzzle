using UnityEngine;
using System.Collections;

public class NotificationManager
{
	#region Delegates
	
	public delegate void ManaEventHandler(int mana);
	public delegate void CoinEventHandler(int coin);

	#endregion

	#region Event Handlers
	
	private static event ManaEventHandler _OnManaChanged;
	private static event CoinEventHandler _OnCoinChanged;

	#endregion

	public static void AddManaEventHandler(ManaEventHandler handler)
	{
		_OnManaChanged += handler;
	}
	
	public static void RemoveManaEventHandler(ManaEventHandler handler)
	{
		_OnManaChanged -= handler;
	}
	
	public static void AddCoinEventHandler(CoinEventHandler handler)
	{
		_OnCoinChanged += handler;
	}
	
	public static void RemoveCoinEventHandler(CoinEventHandler handler)
	{
		_OnCoinChanged -= handler;
	}

	public static void ManaChanged(int mana)
	{
		UserData.Instance.Mana = mana;

		if (_OnManaChanged != null)
		{
			_OnManaChanged(mana);
		}
	}
	
	public static void CoinChanged(int coin)
	{
		UserData.Instance.Coin = coin;

		if (_OnCoinChanged != null)
		{
			_OnCoinChanged(coin);
		}
	}

	public static void OnManaChanged(int mana)
	{
		if (_OnManaChanged != null)
		{
			_OnManaChanged(mana);
		}
	}

	public static void OnCoinChanged(int coin)
	{
		if (_OnCoinChanged != null)
		{
			_OnCoinChanged(coin);
		}
	}
}
