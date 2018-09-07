using UnityEngine;
using UnityEngine.UI;
using System;

public class BuyManaPopupScript : MonoBehaviour
{
	// The buy coin popup prefab
	[SerializeField]
	private GameObject _buyCoinPopupPrefab;

	// The coin to buy
	[SerializeField]
	private Text _coinToBuy;

	// The current coin
	[SerializeField]
	private Text _currentCoin;

	// The buy callback
	private Action _buyCallback;

	// The close popup callback
	private Action _closeCallback;

	public void Show(Action buyCallback, Action showCallback = null, Action closeCallback = null)
	{
		// Set buy callback
		_buyCallback = buyCallback;

		// Set close callback
		_closeCallback = closeCallback;

		int coin = Settings.CoinToBuyFullMana;

		// Set coin to buy
		_coinToBuy.text = coin.ToString();

		// Set coin color
		_coinToBuy.color = (UserData.Instance.Coin >= coin ? Constants.CoinColor : Constants.NotEnoughCoinColor);

		// Set current coin
		_currentCoin.text = UserData.Instance.Coin.ToString();

		// Show popup
		UIHelper.ShowPopup(gameObject, showCallback);
	}

	public void AskMana()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		if (!Helper.IsOnline())
		{
			Manager.Instance.ShowMessage(Settings.NoInternetConnection);
			return;
		}

		FBHelper.AskForObject(null, "mana", Settings.AskManaTitle, Settings.AskManaMessage, (error) => {
			if (!string.IsNullOrEmpty(error))
			{
				///Log.Debug("Ask mana error: " + error);
				Manager.Instance.ShowMessage(Settings.AskManaFailed);
			}
			else
			{
				// Close popup
				Close();
			}
		});
	}

	public void Invite()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		if (!Helper.IsOnline())
		{
			Manager.Instance.ShowMessage(Settings.NoInternetConnection);
			return;
		}

		FBHelper.Invite(Settings.InviteTitle, Settings.InviteMessage, FriendType.Invitable, FBObjectType.Mana, (error) => {
			if (!string.IsNullOrEmpty(error))
			{
				//Log.Debug("Invite error: " + error);
				Manager.Instance.ShowMessage(Settings.InviteFailed);
			}
			else
			{
				// Close popup
				Close();
			}
		});
	}

	public void BuyMana()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Close popup
		//Close();

		UserData userData = UserData.Instance;

		if (userData.Coin >= Settings.CoinToBuyFullMana)
		{
			Close();

			if (_buyCallback != null)
			{
				_buyCallback();
			}
		}
		else
		{
			if (_buyCoinPopupPrefab != null)
			{
				GameObject buyCoinPopup = _buyCoinPopupPrefab.CreateUI(transform.parent);
				BuyCoinPopupScript script = buyCoinPopup.GetComponent<BuyCoinPopupScript>();

				if (script != null)
				{
					script.Show();
				}
			}
			else
			{
				//Log.Debug("Not enough coin!");
			}
		}
	}
    
    public void WatchVideo() {
		// Watch video for number of mana reward

		UnityAds.Instance.WatchVideoForMana (Settings.ManaToWatchVideo);
    }
		

	public void ClosePopup()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Close popup
		Close();
	}

	void Close()
	{
		UIHelper.HidePopup(gameObject, () => {
			if (_closeCallback != null)
			{
				_closeCallback();
			}

			// Self-destroy
			Destroy(gameObject);
		});
	}
}
