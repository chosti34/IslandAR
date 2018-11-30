using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
	private CharacterController m_cc;

	void Start()
	{
		m_cc = GetComponent<CharacterController>();
	}

	void Update()
	{
		m_cc.SimpleMove(new Vector3(0, -1, 0));
	}
}
