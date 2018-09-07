#if UNITY_EDITOR
#define SHOW_DEBUG
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class MapSelectionScript : MonoBehaviour
{
	private static readonly Vector3 disableColor = new Vector3(0.5f, 0.5f, 0.5f);
	private static readonly float jumpDelay      = 1.0f;
	private static readonly float mapInfoDelay   = 0.1f;

	/// <summary>
	/// The map.
	/// </summary>
	public MapScript map;

	[Header("Info")]

	// The mana text
	[SerializeField]
	private Text _manaText;

	// The mana countdown
	[SerializeField]
	private Text _manaCountdown;

	// The coin text
	[SerializeField]
	private Text _coinText;

	// The buy mana popup prefab
	[SerializeField]
	private GameObject _buyManaPopupPrefab;

	// The buy coin popup prefab
	[SerializeField]
	private GameObject _buyCoinPopupPrefab;

	// The no more mana popup prefab
	[SerializeField]
	private GameObject _noMoreManaPopupPrefab;

	[Header("Map Info")]

	// The Map Info popup
	[SerializeField]
	private GameObject _mapInfoPopup;

	// The top friends
	[SerializeField]
	private TopFriendsScript _topFriends;

	// The level
	[SerializeField]
	private AtlasImage _level;

	// Cutscene 
	[SerializeField]
	GameObject cutscene2;
	[SerializeField]
	GameObject cutscene3;

	// The user data (cache)
	private UserData _userData;

	// The last mana countdown
	private float _lastManaCountdown;

	// The selected map (one-based)
	private int _selectedMap;

	// The canvas
	[SerializeField]
	private Canvas _canvas;

	// The run mana action
	private ActionScript _runManaAction;

	string appstoreLink = "";
	string googlePlayLink = "https://play.google.com/store/apps/details?id=com.puzzlegame.thepuzzlefree";

	void OnEnable()
	{
		NotificationManager.AddManaEventHandler(OnManaChanged);
		NotificationManager.AddCoinEventHandler(OnCoinChanged);

		map.AddEventHandler(OnMapSelected);

		KeyManager.AddBackEventHandler(OnKeyBack);
	}

	void OnDisable()
	{
		NotificationManager.RemoveManaEventHandler(OnManaChanged);
		NotificationManager.RemoveCoinEventHandler(OnCoinChanged);

		map.RemoveEventHandler(OnMapSelected);

		KeyManager.RemoveBackEventHandler(OnKeyBack);
	}

	void OnKeyBack()
	{
		Back();
	}

	public void UpdateStatus()
	{
		//Debug.Log("update status ...");
		OnManaChanged(UserData.Instance.Mana);
		OnCoinChanged(UserData.Instance.Coin);
	}

	void Start()
	{
		// Set user data
		_userData = UserData.Instance;

		//TODO
		SoundManager.Instance.StopMusic();

		// Get canvas
		//_canvas = GameObject.FindObjectOfType<Canvas>();

		_lastManaCountdown = TimeManager.ManaCountdown;
		OnManaCountdownChanged(_lastManaCountdown);

		// Update mana
		OnManaChanged(_userData.Mana);

		// Update coin
		OnCoinChanged(_userData.Coin);

		// Check if show cutscene
		if (!_userData.PlayedCutscene2 && _userData.Level == 34)
		{
			Invoke("ShowCutscene2", jumpDelay);
		}

		if (!_userData.PlayedCutscene3 && _userData.Level == 67)
		{
			Invoke("ShowCutscene3", jumpDelay);
		}

		// Check if new level
		if (_userData.NewLevel)
		{
			// Disable interaction
			SetInteractable(false);

			Invoke("UnlockMap", jumpDelay);
		}
		// Check if next level
		else if (_userData.NextLevel)
		{
			// Disable interaction
			SetInteractable(false);
			
			Invoke("NextMap", jumpDelay);
		}

		// Check if show rate game popup
		if (!Manager.Instance.isShowRateGame
			&& UserData.Instance.ShowRateGameTime < 10
			&& UserData.Instance.Map > 5)
		{
			Manager.Instance.isShowRateGame = true;
			UserData.Instance.ShowRateGameTime++;
			Manager.Instance.ShowConfirm("Feedback", "Enjoy to play? Please rate it and give us your feedback? Thanks for your support!",
				(isOK) => {
					if (isOK)
					{
						UserData.Instance.ShowRateGameTime = 10;
						Debug.Log("Giving feedback ...");
						#if UNITY_ANDROID
						Application.OpenURL(googlePlayLink);
						#else
						Application.OpenURL(appstoreLink);
						#endif
					}
					else 
					{
						Debug.Log("Dont give feedback");
					}
				});
		}

		Manager.Instance.analytics.LogScreen("Map Selection");
		Manager.Instance.SetUpdateSendRequests(true);

		MyAdmob.Instance.ShowBanner();
	}

	void ShowCutscene2()
	{
		_userData.PlayedCutscene2 = true;
		cutscene2.SetActive(true);
	}

	void ShowCutscene3()
	{
		_userData.PlayedCutscene3 = true;
		cutscene3.SetActive(true);
	}

	void UnlockMap()
	{
		int newMap = _userData.Level;

		map.UnlockMap(newMap, () => {
			// Reset new level
			_userData.NewLevel = false;

			// Set current map
			_userData.Map = newMap;

			// Set selected map
			_selectedMap = newMap;

			Invoke("UnlockMapCallback", mapInfoDelay);
		});
	}

	void UnlockMapCallback()
	{
		// Show map info
		ShowMapInfo(_selectedMap);
	}

	void NextMap()
	{
		int nextMap = _userData.Map + 1;
		
		map.NextMap(nextMap, () => {
			// Reset next level
			_userData.NextLevel = false;
			
			// Set current map
			_userData.Map = nextMap;
			
			// Set selected map
			_selectedMap = nextMap;
			
			Invoke("NextMapCallback", mapInfoDelay);
		});
	}
	
	void NextMapCallback()
	{
		// Show map info
		ShowMapInfo(_selectedMap);
	}

	public void AddMana()
	{
		if (_runManaAction != null) return;

		if (_userData.Mana >= Settings.MaxMana)
		{
			// Play sound
			SoundManager.PlayButtonClick();

			//Log.Debug("Full mana!");

			return;
		}

		// Play sound
		SoundManager.PlayButtonOpen();

		// Create popup
		GameObject buyManaPopup = _buyManaPopupPrefab.CreateUI(_canvas.transform);
		BuyManaPopupScript script = buyManaPopup.GetComponent<BuyManaPopupScript>();

		if (script != null)
		{
			script.Show(OnBuyManaCallback);
		}
	}

	public void AddCoin()
	{
		// Play sound
		SoundManager.PlayButtonOpen();

		// Create popup
		GameObject buyCoinPopup = _buyCoinPopupPrefab.CreateUI(_canvas.transform);
		BuyCoinPopupScript script = buyCoinPopup.GetComponent<BuyCoinPopupScript>();

		if (script != null)
		{
			script.Show();
		}
	}

	public void Invite()
	{
		// Play sound
		SoundManager.PlayButtonClick();
		
		if (!Helper.IsOnline())
		{
			map.Interactable = false;

			Manager.Instance.ShowMessage(Settings.NoInternetConnection, () => {
				map.Interactable = true;
			});

			return;
		}

		FBHelper.Invite(Settings.InviteTitle, Settings.InviteMessage, FriendType.Invitable, FBObjectType.Coin, (error) => {
			if (!string.IsNullOrEmpty(error))
			{
				//Log.Debug("Invite error: " + error);
				Manager.Instance.ShowMessage(Settings.InviteFailed);
			}
		});
	}

	public void Back()
	{
		Manager.Instance.SetUpdateSendRequests(false);

		if (_runManaAction != null)
		{
			_runManaAction.Stop(true);
			_runManaAction = null;
		}

		// Play sound
		SoundManager.PlayButtonOpen();
		
		TransitionManager.Instance.FadeTransitionScene(Settings.MainMenuScene);
	}

	void GoToMainGame()
	{
		Manager.Instance.SetUpdateSendRequests(false);

		// Set selected map
		_userData.Map = _selectedMap;

		// Go to main game
		TransitionManager.Instance.FadeTransitionScene(Settings.MainGameScene);
	}

	#region Map Info Popup
	
	public void Play()
	{
		if (_runManaAction != null)
		{
			_runManaAction.Stop(true);
			_runManaAction = null;
		}

		// Play sound
		SoundManager.PlayButtonOpen();

		// Double check if enough mana
		// Dont check mana
		//if (_userData.Mana > 0)
		{
			// Stop top friends
			_topFriends.Stop();

			// Hide Map Info popup
			_mapInfoPopup.Hide();

			// Decrease mana
			NotificationManager.ManaChanged(_userData.Mana - 1);

			// Go to main game
			GoToMainGame();
		}
	}
	
	public void CloseMapInfoPopup()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Stop top friends
		_topFriends.Stop();

		// Hide Map Info popup
		HidePopup(_mapInfoPopup);
	}
	
	#endregion

	void OnBuyManaCallback()
	{
		if (_userData.Mana < Settings.MaxMana)
		{
			// Play sound
			SoundManager.Instance.PlaySound(SoundID.Coin);

			// Decrease coin
			NotificationManager.CoinChanged(_userData.Coin - Settings.CoinToBuyFullMana);

			float duration = (Settings.MaxMana - _userData.Mana) / 20.0f;

			var action = LerpIntAction.Create(_userData.Mana, Settings.MaxMana, duration, (mana) => {
				NotificationManager.ManaChanged(mana);
			});

			_runManaAction = gameObject.Play(action, () => {
				_runManaAction = null;
			});
		}
		else
		{
			//Log.Debug("Full Mana!");
		}
	}

	void OnManaChanged(int mana)
	{
		if (_manaText != null)
		{
			_manaText.text = mana.ToString();
		}

		if (mana >= Settings.MaxMana)
		{
			if (_manaCountdown != null)
			{
				_manaCountdown.text = "FULL";
			}

			_lastManaCountdown = TimeManager.ManaCountdown;
		}

		if (mana > 0)
		{
			NoMoreManaPopupScript script = FindObjectOfType<NoMoreManaPopupScript>();

			if (script != null)
			{
				Manager.Instance.ForceCloseDialog();

				// Force close
				script.ForceClose();

				if (_selectedMap != _userData.Map)
				{
					// Disable interaction
					SetInteractable(false);

					map.JumpToMap(_selectedMap, () => {
						// Set current map
						_userData.Map = _selectedMap;

						// Enable interaction
						SetInteractable(true);

						// Show map info
						ShowMapInfo(_selectedMap);
					});
				}
				else
				{
					// Show map info
					ShowMapInfo(_selectedMap);
				}
			}
		}
	}

	void OnManaCountdownChanged(float manaCountdown)
	{
		_manaCountdown.text = manaCountdown.ToTimeString();
	}

	void OnCoinChanged(int coin)
	{
		//Debug.Log("Coin changed ...");
		if (_coinText != null)
		{
			_coinText.text = coin.ToString();
		}
	}

	void OnMapSelected(int map, bool unlocked)
	{
		if (unlocked)
		{
			// Set selected map
			_selectedMap = map;

			// Check if enough mana
			// Dont check mana
			if (true)
			{
				// Play sound
				SoundManager.PlayButtonClick();

				if (map != _userData.Map)
				{
					// Disable interaction
					SetInteractable(false);

					this.map.JumpToMap(map, () => {
						// Set current map
						_userData.Map = map;

						// Enable interaction
						SetInteractable(true);

						// Show map info
						ShowMapInfo(map);
					});
				}
				else
				{
					// Show map info
					ShowMapInfo(map);
				}
			}
			else
			{
				// Play sound
				SoundManager.Instance.PlaySound(SoundID.CyrusWrongInput);

				// Create popup
				GameObject noMoreManaPopup = _noMoreManaPopupPrefab.CreateUI(_canvas.transform);
				NoMoreManaPopupScript script = noMoreManaPopup.GetComponent<NoMoreManaPopupScript>();

				if (script != null)
				{
					script.Show(OnBuyManaCallback);
				}
			}
		}
		else
		{
			SoundManager.Instance.PlaySound(SoundID.SelectLockedMap);
		}
	}

	void ShowMapInfo(int map)
	{
		// Set level
		_level.Text = string.Format("L {0:00}", map);
		
		// Show Map Info popup
//		ShowPopup(_mapInfoPopup);

		// Disable interaction
		SetInteractable(false);

		UIHelper.ShowPopup(_mapInfoPopup, () => {
			// Enable UI
			SetUIEnabled(true);

			if (Helper.IsOnline() && FB.IsLoggedIn)
			{
				// Show top friends
				_topFriends.ShowTopFriends(map);
			}
		});
	}

	void LateUpdate()
	{
		float manaCountdown = TimeManager.ManaCountdown;

		if (manaCountdown != _lastManaCountdown)
		{
			_lastManaCountdown = manaCountdown;

			OnManaCountdownChanged(manaCountdown);
		}

		// if (Input.GetKeyDown(KeyCode.Escape))
		// {
		// 	Back();
		// }
	}

	void ShowPopup(GameObject popup)
	{
		// Disable interaction
		SetInteractable(false);

		UIHelper.ShowPopup(popup, () => {
			// Enable UI
			SetUIEnabled(true);
		});
	}
	
	void HidePopup(GameObject popup)
	{
		// Disable interaction
		SetInteractable(false);

		UIHelper.HidePopup(popup, () => {
			// Enable interaction
			SetInteractable(true);
		});
	}

	void SetButtonEnabled(Button button, bool enabled)
	{
		if (enabled)
		{
			if (!button.interactable)
			{
				button.gameObject.SetImageRGB(Vector3.one, true);
				button.interactable = true;
			}
		}
		else
		{
			if (button.interactable)
			{
				button.gameObject.SetImageRGB(disableColor, true);
				button.interactable = false;
			}
		}
	}

	void SetInteractable(bool interactable)
	{
		map.Interactable = interactable;

		_canvas.SetInteractable(interactable);
	}

	void SetUIEnabled(bool enabled)
	{
		_canvas.SetInteractable(enabled);
	}

	void GoToRateGame()
	{
		#if UNITY_ANDROID

		#endif

		#if UNITY_IPHONE

		#endif
	}

#if SHOW_DEBUG
	private double _lastTime = 0.0;
	private bool _showDebug = false;
	
	void OnGUI()
	{
#if UNITY_EDITOR
		float top = 50f;
#else
		float top = 100f;
#endif

		if (!TransitionManager.Instance.TransitionFinished) return;

		if (!_showDebug)
		{
			if (GUI.Button(new Rect(Screen.width - 100, top, 100, 100), "", GUIStyle.none))
			{
				double time = System.DateTime.Now.TimeOfDay.TotalSeconds;
				
				if (time - _lastTime < 0.35)
				{
					_lastTime = 0;
					
					_showDebug = !_showDebug;
				}
				else
				{
					_lastTime = time;
				}
			}
		}
		
		if (_showDebug)
		{
			GUILayout.BeginArea(new Rect(10, top, Screen.width, Screen.height));
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			
			GUI.enabled = (_userData.Mana < Settings.MaxMana);
			if (GUILayout.Button("Get Mana"))
			{
				NotificationManager.ManaChanged(_userData.Mana + 1);
			}
			GUI.enabled = true;
			
			GUI.enabled = (_userData.Mana > 0);
			if (GUILayout.Button("Clear Mana"))
			{
				NotificationManager.ManaChanged(0);
			}
			GUI.enabled = true;

#if UNITY_EDITOR
			GUI.enabled = (_userData.Mana < Settings.MaxMana);
			if (GUILayout.Button("Cheat Mana"))
			{
				TimeManager.CheatManaCountdown();
			}
			GUI.enabled = true;
#endif
			
			if (GUILayout.Button("Get Coin"))
			{
				NotificationManager.CoinChanged(_userData.Coin + 10);
			}
			
			GUI.enabled = (_userData.Coin > 0);
			if (GUILayout.Button("Clear Coin"))
			{
				NotificationManager.CoinChanged(0);
			}
			GUI.enabled = true;

			int level = _userData.Level;

			GUI.enabled = (level < Settings.MapCount);
			if (GUILayout.Button("Unlock Next Map"))
			{
				ZoneType currentZone = ZoneTypeHelper.GetZoneType(level);

				level++;

				_userData.Level = level;

				map.UnlockMap(level);

				ZoneType newZone = ZoneTypeHelper.GetZoneType(level);

				if (newZone != currentZone)
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				}
			}

			if (GUILayout.Button("Unlock Zone"))
			{
				for (int zone = 0; zone < 3; zone++)
				{
					ZoneType zoneType = zone.ToZoneType();
					int mapCount = zoneType.MapCount();

					if (level <= mapCount)
					{
						for (int i = level + 1; i <= mapCount; i++)
						{
							map.UnlockMap(i);
						}
						
						_userData.Level = mapCount;

						break;
					}
				}
			}

			if (GUILayout.Button("Unlock All Maps"))
			{
				ZoneType currentZone = ZoneTypeHelper.GetZoneType(level);

				for (int i = level + 1; i <= Settings.MapCount; i++)
				{
					map.UnlockMap(i);
				}

				_userData.Level = Settings.MapCount;

				if (currentZone != ZoneType.Volcano)
				{
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				}
			}
			GUI.enabled = true;

			if (GUILayout.Button("Close"))
			{
				_showDebug = false;
			}

			if (GUILayout.Button("Reset"))
			{
				UserData.Instance.Reset();
			}
			
			GUILayout.EndVertical();
			GUILayout.Space(20);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
#endif
}
