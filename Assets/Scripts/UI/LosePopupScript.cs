using UnityEngine;
using UnityEngine.UI;
using System;

public class LosePopupScript : FullPopupBehaviour
{
	/// <summary>
	/// The cyrus.
	/// </summary>
	public Image cyrus;

	public GameObject restartButton;
	public GameObject homeButton;
	public GameObject leaderboardButton;
	public GameObject videoButton;
	public GameObject watchVideoReward;
	public Animator cyrusAnim;

	void Start()
	{
		// Get zone setting
		ZoneSetting zone = GameManager.Instance.Zone;

		// Set cyrus
		cyrus.sprite = zone.loseCyrus;
		Manager.Instance.analytics.LogScreen("Lose Game - Level "+ UserData.Instance.Map.ToString("000"));
		Invoke("PlayAnim", 2f);

	}

	void PlayAnim()
	{
		cyrusAnim.Play("Lose");
	}

	public void WatchVideo() {
		// Watch video for number of coin reward
		UnityAds.Instance.WatchVideoForCoin(Settings.CoinToWatchVideo);

		Manager.Instance.analytics.LogEvent("Lose Game", "Button Press", "Watch Video", 1);
	}

	void CheckShowAds()
	{
		//Manager.Instance.checkShowAdsTimes++;
		if (UserData.Instance.Map >= 4)
		{
			MyAdmob.Instance.ShowInterstitial();
		}
	}

	void OnReady()
	{		
		SetUIEnabled(true);
	}

	protected override void SetShowTouchAndOverlay(bool show)
	{
		SetChildrenEnabled(restartButton, show);
		SetChildrenEnabled(homeButton, show);
		SetChildrenEnabled(leaderboardButton, show);
		SetChildrenEnabled(videoButton, show);
	}

	protected override void OnShowFinished(Action callback)
	{
		// Show touch and overlay
		SetShowTouchAndOverlay(true);

		Invoke("CheckShowAds", 0.4f);
		Invoke("OnReady", 1f);

		if (callback != null)
		{
			callback();
		}
	}

// #if UNITY_EDITOR
// 	void Update()
// 	{
// 		if (Input.GetKeyDown(KeyCode.Return))
// 		{
// 			Restart();
// 		}
// 		else if (Input.GetKeyDown(KeyCode.Escape))
// 		{
// 			Home();
// 		}
// 	}
// #endif
}
