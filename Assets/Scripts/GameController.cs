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

	public GridCell[] m_grid;

	public GameObject m_playPanel;
	public GameObject m_pausePanel;

	public GameObject m_chestPrefab;

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

	public void StartGame()
	{
		ActivatePlayState();

		// Spawn chests
		HashSet<int> indices = new HashSet<int>();
		while (indices.Count != 5)
		{
			int index = Random.Range(7, 42);
			indices.Add(index);
		}

		foreach (int index in indices)
		{
			Vector3 position = new Vector3(
				m_grid[index].transform.position.x,
				m_grid[index].transform.position.y,
				m_grid[index].transform.position.z
			);
			Vector3 rotation = new Vector3(
				0.0f,
				Random.Range(-360.0f, 360.0f),
				0.0f
			);
			GameObject chest = Instantiate(m_chestPrefab, position, Quaternion.Euler(rotation));
			m_grid[index].SetGameObject(chest);
		}
	}

	public void ActivatePauseState()
	{
		m_pausePanel.SetActive(true);
		m_playPanel.SetActive(false);
		m_state = State.Play;
	}

	void Update()
	{
		int random = Random.Range(0, 49);
		//m_grid[random].SetColor(new Color(1, 0, 0, 0.3f));
	}
}
