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

	public Grid m_grid;

	public GameObject m_playPanel;
	public GameObject m_pausePanel;

	public GameObject m_chestPrefab;
	public GameObject m_germanPiratePrefab;

	private Pirate m_pirate;

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
			Vector3 rotation = new Vector3(0.0f, Random.Range(-360.0f, 360.0f), 0.0f);
			Chest chest = Instantiate(m_chestPrefab, m_grid.GetCell(index).transform.position, Quaternion.Euler(rotation))
				.GetComponent<Chest>();
			chest.SetCellIndex(index);
		}

		// Spawn player
		int randomCellIndex = Random.Range(0, 7);
		m_pirate = Instantiate(m_germanPiratePrefab, m_grid.GetCell(randomCellIndex).transform.position, Quaternion.Euler(new Vector3(0, 0, 0)))
			.GetComponent<Pirate>();
		m_pirate.SetCellIndex(randomCellIndex);
	}

	public void ActivatePauseState()
	{
		m_pausePanel.SetActive(true);
		m_playPanel.SetActive(false);
		m_state = State.Play;
	}
}
