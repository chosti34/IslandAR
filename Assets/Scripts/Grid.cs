using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
	public static int NON_REACHABLE_CELL = -1;
	public GridCell[] m_cells;
	private int m_rows;

	void Start()
	{
		m_rows = 7;
		if (m_cells.Length != m_rows * m_rows)
		{
			throw new Exception("Game designed to have 49 cells");
		}
	}

	public GridCell GetCell(int index)
	{
		if (CellIndexIsValid(index))
		{
			return m_cells[index];
		}
		throw new IndexOutOfRangeException("cell's index must be in range from 0 to 48");
	}

	public GridCell GetCell(int row, int col)
	{
		return GetCell(row * m_rows + col);
	}

	public int GetRowsCount()
	{
		return m_rows;
	}

	public int GetUpperCellIndex(int center)
	{
		int upper = center + m_rows;
		return CellIndexIsValid(upper) ? upper : NON_REACHABLE_CELL;
	}

	public int GetRightCellIndex(int center)
	{
		int col = center % m_rows;
		return (col == (m_rows - 1)) ? NON_REACHABLE_CELL : center + 1;
	}

	public int GetDownCellIndex(int center)
	{
		int down = center - m_rows;
		return CellIndexIsValid(down) ? down : NON_REACHABLE_CELL;
	}

	public int GetLeftCellIndex(int center)
	{
		int col = center % m_rows;
		return (col == 0) ? NON_REACHABLE_CELL : center - 1;
	}

	bool CellIndexIsValid(int index)
	{
		return index >= 0 && index < m_cells.Length;
	}
}
