using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.UI;

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

public class Pirate : NetworkBehaviour
{
	enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	private QuestionAndAnswer[] m_questionsAndAnswers;
	private Dictionary<int, Direction> m_directionMap;

	private NetworkAnimator m_animator;

	[SyncVar]
	public int m_cellIndex;

	[SerializeField]
	private float m_speed;

	[SyncVar]
	public bool m_run = false;

	[SyncVar]
	public int m_nScore;

	public GameObject m_answers;
	public Quaternion m_answersRotationIdentity;

	float gravity = 15;

	[SyncVar]
	float m_seconds = 45;

	CharacterController m_characterController;

	[SyncVar]
	Vector3 moveDirection = Vector3.zero;

	[SyncVar]
	Vector3 m_destination;

	[SyncVar]
	bool m_moving;

	// Player user interface
	private Canvas m_canvas;
	private GameObject m_questionsPanel;
	private Button[] m_buttons;

	public AudioClip m_clip;
	public AudioClip m_winningSound;

	private void Awake()
	{
		m_answersRotationIdentity = Quaternion.identity;
	}

	void Start()
	{
		m_buttons = new Button[4];

		// Managing user interface for player
		m_answers.SetActive(isLocalPlayer);
		m_canvas = gameObject.GetComponentInChildren<Canvas>();
		m_canvas.gameObject.SetActive(isLocalPlayer);

		m_questionsPanel = GameObject.Find("QuestionsPanel");
		m_buttons[0] = GameObject.Find("Button").GetComponent<Button>();
		m_buttons[1] = GameObject.Find("Button (1)").GetComponent<Button>();
		m_buttons[2] = GameObject.Find("Button (2)").GetComponent<Button>();
		m_buttons[3] = GameObject.Find("Button (3)").GetComponent<Button>();

		if (isLocalPlayer)
		{
			m_buttons[0].onClick.RemoveAllListeners();
			m_buttons[1].onClick.RemoveAllListeners();
			m_buttons[2].onClick.RemoveAllListeners();
			m_buttons[3].onClick.RemoveAllListeners();

			m_buttons[0].onClick.AddListener(OnFirstButtonClick);
			m_buttons[1].onClick.AddListener(OnSecondButtonClick);
			m_buttons[2].onClick.AddListener(OnThirdButtonClick);
			m_buttons[3].onClick.AddListener(OnFourthButtonClick);
		}

		m_characterController = transform.GetComponent<CharacterController>();
		m_animator = GetComponent<NetworkAnimator>();
		m_moving = false;

		m_questionsAndAnswers = new QuestionAndAnswer[30]
		{
			new QuestionAndAnswer("18 - 17 = ?", "1"),
			new QuestionAndAnswer("10 - 8 = ?", "2"),
			new QuestionAndAnswer("13 - 10 = ?", "3"),
			new QuestionAndAnswer("12 - 8 = ?", "4"),
			new QuestionAndAnswer("16 - 11 = ?", "5"),
			new QuestionAndAnswer("2 + 4 = ?", "6"),
			new QuestionAndAnswer("10 - 3 = ?", "7"),
			new QuestionAndAnswer("3 + 4 = ?", "8"),
			new QuestionAndAnswer("14 - 5 = ?", "9"),
			new QuestionAndAnswer("20 - 10 = ?", "10"),
			new QuestionAndAnswer("23 - 12 = ?", "11"),
			new QuestionAndAnswer("25 - 13 = ?", "12"),
			new QuestionAndAnswer("30 - 17 = ?", "13"),
			new QuestionAndAnswer("18 - 4 = ?", "14"),
			new QuestionAndAnswer("7 + 8 = ?", "15"),
			new QuestionAndAnswer("8 + 8 = ?", "16"),
			new QuestionAndAnswer("10 + 7 = ?", "17"),
			new QuestionAndAnswer("6 + 12 = ?", "18"),
			new QuestionAndAnswer("9 + 10 = ?", "19"),
			new QuestionAndAnswer("17 + 3 = ?", "20"),
			new QuestionAndAnswer("1 + 20 = ?", "21"),
			new QuestionAndAnswer("18 + 4 = ?", "22"),
			new QuestionAndAnswer("11 + 12 = ?", "23"),
			new QuestionAndAnswer("10 + 14 = ?", "24"),
			new QuestionAndAnswer("15 + 10 = ?", "25"),
			new QuestionAndAnswer("30 - 4 = ?", "26"),
			new QuestionAndAnswer("18 + 9 = ?", "27"),
			new QuestionAndAnswer("30 - 2 = ?", "28"),
			new QuestionAndAnswer("10 + 19 = ?", "29"),
			new QuestionAndAnswer("14 + 16 = ?", "30")
		};
		m_directionMap = new Dictionary<int, Direction>();

		if (isLocalPlayer)
		{
			UpdateAnswersAndQuestionsAvailability();
		}

		m_animator.SetParameterAutoSend(0, true);
	}

	[ClientCallback]
	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		if (isServer && GameController.Instance.m_players.Count == 2)
		{
			m_seconds -= Time.deltaTime;
			RpcUpdateTime();
		}
		else if (!isServer)
		{
			m_seconds = GameController.Instance.m_timer.GetTime();
			CmdUpdateTime();
		}

		//APPLY GRAVITY
		if (moveDirection.y > gravity * -1)
		{
			moveDirection.y -= gravity * Time.deltaTime;
		}

		m_characterController.Move(moveDirection * Time.deltaTime);
		//var left = transform.TransformDirection(Vector3.left);

		if (m_characterController.isGrounded)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				moveDirection.y = m_speed;
				//m_animator.SetTrigger("RunTrigger");
			}
			else if (Input.GetKey("w"))
			{
				CmdChangeRotationAndMove(0);
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_characterController.SimpleMove(transform.forward * m_speed * 2);
				}
				else
				{
					m_characterController.SimpleMove(transform.forward * m_speed);
				}
				transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
			}
			else if (Input.GetKey("s"))
			{
				CmdChangeRotationAndMove(180);
				transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_characterController.SimpleMove(transform.forward * m_speed * 2);
				}
				else
				{
					m_characterController.SimpleMove(transform.forward * m_speed);
				}
				//m_animator.SetTrigger("RunTrigger");
			}
			else if (Input.GetKey("a"))
			{
				CmdChangeRotationAndMove(270);
				transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_characterController.SimpleMove(transform.forward * m_speed * 2);
				}
				else
				{
					m_characterController.SimpleMove(transform.forward * m_speed);
				}
				//m_animator.SetTrigger("RunTrigger");
			}
			else if (Input.GetKey("d"))
			{
				CmdChangeRotationAndMove(90);
				transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				//m_animation.Play("Walk");
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_characterController.SimpleMove(transform.forward * m_speed * 2);
				}
				else
				{
					m_characterController.SimpleMove(transform.forward * m_speed);
				}
			//m_animator.SetTrigger("RunTrigger");
			}
		}
		else
		{
			if (Input.GetKey("w"))
			{
				Vector3 relative = new Vector3();
				relative = transform.TransformDirection(0, 0, 1);
				if (Input.GetKey(KeyCode.LeftShift))
				{
					m_characterController.Move(relative * Time.deltaTime * m_speed * 2);
				}
				else
				{
					m_characterController.Move(relative * Time.deltaTime * m_speed);
				}
			}
		}

		if (m_moving)
		{
			transform.position = Vector3.Lerp(transform.position, m_destination, m_speed * Time.deltaTime);
			if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_destination.x, m_destination.z)) <= 0.6f)
			{
				if (isServer)
				{
					Debug.Log("host stop anim");
					// if (m_moving) CmdSetAnimationTrigger();
					StartCoroutine(WaitSetAnimationTrigger());
				}
				else
				{
					Debug.Log("client stop anim");
					if (m_moving) CmdSetAnimationTrigger();
				}

				m_moving = false;
				m_destination = Vector3.zero;

				m_questionsPanel.SetActive(true);
				m_answers.SetActive(true);

				MoveAnswersToPirate();
				UpdateAnswersAndQuestionsAvailability();
			}
		}
	}

	void OnFirstButtonClick()
	{
		MovePirate(m_directionMap[0]);
	}

	void OnSecondButtonClick()
	{
		MovePirate(m_directionMap[1]);
	}

	void OnThirdButtonClick()
	{
		MovePirate(m_directionMap[2]);
	}

	void OnFourthButtonClick()
	{
		MovePirate(m_directionMap[3]);
	}

	[ClientCallback]
	private void LateUpdate()
	{
		m_answers.transform.rotation = m_answersRotationIdentity;
	}

	[Command]
	public void CmdChangeRotationAndMove(float rotation)
	{
		transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
		m_characterController.SimpleMove(transform.forward * m_speed);
	}

	[ClientCallback]
	public void CmdMoveTo(Vector3 pos)
	{
		if (!isLocalPlayer) return;

		// Можно просто мнгновенно менять позицию персонажа:
		//  transform.position = new Vector3(pos.x, transform.position.y, pos.x);

		if (isServer)
		{
			Debug.Log("host start anim");
			if (!m_moving) CmdSetAnimationTrigger();
		}
		else
		{
			Debug.Log("client start anim");
			if (!m_moving) CmdSetAnimationTrigger();
		}

		// m_animator.SetTrigger("Trigger");
		m_destination = new Vector3(pos.x, transform.position.y, pos.z);
		m_moving = true;
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
	}

	[ClientCallback]
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Chest")
		{
			GetComponent<AudioSource>().PlayOneShot(m_clip);
			Destroy(other.gameObject);

			m_nScore += 1;
			GameController island = GameController.Instance;

			if (isServer && island.m_players.Count == 2)
			{
				RpcUpdateScoreUI(
					"Player #1: " + island.m_players[0].m_nScore,
					"Player #2: " + island.m_players[1].m_nScore
				);
			}
			else if (island.m_players.Count == 2)
			{
				CmdUpdateScoreUI(
					"Player #1: " + island.m_players[0].m_nScore,
					"Player #2: " + island.m_players[1].m_nScore
				);
			}
		}
	}

	[ClientRpc]
	void RpcUpdateScoreUI(string hostScoreText, string clientScoreText)
	{
		GameController.Instance.m_hostScoreText.text = hostScoreText;
		GameController.Instance.m_clientScoreText.text = clientScoreText;
	}

	[Command]
	void CmdUpdateScoreUI(string hostScoreText, string clientScoreText)
	{
		GameController.Instance.m_hostScoreText.text = hostScoreText;
		GameController.Instance.m_clientScoreText.text = clientScoreText;
	}

	[Command]
	public void CmdUpdateTime()
	{
		GameController.Instance.m_timer.SetTime(m_seconds);
	}

	[ClientRpc]
	public void RpcUpdateTime()
	{
		GameController.Instance.m_timer.SetTime(m_seconds);
	}

	[ClientRpc]
	public void RpcShowScoreAndTimePanels()
	{
		GameController.Instance.m_scorePanel.SetActive(true);
		GameController.Instance.m_timePanel.SetActive(true);
		GameController.Instance.m_timer.SetPaused(false);
	}

	[Command]
	public void CmdShowScoreAndTimePanels()
	{
		GameController.Instance.m_scorePanel.SetActive(true);
		GameController.Instance.m_timePanel.SetActive(true);
		GameController.Instance.m_timer.SetPaused(false);
	}

	[ClientRpc]
	public void RpcShowGameResultsText(string text)
	{
		GameController.Instance.m_gameResultsPanel.GetComponentInChildren<Text>().text = text;
		GameController.Instance.m_gameResultsPanel.SetActive(true);
	}

	[Command]
	public void CmdShowGameResultsText(string text)
	{
		GameController.Instance.m_gameResultsPanel.GetComponentInChildren<Text>().text = text;
		GameController.Instance.m_gameResultsPanel.SetActive(true);
	}

	[ClientRpc]
	public void RpcSetAnimationTrigger()
	{
		m_animator.SetTrigger("Trigger");
	}

	[Client]
	public void CmdSetAnimationTrigger()
	{
		m_animator.SetTrigger("Trigger");
	}

	[ClientRpc]
	public void RpcSetCellIndex(int index)
	{
		m_cellIndex = index;
	}

	[Command]
	public void CmdSetCellIndex(int index)
	{
		m_cellIndex = index;
	}

	[ClientRpc]
	public void RpcUpdateAnswersAvailability()
	{
		UpdateAnswersAndQuestionsAvailability();
	}

	[Command]
	public void CmdUpdateAnswersAvailability()
	{
		UpdateAnswersAndQuestionsAvailability();
	}

	[ClientRpc]
	public void RpcPlayWinningSound()
	{
		GetComponent<AudioSource>().Play();
	}

	[Command]
	public void CmdPlayWinningSound()
	{
		GetComponent<AudioSource>().Play();
	}

	public IEnumerator WaitSetAnimationTrigger() {
		yield return new WaitForSeconds(1.5f);
		Debug.Log("Stop wait animation");
		m_animator.SetTrigger("Trigger");
	}

	void MovePirate(Direction direction)
	{
		if (!isLocalPlayer) return;

		int index = Grid.NON_REACHABLE_CELL;
		Quaternion rotation = Quaternion.identity;

		switch (direction)
		{
			case Direction.Up:
				index = GameController.Instance.m_grid.GetUpperCellIndex(m_cellIndex);
				rotation = Quaternion.Euler(new Vector3(0, 0, 0));
				break;
			case Direction.Right:
				index = GameController.Instance.m_grid.GetRightCellIndex(m_cellIndex);
				rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				break;
			case Direction.Down:
				index = GameController.Instance.m_grid.GetDownCellIndex(m_cellIndex);
				rotation = Quaternion.Euler(new Vector3(0, 180, 0));
				break;
			case Direction.Left:
				index = GameController.Instance.m_grid.GetLeftCellIndex(m_cellIndex);
				rotation = Quaternion.Euler(new Vector3(0, 270, 0));
				break;
		}

		if (index != Grid.NON_REACHABLE_CELL)
		{
			GridCell cell = GameController.Instance.m_grid.GetCell(index);
			CmdMoveTo(cell.transform.position);
			if (isServer) RpcSetCellIndex(index);
			else CmdSetCellIndex(index);
			transform.rotation = rotation;
			m_answers.SetActive(false);
			m_questionsPanel.SetActive(false);
		}
	}

	void MoveAnswersToPirate()
	{
		if (!isLocalPlayer) return;

		m_answers.transform.position = new Vector3(
			transform.position.x,
			transform.position.y + 2.68f,
			transform.position.z
		);
	}

	void UpdateAnswersAndQuestionsAvailability()
	{
		if (!isLocalPlayer) return;

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

		int upper = GameController.Instance.m_grid.GetUpperCellIndex(m_cellIndex);
		int right = GameController.Instance.m_grid.GetRightCellIndex(m_cellIndex);
		int down = GameController.Instance.m_grid.GetDownCellIndex(m_cellIndex);
		int left = GameController.Instance.m_grid.GetLeftCellIndex(m_cellIndex);

		GameController controller = GameController.Instance;
		if (controller.m_players.Count == 2)
		{
			int otherPirateCellIndex = isServer ? controller.m_players[1].m_cellIndex : controller.m_players[0].m_cellIndex;
			if (upper == otherPirateCellIndex) upper = Grid.NON_REACHABLE_CELL;
			if (right == otherPirateCellIndex) right = Grid.NON_REACHABLE_CELL;
			if (down == otherPirateCellIndex) down = Grid.NON_REACHABLE_CELL;
			if (left == otherPirateCellIndex) left = Grid.NON_REACHABLE_CELL;
		}

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
