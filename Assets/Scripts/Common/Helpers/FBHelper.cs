using UnityEngine;
using Facebook.Unity;
using Facebook.MiniJSON;
using System.Collections.Generic;
using System.Text;
using System;

public enum FriendType
{
	None,
	Game,
	Invitable
}

public class FBHelper
{
	private static readonly string Namespace = "cyrusbeanjump";
	private static readonly string[] readPermissions    = { "public_profile", "email", "user_friends" };
	private static readonly string[] publishPermissions = { "publish_actions"};
	private static readonly int avatarWidth  = 128;
	private static readonly int avatarHeight = 128;

	private static readonly string Coin = "coin";
	private static readonly string Mana = "mana";
//	private static readonly string InviteToGetCoin = "invite_coin";
//	private static readonly string InviteToGetMana = "invite_mana";

	private static string _userID   = "";
	private static string _userName = "";

	public static string UserID
	{
		get
		{
			return _userID;
		}
	}
	
	public static string UserName
	{
		get
		{
			return _userName;
		}
	}

	// Initialize Facebook
	public static void Init()
	{
		if (!Helper.IsOnline()) return;

		if (FB.IsInitialized)
		{
			FB.ActivateApp();
		}
		else
		{
			FB.Init(OnInitComplete, OnHideUnity);
		}
	}

	static void OnInitComplete()
	{
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
		}
		else
		{
			//Log.Debug("Failed to initialize the Facebook SDK!");
		}
	}

	static void OnHideUnity(bool isUnityShown)
	{
		// Resume game
		if (isUnityShown)
		{
			Time.timeScale = 1.0f;
		}
		// Pause game
		else
		{
			Time.timeScale = 0.0f;
		}
	}

	/// <summary>
	/// Log in Facebook.
	/// Return callback(error).
	/// </summary>
	public static void LogIn(Action<string> callback)
	{
		if (FB.IsInitialized)
		{
//			LogInWithPublishPermissions(callback);
			LogInWithAllPermissions(callback);
		}
		else
		{
			FB.Init(() => {
				if (FB.IsInitialized)
				{
					FB.ActivateApp();
					
//					LogInWithPublishPermissions(callback);
					LogInWithAllPermissions(callback);
				}
				else
				{
					if (callback != null)
					{
						callback("Failed to initialize the Facebook SDK!");
					}
				}
			}, OnHideUnity);
		}
	}

	public static void LogInWithReadPermissions(Action<string> callback)
	{
		FB.LogInWithReadPermissions(readPermissions, (result) => {
			if (FB.IsLoggedIn)
			{
				FB.API("/me", HttpMethod.GET, (result2) => {
					if (string.IsNullOrEmpty(result2.Error))
					{
						var dict = result2.ResultDictionary;
						var name = dict["name"];
						var id   = dict["id"];
						
						_userName = name.ToString();
						_userID   = id.ToString();
					}

					if (callback != null)
					{
						callback(result.Error);
					}
					else
					{
						var accessToken = result.AccessToken;
						
						foreach (var permission in accessToken.Permissions)
						{
							//Log.Debug(permission);
						}
					}
				});
			}
			else
			{
				if (callback != null)
				{
					callback("User cancelled login");
				}
			}
		});
	}

	public static void LogInWithAllPermissions(Action<string> callback)
	{
		FB.LogInWithReadPermissions(readPermissions, (result) => {
			Manager.Instance.AddSchedule(() => {
				if (FB.IsLoggedIn)
				{
					FB.LogInWithPublishPermissions(publishPermissions, (result2) => {
						if (callback != null)
						{
							callback(result.Error);
						}
					});
				}
				else
				{
					if (callback != null)
					{
						callback("User cancelled login");
					}
				}
			}, 0.5f);
		});
	}

	public static void LogInWithPublishPermissions(Action<string> callback)
	{
		FB.LogInWithPublishPermissions(publishPermissions, (result) => {
			if (FB.IsLoggedIn)
			{
				if (callback != null)
				{
					callback(result.Error);
				}
				else
				{
					var accessToken = result.AccessToken;
					
					foreach (var permission in accessToken.Permissions)
					{
						//Log.Debug(permission);
					}
				}
			}
			else
			{
				if (callback != null)
				{
					callback("User cancelled login");
				}
			}
		});
	}

	// Log out Facebook
	public static void LogOut()
	{
		FB.LogOut();
	}

	// Get avatar
	// Return callback(texture, error)
	public static void GetAvatar(string userID, Action<Texture2D, string> callback)
	{
		FB.API(GetPictureURL(string.IsNullOrEmpty(userID) ? "me" : userID, avatarWidth, avatarHeight), HttpMethod.GET, (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback(null, result.Error);
			}
			else
			{
				callback(result.Texture, null);
			}
		});
	}

	// Get score
	// Return callback(score, error)
	public static void GetScore(string userID, Action<int, string> callback)
	{
		string query = string.IsNullOrEmpty(userID) ? "/me/scores?fields=score,user.limit(0)" : string.Format("/{0}/scores?fields=score,user.limit(0)", userID);
		
		FB.API(query, HttpMethod.GET, (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback(0, result.Error);
			}
			else
			{
				var dict = result.ResultDictionary;
				var data = (List<object>)dict["data"];

				// Not existed
				if (data.Count == 0)
				{
					callback(0, null);
				}
				else
				{
					var entry = (Dictionary<string, object>)data[0];
					var score = entry["score"];

					callback(score.ToString().ToInt(), null);
				}
			}
		});
	}

	/// <summary>
	/// Post score.
	/// Return callback(success, error).
	/// </summary>
	public static void PostScore(string userID, int score, Action<bool, string> callback = null)
	{
		string query = string.IsNullOrEmpty(userID) ? "/me/scores" : string.Format("/{0}/scores", userID);
		var scoreData = new Dictionary<string, string>() {{"score", score.ToString()}};
		
		FB.API(query, HttpMethod.POST, (result) => {
			DoCallback(result, callback);
		},
		scoreData);
	}
	
	// Delete score
	// Return callback(success, error)
	public static void DeleteScore(string userID, Action<bool, string> callback = null)
	{
		string query = string.IsNullOrEmpty(userID) ? "/me/scores" : string.Format("/{0}/scores", userID);
		
		FB.API(query, HttpMethod.DELETE, (result) => {
			DoCallback(result, callback);
		});
	}

	/// <summary>
	/// Get highscores.
	/// Return callback(ids, names, scores, error).
	/// </summary>
	public static void GetHighscores(Action<string[], string[], int[], string> callback)
	{
		string query = "/app/scores?fields=score,user.fields(id,name)";
		
		FB.API(query, HttpMethod.GET, (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback(null, null, null, result.Error);
			}
			else
			{
				var response = result.ResultDictionary;
				var data = (List<object>)response["data"];
				int count = data.Count;
				
				string[] ids   = new string[count];
				string[] names = new string[count];
				int[] scores   = new int[count];

				for (int i = 0; i < count; i++)
				{
					var entry = (Dictionary<string, object>)data[i];
					var score = entry["score"];
					var user = (Dictionary<string, object>)entry["user"];

					ids[i]    = user["id"].ToString();
					names[i]  = user["name"].ToString();
					scores[i] = score.ToString().ToInt();
				}
				
				callback(ids, names, scores, null);
			}
		});
	}
	
	// Delete highscores
	// Return callback(success, error)
	public static void DeleteHighscores(Action<bool, string> callback)
	{
		string query = "/app/scores";
		
		FB.API(query, HttpMethod.DELETE, (result) => {
			DoCallback(result, callback);
		});
	}

	// Invite friends
	// Return callback(error)
	public static void Invite(string title, string message, FriendType friendType = FriendType.Invitable, FBObjectType objectType = FBObjectType.Coin, Action<string> callback = null)
	{
#if UNITY_EDITOR
		if (callback != null)
		{
			callback("Not supported!");
		}
#else
		if (FB.IsLoggedIn)
		{
			InviteInternal(title, message, friendType, objectType, callback);
		}
		else
		{
			LogIn((error) => {
				if (!string.IsNullOrEmpty(error))
				{
					if (callback != null)
					{
						callback(error);
					}
				}
				else
				{
					InviteInternal(title, message, friendType, objectType, callback);
				}
			});
		}
#endif
	}

	static void InviteInternal(string title, string message, FriendType friendType = FriendType.Invitable, FBObjectType objectType = FBObjectType.Coin, Action<string> callback = null)
	{
#if UNITY_EDITOR
		if (callback != null)
		{
			callback("Not supported!");
		}
#else	
		FB.AppRequest(
			message,
			null,
			GetFriendFilters(friendType),
			null,
			null,
			objectType == FBObjectType.Coin ? Coin : Mana,
			title,
			(result) => {
				if (string.IsNullOrEmpty(result.Error))
				{
					var dictionary = result.ResultDictionary;

//					foreach (var key in dictionary.Keys)
//					{
//						Log.Debug(string.Format("{0}:{1}", key, dictionary[key]));
//					}

					if (dictionary.ContainsKey("to"))
					{
						var to = dictionary["to"] as List<object>;

						if (to == null)
						{
							string[] tos = dictionary["to"].ToString().Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

							// Coin
							if (objectType == FBObjectType.Coin)
							{
								for (int i = 0; i < tos.Length; i++)
								{
									FriendData.Instance.AddCoinInvitedFriend(tos[i]);
								}
							}
							// Mana
							else
							{
								for (int i = 0; i < tos.Length; i++)
								{
									FriendData.Instance.AddManaInvitedFriend(tos[i]);
								}
							}
						}
						else
						{
							// Coin
							if (objectType == FBObjectType.Coin)
							{
								for (int i = 0; i < to.Count; i++)
								{
									FriendData.Instance.AddCoinInvitedFriend(to[i].ToString());
								}
							}
							// Mana
							else
							{
								for (int i = 0; i < to.Count; i++)
								{
									FriendData.Instance.AddManaInvitedFriend(to[i].ToString());
								}
							}
						}

						Log.Debug(FriendData.Instance.ToString());
					}
				}
				else
				{
					Log.Debug("Invite error: " + result.Error);
				}

				if (callback != null)
				{
					callback(result.Error);
				}
			}
		);
#endif
	}

	public static void InviteDeepLink(string appURI, string imageURI, Action<string, string> callback)
	{
		if (FB.IsLoggedIn)
		{
			InviteDeepLinkInternal(appURI, imageURI, callback);
		}
		else
		{
			LogIn((error) => {
				if (!string.IsNullOrEmpty(error))
				{
					if (callback != null)
					{
						callback(null, error);
					}
				}
				else
				{
					InviteDeepLinkInternal(appURI, imageURI, callback);
				}
			});
		}
	}

	static void InviteDeepLinkInternal(string appURI, string imageURI, Action<string, string> callback)
	{
		FB.Mobile.AppInvite(new System.Uri(appURI), new System.Uri(imageURI), (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback(null, result.Error);
			}
			else
			{
				callback(result.RawResult, null);
			}
		});
	}

	/// <summary>
	/// Send object.
	/// Return callback(error).
	/// </summary>
	public static void SendObject(string objectId, string name, string title, string message, Action<string> callback, params string[] recipients)
	{
		if (FB.IsLoggedIn)
		{
			RequestObject(OGActionType.SEND, objectId, name, title, message, callback, recipients);
		}
		else
		{
			LogIn((error) => {
				if (!string.IsNullOrEmpty(error))
				{
					if (callback != null)
					{
						callback(error);
					}
				}
				else
				{
					RequestObject(OGActionType.SEND, objectId, name, title, message, callback, recipients);
				}
			});
		}
	}
	
	// Ask for object
	// Return callback(error)
	public static void AskForObject(string objectId, string name, string title, string message, Action<string> callback, params string[] recipients)
	{
		if (FB.IsLoggedIn)
		{
			RequestObject(OGActionType.ASKFOR, objectId, name, title, message, callback, recipients);
		}
		else
		{
			LogIn((error) => {
				if (!string.IsNullOrEmpty(error))
				{
					if (callback != null)
					{
						callback(error);
					}
				}
				else
				{
					RequestObject(OGActionType.ASKFOR, objectId, name, title, message, callback, recipients);
				}
			});
		}
	}

	// Request object
	// Return callback(error)
	static void RequestObject(OGActionType actionType, string objectId, string name, string title, string message, Action<string> callback, params string[] recipients)
	{
#if UNITY_EDITOR
		if (callback != null)
		{
			callback("Not supported!");
		}
#else
		if (string.IsNullOrEmpty(objectId))
		{
			CreateObject(name, name, (id, error) => {
				if (!string.IsNullOrEmpty(error))
				{
					if (callback != null)
					{
						callback(error);
					}
				}
				else
				{
					RequestObject(actionType, id, name, title, message, callback, recipients);
				}
			});
			
			return;
		}
		
		if (recipients == null)
		{
			FB.AppRequest(
				message,
				actionType,
				objectId,
				new List<object>() {"app_users"},
				null,
				null,
				name,
				title,
				(result) =>
				{
					if (callback != null)
					{
						callback(result.Error);
					}
					else
					{
						if (!string.IsNullOrEmpty(result.Error))
						{
							//Log.Debug(result.Error);
						}
						else
						{
							//Log.Debug(result.RawResult);
						}
					}
				}
			);
		}
		else
		{
			FB.AppRequest(
				message,
				actionType,
				objectId,
				recipients,
				name,
				title,
				(result) => {
					if (callback != null)
					{
						callback(result.Error);
					}
					else
					{
						if (!string.IsNullOrEmpty(result.Error))
						{
							//Log.Debug(result.Error);
						}
						else
						{
							//Log.Debug(result.RawResult);
						}
					}
				}
			);
		}
#endif
	}

	// Create object
	// Return callback(objectId, error)
	static void CreateObject(string name, string title, Action<string, string> callback)
	{
		string objectType = string.Format("{0}:{1}", Namespace, name);
		var query = string.Format("me/objects/{0}", objectType);
		
		var dict = new Dictionary<string, string>();
//		dict["url"] = "http://samples.ogp.me/140802769599768";
		dict["title"] = title;
		dict["type"] = objectType;
//		dict["image"] = "https://fbstatic-a.akamaihd.net/images/devsite/attachment_blank.png";
//		dict["description"] = "";
		dict["app_id"] = FB.AppId;
		
		var formData = new Dictionary<string, string>();
		formData["object"] = Json.Serialize(dict);

		FB.API(query, HttpMethod.POST, (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback(null, result.Error);
			}
			else
			{
				var response = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
				object idH;
				
				if (response.TryGetValue("id", out idH))
				{
					callback(idH.ToString(), null);
				}
				else
				{
					callback(null, "\"id\" not found!");
				}
			}
		},
		formData);
	}

	/// <summary>
	/// Get SEND requests.
	/// Return callback(requests, error).
	/// </summary>
	public static void GetSendRequests(Action<List<RequestData>, string> callback)
	{
		FB.API("/me/apprequests?fields=id,from,data,action_type", HttpMethod.GET, (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback(null, result.Error);
			}
			else
			{
				if (!string.IsNullOrEmpty(result.RawResult))
				{
					var pendingRequests = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
					var pendingRequestsData = pendingRequests["data"] as List<object>;

					if (pendingRequestsData != null && pendingRequestsData.Count > 0)
					{
						var list = new List<RequestData>();

						object idH;
						object fromH;
						object dataH;
						object actionTypeH;

						foreach (var entry in pendingRequestsData)
						{
							var requestItem = entry as Dictionary<string, object>;

							string id = "";
							string fromId = "";
							string fromName = "";
							FBObjectType objectType = FBObjectType.None;

							if (requestItem.TryGetValue("id", out idH))
							{
								id = idH.ToString();
							}

							if (requestItem.TryGetValue("from", out fromH))
							{
								var from = (Dictionary<string, object>)fromH;
								fromId   = from["id"].ToString();
								fromName = from["name"].ToString();
							}

							if (requestItem.TryGetValue("data", out dataH))
							{
								string item = dataH.ToString();

								if (item == Coin) objectType = FBObjectType.Coin;
								else if (item == Mana) objectType = FBObjectType.Mana;
//								else if (item == InviteToGetCoin) objectType = FBObjectType.InviteCoin;
//								else if (item == InviteToGetMana) objectType = FBObjectType.InviteMana;
								else objectType = FBObjectType.Unknown;
							}

							if (requestItem.TryGetValue("action_type", out actionTypeH))
							{
								if (actionTypeH.ToString() == "send")
								{
									list.Add(new RequestData(id, fromId, fromName, FBActionType.Send, objectType));
								}
							}
						}

						callback(list, null);
					}
					else
					{
						callback(null, null);
					}
				}
				else
				{
					callback(null, null);
				}
			}
		});
	}

	/// <summary>
	/// Get ASK requests.
	/// Return callback(requests, error).
	/// </summary>
	public static void GetAskRequests(Action<List<RequestData>, string> callback)
	{
		FB.API("/me/apprequests?fields=id,from,data,action_type", HttpMethod.GET, (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback(null, result.Error);
			}
			else
			{
				if (!string.IsNullOrEmpty(result.RawResult))
				{
					var pendingRequests = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
					var pendingRequestsData = pendingRequests["data"] as List<object>;

					if (pendingRequestsData != null && pendingRequestsData.Count > 0)
					{
						var list = new List<RequestData>();

						object idH;
						object fromH;
						object dataH;
						object actionTypeH;

						foreach (var entry in pendingRequestsData)
						{
							var requestItem = entry as Dictionary<string, object>;

							string id = "";
							string fromId = "";
							string fromName = "";
							FBObjectType objectType = FBObjectType.None;

							if (requestItem.TryGetValue("id", out idH))
							{
								id = idH.ToString();
							}

							if (requestItem.TryGetValue("from", out fromH))
							{
								var from = (Dictionary<string, object>)fromH;
								fromId   = from["id"].ToString();
								fromName = from["name"].ToString();
							}

							if (requestItem.TryGetValue("data", out dataH))
							{
								string item = dataH.ToString();

								if (item == Coin) objectType = FBObjectType.Coin;
								else if (item == Mana) objectType = FBObjectType.Mana;
//								else if (item == InviteToGetCoin)
//								{
//									// Send coin
//									SendObject(null, InviteToGetCoin, Settings.SendCoinByInviteTitle, string.Format(Settings.SendCoinByInviteMessage, Settings.CoinByInvite), (error) => {
//										if (!string.IsNullOrEmpty(error))
//										{
//											//Log.Debug("Invite error: " + error);
//										}
//									}, fromId);
//
//									DeleteRequest(id);
//
//									callback(null, null);
//
//									return;
//								}
//								else if (item == InviteToGetMana)
//								{
//									// Send mana
//									SendObject(null, InviteToGetMana, Settings.SendManaByInviteTitle, string.Format(Settings.SendManaByInviteMessage, Settings.ManaByInvite), (error) => {
//										if (!string.IsNullOrEmpty(error))
//										{
//											//Log.Debug("Invite error: " + error);
//										}
//									}, fromId);
//
//									DeleteRequest(id);
//
//									callback(null, null);
//
//									return;
//								}
								else objectType = FBObjectType.Unknown;
							}

							if (requestItem.TryGetValue("action_type", out actionTypeH))
							{
								if (actionTypeH.ToString() == "askfor")
								{
									list.Add(new RequestData(id, fromId, fromName, FBActionType.AskFor, objectType));
								}
							}
						}

						callback(list, null);
					}
					else
					{
						callback(null, null);
					}
				}
				else
				{
					callback(null, null);
				}
			}
		});
	}

	public static void DeleteRequest(string id)
	{
		FB.API(id, HttpMethod.DELETE, (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				//Log.Debug("Delete request failed: " + result.Error);
			}
		});
	}

	/// <summary>
	/// Get app user name.
	/// Return callback(name).
	/// </summary>
	public static void GetAppUserName(string uid, Action<string> callback)
	{
		FB.API(string.Format("me?fields=friends.uid({0})", uid), HttpMethod.GET, (result) => {
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback("");
			}
			else
			{
				var dictionary = result.ResultDictionary;

				if (dictionary.ContainsKey("friends"))
				{
					var friends = dictionary["friends"] as Dictionary<string, object>;
					var data = friends["data"] as List<object>;

					if (data.Count == 0)
					{
						callback("");
					}
					else
					{
						var friend = data[0] as Dictionary<string, object>;
						callback(friend["name"].ToString());
					}
				}
				else
				{
					callback("");
				}
			}
		});
	}

	// Get picture URL
	static string GetPictureURL(string userID, int? width = null, int? height = null)
	{
		if (width != null && height != null)
		{
			return string.Format("/{0}/picture?width={1}&height={2}", userID, width, height);
		}

		if (width != null)
		{
			return string.Format("/{0}/picture?width={1}", userID, width);
		}
		
		if (height != null)
		{
			return string.Format("/{0}/picture?height={1}", userID, height);
		}

		return string.Format("/{0}/picture", userID);
	}

	
	// Get friend filters
	static List<object> GetFriendFilters(FriendType type)
	{
		// Game
		if (type == FriendType.Game)
		{
			return new List<object>() { "app_users" };
		}
		
		// Invitable
		if (type == FriendType.Invitable)
		{
			return new List<object>() { "app_non_users" };
		}
		
		// None
		return null;
	}
	
	static void DoCallback(IGraphResult result, Action<bool, string> callback)
	{
		if (callback != null)
		{
			if (!string.IsNullOrEmpty(result.Error))
			{
				callback(false, result.Error);
			}
			else
			{
				var response = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
				object successH;
				
				if (response.TryGetValue("success", out successH))
				{
					callback(successH.ToString().ToBool(), null);
				}
				else
				{
					callback(false, "\"success\" not found!");
				}
			}
		}
	}
}
