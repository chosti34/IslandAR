using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
	// Список клеток, достижимых из данной
	public GridCell[] m_neighbours;

	// Объект, находящийся в данной клетке
	public GameObject m_object;
}
