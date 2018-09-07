using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements; 

public class UnityAds : Singleton<UnityAds> {

	private bool _isRewardCoin;
	private int _numCoinReward = 0;
	private int _numManaReward = 0;

	public void WatchVideoForCoin(int numCoin) {
		_numManaReward = 0;
		_numCoinReward = numCoin;

		_isRewardCoin = true;

		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}

	public void WatchVideoForMana(int numMana) {
		_numCoinReward = 0;
		_numManaReward = numMana;

		_isRewardCoin = false;

		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}


	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			//Debug.Log ("The ad was successfully shown.");
			// ADD REWARD

			if (_isRewardCoin) {
				// Add reward coin depend in _numCoinReward
				//Debug.Log("Reward Coin: " + _numCoinReward);
				UserData.Instance.Coin += _numCoinReward;
				Manager.Instance.ShowWatchVideoCoinRewad(() => {
					//Debug.Log("Add coins ...");
					});

			} else {
				// Add reward mana depend in _numManaReward
				//Debug.Log("Reward Mana: " + _numManaReward);
				UserData.Instance.Mana += _numManaReward;
				Manager.Instance.ShowWatchVideoManaRewad(() => {
					//Debug.Log("Add Mana ...");
				});
			}

			break;
		case ShowResult.Skipped:
			//Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			//Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
}
