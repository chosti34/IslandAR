using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
	public GridCell[] m_cells;

	public GridCell GetCell(int index)
	{
		if (index >= 0 && index <= 48)
		{
			return m_cells[index];
		}
		throw new IndexOutOfRangeException("cell's index must be in range from 0 to 48");
	}

	public GridCell GetCell(int row, int col)
	{
		return GetCell(row * 7 + col);
	}
}
