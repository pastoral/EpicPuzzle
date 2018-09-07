using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class MyAdmob : Singleton<MyAdmob> {

	InterstitialAd interstitial = null;
	BannerView banner = null;
	public bool isPurchased = false;
	public static string purchaseText = "purchased";

	// Use this for initialization
	void Start () {
		RequestInterstitial();
		RequestBanner();
		if (!PlayerPrefs.HasKey(purchaseText)) PlayerPrefs.SetInt(purchaseText, 0);
		if (PlayerPrefs.GetInt(purchaseText) != 0) isPurchased = true; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowBanner()
	{
		banner.Show();
	}

	public void HideBanner()
	{
		banner.Hide();
	}

	private void RequestBanner()
	{
		#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-4365083222822400/1538311480";
		#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-xxxxxx";
#else
        string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the top of the screen.
		banner = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		banner.LoadAd(request);

		banner.Show();

		Debug.Log("Request banner ads ...");
	}

	public void RequestInterstitial()
	{
		if (isPurchased) return;
			
		#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-4365083222822400/2276678084";
#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-xxxxxx";
#else
        string adUnitId = "unexpected_platform";
		#endif

		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the interstitial with the request.
		interstitial.LoadAd(request);


	}

	public void ShowInterstitial()
	{
		if (isPurchased) return;

		//Debug.Log("Show interstitial");
		if (interstitial != null)
		{
		interstitial.Show();
		}

		RequestInterstitial();
	}
}
