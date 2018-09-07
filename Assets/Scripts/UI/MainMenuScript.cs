#if UNITY_EDITOR
#define SHOW_DEBUG
#endif

using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System.Collections.Generic;

public class MainMenuScript : MonoBehaviour
{
	private static readonly float FBButtonX       = -120.0f;
	private static readonly float FBButtonOffsetX = 320.0f;

	/// <summary>
	/// The canvas.
	/// </summary>
	public Canvas canvas;

	/// <summary>
	/// The help prefab.
	/// </summary>
	public GameObject helpPrefab;

	/// <summary>
	/// The play button.
	/// </summary>
	public GameObject playButton;

	/// <summary>
	/// The connect Facebook button.
	/// </summary>
	public GameObject connectFBButton;

	/// <summary>
	/// The play transform.
	/// </summary>
	public Transform playTransform;

	/// <summary>
	/// The sunlight.
	/// </summary>
	public GameObject sunlight;

	/// <summary>
	/// The loading.
	/// </summary>
	public GameObject loading;

	public Sprite musicOn;
	public Sprite musicOff;
	public Sprite soundOn;
	public Sprite soundOff;
	public Sprite logIn;
	public Sprite logOut;
	public GameObject cutscene1;

	[Header("Settings")]

	/// <summary>
	/// The settings popup.
	/// </summary>
	public GameObject settingsPopup;

	/// <summary>
	/// The music button.
	/// </summary>
	public Button musicButton;

	/// <summary>
	/// The sound button.
	/// </summary>
	public Button soundButton;

	/// <summary>
	/// The login button.
	/// </summary>
	public Button loginButton;

	// The ignore raycast
	private IgnoreRaycast _ignoreRaycast;

	public GoogleAnalyticsV4 googleAnalytics;

	// The FB request updater
	private FixedUpdater _requestUpdater = new FixedUpdater(20.0f);

	void Awake()
	{
		// Set Play button position
		if (playTransform != null)
		{
			playButton.transform.position = playTransform.position;
		}
	}

	void OnEnable()
	{
		KeyManager.AddBackEventHandler(OnKeyBack);
	}

	void OnDisable()
	{
		KeyManager.RemoveBackEventHandler(OnKeyBack);
	}

	void OnKeyBack()
	{
		Manager.Instance.ShowConfirm("Wait!", Settings.QuitGameConfirm, (quit) => {
			if (quit)
			{
				Application.Quit();
			}
		});
	}

	void Start()
	{
		// Always initialize FB
		if (FB.IsInitialized)
		{
			OnStart();
		}
		else
		{
			if (Helper.IsOnline())
			{
				InitDelegate onInitComplete = () => {
					FB.ActivateApp();

					OnStart();
				};

				HideUnityDelegate onHideUnity = (isUnityShown) => {
					Time.timeScale = isUnityShown ? 1.0f : 0.0f;
				};

				FB.Init(onInitComplete, onHideUnity);
			}
			else
			{
				OnStart();
			}
		}

		MyAdmob.Instance.ShowBanner();
	}

	void OnStart()
	{
		if (!FB.IsLoggedIn)
		{
			// Check to show login FB
			if (UserData.Instance.ShowLoginFBTimes < 3
				&& !Manager.Instance.isShowLoginFB
				&& UserData.Instance.Level >= 10)
			{
				UserData.Instance.ShowLoginFBTimes++;
				Manager.Instance.isShowLoginFB = true;
				Manager.Instance.ShowConfirm("Connect Facebook", "Please connect with facebook to save your progress and see your friends",
					(isOK) => 
					{
						if (isOK)
						{
							Debug.Log("Connect FB, choosse OK ...");
							ConnectFacebook();

							UserData.Instance.ShowLoginFBTimes = 3;
						}
						else
						{
							Debug.Log("Cancel, no connect FB ...");
						}
					});
			}
		}

		if (!Manager.Instance.isShowNewVersion && UserData.Instance.Level >= 4)
		{
			SCross.Instance.ShowMessage ("cyrus_bean_jump", "1.6.0", (result, message) => {

			});
			Manager.Instance.isShowNewVersion = true;
		}
		 

		if (UserData.Instance.Level > 15)
		{
			SCross.Instance.GetImage ("cyrus_bean_jump", (result, message) => {
				if (result) {
					SCross.Instance.ShowPopupScross();
				}
			});
		}

		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		// Update Facebook
		FBManager.Instance.OnUpdate();

		// Play BGM
		SoundManager.Instance.PlayMusic(SoundID.MainMenu);

		// Sunlight
		if (sunlight != null)
		{
			// Get top
			float top = Camera.main.orthographicSize;
			
			// Get right
			float right = Camera.main.GetOrthographicWidth();

			sunlight.transform.position = new Vector3(right - Random.Range(0.0f, 0.1f), top + 2.0f, 0);
			sunlight.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(240.0f, 250.0f));

			int count = sunlight.transform.childCount;

			for (int i = 0; i < count; i++)
			{
				sunlight.transform.GetChild(i).localScale = new Vector3(Random.Range(9.0f, 10.0f), Random.Range(0.85f, 1.0f), 1.0f);
			}

			float startOpacity = Random.value * 0.1f;
			sunlight.SetAlpha(startOpacity, true);

			var rotate = RotateAction.RotateBy(Random.Range(-20.0f, -25.0f), 18.0f, Ease.Linear, LerpDirection.PingPongForever);
			var fadeIn = FadeAction.RecursiveFadeTo(Random.Range(0.6f, 0.7f), 18.0f);
			var delay1 = DelayAction.Create(6.0f);
			var fadeOut = FadeAction.RecursiveFadeTo(startOpacity, 18.0f);
			var delay2 = DelayAction.Create(6.0f);
			var fade = SequenceAction.Create(fadeIn, delay1, fadeOut, delay2);
			var action = ParallelAction.ParallelAll(rotate, RepeatAction.RepeatForever(fade, false));
			
			sunlight.Play(action);
		}

		// Get ignore raycast
		_ignoreRaycast = GameObject.FindObjectOfType<IgnoreRaycast>();

		if(!UserData.Instance.PlayedCutscene1)
		{
			UserData.Instance.PlayedCutscene1 = true;
			cutscene1.SetActive(true);
		}

		// Update Connect button
		UpdateConnectFacebook(true);

		googleAnalytics.LogScreen("Main Menu");

		_requestUpdater.Play(UpdateRequests);

		Manager.Instance.SetUpdateSendRequests(true);
	}

	public void Play()
	{
		_requestUpdater.Stop();

		Manager.Instance.SetUpdateSendRequests(false);

		// Play sound
		SoundManager.PlayButtonOpen();
		
		TransitionManager.Instance.FadeTransitionScene(Settings.MapSelectionScene, true);
	}

	public void ConnectFacebook()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Check if offline
		if (!Helper.IsOnline())
		{
			// Hide sunlight
			sunlight.Hide();

			Manager.Instance.ShowMessage(Settings.NoInternetConnection, () => {
				// Show sunlight
				sunlight.Show();
			});

			return;
		}

		// Disable interaction
		SetInteractable(false);

		// Show loading
		SetShowLoading(true);

		// Login Facebook
		FBManager.LogIn((error) =>
		{
			if (error != null)
			{
				Log.Error(error);
			}
			else
			{
				// Update Facebook
				FBManager.Instance.OnUpdate();
			}

			// Hide loading
			SetShowLoading(false);

			// Update Connect button
			UpdateConnectFacebook();

			// Enable interaction
			SetInteractable(true);
		});
	}

	void LoginFacebook()
	{
		// Disable interaction
		SetInteractable(false);
		
		// Show loading
		SetShowLoading(true);

		// Login Facebook
		FBManager.LogIn((error) =>
		{
			if (error != null)
			{
				Log.Error(error);
			}
			else
			{
				// Update Facebook
				FBManager.Instance.OnUpdate();
			}

			// Hide loading
			SetShowLoading(false);

			// Set sprite
			loginButton.SetSprite(FB.IsLoggedIn ? logOut : logIn);

			// Update Connect button
			UpdateConnectFacebook();

			// Enable interaction
			SetInteractable(true);
		});
	}

	void LogoutFacebook()
	{
		FBHelper.LogOut();

		// Set sprite
		loginButton.SetSprite(logIn);
		
		// Show Facebook button
		SetShowFBButton(true, false);
	}

	void SetShowFBButton(bool show, bool start)
	{
		connectFBButton.StopAction();

		float y = connectFBButton.GetComponent<RectTransform>().anchoredPosition.y;

		if (show)
		{
			var move = AnchoredMoveAction.Create(new Vector2(FBButtonX, y), false, 0.5f, Ease.SineOut);

			if (start)
			{
				connectFBButton.Play(SequenceAction.Create(DelayAction.Create(1.0f), move));
			}
			else
			{
				connectFBButton.Play(move);
			}
		}
		else
		{
			var move = AnchoredMoveAction.Create(new Vector2(FBButtonX + FBButtonOffsetX, y), false, 0.5f, Ease.SineIn);

			connectFBButton.Play(move);
		}
	}

	public void ToggleMusic()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		bool musicEnabled = !SoundManager.Instance.MusicEnabled;

		SoundManager.Instance.MusicEnabled = musicEnabled;

		musicButton.SetSprite(musicEnabled ? musicOn : musicOff);

		// Persistent data
		UserData.Instance.BGMOn = musicEnabled;
	}
	
	public void ToggleSFX()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		bool soundEnabled = !SoundManager.Instance.SoundEnabled;

		SoundManager.Instance.SoundEnabled = soundEnabled;
		
		soundButton.SetSprite(soundEnabled ? soundOn : soundOff);
		
		// Persistent data
		UserData.Instance.SFXOn = soundEnabled;
	}

	public void ToggleFacebook()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Check if offline
		if (!Helper.IsOnline())
		{
			Manager.Instance.ShowMessage(Settings.NoInternetConnection);
			return;
		}

		if (FB.IsLoggedIn)
		{
			LogoutFacebook();
		}
		else
		{
			LoginFacebook();
		}
	}


	public void ShowSettings()
	{
		// Play sound
		SoundManager.PlayButtonOpen();

		// Hide sunlight
		sunlight.Hide();

		// Music
		musicButton.SetSprite(SoundManager.Instance.MusicEnabled ? musicOn : musicOff);
		
		// SFX
		soundButton.SetSprite(SoundManager.Instance.SoundEnabled ? soundOn : soundOff);
		
		// Set sprite
		loginButton.SetSprite(FB.IsLoggedIn ? logOut : logIn);

		// Disable interaction
		SetInteractable(false);
		
		// Show Settings popup
		UIHelper.ShowPopup(settingsPopup, () => {
			// Enable interaction
			SetInteractable(true);
		});
	}
	
	public void HideSettings()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Disable interaction
		SetInteractable(false);
		
		UIHelper.HidePopup(settingsPopup, () => {
			// Show sunlightf
			sunlight.Show();

			// Enable interaction
			SetInteractable(true);
		});
	}

	public void ShowHelpPopup()
	{
		if (helpPrefab != null)
		{
			// Play sound
			SoundManager.PlayButtonOpen();

			// Hide sunlight
			sunlight.Hide();

			GameObject help = helpPrefab.CreateUI(canvas.transform, Vector2.zero);

			HelpPopupScript script = help.GetComponent<HelpPopupScript>();
			script.CloseCallback = () => {
				// Show sunlight
				sunlight.Show();
			};
		}
		else
		{
			// Play sound
			SoundManager.PlayButtonClick();
		}
	}

	void UpdateConnectFacebook(bool start = false)
	{
		SetShowFBButton(!FB.IsLoggedIn, start);
	}

	void SetInteractable(bool interactable)
	{
		if (_ignoreRaycast != null)
		{
			_ignoreRaycast.interactable = interactable;
		}
	}

	void SetShowLoading(bool show)
	{
		if (loading != null)
		{
			loading.SetActive(show);
		}
	}

	void Update()
	{
		_requestUpdater.Update();
	}

	void UpdateRequests()
	{
		if (!FB.IsLoggedIn) return;

		_requestUpdater.Stop();

		FBHelper.GetAskRequests((result, error) => {
			_requestUpdater.Play();

			if (!string.IsNullOrEmpty(error)) return;
			if (result == null) return;

			RequestData[] requests = result.ToArray();
			int requestCount = requests.Length;
			if (requestCount == 0) return;

			_requestUpdater.Stop();

			// Ask mana
			List<string> askManaIds = new List<string>(requestCount);

			for (int i = 0; i < requestCount; i++)
			{
				RequestData request = requests[i];

				if (request.ObjectType == FBObjectType.Mana)
				{
					askManaIds.Add(request.FromId);

					FBHelper.DeleteRequest(request.Id);
				}
			}

			if (askManaIds.Count > 0)
			{
				FBHelper.SendObject(null, "mana", Settings.SendManaTitle, Settings.SendManaMessage, (err) => {
					_requestUpdater.Play();
				}, askManaIds.ToArray());
			}
		});
	}

#if SHOW_DEBUG
	private double _lastTime = 0.0;
	private bool _showDebug = false;

	void OnGUI()
	{
		if (!_showDebug)
		{
			if (GUI.Button(new Rect(0, 0, 100, 100), "", GUIStyle.none))
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
			GUILayout.BeginArea(new Rect(10, 10, Screen.width, Screen.height));
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();

//			if (!FB.IsLoggedIn)
//			{
//				if (settingsPopup.activeSelf)
//				{
//					if (GUILayout.Button("Log in Facebook"))
//					{
//						LoginFacebook();
//					}
//				}
//				else
//				{
//					if (GUILayout.Button("Connect Facebook"))
//					{
//						ConnectFacebook();
//					}
//				}
//			}

			if (FB.IsLoggedIn)
			{
				if (GUILayout.Button("Get score"))
				{
					FBHelper.GetScore(null, (score, error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Get score error: " + error);
						}
						else
						{
							//Log.Debug("Score: " + score);
						}
					});
				}

				if (GUILayout.Button("Post score"))
				{
					int score = Random.Range(1, 10);

					FBHelper.PostScore(null, score, (result, error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Post score error: " + error);
						}
						else
						{
							//Log.Debug(string.Format("Post score {0}: {1}", score, result));
						}
					});
				}
				
				if (GUILayout.Button("Delete score"))
				{
					FBHelper.DeleteScore(null, (result, error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Delete score error: " + error);
						}
						else
						{
							//Log.Debug("Delete score: " + result);
						}
					});
				}
				
				if (GUILayout.Button("Get highscores"))
				{
					FBHelper.GetHighscores((ids, names, scores, error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Get highscores error: " + error);
						}
						else
						{
							int count = ids.Length;

							for (int i = 0; i < count; i++)
							{
								//Debug.Log(string.Format("{0}. {1}\t{2}\t{3}", i + 1, ids[i], names[i], scores[i]));
							}
						}
					});
				}
				
				if (GUILayout.Button("Delete highscores"))
				{
					FBHelper.DeleteHighscores((result, error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Delete highscores error: " + error);
						}
						else
						{
							//Log.Debug("Delete highscores: " + result);
						}
					});
				}

				if (GUILayout.Button("Invite friends"))
				{
					FBHelper.Invite(Settings.InviteTitle, Settings.InviteMessage, FriendType.Invitable, FBObjectType.Coin, (error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Invite friends error: " + error);
						}
					});
				}
				
				if (GUILayout.Button("Invite users"))
				{
					FBHelper.Invite(Settings.InviteTitle, Settings.InviteMessage, FriendType.Game, FBObjectType.Coin, (error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Invite users error: " + error);
						}
					});
				}

				if (GUILayout.Button("Send Coin"))
				{
					FBHelper.SendObject(null, "coin", "Send a coin to your friend", "Take this coin", (error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Send coin error: " + error);
						}
					});
				}

				if (GUILayout.Button("Ask Coin"))
				{
					FBHelper.AskForObject(null, "coin", "Request a coin from your friend", "Give me a coin", (error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Ask coin error: " + error);
						}
					});
				}
				
				if (GUILayout.Button("Send Mana"))
				{
					FBHelper.SendObject(null, "mana", Settings.SendManaTitle, Settings.SendManaMessage, (error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Send mana error: " + error);
						}
					});
				}
				
				if (GUILayout.Button("Ask Mana"))
				{
					FBHelper.AskForObject(null, "mana", Settings.AskManaTitle, Settings.AskManaMessage, (error) => {
						if (!string.IsNullOrEmpty(error))
						{
							//Log.Debug("Ask mana error: " + error);
						}
					});
				}
			}

			if (GUILayout.Button("Reset Data"))
			{
				UserData.Instance.Reset();

				NotificationManager.ManaChanged(UserData.Instance.Mana);
				NotificationManager.CoinChanged(UserData.Instance.Coin);

				if (FB.IsLoggedIn)
				{
					// Delete Facebook score
					FBHelper.DeleteScore(null);
				}
			}

			if (GUILayout.Button("Close"))
			{
				_showDebug = false;
			}

			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
#endif
}
