using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
	// The mana time
	private static long _manaTime;

	// The mana date time
	private static DateTime _manaDateTime;

	// The mana countdown
	private static float _manaCountdown;

	// The mana fixed time
	private float _manaFixedTime;
	
	void Awake()
	{
		NotificationManager.AddManaEventHandler(OnManaChanged);
	}

	void Start()
	{
		// Get mana time
		_manaTime = UserData.Instance.ManaTime;

		// Set mana date time
		if (_manaTime > 0)
		{
			_manaDateTime = DateTime.FromFileTime(_manaTime);
//			Log.Debug("Last mana time: " + _manaDateTime);
		}

		OnManaChanged(UserData.Instance.Mana);
	}

	void Update()
	{
		if (_manaCountdown > 0)
		{
			_manaFixedTime += Time.deltaTime;

			// Update mana countdown each 1 second
			if (_manaFixedTime >= 1f)
			{
				// Reset time
				_manaFixedTime = 0;

				// Calculate passed seconds
				float seconds = (float)(DateTime.Now - _manaDateTime).TotalSeconds;

				SetManaCountdown(Settings.ManaDelayTime - seconds);
			}
		}
	}

	void SetManaCountdown(float countdown)
	{
		_manaCountdown = countdown;
		
		if (_manaCountdown <= 0)
		{
			// Get current mana
			int mana = UserData.Instance.Mana;
			
			if (mana < Settings.MaxMana)
			{
				do
				{
					// Increase mana
					mana++;
					
					if (mana == Settings.MaxMana)
					{
						break;
					}
					else
					{
						_manaCountdown += Settings.ManaDelayTime;

						if (_manaTime > 0)
						{
							_manaDateTime = _manaDateTime.AddSeconds(Settings.ManaDelayTime);
						}
						else
						{
							_manaDateTime = DateTime.Now.AddSeconds(Settings.ManaDelayTime);
						}

						_manaTime = _manaDateTime.ToFileTime();

						UserData.Instance.ManaTime = _manaTime;
					}
				}
				while (_manaCountdown <= 0);

				// Notify mana changed
				NotificationManager.ManaChanged(mana);
			}
		}
	}

	void OnManaChanged(int mana)
	{
		if (mana < Settings.MaxMana)
		{
			// Check if not being countdown mana
			if (_manaCountdown <= 0)
			{
				if (_manaTime <= 0)
				{
					_manaDateTime = DateTime.Now;
					_manaTime = _manaDateTime.ToFileTime();

					UserData.Instance.ManaTime = _manaTime;

					SetManaCountdown(Settings.ManaDelayTime);
				}
				else
				{
					float seconds = (float)(DateTime.Now - _manaDateTime).TotalSeconds;

					SetManaCountdown(Settings.ManaDelayTime - seconds);
				}
			}
		}
		// Full mana
		else
		{
			// Stop countdown mana
			_manaCountdown = 0;

			// Reset mana time
			_manaTime = 0;

			UserData.Instance.ManaTime = _manaTime;
		}
	}

	public static float ManaCountdown
	{
		get
		{
			return _manaCountdown;
		}
	}

#if UNITY_EDITOR
	public static void CheatManaCountdown()
	{
		float remainingSeconds = 5.0f;

		if (_manaCountdown > remainingSeconds)
		{
			if (_manaTime > 0)
			{
				_manaDateTime = _manaDateTime.AddSeconds(remainingSeconds - _manaCountdown);
			}
			else
			{
				_manaDateTime = DateTime.Now.AddSeconds(remainingSeconds - _manaCountdown);
			}

			_manaTime = _manaDateTime.ToFileTime();

			UserData.Instance.ManaTime = _manaTime;
		}
	}
#endif
}
