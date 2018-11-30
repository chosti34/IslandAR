using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
	private CharacterController m_cc;
	private int m_cellIndex;

	void Start()
	{
		m_cc = GetComponent<CharacterController>();
	}

	void Update()
	{
		m_cc.SimpleMove(new Vector3(0, -1, 0));
	}

	public void SetCellIndex(int index)
	{
		m_cellIndex = index;
	}

	public int GetCellIndex()
	{
		return m_cellIndex;
	}
}
