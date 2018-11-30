using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
	// Объект, находящийся в данной клетке
	public GameObject m_object;

	public void SetColor(Color color)
	{
		GetComponent<MeshRenderer>().material.color = color;
	}

	public void SetGameObject(GameObject obj)
	{
		m_object = obj;
	}

	public GameObject GetGameObject()
	{
		return m_object;
	}
}
