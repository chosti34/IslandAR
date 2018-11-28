using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	Text m_text;
	float m_seconds;

	void Start()
	{
		m_text = GetComponent<Text>();
		m_seconds = 45.0f;
	}

	void Update()
	{
		if (m_seconds > 0.0f)
		{
			m_seconds -= Time.deltaTime;
		}
		else
		{
			m_seconds = 0.0f;
		}
		m_text.text = string.Format("{0:D2}:{1:D2}", (int)(m_seconds / 60), (int)(m_seconds % 60));
		UpdateTimerColor();
	}

	void UpdateTimerColor()
	{
		if (m_seconds >= 30.0f)
		{
			m_text.color = Color.white;
		}
		else if (m_seconds >= 10.0f)
		{
			m_text.color = Color.yellow;
		}
		else
		{
			m_text.color = Color.red;
		}
	}
}
