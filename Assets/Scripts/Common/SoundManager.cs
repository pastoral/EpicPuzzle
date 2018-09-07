using UnityEngine;
using System.Collections.Generic;

public enum SoundType
{
	Replace,
	New,
	Only,
	Loop
}

public enum SoundID
{
	// Background musics
	MainMenu,
	MainGame1,
	MainGame2,
	MainGame3,

	// Sound effects
	ButtonClick,
	ButtonOpen,
	SelectLockedMap,
	Explose,
	Cyrus1,
	Cyrus2,
	Cyrus3,
	Cyrus4,
	CyrusStartJump,
	CyrusFinishJump,
	CyrusWrongInput,
	Tick,
	TimeOut,
	WinGame,
	LoseGame,
	BigFirework,
	ChangeDirection,
	Coin,
	UnlockMap,
	UnlockBooster,
	ObjectFinishMove,
	Woosh,
	Typing,
	Ring,
	GotUnlock,

	Count
}

public class SoundManager : Singleton<SoundManager>
{
	[Header("Background Musics")]
	
	public AudioClip mainMenu;
	public AudioClip mainGame1;
	public AudioClip mainGame2;
	public AudioClip mainGame3;

	[Header("Sound Effects")]

	public AudioClip buttonClick;
	public AudioClip buttonOpen;
	public AudioClip selectLockedMap;
	public AudioClip explose;
	public AudioClip cyrus1;
	public AudioClip cyrus2;
	public AudioClip cyrus3;
	public AudioClip cyrus4;
	public AudioClip cyrusStartJump;
	public AudioClip cyrusFinishJump;
	public AudioClip cyrusWrongInput;
	public AudioClip tick;
	public AudioClip timeOut;
	public AudioClip winGame;
	public AudioClip loseGame;
	public AudioClip bigFirework;
	public AudioClip changeDirection;
	public AudioClip coin;
	public AudioClip unlockMap;
	public AudioClip unlockBooster;
	public AudioClip objectFinishMove;
	public AudioClip woosh;
	public AudioClip typing;
	public AudioClip ring;
	public AudioClip gotUnlock;

	[Header("Volume")]

	/// <summary>
	/// The music volume.
	/// </summary>
	[Range(0,1)]
	public float musicVolume = 1f;
	
	/// <summary>
	/// The sound volume.
	/// </summary>
	[Range(0,1)]
	public float soundVolume = 1f;

	/// <summary>
	/// The audio source to play music.
	/// </summary>
	private AudioSource musicSource;

	/// <summary>
	/// The lookup table for background musics.
	/// </summary>
	private Dictionary<SoundID, AudioClip> musicLookup;

	/// <summary>
	/// The lookup table for sound effects.
	/// </summary>
	private Dictionary<SoundID, AudioSource> soundLookup;

	/// <summary>
	/// True if enable to play background music.
	/// </summary>
	private bool isMusicEnabled = true;

	/// <summary>
	/// True if enable to play sound effect.
	/// </summary>
	private bool isSoundEnabled = true;

	protected override void Awake()
	{
		base.Awake();

		// Set instance
		_instance = this;

		// Create audio source
		musicSource = gameObject.AddComponent<AudioSource>();
		musicSource.loop = true;
		musicSource.volume = musicVolume;

		// Create loopkup table for background musics
		musicLookup = new Dictionary<SoundID, AudioClip>();
		
		// Create loopkup table for sound effects
		soundLookup = new Dictionary<SoundID, AudioSource>();

		// Add background musics
		musicLookup.Add(SoundID.MainMenu, mainMenu);
		musicLookup.Add(SoundID.MainGame1, mainGame1);
		musicLookup.Add(SoundID.MainGame2, mainGame2);
		musicLookup.Add(SoundID.MainGame3, mainGame3);

		// Add sound effects
		soundLookup.Add(SoundID.ButtonClick, AddAudioSource(buttonClick));
		soundLookup.Add(SoundID.ButtonOpen, AddAudioSource(buttonOpen));
		soundLookup.Add(SoundID.SelectLockedMap, AddAudioSource(selectLockedMap));
		soundLookup.Add(SoundID.Explose, AddAudioSource(explose));
		soundLookup.Add(SoundID.Cyrus1, AddAudioSource(cyrus1));
		soundLookup.Add(SoundID.Cyrus2, AddAudioSource(cyrus2));
		soundLookup.Add(SoundID.Cyrus3, AddAudioSource(cyrus3));
		soundLookup.Add(SoundID.Cyrus4, AddAudioSource(cyrus4));
		soundLookup.Add(SoundID.CyrusStartJump, AddAudioSource(cyrusStartJump));
		soundLookup.Add(SoundID.CyrusFinishJump, AddAudioSource(cyrusFinishJump));
		soundLookup.Add(SoundID.CyrusWrongInput, AddAudioSource(cyrusWrongInput));
		soundLookup.Add(SoundID.Tick, AddAudioSource(tick));
		soundLookup.Add(SoundID.TimeOut, AddAudioSource(timeOut));
		soundLookup.Add(SoundID.WinGame, AddAudioSource(winGame));
		soundLookup.Add(SoundID.LoseGame, AddAudioSource(loseGame));
		soundLookup.Add(SoundID.BigFirework, AddAudioSource(bigFirework));
		soundLookup.Add(SoundID.ChangeDirection, AddAudioSource(changeDirection));
		soundLookup.Add(SoundID.Coin, AddAudioSource(coin));
		soundLookup.Add(SoundID.UnlockMap, AddAudioSource(unlockMap));
		soundLookup.Add(SoundID.UnlockBooster, AddAudioSource(unlockBooster));
		soundLookup.Add(SoundID.ObjectFinishMove, AddAudioSource(objectFinishMove));
		soundLookup.Add(SoundID.Woosh, AddAudioSource(woosh));
		soundLookup.Add(SoundID.Typing, AddAudioSource(typing));
		soundLookup.Add(SoundID.Ring, AddAudioSource(ring));
		soundLookup.Add(SoundID.GotUnlock, AddAudioSource(gotUnlock));
		
		//
		isMusicEnabled = UserData.Instance.BGMOn;
		isSoundEnabled = UserData.Instance.SFXOn;
	}

	// Change music volume
	public float MusicVolume
	{
		get
		{
			return musicVolume;
		}

		set
		{
			musicVolume = Mathf.Clamp01(value);

			if (isMusicEnabled)
			{
				musicSource.volume = musicVolume;
			}
		}
	}

	// Change sound volume
	public float SoundVolume
	{
		get
		{
			return soundVolume;
		}
		
		set
		{
			soundVolume = Mathf.Clamp01(value);
			
			foreach (AudioSource audioSource in soundLookup.Values)
			{
				audioSource.volume = soundVolume;
			}
		}
	}

	// Enable/Disable background music
	public bool MusicEnabled
	{
		get
		{
			return isMusicEnabled;
		}

		set
		{
			if (isMusicEnabled != value)
			{
				isMusicEnabled = value;

//				musicSource.enabled = isMusicEnabled;
				musicSource.volume = isMusicEnabled ? musicVolume : 0;
			}
		}
	}

	// Enable/Disable sound effect
	public bool SoundEnabled
	{
		get
		{
			return isSoundEnabled;
		}

		set
		{
			if (isSoundEnabled != value)
			{
				isSoundEnabled = value;

				foreach (AudioSource audioSource in soundLookup.Values)
				{
					audioSource.enabled = isSoundEnabled;
				}
			}
		}
	}

	public bool PlaySound(SoundID soundID, SoundType type = SoundType.Replace, float delay = 0f)
	{
		if (!isSoundEnabled) return false;

		// Get audio source
		AudioSource audioSource = soundLookup[soundID];

		if (audioSource != null)
		{
			if (type == SoundType.Loop)
			{
				audioSource.loop = true;

				if (!audioSource.isPlaying)
				{
					if (delay > 0)
					{
						audioSource.PlayDelayed(delay);
					}
					else
					{
						audioSource.Play();
					}
				}
			}
			else
			{
				audioSource.loop = false;

				if (type == SoundType.Replace)
				{
					if (delay > 0)
					{
						audioSource.PlayDelayed(delay);
					}
					else
					{
						audioSource.Play();
					}
				}
				else if (type == SoundType.New)
				{
					audioSource.PlayOneShot(audioSource.clip);
				}
				else if (type == SoundType.Only)
				{
					if (!audioSource.isPlaying)
					{
						if (delay > 0)
						{
							audioSource.PlayDelayed(delay);
						}
						else
						{
							audioSource.Play();
						}
					}
				}
			}

			return true;
		}

		return false;
	}

	public bool PlayRandomSound(params SoundID[] soundIDs)
	{
		return PlaySound(soundIDs.Any());
	}

	public bool PlayMusic(SoundID soundID)
	{
		// Get audio clip
		AudioClip audioClip = musicLookup[soundID];

		if (audioClip != null)
		{
			// Set clip
			musicSource.clip = audioClip;

			// Play music
			musicSource.Play();

			//
			musicSource.volume = isMusicEnabled ? musicVolume : 0;

			return true;
		}

		return false;
	}
	
	public bool PlayRandomMusic(params SoundID[] musicIDs)
	{
		return PlayMusic(musicIDs.Any());
	}

	public void StopSound(SoundID soundID)
	{
		AudioSource audioSource = soundLookup[soundID];
		
		if (audioSource != null)
		{
			audioSource.Stop();
		}
	}

	public void StopAllSounds()
	{
		foreach (AudioSource audioSource in soundLookup.Values)
		{
			audioSource.Stop();
		}
	}

	public void StopMusic()
	{
		musicSource.Stop();
	}

	public static void PlayButtonClick()
	{
		_instance.PlaySound(SoundID.ButtonClick);
	}
	
	public static void PlayButtonOpen()
	{
		_instance.PlaySound(SoundID.ButtonOpen);
	}

	AudioSource AddAudioSource(AudioClip clip)
	{
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.clip = clip;
		source.playOnAwake = false;
		source.volume = soundVolume;

		return source;
	}
}
