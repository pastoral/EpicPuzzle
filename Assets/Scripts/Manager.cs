#define GUI_ENABLED

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using System;

public class Manager : Singleton<Manager>
{
	/// <summary>
	/// The message prefab.
	/// </summary>
	public GameObject messagePrefab;

	/// <summary>
	/// The confirm prefab.
	/// </summary>
	public GameObject confirmPrefab;

	/// <summary>
	/// The watch video reward info prefab
	/// </summary>
	public GameObject watchVideoReawardPrefab;

	/// <summary>
	/// The notification prefab.
	/// </summary>
	public GameObject notificationPrefab;

	// The user data (cache)
	private UserData _userData;

	// The friend data (cache)
	private FriendData _friendData;

	public GoogleAnalyticsV4 analytics;

	// The top dialog
	private DialogBehaviour _dialog;

	// The list of pending notifications
	private static Queue<string> _notifications = new Queue<string>(4);

	// The FB send request updater
	private FixedUpdater _sendRequestUpdater = new FixedUpdater(20.0f);

	// The invited friend updater
	private FixedUpdater _invitedFriendUpdater = new FixedUpdater(20.0f);

	public bool isShowNewVersion = false;
	public bool isShowMoreGame = false;
	public int checkShowAdsTimes = 0;
	public bool isShowRateGame = false;
	public bool isShowLoginFB = false;

	public DialogBehaviour Dialog
	{
		get
		{
			return _dialog;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		// Set FPS to 60
		Application.targetFrameRate = 60;

#if UNITY_EDITOR
		// Disable reporter
		Reporter reporter = FindObjectOfType<Reporter>();

		if (reporter != null)
		{
			reporter.gameObject.SetActive(false);
		}
#endif

		// Set user data
		_userData = UserData.Instance;

		// Set friend data
		_friendData = FriendData.Instance;

		_sendRequestUpdater.UpdateCallback = UpdateSendRequests;
		_invitedFriendUpdater.Play(UpdateInvitedFriend);
	}

	void OnApplicationPause(bool pauseStatus)
	{
		//Log.Debug(pauseStatus ? "OnApplicationPause" : "OnApplicationResume");

//		if (!pauseStatus)
//		{
//			FBHelper.Init();
//		}
	}

	void OnApplicationQuit()
	{
		//Log.Debug("OnApplicationQuit");

		// Save data
		if (_userData != null)
		{
			_userData.Save();
		}
	}

	public void ShowMessage(string title, string message, Action callback = null)
	{
		GameObject dialog = CreateDialog(messagePrefab);

		var script = dialog.GetComponent<MessageScript>();
		_dialog = script;

		script.Construct(title, message, () => {
			_dialog = null;

			if (callback != null)
			{
				callback();
			}
		});
	}
	
	public void ShowMessage(string message, Action callback = null)
	{
		GameObject dialog = CreateDialog(messagePrefab);

		var script = dialog.GetComponent<MessageScript>();
		_dialog = script;

		script.Construct(message, () => {
			_dialog = null;

			if (callback != null)
			{
				callback();
			}
		});
	}

	public void ShowMessage(string title, string message, Sprite sprite, Action callback = null)
	{
		GameObject dialog = CreateDialog(messagePrefab);

		var script = dialog.GetComponent<MessageScript>();
		_dialog = script;

		script.Construct(title, message, sprite, () => {
			_dialog = null;

			if (callback != null)
			{
				callback();
			}
		});
	}

	public void ShowMessage(string message, Sprite sprite, Action callback = null)
	{
		GameObject dialog = CreateDialog(messagePrefab);

		var script = dialog.GetComponent<MessageScript>();
		_dialog = script;

		script.Construct(message, sprite, () => {
			_dialog = null;

			if (callback != null)
			{
				callback();
			}
		});
	}

	public void ShowConfirm(string title, string message, Action<bool> callback = null)
	{
		GameObject dialog = CreateDialog(confirmPrefab);

		var script = dialog.GetComponent<ConfirmScript>();
		_dialog = script;

		script.Construct(title, message, (yes) => {
			_dialog = null;

			if (callback != null)
			{
				callback(yes);
			}
		});
	}
	


	public void ForceCloseDialog()
	{
		if (_dialog != null)
		{
			Destroy(_dialog.gameObject);
			_dialog = null;
		}
	}

	public void ShowWatchVideoCoinRewad(Action callback)
	{
		GameObject dialog = CreateDialog(watchVideoReawardPrefab);

		var script = dialog.GetComponent<WatchVideoRewardScript>();
		script.Construct("You got "+ Settings.CoinToWatchVideo + " coins for watching video",
			() => {
				if (callback != null)
				{
					callback();
				}
			}
		);
	}

	public void ShowWatchVideoManaRewad(Action callback)
	{
		GameObject dialog = CreateDialog(watchVideoReawardPrefab);

		var script = dialog.GetComponent<WatchVideoRewardScript>();
		script.Construct("You got "+ Settings.ManaToWatchVideo + " mana for watching video",
			() => {
				if (callback != null)
				{
					callback();
				}
			}
		);
	}

	public void ShowNotification(string message)
	{
		if (_notifications.Count > 0)
		{
			_notifications.Enqueue(message);
		}
		else
		{
			NotificationPopup notification = FindObjectOfType<NotificationPopup>();

			if (notification != null)
			{
				if (notification.Showing)
				{
					_notifications.Enqueue(message);
				}
				else
				{
					ShowNotificationInternal(message);
				}
			}
			else
			{
				ShowNotificationInternal(message);
			}
		}
	}

	void ShowNotificationInternal(string message)
	{
		NotificationPopup notification = FindObjectOfType<NotificationPopup>();

		if (notification == null)
		{
			// Get canvas
			Canvas canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
			Debug.Log("zzz" + canvas.name);

			// Create popup
			GameObject popup = Instantiate(notificationPrefab);
			popup.transform.SetParent(canvas.transform);
			popup.transform.localScale = Vector3.one;

			RectTransform rectTransform = notificationPrefab.GetComponent<RectTransform>();

			// Reset transform
			RectTransform popupRectTransform = popup.GetComponent<RectTransform>();
			popupRectTransform.anchoredPosition3D = rectTransform.anchoredPosition3D;
			popupRectTransform.sizeDelta = rectTransform.sizeDelta;

			notification = popup.GetComponent<NotificationPopup>();
		}

		notification.Show(message, NextNotification);
	}

	void NextNotification()
	{
		if (_notifications.Count > 0)
		{
			if (_sendRequestUpdater.Playing)
			{
				ShowNotificationInternal(_notifications.Dequeue());
			}
		}
	}

	public void SetUpdateSendRequests(bool enabled)
	{
		if (enabled)
		{
			if (_notifications.Count > 0)
			{
				ShowNotificationInternal(_notifications.Dequeue());
			}

			_sendRequestUpdater.Play();
		}
		else
		{
			_sendRequestUpdater.Stop();
		}
	}

	GameObject CreateDialog(GameObject prefab)
	{
		// Get canvas
		Canvas canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

		// Create dialog
		GameObject dialog = Instantiate(prefab);
		dialog.transform.SetParent(canvas.transform);
		dialog.transform.localScale = Vector3.one;

		// Reset transform
		RectTransform popupRectTransform = dialog.GetComponent<RectTransform>();
		popupRectTransform.anchoredPosition3D = Vector3.zero;
		popupRectTransform.sizeDelta = Vector2.zero;

		return dialog;
	}

	public static void SetTouchEnabled(bool enabled)
	{
		TouchManager.Instance.Enabled = enabled;
		
		if (EventSystem.current != null)
		{
			EventSystem.current.enabled = enabled;
		}
	}

	public void AddSchedule(Action callback, float delay)
	{
		StartCoroutine(Schedule(callback, delay));
	}

	IEnumerator Schedule(Action callback, float delay)
	{
		yield return new WaitForSeconds(delay);

		callback();
	}

	void Update()
	{
		// Update user data
		_userData.Update();

		// Update friend data
		_friendData.Update();

		if (FB.IsLoggedIn)
		{
			// Update invited friend
			_invitedFriendUpdater.Update();

			// Update send requests
			_sendRequestUpdater.Update();
		}
	}

	void UpdateInvitedFriend()
	{
		_invitedFriendUpdater.Stop();

		FriendData.Instance.CheckCoinInvitedFriend((coinName) => {
			if (!string.IsNullOrEmpty(coinName))
			{
				// Add coin
				UserData.Instance.Coin += Settings.CoinByInvite;

				ShowNotification(string.Format("You get {0} coins by invite {1}", Settings.CoinByInvite, coinName));

				NotificationManager.OnCoinChanged(UserData.Instance.Coin);

				_invitedFriendUpdater.Play();
			}
			else
			{
				FriendData.Instance.CheckManaInvitedFriend((manaName) => {
					if (!string.IsNullOrEmpty(manaName))
					{
						// Add mana
						UserData.Instance.Mana += Settings.ManaByInvite;

						ShowNotification(string.Format("You get {0} mana by invite {1}", Settings.ManaByInvite, manaName));

						NotificationManager.OnManaChanged(UserData.Instance.Mana);
					}

					_invitedFriendUpdater.Play();
				});
			}
		});
	}

	void UpdateSendRequests()
	{
		FBHelper.GetSendRequests((result, error) => {
			if (!string.IsNullOrEmpty(error)) return;
			if (result == null) return;

			RequestData[] requests = result.ToArray();
			int requestCount = requests.Length;
			if (requestCount == 0) return;

			// Send coin
			int totalCoin = 0;
			List<string> sendCoinNames = new List<string>(requestCount);

			for (int i = 0; i < requestCount; i++)
			{
				RequestData request = requests[i];

				if (request.ObjectType == FBObjectType.Coin)
				{
					totalCoin++;

					sendCoinNames.Add(request.FromName);

					FBHelper.DeleteRequest(request.Id);
				}
//				else if (request.ObjectType == FBObjectType.InviteCoin)
//				{
//					totalCoin += Settings.CoinByInvite;
//
//					sendCoinNames.Add(request.FromName);
//
//					FBHelper.DeleteRequest(request.Id);
//				}
			}

			if (totalCoin > 0)
			{
				// Add coins
				UserData.Instance.Coin += totalCoin;

				ShowNotification(string.Format("You get {0} {1} from {2}.", totalCoin, totalCoin > 1 ? "coins" : "coin", sendCoinNames.ToUniqueNames()));
			}

			// Send mana
			int totalMana = 0;
			List<string> sendManaNames = new List<string>(requestCount);

			for (int i = 0; i < requestCount; i++)
			{
				RequestData request = requests[i];

				if (request.ObjectType == FBObjectType.Mana)
				{
					totalMana++;

					sendManaNames.Add(request.FromName);

					FBHelper.DeleteRequest(request.Id);
				}
//				else if (request.ObjectType == FBObjectType.InviteMana)
//				{
//					totalMana += Settings.ManaByInvite;
//
//					sendManaNames.Add(request.FromName);
//
//					FBHelper.DeleteRequest(request.Id);
//				}
			}

			if (totalMana > 0)
			{
				// Add mana
				UserData.Instance.Mana += totalMana;

				ShowNotification(string.Format("You get {0} {1} from {2}.", totalMana, totalMana > 1 ? "manas" : "mana", sendManaNames.ToUniqueNames()));
			}
		});
	}

#if GUI_ENABLED
	private bool _isFirstTime = true;

	void OnGUI()
	{
		if (_isFirstTime)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				// Bigger controls for higher mobile devices
//				if (Screen.width >= 1500 || Screen.height >= 1500)
				{
					GUI.skin.button.fixedHeight = 60;
					GUI.skin.toggle.fixedHeight = 60;
				}
			}

			_isFirstTime = false;
		}
	}
#endif
}
