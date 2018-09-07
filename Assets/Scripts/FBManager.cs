using UnityEngine;
using System;
using System.IO;
using Facebook.Unity;

public class FBManager : Singleton<FBManager>
{
#if UNITY_EDITOR
	private static Action<string> _logInCallback;
	private static bool _isLogIn;
	private static bool _isLoggingIn;

	private static Action _updateScoreCallback;
	private bool _isUpdateScore;
#endif

	public void OnUpdate()
	{
		// Update score
		UpdateScore();
	}

	public static void LogIn(Action<string> callback)
	{
#if UNITY_EDITOR
		_logInCallback = callback;
		_isLogIn = true;
#else
		FBHelper.LogIn(callback);
#endif
	}

	public static void GetAvatar(string id, Action<Sprite> callback)
	{
#if UNITY_EDITOR
		FBHelper.GetAvatar(id, (texture, error) => {
			if (string.IsNullOrEmpty(error))
			{
				callback(texture.ToSprite());
			}
			else
			{
				callback(null);
			}
		});
#else
		string path = Helper.GetFilePath("avatars");

		try
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		catch (IOException ex)
		{
			//Debug.Log(ex.Message);
		}

		path = string.Format("{0}/{1}.png", path, id);

		Texture2D texture = TextureHelper.Load(path);

		if (texture != null)
		{
			callback(texture.ToSprite());
		}
		else
		{
			FBHelper.GetAvatar(id, (texture2, error) => {
				if (string.IsNullOrEmpty(error))
				{
					texture2.Save(path);

					callback(texture2.ToSprite());
				}
				else
				{
					callback(null);
				}
			});
		}
#endif
	}

	public void UpdateScore(Action callback = null)
	{
		// Check if online
		if (Helper.IsOnline())
		{
			// Check if logged in Facebook
			if (FB.IsLoggedIn)
			{
#if UNITY_EDITOR
				_updateScoreCallback = callback;
				_isUpdateScore = true;
#else
				UpdateScoreInternal(callback);
#endif
			}
			else
			{
				if (callback != null)
				{
					callback();
				}
			}
		}
		else
		{
			if (callback != null)
			{
				callback();
			}
		}
	}
	
	void UpdateScoreInternal(Action callback)
	{
		
		// Get score
		FBHelper.GetScore(null, (score, error) => {
			// Get current level (zero-based)
			int level = UserData.Instance.Level - 1;
			
			if (score < level)
			{
				// Update score
				FBHelper.PostScore(null, level, (success, err) => {
					if (callback != null)
					{
						callback();
					}
				});
			}
			else
			{
				if (score > level)
				{
					// Anti-hack
					if (score > Settings.MapCount)
					{
						score = Settings.MapCount;

						// Update score
						FBHelper.PostScore(null, score);
					}

					// Set winned
					UserData.Instance.SetWinned(level + 1, score);

					// Update level
					UserData.Instance.Level = score + 1;
				}

				if (callback != null)
				{
					callback();
				}
			}
		});
	}

#if UNITY_EDITOR
	void OnGUI()
	{
		if (_isLoggingIn) return;

		if (_isLogIn)
		{
			_isLogIn = false;
			_isLoggingIn = true;

			FBHelper.LogIn((error) => {
				_isLoggingIn = false;

				if (_logInCallback != null)
				{
					_logInCallback(error);
				}
			});

			return;
		}

		if (_isUpdateScore)
		{
			_isUpdateScore = false;

			UpdateScoreInternal(_updateScoreCallback);
		}
	}
#endif
}
