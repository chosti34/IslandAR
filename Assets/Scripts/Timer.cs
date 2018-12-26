using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	bool m_paused;
	float m_seconds;
	Text m_text;

	public void Reset()
	{
		m_paused = true;
		m_seconds = 45.0f;
		UpdateTimerColor();
	}

	public void SetPaused(bool paused)
	{
		m_paused = paused;
	}

	public float GetTime()
	{
		return m_seconds;
	}

	void Start()
	{
		m_text = GetComponent<Text>();
		Reset();
	}

	void Update()
	{
		Debug.Log(m_seconds);
		if (m_paused)
		{
			return;
		}

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
