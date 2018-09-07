using UnityEngine;
using UnityEngine.UI;
using System;

public class BuyBoosterPopupScript : MonoBehaviour
{
	private static readonly Vector3 disableColor = new Vector3(0.5f, 0.5f, 0.5f);

	// The buy coin popup prefab
	[SerializeField]
	private GameObject _buyCoinPopupPrefab;

	/// <summary>
	/// The booster icons.
	/// </summary>
	public Sprite[] icons = new Sprite[4];

	/// <summary>
	/// The coin info.
	/// </summary>
	public Text coinInfo;
	
	/// <summary>
	/// The booster title.
	/// </summary>
	public Text boosterTitle;

	/// <summary>
	/// The booster icon.
	/// </summary>
	public Image boosterIcon;
	
	/// <summary>
	/// The booster quantity.
	/// </summary>
	public Text boosterQuantity;

	/// <summary>
	/// The booster description.
	/// </summary>
	public Text boosterDesc;

	/// <summary>
	/// The booster coin.
	/// </summary>
	public Text boosterCoin;

	// The booster type
	private BoosterType _type;

	// The callback
	private Action _callback;

	public void Show(BoosterType type, Action callback)
	{
		// Set booster type
		_type = type;

		// Set callback
		_callback = callback;

		// Set current coin
		coinInfo.text = UserData.Instance.Coin.ToString();
		
		// Set title
		boosterTitle.text = "Buy " + type.GetTitle();

		// Set icon
		boosterIcon.sprite = icons[type.ToInt()];
		
		// Set quantity
		boosterQuantity.text = string.Format("x{0}", type.GetBuyQuantity());

		// Set description
		boosterDesc.text = type.GetDescription();

		int coins = type.GetBuyCoin();

		// Set coin to buy
		boosterCoin.text = coins.ToString();

		// Set coin color
		boosterCoin.color = (UserData.Instance.Coin >= coins ? Constants.CoinColor : Constants.NotEnoughCoinColor);

		// Disable interaction
		SetInteractable(false);

		// Show popup
		UIHelper.ShowPopup(gameObject, () => {
			// Enable interaction
			SetInteractable(true);
		});
	}

	public void Buy()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Hide popup
		HidePopup();

		// Get coin
		int coins = _type.GetBuyCoin();

		// Check if enough coins
		if (UserData.Instance.Coin >= coins)
		{
			// Get quantity
			int quantity = _type.GetBuyQuantity();

			// Increase booster
			if (_type.IsUndo())
			{
				UserData.Instance.Undo += quantity;

				Manager.Instance.analytics.LogEvent("Main Game", "Buy Booster", "Undo", 1);
			}
			else if (_type.IsHint())
			{
				UserData.Instance.Hint += quantity;

				Manager.Instance.analytics.LogEvent("Main Game", "Buy Booster", "Hint", 1);
			}
			else if (_type.IsHammer())
			{
				UserData.Instance.Hammer += quantity;

				Manager.Instance.analytics.LogEvent("Main Game", "Buy Booster", "Hammer", 1);
			}
			else if (_type.IsDeploy())
			{
				UserData.Instance.Deploy += quantity;

				Manager.Instance.analytics.LogEvent("Main Game", "Buy Booster", "Deploy", 1);
			}

			// Decrease coins
			UserData.Instance.Coin -= coins;
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

	public void Close()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Hide popup
		HidePopup();
	}

	void SetInteractable(bool interactable)
	{
		Canvas canvas = FindObjectOfType<Canvas>();

		if (canvas != null)
		{
			canvas.SetInteractable(interactable);
		}
	}
	
	void HidePopup()
	{
		// Disable interaction
		SetInteractable(false);
		
		UIHelper.HidePopup(gameObject, () => {
			// Enable interaction
			SetInteractable(true);

			if (_callback != null)
			{
				_callback();
			}
		});
	}
}
