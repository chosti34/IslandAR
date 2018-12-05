using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class QuestionAndAnswer
{
	public QuestionAndAnswer(string question, string answer)
	{
		this.question = question;
		this.answer = answer;
	}

	public string question;
	public string answer;
}

public static class IListExtensions
{
	public static void Shuffle<T>(this IList<T> list)
	{
		var count = list.Count;
		var last = count - 1;
		for (var i = 0; i < last; ++i)
		{
			var r = UnityEngine.Random.Range(i, count);
			var tmp = list[i];
			list[i] = list[r];
			list[r] = tmp;
		}
	}
}

public class GameController : MonoBehaviour
{
	enum State
	{
		Pause,
		Play
	}

	enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	State m_state;

	public Grid m_grid;

	public GameObject m_playPanel;
	public GameObject m_pausePanel;
	public GameObject m_questionsPanel;

	public GameObject m_chestPrefab;
	public GameObject m_germanPiratePrefab;

	public GameObject m_answers;
	public Button[] m_buttons;

	public Terrain m_terrain;

	private Pirate m_pirate;
	private QuestionAndAnswer[] m_questionsAndAnswers;
	private Dictionary<int, Direction> m_directionMap;

	void Start()
	{
		if (m_buttons.Length != 4)
		{
			throw new System.Exception("GameController must have 4 buttons!");
		}

		m_questionsAndAnswers = new QuestionAndAnswer[10]
		{
			new QuestionAndAnswer("1 + 3 = ?", "4"),
			new QuestionAndAnswer("1 - 3 = ?", "-2"),
			new QuestionAndAnswer("10 + 1 = ?", "11"),
			new QuestionAndAnswer("3 * 8 = ?", "24"),
			new QuestionAndAnswer("8 / 4 = ?", "2"),
			new QuestionAndAnswer("98 - 27 = ?", "71"),
			new QuestionAndAnswer("1 + 4 = ?", "5"),
			new QuestionAndAnswer("10 - 11 = ?", "-1"),
			new QuestionAndAnswer("10 * 3 = ?", "30"),
			new QuestionAndAnswer("7 + 7 = ?", "14")
		};
		m_directionMap = new Dictionary<int, Direction>();

		m_answers.SetActive(false);
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
			int index = UnityEngine.Random.Range(7, 42);
			indices.Add(index);
		}

		foreach (int index in indices)
		{
			Vector3 rotation = new Vector3(0.0f, UnityEngine.Random.Range(-360.0f, 360.0f), 0.0f);
			Chest chest = Instantiate(m_chestPrefab, m_grid.GetCell(index).transform.position, Quaternion.Euler(rotation))
				.GetComponent<Chest>();
			chest.SetCellIndex(index);
		}

		// Spawn player
		int randomCellIndex = UnityEngine.Random.Range(0, 7);
		m_pirate = Instantiate(m_germanPiratePrefab, m_grid.GetCell(randomCellIndex).transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), m_terrain.transform)
			.GetComponent<Pirate>();
		m_pirate.SetCellIndex(randomCellIndex);

		m_pirate.SetCallback(() => {
			m_questionsPanel.SetActive(true);
			m_answers.SetActive(true);
			MoveAnswersToPirate();
			UpdateAnswersAndQuestionsAvailability(); 
		});

		m_answers.SetActive(true);
		MoveAnswersToPirate();
		UpdateAnswersAndQuestionsAvailability();
	}

	public void ActivatePauseState()
	{
		m_pausePanel.SetActive(true);
		m_playPanel.SetActive(false);
		m_state = State.Play;
	}

	public void OnFirstButtonClick()
	{
		Debug.Log(m_directionMap[0]);
		MovePirate(m_directionMap[0]);
	}

	public void OnSecondButtonClick()
	{
		Debug.Log(m_directionMap[1]);
		MovePirate(m_directionMap[1]);
	}

	public void OnThirdButtonClick()
	{
		Debug.Log(m_directionMap[2]);
		MovePirate(m_directionMap[2]);
	}

	public void OnFourthButtonClick()
	{
		Debug.Log(m_directionMap[3]);
		MovePirate(m_directionMap[3]);
	}

	void MovePirate(Direction direction)
	{
		int index = Grid.NON_REACHABLE_CELL;
		Quaternion rotation = Quaternion.identity;

		switch (direction)
		{
			case Direction.Up:
				index = m_grid.GetUpperCellIndex(m_pirate.GetCellIndex());
				rotation = Quaternion.Euler(new Vector3(0, 0, 0));
				break;
			case Direction.Right:
				index = m_grid.GetRightCellIndex(m_pirate.GetCellIndex());
				rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				break;
			case Direction.Down:
				index = m_grid.GetDownCellIndex(m_pirate.GetCellIndex());
				rotation = Quaternion.Euler(new Vector3(0, 180, 0));
				break;
			case Direction.Left:
				index = m_grid.GetLeftCellIndex(m_pirate.GetCellIndex());
				rotation = Quaternion.Euler(new Vector3(0, 270, 0));
				break;
		}

		if (index != Grid.NON_REACHABLE_CELL)
		{
			GridCell cell = m_grid.GetCell(index);
			m_pirate.MoveTo(cell.transform.position);
			m_pirate.SetCellIndex(index);
			m_pirate.transform.rotation = rotation;
			MoveAnswersToPirate();
			m_answers.SetActive(false);
			m_questionsPanel.SetActive(false);
			//UpdateAnswersAndQuestionsAvailability();
		}
	}

	void MoveAnswersToPirate()
	{
		m_answers.transform.position = new Vector3(
			m_pirate.transform.position.x,
			m_answers.transform.position.y,
			m_pirate.transform.position.z
		);
	}

	public void UpdateAnswersAndQuestionsAvailability()
	{
		List<TextMesh> answersText = new List<TextMesh>();
		for (int i = 0; i < m_answers.transform.childCount; ++i)
		{
			answersText.Add(m_answers.transform.GetChild(i).gameObject.GetComponentInChildren<TextMesh>());
		}

		List<int> directionIndices = Enumerable.Range(0, 4).ToList();
		List<int> questionsAndAnswersIndices = Enumerable.Range(0, 10).ToList();

		for (int i = 0; i < m_buttons.Length; ++i)
		{
			int rndIdx = UnityEngine.Random.Range(0, directionIndices.Count);
			int rnd = directionIndices[rndIdx];
			directionIndices.RemoveAt(rndIdx);
			m_directionMap[i] = ToDirection(rnd);

			rndIdx = UnityEngine.Random.Range(0, questionsAndAnswersIndices.Count);
			rnd = questionsAndAnswersIndices[rndIdx];
			questionsAndAnswersIndices.RemoveAt(rndIdx);
			QuestionAndAnswer questionAndAnswer = m_questionsAndAnswers[rnd];

			m_buttons[i].GetComponentInChildren<Text>().text = questionAndAnswer.question;
			answersText[ToDirectionIndex(m_directionMap[i])].text = questionAndAnswer.answer;
		}

		int upper = m_grid.GetUpperCellIndex(m_pirate.GetCellIndex());
		int right = m_grid.GetRightCellIndex(m_pirate.GetCellIndex());
		int down = m_grid.GetDownCellIndex(m_pirate.GetCellIndex());
		int left = m_grid.GetLeftCellIndex(m_pirate.GetCellIndex());

		m_buttons[GetButtonIndexAssociatedWithDirection(Direction.Up)].interactable = upper != Grid.NON_REACHABLE_CELL;
		m_buttons[GetButtonIndexAssociatedWithDirection(Direction.Right)].interactable = right != Grid.NON_REACHABLE_CELL;
		m_buttons[GetButtonIndexAssociatedWithDirection(Direction.Down)].interactable = down != Grid.NON_REACHABLE_CELL;
		m_buttons[GetButtonIndexAssociatedWithDirection(Direction.Left)].interactable = left != Grid.NON_REACHABLE_CELL;

		for (int i = 0; i < m_answers.transform.childCount; ++i)
		{
			m_answers.transform.GetChild(i).gameObject.SetActive(
				m_buttons[GetButtonIndexAssociatedWithDirection(ToDirection(i))].interactable);
		}
	}

	int GetButtonIndexAssociatedWithDirection(Direction direction)
	{
		foreach (KeyValuePair<int, Direction> pair in m_directionMap)
		{
			if (pair.Value.Equals(direction))
			{
				return pair.Key;
			}
		}
		throw new System.Exception("no button associated with passed direction");
	}

	Direction ToDirection(int direction)
	{
		switch (direction)
		{
		case 0:
			return Direction.Up;
		case 1:
			return Direction.Right;
		case 2:
			return Direction.Down;
		case 3:
			return Direction.Left;
		}
		throw new System.Exception("GetRandomDirection(): invalid direction's integer representation");
	}

	int ToDirectionIndex(Direction direction)
	{
		switch (direction)
		{
		case Direction.Up:
			return 0;
		case Direction.Right:
			return 1;
		case Direction.Down:
			return 2;
		case Direction.Left:
			return 3;
		}
		throw new System.Exception("GetRandomDirection(): invalid direction representation");
	}
}
