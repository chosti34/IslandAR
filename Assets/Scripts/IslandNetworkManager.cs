using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IslandNetworkManager : NetworkManager
{
	public List<Pirate> m_players = new List<Pirate>();
	public Grid m_grid;

	public GameObject scorePanel;
	public GameObject timePanel;
	public Text m_hostScoreText;
	public Text m_clientScoreText;

	#region
	public static IslandNetworkManager Instance { get; private set; }
	#endregion

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		int playersCount = NetworkServer.connections.Count;
		if (playersCount <= startPositions.Count)
		{
			if (playersCount == 2)
			{
				SpawnChests();
			}

			GameObject player = Instantiate(
				playerPrefab,
				startPositions[playersCount - 1].position,
				Quaternion.identity);
			NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

			player.GetComponent<Pirate>().CellIndex = startPositions[playersCount - 1].GetComponentInParent<GridCell>().m_index;
			m_players.Add(player.GetComponent<Pirate>());
		}
		else
		{
			// Не позволяем соединиться третьему игроку?
			conn.Disconnect();
		}
	}
		
	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
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
}
