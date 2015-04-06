using UnityEngine;
using System.Collections;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public class Server : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;
	public int enemyAmount = 3;

	public int latency = 0;


	//public Hashtable players;
	NetworkPlayer player1 ;
	NetworkPlayer player2 ;
	string net1 = null;
	string net2 = null;





	void Start(){
		Debug.Log ("server started");
		game.enemyAmount = enemyAmount;
		game.createMap ();


		//game.instantiateEnemies ();

	}

	int connections = 0;

	public void destroy(){
		game.server = null;
		game.destroyPlayers ();
		game.destroyCity ();
		game.destroyEnemies ();
		Network.Disconnect(250);
		Destroy (this);
	}

	void Update(){

	}
	void LateUpdate(){
		if (connections == 2) {
			sendPlayerUpdates ();
			sendEnemyUpdates();

		}
	}

	void OnPlayerConnected(NetworkPlayer player){
		Debug.Log("Player " + " connected from " + player.ipAddress);

		connections++;
		if (net1 == null) {
			player1 = player;
			net1 = player1.ToString();
		} else if (net2 == null) {
			player2 = player;
			net2 = player2.ToString();
		}else
			Debug.LogError ("error : both player slots are filled");

		sendMap (player);

		if (connections == 2) {
			game.instantiatePlayers ();
			game.createEnemies ();
			game.instantiateEnemies ();
			sendEnemies ();
		} else {
			game.destroyEnemies();
			//game.destroyPlayers();
		}
		networkView.RPC ("connectedToServer", RPCMode.All, player, connections);

	}
	[RPC]
	void sendEnemies(){
		foreach (DictionaryEntry d in game.enemies) {
			networkView.RPC ("receiveEnemies", RPCMode.All, (int)d.Key ,((Enemy)d.Value).transform.position, enemyAmount);
		}
	}
	[RPC]
	void receiveEnemies ( int key, Vector3 enPos, int enemyAmount){
	}
	[RPC]
	void sendMap(NetworkPlayer p){
		Debug.Log ("sending map...");

		ArrayList sizes = game.getMap ().buildingSizes;
		ArrayList pos = game.getMap ().buildingPositions;
		Vector3 planeSize = game.getMap ().plane.transform.localScale;

		for (int i = 0; i < sizes.Count; i++) {
			networkView.RPC ("receiveMap", RPCMode.All, p, sizes[i],pos[i],planeSize, i, sizes.Count);
		}


	}

	[RPC]
	void sendPlayerUpdates(){
		Debug.Log ("send Player updates");
		networkView.RPC ("receivePlayerUpdate", RPCMode.All, game.Player1.transform.position, game.Player2.transform.position);
	}
	[RPC]
	void sendEnemyUpdates(){
		foreach (DictionaryEntry d in game.getEnemies()) {
			networkView.RPC ("receiveEnemyUpdate", RPCMode.All, (int)d.Key, ((Enemy)d.Value).transform.position);
		}

	}
	[RPC]
	void updatePlayerPosition(Vector3 position, NetworkPlayer player){
		if (player == player1) {
			game.updatePlayer(1,position);
		} else if (player == player2) {
			game.updatePlayer(2,position);
		} else {
			Debug.Log ("trying to update wrong player");
		}


	}


	[RPC]
	public void removePlayer(NetworkPlayer player){
		if (player1 == player) {
			net1 = null;
			game.destroyPlayer(1);
		} else if (player2 == player){
			net2 = null;
			game.destroyPlayer(2);
		}else
			Debug.LogError ("Couldn't Remove Player from server");

		connections--;

	}


	[RPC]
	void receivePlayerUpdate(Vector3 pos1, Vector3 pos2){}
	[RPC]
	void receiveEnemyUpdate(int key, Vector3 pos){}
	[RPC]
	void connectedToServer(NetworkPlayer player, int connections){}
	[RPC]
	void receiveMap(NetworkPlayer p, Vector3 sizes, Vector3 pos, Vector3 plane, int count, int size){}

}
