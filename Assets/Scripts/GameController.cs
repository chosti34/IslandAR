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

	public GameObject m_answers;

	public Terrain m_terrain;
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
		m_pirate = Instantiate(m_germanPiratePrefab, m_grid.GetCell(randomCellIndex).transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), m_terrain.transform)
			.GetComponent<Pirate>();
		m_pirate.SetCellIndex(randomCellIndex);

		// Move answers
		m_answers.transform.position = new Vector3(
			m_pirate.transform.position.x,
			m_answers.transform.position.y,
			m_pirate.transform.position.z
		);
	}

	private void Update()
	{
	}

	public void ActivatePauseState()
	{
		m_pausePanel.SetActive(true);
		m_playPanel.SetActive(false);
		m_state = State.Play;
	}

	public void OnFirstButtonClick()
	{
		int index = m_pirate.GetCellIndex();
		GridCell upCell = m_grid.GetCell(index + 7);
		m_pirate.MoveTo(upCell.transform.position);
		m_pirate.SetCellIndex(index + 7);
		m_pirate.transform.rotation = Quaternion.Euler(0, 0, 0);

		// Move answers
		m_answers.transform.position = new Vector3(
			m_pirate.transform.position.x,
			m_answers.transform.position.y,
			m_pirate.transform.position.z
		);
	}

	public void OnSecondButtonClick()
	{
		int index = m_pirate.GetCellIndex();
		GridCell rightCell = m_grid.GetCell(index + 1);
		m_pirate.MoveTo(rightCell.transform.position);
		m_pirate.SetCellIndex(index + 1);
		m_pirate.transform.rotation = Quaternion.Euler(0, 90, 0);

		// Move answers
		m_answers.transform.position = new Vector3(
			m_pirate.transform.position.x,
			m_answers.transform.position.y,
			m_pirate.transform.position.z
		);
	}

	public void OnThirdButtonClick()
	{
		int index = m_pirate.GetCellIndex();
		GridCell downCell = m_grid.GetCell(index - 7);
		m_pirate.MoveTo(downCell.transform.position);
		m_pirate.SetCellIndex(index - 7);
		m_pirate.transform.rotation = Quaternion.Euler(0, 180, 0);

		// Move answers
		m_answers.transform.position = new Vector3(
			m_pirate.transform.position.x,
			m_answers.transform.position.y,
			m_pirate.transform.position.z
		);
	}

	public void OnFourthButtonClick()
	{
		int index = m_pirate.GetCellIndex();
		GridCell leftCell = m_grid.GetCell(index - 1);
		m_pirate.MoveTo(leftCell.transform.position);
		m_pirate.SetCellIndex(index - 1);
		m_pirate.transform.rotation = Quaternion.Euler(0, 270, 0);

		// Move answers
		m_answers.transform.position = new Vector3(
			m_pirate.transform.position.x,
			m_answers.transform.position.y,
			m_pirate.transform.position.z
		);
	}
}
