using UnityEngine;
using System.Collections.Generic;
using System.Text;

public static class ArrayHelper
{
	#region 1-D

	public static T Get<T>(this T[] a, int index)
	{
		return index < 0 ? a[0] : (index > a.Length - 1 ? a[a.Length - 1] : a[index]);
	}

	public static T Any<T>(this T[] a)
	{
		return a[Random.Range(0, a.Length)];
	}

	public static int[] RandomIndices(this int n)
	{
		int[] a = new int[n];

		for (int i = 0; i < n; i++)
		{
			a[i] = i;
		}

		a.Swap();

		return a;
	}

	public static void Swap(this int[] a)
	{
		int length = a.Length;
		int tmp;

		if (length < 2)
		{
			return;
		}

		if (length == 2)
		{
			if (Random.value > 0.5f)
			{
				tmp = a[0];
				a[0] = a[1];
				a[1] = tmp;
			}

			return;
		}

		for (int i = 0; i < length; i++)
		{
			int j = Random.Range(0, length);

			if (i != j)
			{
				tmp = a[i];
				a[i] = a[j];
				a[j] = tmp;
			}
		}
	}

	public static string ToString<T>(this T[] a)
	{
		int length = a.Length;
		
		if (length < 1)
		{
			return "[]";
		}
		
		if (length == 1)
		{
			return string.Format("[{0}]", a[0].ToString());
		}
		
		StringBuilder sb = new StringBuilder();
		
		sb.Append("[" + a[0].ToString());
		
		for (int i = 1; i < length; i++)
		{
			sb.Append(", " + a[i].ToString());
		}
		
		sb.Append("]");
		
		return sb.ToString();
	}

	#endregion // 1-D

	#region 2-D

	public static int GetRow<T>(this T[,] a)
	{
		return a.GetUpperBound(0) + 1;
	}
	
	public static int GetColumn<T>(this T[,] a)
	{
		return a.GetUpperBound(1) + 1;
	}

	public static void ShiftLeft<T>(this T[,] a, int r, int c, T last)
	{
		int column = a.GetUpperBound(1) + 1;

		for (; c < column - 1; c++)
		{
			a[r, c] = a[r, c + 1];
		}

		a[r, c] = last;
	}
	
	public static void ShiftRight<T>(this T[,] a, int r, int c, T last)
	{
		for (; c > 0; c--)
		{
			a[r, c] = a[r, c - 1];
		}
		
		a[r, c] = last;
	}
	
	public static void ShiftTop<T>(this T[,] a, int r, int c, T last)
	{
		int row = a.GetUpperBound(0) + 1;
		
		for (; r < row - 1; r++)
		{
			a[r, c] = a[r + 1, c];
		}
		
		a[r, c] = last;
	}
	
	public static void ShiftBottom<T>(this T[,] a, int r, int c, T last)
	{
		for (; r > 0; r--)
		{
			a[r, c] = a[r - 1, c];
		}
		
		a[r, c] = last;
	}

	public static string ToString<T>(this T[,] a)
	{
		int row    = a.GetUpperBound(0) + 1;
		int column = a.GetUpperBound(1) + 1;
		
		if (row == 0 || column == 0)
		{
			return "[]";
		}
		
		if (row == 1)
		{
			if (column == 1)
			{
				return string.Format("[{0}]", a[0, 0].ToString());
			}

			StringBuilder sb = new StringBuilder();
			
			sb.Append("[" + a[0, 0].ToString());
			
			for (int i = 1; i < column; i++)
			{
				sb.Append("\t" + a[0, i].ToString());
			}
			
			sb.Append("]");
			
			return sb.ToString();
		}

		if (column == 1)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < row; i++)
			{
				sb.AppendLine(string.Format("[{0}]", a[i, 0].ToString()));
			}
			
			return sb.ToString();
		}

		StringBuilder sb1 = new StringBuilder();
		
		for (int i = 0; i < row; i++)
		{
			StringBuilder sb2 = new StringBuilder();

			sb2.Append(string.Format("[{0}", a[i, 0].ToString()));

			for (int j = 1; j < column; j++)
			{
				sb2.Append(string.Format("\t{0}", a[i, j].ToString()));
			}

			sb2.Append("]");

			sb1.AppendLine(sb2.ToString());
		}
		
		return sb1.ToString();
	}

	public static void Test()
	{
		int[,] a = new int[3, 3]
		{
			{ 0, 1, 2},
			{ 3, 4, 5},
			{ 6, 7, 8}
		};
		//Log.Debug(a.ToString<int>());

		//Log.Debug("ShiftLeft(0, 0)");
		a.ShiftLeft(0, 0, 0);
		//Log.Debug(a.ToString<int>());

		//Log.Debug("ShiftRight(1, 1)");
		a.ShiftRight(1, 1, -4);
		//Log.Debug(a.ToString<int>());

		//Log.Debug("ShiftTop(0, 1)");
		a.ShiftTop(0, 1, -2);
		//Log.Debug(a.ToString<int>());

		//Log.Debug("ShiftBottom(2, 2)");
		a.ShiftBottom(2, 2, -8);
		//Log.Debug(a.ToString<int>());
	}

	#endregion // 2-D
}
