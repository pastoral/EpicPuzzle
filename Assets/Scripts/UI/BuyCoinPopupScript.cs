using UnityEngine;
using UnityEngine.UI;
using System;

public class BuyCoinPopupScript : MonoBehaviour
{
	/// <summary>
	/// The coins package1.
	/// </summary>
	[SerializeField]
	private Text _coinsPackage1;

	/// <summary>
	/// The coins package2.
	/// </summary>
	[SerializeField]
	private Text _coinsPackage2;

	/// <summary>
	/// The coins package3.
	/// </summary>
	[SerializeField]
	private Text _coinsPackage3;
	[SerializeField]
	private Text _coinsPackage4;

	// The purchase callback
	private Action<CoinPackage> _purchaseCallback;

	// The close popup callback
	private Action _closeCallback;

	void Awake()
	{
		if (_coinsPackage1 != null)
		{
			_coinsPackage1.text = CoinPackage.Package1.GetCoins().ToString();
		}

		if (_coinsPackage2 != null)
		{
			_coinsPackage2.text = CoinPackage.Package2.GetCoins().ToString();
		}

		if (_coinsPackage3 != null)
		{
			_coinsPackage3.text = CoinPackage.Package3.GetCoins().ToString();
		}

		if (_coinsPackage4 != null)
		{
			_coinsPackage4.text = CoinPackage.Package4.GetCoins().ToString();
		}
	}

	public void Show(Action<CoinPackage> purchaseCallback = null, Action showCallback = null, Action closeCallback = null)
	{
		// Set purchase callback
		_purchaseCallback = purchaseCallback;

		// Set close callback
		_closeCallback = closeCallback;

		// Show popup
		UIHelper.ShowPopup(gameObject, showCallback);
	}

	public void BuyPackage1()
	{
		BuyPackage(CoinPackage.Package1);

		Manager.Instance.analytics.LogEvent("IAP", "Buy Coin", "Purchase 1", 1);
	}

	public void BuyPackage2()
	{
		BuyPackage(CoinPackage.Package2);

		Manager.Instance.analytics.LogEvent("IAP", "Buy Coin", "Purchase 5", 5);
	}

	public void BuyPackage3()
	{
		BuyPackage(CoinPackage.Package3);

		Manager.Instance.analytics.LogEvent("IAP", "Buy Coin", "Purchase 10", 10);
	}

	public void BuyPackage4()
	{
		BuyPackage(CoinPackage.Package4);

		Manager.Instance.analytics.LogEvent("IAP", "Buy Coin", "Purchase 20", 20);
	}

    public void WatchVideo()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Watch video for number of coin reward
		UnityAds.Instance.WatchVideoForCoin (Settings.CoinToWatchVideo);

		Manager.Instance.analytics.LogEvent("IAP", "Buy Coin", "Watch Video", 1);
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

		FBHelper.Invite(Settings.InviteTitle, Settings.InviteMessage, FriendType.Invitable, FBObjectType.Coin, (error) => {
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

	public void ClosePopup()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Close popup
		Close();

		Manager.Instance.analytics.LogEvent("IAP", "Buy Coin", "Exit", 0);
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

	void BuyPackage(CoinPackage package)
	{
		// Play sound
		SoundManager.PlayButtonClick();

		if (!Helper.IsOnline())
		{
			Manager.Instance.ShowMessage(Settings.NoInternetConnection);
			return;
		}

		// Close popup
		Close();

		// Purchase
		CompleteProject.Purchaser purchaser = GetComponent<CompleteProject.Purchaser>();

		if (purchaser != null)
		{
			if (_purchaseCallback != null)
			{
				purchaser.BuyPackage(package, _purchaseCallback);
			}
			else
			{
				purchaser.BuyPackage(package, (purchasedPackage) => {
					//Debug.Log("Purchase finish: add " + purchasedPackage.GetCoins() + " coins");
					NotificationManager.CoinChanged(UserData.Instance.Coin + purchasedPackage.GetCoins());
					MyAdmob.Instance.isPurchased = true;
					PlayerPrefs.SetInt(MyAdmob.purchaseText, 1);
				});
			}
		}
	}
}
