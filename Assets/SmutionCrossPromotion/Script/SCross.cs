using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SCross : SimpleSingleton<SCross> {

	[SerializeField]
	private Restifizer.RestifizerManager restApi;

	[SerializeField]
	private RectTransform scrossPopup;
	[SerializeField]
	private RectTransform checkVersionPopup;

	private string ActionURL;
	// Use this for initialization
	void Start () {
	   
	}

	#region Cross Promotion

	public void GetImage(string currentGameId, System.Action<bool, object> callback) {
		var url = string.Format (SConstants.ScrossUrl + "?name_code={0}", currentGameId);

		restApi.ResourceAt (url)
			.Get (response => {

				if (response != null && !response.HasError) {
					if (response.Resource != null) {

						SCrossResult result = new SCrossResult(response.Resource);

						if (result.listGames != null && result.listGames.Count != 0 ){
							SCrossResourse.Instance.CrossData = result.listGames;
							SCrossResourse.Instance.CrossRatio = result.Ratio;
							SCrossResourse.Instance.Save();
						}

						callback(true, result.listGames);
					}
					else {
						callback(false, "Null response");
					}
				}
				else {
					callback(false, "Cannot handle response.");
				}
		});

	}

	public void ShowPopupScross() {
		int rand = Random.Range (0, 100);
		Common.Log("Ratio: {0} -- {1}", rand, SCrossResourse.Instance.CrossRatio);

		if (rand > SCrossResourse.Instance.CrossRatio) {
			Debug.Log (string.Format("SCross: Not Show Popup Because Of Ratio ( Current Ratio is {0})", SCrossResourse.Instance.CrossRatio));
			return;
		}

		Debug.Log (string.Format ("SCross: Now Showing Popup with Ratio {0}", SCrossResourse.Instance.CrossRatio));

		Game game = GetGame ();

		if (game == null) {
			Debug.Log ("SCross: Dont Have Any Game To Show ( Or All Game With Weight = 0)");
			return;
		}

		if (!scrossPopup) {
			Debug.Log ("SCross: Dont have popup");
			if (GameObject.Find (SConstants.SCrossPopup)) {
				scrossPopup = (RectTransform) GameObject.Find (SConstants.SCrossPopup).transform;
			}
		}

		if (scrossPopup) {
			Debug.Log (string.Format("SCross: Showing SCross for game {0}", game.Name) );
			scrossPopup.GetComponent<SCrossButton> ().appstoreUrl = game.AppStoreUrl;
			scrossPopup.GetComponent<SCrossButton> ().ggplayUrl = game.GGPlayUrl;
			scrossPopup.GetComponent<SCrossButton> ().wpUrl = game.WPUrl;

			StartCoroutine (GetImage (scrossPopup, game.ImageUrl));


		}
	}

	private static System.Random _rnd = new System.Random();
	public static Game GetGame()
	{
		// totalWeight is the sum of all brokers' weight
		List<Game> games = SCrossResourse.Instance.CrossData;

		if (games == null) {
			return null;
		}

		int totalWeight = 0;
		foreach (Game game in games) {
			totalWeight += game.Weight;
		}


		int randomNumber = _rnd.Next(0, totalWeight);

		Game selectedBroker = null;
		foreach (Game game in games)
		{
			if (randomNumber < game.Weight)
			{
				selectedBroker = game;
				break;
			}

			randomNumber = randomNumber - game.Weight;
		}

		return selectedBroker;
	}

	private IEnumerator GetImage(RectTransform popup, string stringUrl) {
		WWW www = new WWW(stringUrl);
		yield return www;

		if (www.error == null) {
			popup.GetComponent<Image> ().sprite = www.texture.ToSprite ();
			popup.gameObject.SetActive (true);

			Debug.Log ("SCross: Popup has showed");
		} else {
			Debug.Log (string.Format("SCross: Cannot download Current Image With URL: {0}", stringUrl));
		}

	
	}

	#endregion

	#region Show Message

	public void ShowMessage(string gameName, string currentVersion, System.Action<bool, string> callback) {
		restApi.ResourceAt (SConstants.CheckVersionUrl + gameName)
			.Get (response => {
				if (response != null && !response.HasError) {
					if (response.Resource != null) {
//						Hashtable data = (Hashtable)response.Resource["data"];

						SMessageResult result = new SMessageResult(response.Resource);
						if (result.ShouldShow) {

							switch(result.Condition) {
							case SMessageCondition.CheckVersion:
								var ver1 = new System.Version(currentVersion);
								var ver2 = new System.Version(result.Version);

								var compareResult = ver2.CompareTo(ver1);


								if (compareResult > 0) {

									MobileNativeDialog dialog = new MobileNativeDialog(result.Title, result.MessageDialog, "OK", "Cancel");
									dialog.OnComplete += OnDialogClose;

									#if UNITY_IOS 
									this.ActionURL = result.iOSAction;
									#elif UNITY_ANDROID
									this.ActionURL = result.AndroidAction;
									#elif UNITY_WP8 || UNITY_WP8_1
									this.ActionURL = result.WPAction;
									#endif

									callback(true, "Show Message");
								}
								break;

							case SMessageCondition.UnCheckVersion:

								MobileNativeDialog dialogMessage = new MobileNativeDialog(result.Title, result.MessageDialog, "OK", "Cancel");
								dialogMessage.OnComplete += OnDialogClose;

								#if UNITY_IOS 
								this.ActionURL = result.iOSAction;
								#elif UNITY_ANDROID
								this.ActionURL = result.AndroidAction;
								#elif UNITY_WP8 || UNITY_WP8_1
								this.ActionURL = result.WPAction;
								#endif

								callback(true, "Show Message");
								break;
							}


						}
						else {
							callback(false, "Not show popup.");
						}

					}
					else {
						callback(false, "Null response.");
					}
				}
				else {
					callback(false, "Cannot handle response.");
				}
		});
	}

	private void OnDialogClose(MNDialogResult result) {
		//parsing result
		switch(result) {
		case MNDialogResult.YES:
			Debug.Log ("Yes button pressed");
			Application.OpenURL (this.ActionURL);

			break;
		case MNDialogResult.NO:
			Debug.Log ("No button pressed");
			break;
		}
	}

	#endregion


}

public class SApiResult {
	public int Code;
	public string Message;
	public bool IsSuccess;

	public void GetCodeAndMessage(Hashtable resource) {

		try {
			this.Code = int.Parse (resource ["code"].ToString ());
			this.Message = resource ["message"].ToString ();

			this.IsSuccess = this.Code > SConstants.RequestSuccessCode ? false : true;
		}
		catch {
			this.IsSuccess = false;
		}
	}
		
}

public class SCrossResult : SApiResult {	
	
	public List<Game> listGames;
	public int Ratio;

	public SCrossResult(Hashtable resource) {

		GetCodeAndMessage (resource);

		if (!IsSuccess) {
			return;
		}

		try {
			var data = (Hashtable)resource ["result"];

			this.Ratio = int.Parse(data["ratio"].ToString());

			var games = (ArrayList)data["games"];

			listGames = new List<Game> ();
			foreach (Hashtable item in games) {
				Game game = new Game (item);
				listGames.Add (game);
			}

		}
		catch {
			this.IsSuccess = false;
		}
	}
}

[System.Serializable]
public class Game {
	public string Id;
	public string Name;

	public string AppStoreUrl;
	public string GGPlayUrl;
	public string WPUrl;

	public string ImageUrl;

	public int Weight;

	public Game(Hashtable data) {
		this.Id = data ["name_code"].ToString ();
		this.Name = data ["name"].ToString ();
		this.AppStoreUrl = data ["ios_url"].ToString ();
		this.GGPlayUrl = data ["android_url"].ToString ();
		this.WPUrl = data ["wp_url"].ToString ();

		this.ImageUrl = data ["image_url"].ToString ();
	
		this.Weight = int.Parse(data ["weight"].ToString ());
	}
}

public enum SMessageCondition {
	CheckVersion,
	UnCheckVersion,
	None
}

public class SMessageResult : SApiResult {
	public SMessageCondition Condition;
	public Hashtable Result;

	public bool ShouldShow;
	public string Title;
	public string MessageDialog;
	public int ShowType;
	public string Version;

	public string iOSAction;
	public string AndroidAction;
	public string WPAction;

	public SMessageResult(Hashtable resource) {
		GetCodeAndMessage (resource);

		if (!IsSuccess) {
			return;
		}

		try {

			this.Result = (Hashtable)resource ["result"];
			var condition = this.Result ["condition"].ToString ();

			this.Condition = GetEnum(condition);

			if (this.Condition == SMessageCondition.None) {
				this.ShouldShow = false;
				return;
			}

			var isShow = this.Result ["status"].ToString ();
			if (isShow.Equals ("on")) {
				this.ShouldShow = true;
			} else {
				this.ShouldShow = false;
				return;
			}

			this.Version = this.Result ["version_on_server"].ToString ();
			this.Title = this.Result ["title"].ToString ();
			this.MessageDialog = this.Result ["message"].ToString ();

			this.iOSAction = this.Result ["ios_action"].ToString ();
			this.AndroidAction = this.Result ["android_action"].ToString ();
			this.WPAction = this.Result ["wp_action"].ToString ();

			if (string.IsNullOrEmpty (this.Title) || string.IsNullOrEmpty (this.Message)) {
				this.ShouldShow = false;
			}

		}
		catch {
			this.ShouldShow = false;
		}
	}

	public SMessageCondition GetEnum(string value) {
		switch (value) {
		case "check_version":
			return SMessageCondition.CheckVersion;
		case "un_check_version":
			return SMessageCondition.UnCheckVersion;

		default:
			return SMessageCondition.None;
		}
	}
}


