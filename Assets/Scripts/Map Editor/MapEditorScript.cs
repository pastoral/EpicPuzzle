using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapEditorScript : MonoBehaviour
{
	private static readonly string MapFormat = "Maps/{0}.bytes";

	// The map editor container
	private Transform mapEditorContainer;

	// The map solution container
	private Transform mapSolutionContainer;

	// The selector
	private Transform selector;

	// The file name text
	private Text fileNameText;
	
	// The map editor
	private MapEditor mapEditor;

	// The map solution
	private MapSolution mapSolution = new MapSolution();

	// The visual map solution
	private VisualMapSolution visualMapSolution;

	// True if replay solution, else replay resolve
	private bool replaySolution;

	void Start()
	{
		// Get canvas
		Canvas canvas = GameObject.FindObjectOfType<Canvas>();

		if (canvas != null)
		{
			// Get map editor container
			mapEditorContainer = canvas.FindInChildren("Map Editor");

			// Get map solution container
			mapSolutionContainer = canvas.FindInChildren("Map Solution");

			if (mapEditorContainer != null)
			{
				// Get selector
				selector = mapEditorContainer.FindInChildren("Selector");

				// Get file name
				Transform fileName = mapEditorContainer.FindInChildren("FileName");

				if (fileName != null)
				{
					Transform text = fileName.FindInChildren("Text");

					if (text != null)
					{
						fileNameText = text.GetComponent<Text>();
					}
				}
			}
		}

		// Get map editor
		mapEditor = GameObject.FindObjectOfType<MapEditor>();

		if (mapEditor != null)
		{
			mapEditor.SetItemType(ItemType.Foothold);
		}

		// Get visual map solution
		visualMapSolution = GameObject.FindObjectOfType<VisualMapSolution>();
	}

	public void SelectFoothold(GameObject sender)
	{
		SelectItem(sender, ItemType.Foothold);
	}
	
	public void SelectFootholdDouble(GameObject sender)
	{
		SelectItem(sender, ItemType.FootholdDouble);
	}
	
	public void SelectFootholdTime(GameObject sender)
	{
		SelectItem(sender, ItemType.FootholdTime);
	}

	public void SelectFootholdRedirectLeft(GameObject sender)
	{
		SelectItem(sender, ItemType.FootholdRedirectLeft);
	}
	
	public void SelectFootholdRedirectUp(GameObject sender)
	{
		SelectItem(sender, ItemType.FootholdRedirectUp);
	}
	
	public void SelectFootholdRedirectRight(GameObject sender)
	{
		SelectItem(sender, ItemType.FootholdRedirectRight);
	}
	
	public void SelectFootholdRedirectDown(GameObject sender)
	{
		SelectItem(sender, ItemType.FootholdRedirectDown);
	}

	public void SelectFrogLeft(GameObject sender)
	{
		SelectItem(sender, ItemType.FrogLeft);
	}
	
	public void SelectFrogUp(GameObject sender)
	{
		SelectItem(sender, ItemType.FrogUp);
	}
	
	public void SelectFrogRight(GameObject sender)
	{
		SelectItem(sender, ItemType.FrogRight);
	}
	
	public void SelectFrogDown(GameObject sender)
	{
		SelectItem(sender, ItemType.FrogDown);
	}

	void SelectItem(GameObject sender, ItemType itemType)
	{
		if (selector != null)
		{
			selector.position = sender.transform.position;
		}

		if (mapEditor != null)
		{
			mapEditor.SetItemType(itemType);
		}
	}

	public void ShiftLeft()
	{
		if (mapEditor != null)
		{
			mapEditor.ShiftLeft();
		}
	}
	
	public void ShiftRight()
	{
		if (mapEditor != null)
		{
			mapEditor.ShiftRight();
		}
	}

	public void ShiftUp()
	{
		if (mapEditor != null)
		{
			mapEditor.ShiftUp();
		}
	}
	
	public void ShiftDown()
	{
		if (mapEditor != null)
		{
			mapEditor.ShiftDown();
		}
	}
	
	public void Clear()
	{
		if (mapEditor != null)
		{
			mapEditor.Clear();
		}
	}

	public void Load()
	{
		if (mapEditor != null)
		{
			if (fileNameText != null && !string.IsNullOrEmpty(fileNameText.text))
			{
				mapEditor.Load(string.Format(MapFormat, fileNameText.text.Trim()));
			}
			else
			{
				Debug.Log("File name required!");
			}
		}
	}

	public void Save()
	{
		if (mapEditor != null)
		{
			if (fileNameText != null && !string.IsNullOrEmpty(fileNameText.text))
			{
				mapEditor.Save(string.Format(MapFormat, fileNameText.text.Trim()));
			}
			else
			{
				Debug.Log("File name required!");
			}
		}
	}
	
	public void Count()
	{
		// Get map data
		MapData mapData = GetMapData();
		
		if (mapData != null)
		{
			Debug.Log("Count: " + mapSolution.Resolve(mapData));
		}
	}

	public void Solution()
	{
		// Get map data
		MapData mapData = GetMapData();
		
		if (mapData == null)
		{
			return;
		}

		if (mapSolution.Resolve(mapData) == 0)
		{
			Debug.Log("No solution!");
			return;
		}

		// Show map solution
		SetShowMapSolution(true);
		
		if (visualMapSolution != null)
		{
			replaySolution = true;

			visualMapSolution.Solution(mapData);
		}
	}

	public void Resolve()
	{
		// Get map data
		MapData mapData = GetMapData();

		if (mapData == null)
		{
			return;
		}

		// Show map solution
		SetShowMapSolution(true);

		if (visualMapSolution != null)
		{
			replaySolution = false;

			visualMapSolution.Resolve(mapData);
		}
	}

	public void Quit()
	{
		SceneManager.LoadScene("Menu");
	}

	public void Back()
	{
		// Hide map solution
		SetShowMapSolution(false);
	}

	public void Replay()
	{
		if (visualMapSolution != null)
		{
			if (replaySolution)
			{
				visualMapSolution.ReplaySolution();
			}
			else
			{
				visualMapSolution.ReplayResolve();
			}
		}
	}

	MapData GetMapData()
	{
		if (mapEditor != null)
		{
			// Get map data
			MapData mapData = mapEditor.GetMapData();
			
			if (mapData == null)
			{
				Debug.Log("Map invalid!");
			}

			return mapData;
		}

		return null;
	}

	void SetShowMapSolution(bool isShow)
	{
		if (mapEditorContainer != null)
		{
			mapEditorContainer.gameObject.SetActive(!isShow);
		}
		
		if (mapEditor != null)
		{
			mapEditor.gameObject.SetActive(!isShow);
		}
		
		if (mapSolutionContainer != null)
		{
			mapSolutionContainer.gameObject.SetActive(isShow);
		}
		
		if (visualMapSolution != null)
		{
			visualMapSolution.gameObject.SetActive(isShow);
		}
	}
}
