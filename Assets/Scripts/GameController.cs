using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	enum State
	{
		Pause,
		Play
	}

	State m_state;

	public GameObject m_playPanel;
	public GameObject m_pausePanel;

	void Start()
	{
		ActivatePauseState();
	}

	public void ActivatePlayState()
	{
		m_pausePanel.SetActive(false);
		m_playPanel.SetActive(true);
		m_state = State.Play;
	}

	public void ActivatePauseState()
	{
		m_pausePanel.SetActive(true);
		m_playPanel.SetActive(false);
		m_state = State.Play;
	}
}
