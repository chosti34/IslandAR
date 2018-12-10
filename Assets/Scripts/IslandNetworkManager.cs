using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IslandNetworkManager : NetworkManager
{
	public List<Pirate> m_players = new List<Pirate>();

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

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void SpawnChests()
	{
		// TODO: implement this
		Debug.Log("Spawning chests...");
	}
}
