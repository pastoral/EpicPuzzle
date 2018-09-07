using UnityEngine;
using UnityEngine.UI;
using System;
using Facebook.Unity;

public class WinPopupScript : FullPopupBehaviour
{
	private static readonly int minFirework = 3;

	/// <summary>
	/// The cyrus.
	/// </summary>
	public Image cyrus;
	[SerializeField] Animator cyrusAnim;

	/// <summary>
	/// The firework prefab.
	/// </summary>
	public GameObject fireworkPrefab;

	/// <summary>
	/// The firework positions.
	/// </summary>
	public GameObject[] fireworkPositions = new GameObject[6];

	/// <summary>
	/// The bonus.
	/// </summary>
	public GameObject bonus;

	/// <summary>
	/// The coin text.
	/// </summary>
	public Text coinText;

	public GameObject nextButton;
	//public GameObject restartButton;
	public GameObject homeButton;
	public GameObject leaderboardButton;
	public GameObject shareButton;
	public GameObject videoButton;
	public GameObject likeButton;

	public GameObject cutscene4;

	[SerializeField] string[] imageLinks;
	void Start()
	{
		// Get zone setting
		ZoneSetting zone = GameManager.Instance.Zone;

		// Set cyrus
		//cyrus.sprite = zone.winCyrus;

		Manager.Instance.analytics.LogScreen("Win Game - Level "+ UserData.Instance.Map.ToString("000"));
		Invoke("PlayCyrusAnim", 2f);
	}

	void PlayCyrusAnim()
	{
		cyrusAnim.Play("New Animation");
	}

	void ShowEndgameStory()
	{
		UserData.Instance.PlayedCutscene4 = true;
		cutscene4.SetActive(true);
	}

	void OnDestroy()
	{
		// Stop sound
		SoundManager.Instance.StopSound(SoundID.Coin);
	}

	protected override void OnShowFinished(Action callback)
	{
		// Play firework
		if (fireworkPrefab != null)
		{
			int total = fireworkPositions.Length;
			int[] positions = total.RandomIndices();
			int count = total > minFirework ? UnityEngine.Random.Range(minFirework, total) : total;
			float delay = 0.0f;

			for (int i = 0; i < count; i++)
			{
				GameObject firework = Instantiate(fireworkPrefab);
				firework.transform.position = fireworkPositions[positions[i]].transform.position;

				if (delay > 0)
				{
					ParticleSystem ps = firework.GetComponent<ParticleSystem>();
					ps.Pause();

					firework.Play(SequenceAction.Create(DelayAction.Create(delay), CallFuncAction.Create(() => {
						SoundManager.Instance.PlaySound(SoundID.BigFirework, SoundType.New);
						ps.Play();
					})));
				}
				else
				{
					SoundManager.Instance.PlaySound(SoundID.BigFirework, SoundType.New);
				}

				delay += 0.3f;
			}

			if (UserData.Instance.Map == 99 && !UserData.Instance.PlayedCutscene4)
			{
				ShowEndgameStory();
			}
		}

		int level = UserData.Instance.Map;

		// Check if win new map
		if (!UserData.Instance.IsWinned(level))
		{
			UserData.Instance.SetWinned(level);

			// Add bonus coins
			int coins = Settings.GetBonusCoins(level);
			NotificationManager.CoinChanged(UserData.Instance.Coin + coins);

			if (bonus != null)
			{
				// Show bonus
				bonus.Show();
				bonus.SetAlpha(1.0f, true);

				if (coinText != null)
				{
					// Play sound
					SoundManager.Instance.PlaySound(SoundID.Coin, SoundType.Loop);

					var action = LerpIntAction.Create(1, coins, 0.5f, (coin) => {
						coinText.text = coin.ToString();
					});

					gameObject.Play(action, () => {
						// Stop sound
						SoundManager.Instance.StopSound(SoundID.Coin);
					});
				}
			}
		}

		// Show touch and overlay
		SetShowTouchAndOverlay(true);

		Invoke("CheckShowAds", 1.2f);
		Invoke("OnReady", 2f);

		if (callback != null)
		{
			callback();
		}
	}

	public void Next()
	{
		// Play sound
		SoundManager.PlayButtonOpen();

		if (!UserData.Instance.NewLevel)
		{
			// Go to next map
//			UserData.Instance.Map++;

			if (UserData.Instance.Map < Settings.MapCount)
			{
				UserData.Instance.NextLevel = true;
			}
		}

		// Hide
		Hide(false, () => { TransitionManager.Instance.FadeTransitionScene(Settings.MapSelectionScene); });
	}
    
    public void WatchVideo() {
        // Watch video for number of coin reward
		UnityAds.Instance.WatchVideoForCoin(Settings.CoinToWatchVideo);

		Manager.Instance.analytics.LogEvent("Win Game", "Button Press", "Watch Video", 1);
    }
    

	protected override void SetShowTouchAndOverlay(bool show)
	{
		SetChildrenEnabled(nextButton, show);
		//SetChildrenEnabled(restartButton, show);
		SetChildrenEnabled(homeButton, show);
		SetChildrenEnabled(leaderboardButton, show);
		SetChildrenEnabled(likeButton, show);
		SetChildrenEnabled(videoButton, show);
		SetChildrenEnabled(shareButton, show);
	}


	void CheckShowAds()
	{
//		Manager.Instance.checkShowAdsTimes++;
//		if (Manager.Instance.checkShowAdsTimes % 4 == 0 && UserData.Instance.Map >= 5)
//		{
//			MyAdmob.Instance.ShowInterstitial();
//		}
	}

	void OnReady()
	{		
		SetUIEnabled(true);
	}

	public void ShareFeed() {
		
//		FB.FeedShare(
//			toId: "",
//			link: new Uri(""),
//			linkName: "I won level " + (int)UserData.Instance.Level + ".",
//			linkCaption: "Can you catch me?",
//			linkDescription: "Download this game on Appstore and Play store!",
//			picture: new Uri(""),
//			mediaSource: "",
//			callback: Callback
//		);

		string imageLink = imageLinks[UserData.Instance.Map / 14];

		FB.ShareLink(
			contentURL: new Uri(""),
			contentTitle:  "I won level " + (int)(UserData.Instance.Map) + ".",
			contentDescription: "Download this game on Appstore and Play store!",
			photoURL: new Uri(imageLink),
			callback: Callback

		);

		Manager.Instance.analytics.LogEvent("Win Game", "Button Press", "Share FB", 1);
	}

	public void GoToSmutionFB()
	{
		#if UNITY_ANDROID
			Application.OpenURL("");
		#else
			Application.OpenURL("");
		#endif

		Manager.Instance.analytics.LogEvent("Win Game", "Button Press", "Go To Fanpage", 1);
	}

	private void Callback(IShareResult result) {


	}
	
// #if UNITY_EDITOR
// 	void Update()
// 	{
// 		if (Input.GetKeyDown(KeyCode.Return))
// 		{
// 			Next();
// 		}
// 		else if (Input.GetKeyDown(KeyCode.Escape))
// 		{
// 			Home();
// 		}
// 		else if (Input.GetKeyDown(KeyCode.Space))
// 		{
// 			Restart();
// 		}
// 	}
// #endif
}
