using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : NetworkManager
{
	public List<Pirate> m_players = new List<Pirate>();
	public Grid m_grid;

	public GameObject m_scorePanel;
	public GameObject m_timePanel;

	public GameObject m_networkButtonsPanel;
	public Button m_exitButton;

	public GameObject m_gameResultsPanel;

	public Text m_hostScoreText;
	public Text m_clientScoreText;

	public Timer m_timer;

	#region
	public static GameController Instance { get; private set; }
	#endregion

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		int playersCount = NetworkServer.connections.Count;
		if (playersCount <= startPositions.Count)
		{
			GameObject player = Instantiate(
				playerPrefab,
				startPositions[playersCount - 1].position,
				Quaternion.identity);
			NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

			player.GetComponent<Pirate>().m_cellIndex = startPositions[playersCount - 1].GetComponentInParent<GridCell>().m_index;
			m_players.Add(player.GetComponent<Pirate>());

			if (playersCount == 2)
			{
				SpawnChests();
				m_players[0].RpcShowScoreAndTimePanels();
				m_players[1].CmdShowScoreAndTimePanels();
			}
		}
		else
		{
			// Не позволяем соединиться третьему игроку?
			conn.Disconnect();
		}
	}

	public override void OnStopHost()
	{
		Debug.Log("Host stop");
		NetworkServer.DisconnectAll();
	}

	public override void OnStopClient()
	{
		Debug.Log("Client stop");
		base.OnStopClient();
	}

	public override void OnClientDisconnect(NetworkConnection conn)
	{
		Debug.Log("OnClientDisconnect");
		base.OnClientDisconnect(conn);
		conn.Disconnect();
	}

	private void Awake()
	{
		Instance = this;
		m_scorePanel.SetActive(false);
		m_timePanel.SetActive(false);
		m_gameResultsPanel.SetActive(false);
		m_exitButton.gameObject.SetActive(false);
	}

	void Start()
	{
		hostNameInput.text = NetworkManager.singleton.networkAddress;
	}

	private void Update()
	{
		if (m_players.Count != 2)
		{
			return;
		}

		// TODO: once flag
		if (m_timer.GetTime() <= Mathf.Epsilon)
		{
			string text = "";
			if (m_players[0].m_nScore == m_players[1].m_nScore)
			{
				text = "Draw!";
			}
			else if (m_players[0].m_nScore < m_players[1].m_nScore)
			{
				text = "Client wins!";
			}
			else
			{
				text = "Host wins!";
			}
			m_players[0].RpcShowGameResultsText(text);
			m_players[1].CmdShowGameResultsText(text);
			m_players[0].RpcPlayWinningSound();
			m_players[1].CmdPlayWinningSound();
		}
	}

	private void SpawnChests()
	{
		HashSet<int> indices = new HashSet<int>();
		while (indices.Count != 6)
		{
			int index = UnityEngine.Random.Range(7, 42);
			indices.Add(index);
		}

		foreach (int index in indices)
		{
			Vector3 rotation = new Vector3(0.0f, UnityEngine.Random.Range(-360.0f, 360.0f), 0.0f);
			Chest chest = Instantiate(spawnPrefabs[0], m_grid.GetCell(index).transform.position, Quaternion.Euler(rotation))
				.GetComponent<Chest>();
			chest.SetCellIndex(index);
			NetworkServer.Spawn(chest.gameObject);
		}
	}

	// custom network hud control
	public UnityEngine.UI.Text hostNameInput;

	public void StartLocalGame()
	{
		StartHost();
		m_networkButtonsPanel.SetActive(false);
		m_exitButton.gameObject.SetActive(true);
	}

	public void JoinLocalGame()
	{
		if (hostNameInput.text != "Hostname")
		{
			networkAddress = hostNameInput.text;
		}
		StartClient();
		m_networkButtonsPanel.SetActive(false);
		m_exitButton.gameObject.SetActive(true);
	}

	public void ExitGame()
	{
		if (NetworkServer.active)
		{
			StopServer();
		}
		if (NetworkClient.active)
		{
			StopClient();
		}
		m_players.Clear();
		m_networkButtonsPanel.SetActive(true);
		m_exitButton.gameObject.SetActive(false);
		Application.Quit();
	}
}
