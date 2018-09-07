using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class TransitionManager : Singleton<TransitionManager>
{
	/// <summary>
	/// The shaders.
	/// </summary>
	public Shader[] shaders;

	/// <summary>
	/// The loading.
	/// </summary>
	public GameObject loading;

	// The transition layer (maximum of layer index)
	private static readonly int TransitionLayer = 31;

	// The camera mesh
	private static Mesh _cameraMesh;

	// The level index
	private int _level = -1;

	// The level name
	private string _levelName = "";

	// The camera
	private Camera _camera;

	// The mesh filter
	private MeshFilter _meshFilter;

	// The mesh renderer
	private MeshRenderer _meshRenderer;

	// The material
	private Material _material;

	// Is transition finished?
	private bool _isTransitionFinished = true;

	// Is show loading?
	private bool _isShowLoading;

	// Get material
	public Material Material
	{
		get
		{
			return _material;
		}
	}

	// Determine if transition finished
	public bool TransitionFinished
	{
		get
		{
			return _isTransitionFinished;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		// Get camera
		_camera = GetComponent<Camera>();

		if (_camera == null)
		{
			_camera = gameObject.AddComponent<Camera>();
			_camera.clearFlags = CameraClearFlags.Nothing;
			_camera.cullingMask = 1 << TransitionLayer;
			_camera.orthographic = true;
			_camera.orthographicSize = Camera.main.orthographicSize;
			_camera.nearClipPlane = -1f;
			_camera.farClipPlane = 1f;
			_camera.depth = float.MaxValue;
		}

		// Create camera mesh
		if (_cameraMesh == null)
		{
			_cameraMesh = _camera.GetMesh();
		}

		// Get mesh filter
		_meshFilter = gameObject.GetOrAddComponent<MeshFilter>();

		// Get mesh renderer
		_meshRenderer = GetComponent<MeshRenderer>();

		if (_meshRenderer == null)
		{
			_meshRenderer = gameObject.AddComponent<MeshRenderer>();
			_meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			_meshRenderer.receiveShadows = false;
			_meshRenderer.useLightProbes = false;
			_meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}

		// Get material
		_material = _meshRenderer.material;

		// Set layer
		gameObject.layer = TransitionLayer;

		if (loading != null)
		{
			loading.layer = TransitionLayer;
		}

		// Disable camera
		_camera.enabled = false;

		// Disable mesh renderer
		_meshRenderer.enabled = false;

		// Deactive this object
//		gameObject.SetActive(false);
	}

	public Shader GetShader(string name)
	{
		if (shaders != null)
		{
			for (int i = 0; i < shaders.Length; i++)
			{
				if (shaders[i].name == name)
				{
					return shaders[i];
				}
			}
		}

		return Shader.Find(name);
	}

	public void FadeTransitionScene(int level, bool showLoading = false, Action callback = null)
	{
		TransitionScene(level, showLoading, new FadeTransition(), callback);
	}
	
	public void FadeTransitionScene(string levelName, bool showLoading = false, Action callback = null)
	{
		TransitionScene(levelName, showLoading, new FadeTransition(), callback);
	}

	public void TransitionScene(int level, bool showLoading, TransitionDelegate transitionDelegate, Action callback)
	{
		// Set level
		_level 	   = level;
		_levelName = "";
		_isShowLoading = showLoading;

		TransitionScene(transitionDelegate, callback);
	}
	
	public void TransitionScene(string levelName, bool showLoading, TransitionDelegate transitionDelegate, Action callback)
	{
		// Set level
		_level 	   = -1;
		_levelName = levelName;
		_isShowLoading = showLoading;

		TransitionScene(transitionDelegate, callback);
	}

	public void LoadLevelAsync()
	{
		if (_level >= 0)
		{
			SceneManager.LoadSceneAsync(_level);
		}
		else
		{
			SceneManager.LoadSceneAsync(_levelName);
		}
	}

	public bool IsLevelLoaded()
	{
		if (_level >= 0)
		{
			return (SceneManager.GetActiveScene().buildIndex == _level);
		}

		return SceneManager.GetActiveScene().name == _levelName;
	}

	public Coroutine WaitForLevelToLoad()
	{
		return StartCoroutine(WaitForLevelToLoadEnumerator());
	}
	
	IEnumerator WaitForLevelToLoadEnumerator()
	{
		if (_isShowLoading && loading != null)
		{
			loading.Show();
		}

		if (_level >= 0)
		{
			while (SceneManager.GetActiveScene().buildIndex != _level)
			{
				yield return null;
			}
		}
		else
		{
			while (SceneManager.GetActiveScene().name != _levelName)
			{
				yield return null;
			}
		}

		if (_isShowLoading && loading != null)
		{
			loading.Hide();
		}
	}

	void TransitionScene(TransitionDelegate transitionDelegate, Action callback)
	{
		_isTransitionFinished = false;

		// Active this object
//		gameObject.SetActive(true);
		
		// Start transition
		StartCoroutine(Transition(transitionDelegate, callback));
	}

	IEnumerator Transition(TransitionDelegate transitionDelegate, Action callback)
	{
		yield return new WaitForEndOfFrame();

		// Set texture
		_material.mainTexture = transitionDelegate.GetTexture() ?? TextureHelper.GetScreenshot();

		// Disable main camera
		GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

		if (mainCamera != null)
		{
			mainCamera.GetComponent<Camera>().enabled = false;
		}

		// Disable canvas
		Canvas canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

		if (canvas != null && canvas.gameObject != null)
		{
			canvas.gameObject.SetActive(false);
		}

		// Set mesh
		_meshFilter.mesh = transitionDelegate.GetMesh() ?? _cameraMesh;
		
		// Set material
		_material.shader = transitionDelegate.GetShader() ?? GetShader("Transitions/Texture With Alpha");
		_material.color = Color.white;
		
		// Enable camera
		_camera.enabled = true;

		// Enable mesh renderer
		_meshRenderer.enabled = true;

		// Play transition
		yield return StartCoroutine(transitionDelegate.Play());

		// Disable mesh renderer
		_meshRenderer.enabled = false;

		// Disable camera
		_camera.enabled = false;

		// Clean texture
		_material.mainTexture = null;
		
		// Clean mesh
		_meshFilter.mesh = null;

		// Transition finished
		_isTransitionFinished = true;

		if (callback != null)
		{
			callback();
		}

		// Deactive this object
//		gameObject.SetActive(false);
	}

	public Coroutine TickMaterialProgress(float duration, bool reverseDirection = false)
	{
		return StartCoroutine(TickMaterialProgressEnumerator(duration, reverseDirection));
	}

	IEnumerator TickMaterialProgressEnumerator(float duration, bool reverseDirection)
	{
		float start = reverseDirection ? 1f : 0f;
		float delta = reverseDirection ? -1f : 1f;

		float elapsed = 0;

		do
		{
			elapsed += Time.deltaTime;

			if (elapsed < duration)
			{
				_material.SetFloat("_Progress", start + delta * elapsed / duration);
				yield return null;
			}
			else
			{
				_material.SetFloat("_Progress", start + delta);
				yield break;
			}
		}
		while (true);
	}
}
