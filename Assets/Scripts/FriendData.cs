using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class FriendData
{
	private static readonly string FileName = "friend.data";
	private static readonly char[] Separators = { ',' };

	#region Singleton

	private static FriendData _instance;

	public static FriendData Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FriendData();

				FriendData data = new FriendData();

				if (Helper.Load<FriendData>(FileName, ref data))
				{
					_instance._strCoinInvitedFriends = data._strCoinInvitedFriends;
					_instance._strManaInvitedFriends = data._strManaInvitedFriends;

					_instance.OnLoaded();
				}
//				else
//				{
//					_instance.Reset();
//				}
			}
			
			return _instance;
		}
	}

	#endregion

	// The current version
	private int _version = 1;

	// The invited friends to get coin
	private string _strCoinInvitedFriends;

	// The invited friends to get mana
	private string _strManaInvitedFriends;

	// The list of invited friends to get coin
	[NonSerialized]
	private List<string> _coinInvitedFriends = new List<string>();

	// The list of invited friends to get mana
	[NonSerialized]
	private List<string> _manaInvitedFriends = new List<string>();
	
	// True if dirty
	[NonSerialized]
	private bool _isDirty;

//	public FriendData()
//	{
//		_coinInvitedFriends = new List<string>();
//		_manaInvitedFriends = new List<string>();
//	}

//	public List<string> CoinInvitedFriends
//	{
//		get { return _coinInvitedFriends; }
//	}
//
//	public List<string> ManaInvitedFriends
//	{
//		get { return _manaInvitedFriends; }
//	}

	public bool AddCoinInvitedFriend(string uid)
	{
		if (_coinInvitedFriends.Contains(uid))
		{
			return false;
		}

		if (_manaInvitedFriends.Contains(uid))
		{
			return false;
		}

		_coinInvitedFriends.Add(uid);
		_isDirty = true;

		return true;
	}

	public bool RemoveCoinInvitedFriend(string uid)
	{
		if (_coinInvitedFriends.Remove(uid))
		{
			_isDirty = true;
			return true;
		}

		return false;
	}

	public bool AddManaInvitedFriend(string uid)
	{
		if (_manaInvitedFriends.Contains(uid))
		{
			return false;
		}

		if (_coinInvitedFriends.Contains(uid))
		{
			return false;
		}

		_manaInvitedFriends.Add(uid);
		_isDirty = true;

		return true;
	}

	public bool RemoveManaInvitedFriend(string uid)
	{
		if (_manaInvitedFriends.Remove(uid))
		{
			_isDirty = true;
			return true;
		}

		return false;
	}

	void OnLoaded()
	{
		if (!string.IsNullOrEmpty(_strCoinInvitedFriends))
		{
			_coinInvitedFriends.AddRange(_strCoinInvitedFriends.Split(Separators, StringSplitOptions.RemoveEmptyEntries));
		}

		if (!string.IsNullOrEmpty(_strManaInvitedFriends))
		{
			_manaInvitedFriends.AddRange(_strManaInvitedFriends.Split(Separators, StringSplitOptions.RemoveEmptyEntries));
		}
	}

	public void Reset()
	{
		// Clear invited friends to get coin
		_strCoinInvitedFriends = "";

		// Clear invited friends to get mana
		_strManaInvitedFriends = "";

		_coinInvitedFriends.Clear();
		_manaInvitedFriends.Clear();

		// Set dirty
		_isDirty = true;
	}
	
	public bool Save()
	{
		if (_isDirty)
		{
			Pack();
			_isDirty = false;
		}

		return Helper.Save<FriendData>(this, FileName);
	}

	public void Update()
	{
		if (_isDirty)
		{
			Pack();

			if (Helper.Save<FriendData>(this, FileName))
			{
				_isDirty = false;
			}
		}
	}

	public void CheckCoinInvitedFriend(Action<string> callback)
	{
		CheckCoinInvitedFriend(0, callback);
	}

	void CheckCoinInvitedFriend(int index, Action<string> callback)
	{
		if (index < _coinInvitedFriends.Count)
		{
			string uid = _coinInvitedFriends[index];

			FBHelper.GetAppUserName(uid, (name) => {
				if (string.IsNullOrEmpty(name))
				{
					if (index < _coinInvitedFriends.Count - 1)
					{
						CheckCoinInvitedFriend(index + 1, callback);
					}
					else
					{
						callback(null);
					}
				}
				else
				{
					RemoveCoinInvitedFriend(uid);

					callback(name);
				}
			});
		}
		else
		{
			callback(null);
		}
	}

	public void CheckManaInvitedFriend(Action<string> callback)
	{
		CheckManaInvitedFriend(0, callback);
	}

	void CheckManaInvitedFriend(int index, Action<string> callback)
	{
		if (index < _manaInvitedFriends.Count)
		{
			string uid = _manaInvitedFriends[index];

			FBHelper.GetAppUserName(uid, (name) => {
				if (string.IsNullOrEmpty(name))
				{
					if (index < _manaInvitedFriends.Count - 1)
					{
						CheckManaInvitedFriend(index + 1, callback);
					}
					else
					{
						callback(null);
					}
				}
				else
				{
					RemoveManaInvitedFriend(uid);

					callback(name);
				}
			});
		}
		else
		{
			callback(null);
		}
	}

	public override string ToString()
	{
		return string.Format("Coin:{0} Mana:{1}", _strCoinInvitedFriends, _strManaInvitedFriends);
	}

	void Pack()
	{
		if (_coinInvitedFriends.Count > 0)
		{
			_strCoinInvitedFriends = string.Join(",", _coinInvitedFriends.ToArray());
		}
		else
		{
			_strCoinInvitedFriends = "";
		}

		if (_manaInvitedFriends.Count > 0)
		{
			_strManaInvitedFriends = string.Join(",", _manaInvitedFriends.ToArray());
		}
		else
		{
			_strManaInvitedFriends = "";
		}
	}
}
