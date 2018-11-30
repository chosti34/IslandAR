using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
	CharacterController m_ctrl;

	void Start()
	{
		m_ctrl = GetComponent<CharacterController>();
	}

	void Update()
	{
		//m_ctrl.Move(new Vector3(0, -1, 0));
	}
}
