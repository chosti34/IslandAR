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
	private int m_cellIndex;
	public int CellIndex
	{
		get
		{
			return m_cellIndex;
		}
		set
		{
			if (m_cellIndex != value)
			{
				m_cellIndex = value;
			}
		}
	}

	[SerializeField]
	private float m_speed;

	[SyncVar]
	private int m_nScoreHost;

	[SyncVar]
	private int m_nScoreClient;


	public GameObject m_answers;
	public Quaternion m_answersRotationIdentity;

	float gravity = 15;

	CharacterController m_characterController;

	[SyncVar]
	Vector3 moveDirection = Vector3.zero;

	[SyncVar]
	Vector3 m_destination;

	[SyncVar]
	bool m_moving;

	[SyncVar]
	string m_scoreHostString;
	[SyncVar]
	string m_scoreClientString;

	Text m_scoreHostText;
	Text m_scoreClientText;

	// Player user interface
	private Canvas m_canvas;
	private GameObject m_questionsPanel;
	private Button[] m_buttons;

	public AudioClip m_clip;

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

		m_scoreHostText = GameObject.Find("PlayerOneScore").GetComponent<Text>();
		m_scoreClientText = GameObject.Find("PlayerTwoScore").GetComponent<Text>();

		m_scoreHostString = "Host: 0";
		m_scoreClientString = "Client: 0";

		m_scoreHostText.text = m_scoreHostString;
		m_scoreClientText.text = m_scoreClientString;

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

		//APPLY GRAVITY
		if (moveDirection.y > gravity * -1)
		{
			moveDirection.y -= gravity * Time.deltaTime;
		}

		m_characterController.Move(moveDirection * Time.deltaTime);
		var left = transform.TransformDirection(Vector3.left);

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
			Debug.Log(Vector2.Distance(transform.position, m_destination));
			if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_destination.x, m_destination.z)) <= 0.6f)
			{
				m_moving = false;
				m_destination = Vector3.zero;
				m_animator.SetTrigger("Trigger");

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
		m_animator.SetTrigger("Trigger");
		m_destination = new Vector3(pos.x, transform.position.y, pos.z);
		m_moving = true;
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
	}

	void OnTriggerEnter(Collider other)
	{
		if (!isLocalPlayer)
		{
			return;
		}

		if (other.tag == "Chest")
		{
			GetComponent<AudioSource>().PlayOneShot(m_clip);
			Destroy(other.gameObject);

			if (isServer)
			{
				m_nScoreHost += 1;
			}
			else
			{
				m_nScoreClient += 1;
			}

			m_scoreHostString = "Host: " + m_nScoreHost.ToString();
			m_scoreClientString = "Client: " + m_nScoreClient.ToString();
			m_scoreHostText.text = m_scoreHostString;
			m_scoreClientText.text = m_scoreClientString;
		}
	}

	void MovePirate(Direction direction)
	{
		if (!isLocalPlayer) return;

		int index = Grid.NON_REACHABLE_CELL;
		Quaternion rotation = Quaternion.identity;

		switch (direction)
		{
			case Direction.Up:
				index = IslandNetworkManager.Instance.m_grid.GetUpperCellIndex(CellIndex);
				rotation = Quaternion.Euler(new Vector3(0, 0, 0));
				break;
			case Direction.Right:
				index = IslandNetworkManager.Instance.m_grid.GetRightCellIndex(CellIndex);
				rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				break;
			case Direction.Down:
				index = IslandNetworkManager.Instance.m_grid.GetDownCellIndex(CellIndex);
				rotation = Quaternion.Euler(new Vector3(0, 180, 0));
				break;
			case Direction.Left:
				index = IslandNetworkManager.Instance.m_grid.GetLeftCellIndex(CellIndex);
				rotation = Quaternion.Euler(new Vector3(0, 270, 0));
				break;
		}

		if (index != Grid.NON_REACHABLE_CELL)
		{
			GridCell cell = IslandNetworkManager.Instance.m_grid.GetCell(index);
			CmdMoveTo(cell.transform.position);
			CellIndex = index;
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

	public void UpdateAnswersAndQuestionsAvailability()
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

		int upper = IslandNetworkManager.Instance.m_grid.GetUpperCellIndex(CellIndex);
		int right = IslandNetworkManager.Instance.m_grid.GetRightCellIndex(CellIndex);
		int down = IslandNetworkManager.Instance.m_grid.GetDownCellIndex(CellIndex);
		int left = IslandNetworkManager.Instance.m_grid.GetLeftCellIndex(CellIndex);
		Debug.Log(CellIndex);

		IslandNetworkManager instance = IslandNetworkManager.Instance;
		if (instance.m_players.Count == 2)
		{
			Pirate otherPirate = instance.m_players[1].playerControllerId == playerControllerId ? instance.m_players[1] : instance.m_players[0];
			if (upper == otherPirate.CellIndex) upper = Grid.NON_REACHABLE_CELL;
			if (right == otherPirate.CellIndex) right = Grid.NON_REACHABLE_CELL;
			if (down == otherPirate.CellIndex) down = Grid.NON_REACHABLE_CELL;
			if (left == otherPirate.CellIndex) left = Grid.NON_REACHABLE_CELL;
			Debug.Log(otherPirate.CellIndex);
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
