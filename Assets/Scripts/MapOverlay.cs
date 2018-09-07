using UnityEngine;
using System.Collections;

/*
 * 		+-----+-----+-----+
 * 		|     |	    |	  |
 * 		|  6  |	 7  |  8  |
 * 		+-----+-----+-----+
 * 		|     |	    |	  |
 * y	|  3  |	 4  |  5  |
 * ^	+-----+-----+-----+
 * |	|     |	    |	  |
 * |	|  0  |	 1  |  2  |
 * |	+-----+-----+-----+
 * |
 * +--------> x
 */
public class MapOverlay : MonoBehaviour
{
	/// <summary>
	/// The sprite.
	/// </summary>
	public Sprite sprite;

	/// <summary>
	/// The normal color.
	/// </summary>
	public Color normalColor = Color.white;

	/// <summary>
	/// The selected color.
	/// </summary>
	public Color selectedColor = Color.green;

	/// <summary>
	/// The sorting order.
	/// </summary>
	public int sortingOrder;

	// The grid width
	private int _gridWidth;
	
	// The grid height
	private int _gridHeight;

	// The array of cells
	private SpriteRenderer[,] _cells;

	// The selected cell
	private int _selectedRow    = -1;
	private int _selectedColumn = -1;

	// The alpha helper
	private LerpFloatHelper _alphaHelper = new LerpFloatHelper(0.0f);

	public void Construct(int gridWidth, int gridHeight, float cellSize)
	{
		// Set grid width
		_gridWidth = gridWidth;
		
		// Set grid height
		_gridHeight = gridHeight;

		// Create cells
		_cells = new SpriteRenderer[gridHeight, gridWidth];

		float width  = gridWidth  * cellSize;
		float height = gridHeight * cellSize;

		float left   = -width  * 0.5f;
		float bottom = -height * 0.5f;

		Vector3 position = new Vector3(left + cellSize * 0.5f, bottom + cellSize * 0.5f, 0);
		Vector3 scale = Vector3.one;

		Color color = normalColor;
		color.a = _alphaHelper.Get();

		for (int i = 0; i < gridHeight; i++)
		{
			for (int j = 0; j < gridWidth; j++)
			{
				GameObject cell = new GameObject();
				cell.transform.SetParent(transform);
				cell.transform.localPosition = position;
				cell.transform.localScale = scale;

				SpriteRenderer spriteRenderer = cell.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = sprite;
				spriteRenderer.color = color;
				spriteRenderer.sortingOrder = sortingOrder;

				cell.Hide();

				_cells[i, j] = spriteRenderer;

				position.x += cellSize;
			}

			position.x = left + cellSize * 0.5f;
			position.y += cellSize;
		}
	}

	public void SetShowCell(int row, int column, bool isShow)
	{
		_cells[row, column].gameObject.SetActive(isShow);
	}

	public void SetSelectedCell(int row, int column)
	{
		if (_selectedRow == row && _selectedColumn == column)
		{
			return;
		}

		// Unselect old cell
		if (IsSelectedCellShowing())
		{
			Color color = normalColor;
			color.a = _alphaHelper.Get();
			
			_cells[_selectedRow, _selectedColumn].color = color;
		}

		// Set selected cell
		_selectedRow    = row;
		_selectedColumn = column;

		// Select the specified cell
		if (IsSelectedCellShowing())
		{
			Color color = selectedColor;
			color.a = _alphaHelper.Get();

			_cells[_selectedRow, _selectedColumn].color = color;
		}
	}

	public void Show()
	{
		bool isFinished = _alphaHelper.IsFinished();

		_alphaHelper.Construct(_alphaHelper.Get(), normalColor.a, 0.1f);

		if (isFinished)
		{
			StartCoroutine(UpdateAlpha());
		}
	}
	
	public void Hide()
	{
		bool isFinished = _alphaHelper.IsFinished();
		
		_alphaHelper.Construct(_alphaHelper.Get(), 0.0f, 0.1f);
		
		if (isFinished)
		{
			StartCoroutine(UpdateAlpha());
		}
	}

	bool IsSelectedCellShowing()
	{
		return (_selectedRow >= 0 && _selectedColumn >= 0 && _cells[_selectedRow, _selectedColumn].IsShowing());
	}

	IEnumerator UpdateAlpha()
	{
		Color color = normalColor;

		while (!_alphaHelper.IsFinished())
		{
			color.a = _alphaHelper.Update(Time.deltaTime);

			for (int i = 0; i < _gridHeight; i++)
			{
				for (int j = 0; j < _gridWidth; j++)
				{
					if (_cells[i, j].IsShowing())
					{
						_cells[i, j].color = color;
					}
				}
			}

			if (IsSelectedCellShowing())
			{
				_cells[_selectedRow, _selectedColumn].color = selectedColor.Copy(color.a);
			}

			yield return null;
		}
	}
}
