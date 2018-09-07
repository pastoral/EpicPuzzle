using UnityEngine;
using UnityEngine.UI;
using System;

public class PausePopupScript : FullPopupBehaviour
{
	public Sprite musicOn;
	public Sprite musicOff;
	public Sprite soundOn;
	public Sprite soundOff;

	public GameObject playButton;
	public GameObject restartButton;
	public GameObject homeButton;
	public GameObject musicButton;
	public GameObject soundButton;

	protected override float OverlayDuration
	{
		get
		{
			return 0.2f;
		}
	}

	public override void Show(Action callback = null)
	{
		// Update music image
		SetButtonImage(musicButton, SoundManager.Instance.MusicEnabled ? musicOn : musicOff);

		// Update sound image
		SetButtonImage(soundButton, SoundManager.Instance.SoundEnabled ? soundOn : soundOff);

		base.Show(callback);
	}

	public void Play()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Hide
		Hide(true, GameManager.Instance.Resume);
	}

	public void ToggleMusic()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Toggle music
		bool musicEnabled = !SoundManager.Instance.MusicEnabled;

		// Set music enabled
		SoundManager.Instance.MusicEnabled = musicEnabled;
		
		// Update image
		SetButtonImage(musicButton, musicEnabled ? musicOn : musicOff);
		
		// Persistent data
		UserData.Instance.BGMOn = musicEnabled;
	}
	
	public void ToggleSound()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		// Toggle sound
		bool soundEnabled = !SoundManager.Instance.SoundEnabled;

		// Set sound enabled
		SoundManager.Instance.SoundEnabled = soundEnabled;
		
		// Update image
		SetButtonImage(soundButton, soundEnabled ? soundOn : soundOff);
		
		// Persistent data
		UserData.Instance.SFXOn = soundEnabled;
	}

	void SetButtonImage(GameObject button, Sprite sprite)
	{
		button.GetComponent<Image>().sprite = sprite;
		
		Image overlay = button.GetComponentInChildren<Image>("Overlay");
		
		if (overlay != null)
		{
			overlay.sprite = sprite;
		}
	}

	public override void Restart()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		Manager.Instance.ShowConfirm("Wait!", Settings.ReplayConfirm, (yes) => {
			if (yes)
			{
				Replay();
			}
		});
	}

	public override void Home()
	{
		// Play sound
		SoundManager.PlayButtonClick();

		Manager.Instance.ShowConfirm("Wait!", Settings.QuitConfirm, (yes) => {
			if (yes)
			{
				// Hide
				Hide(false, () => { TransitionManager.Instance.FadeTransitionScene(Settings.MapSelectionScene); });
			}
		});
	}

	protected override void SetShowTouchAndOverlay(bool show)
	{
		SetChildrenEnabled(playButton, show);
		SetChildrenEnabled(restartButton, show);
		SetChildrenEnabled(homeButton, show);
		SetChildrenEnabled(musicButton, show);
		SetChildrenEnabled(soundButton, show);
	}

// #if UNITY_EDITOR
// 	void Update()
// 	{
// 		if (Input.GetKeyDown(KeyCode.Return))
// 		{
// 			Play();
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
