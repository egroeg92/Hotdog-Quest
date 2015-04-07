using UnityEngine;
using System.Collections;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public class Server : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;
	public int enemyAmount = 3;

	public int playerUpdateCountDR = 0;
	public int playerUpdateCountNoDR = 0;
	public int enemyUpdateCountDR = 0;
	public int enemyUpdateCountNoDR = 0;
	public float DRTime = 0;
	public float noDRTime = 0;

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
		Debug.Log (enemyUpdateCountDR);
		if(game.deadReckoningOn)
			DRTime += Time.deltaTime;
		else
			noDRTime += Time.deltaTime;

	}
	void LateUpdate(){
		//if (connections == 2) {

			//sendEnemyUpdates();

		//}
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
			game.instantiateEnemyMovement ();
			sendEnemies ();
		} else {
			game.destroyEnemies();
			//game.destroyPlayers();
		}
		networkView.RPC ("connectedToServer", RPCMode.All, player, connections);

	}
	[RPC]
	public void updateDeadReckoning(){
		networkView.RPC ("updateDeadReckoningClients", RPCMode.All, game.deadReckoningOn);
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
	public void sendEnemyUpdates(){
		foreach (DictionaryEntry d in game.getEnemies()) {
			enemyUpdateCountNoDR++;
			networkView.RPC ("receiveEnemyUpdate", RPCMode.All, (int)d.Key, ((Enemy)d.Value).transform.position, ((Enemy)d.Value).velocity);
		}
	}
	[RPC]
	public void sendEnemyUpdates(int key){
		enemyUpdateCountDR++;
		networkView.RPC ("receiveEnemyUpdate", RPCMode.All, key, ((Enemy)game.enemies[key]).transform.position, ((Enemy)game.enemies[key]).velocity);

	}
	[RPC]
	void updatePlayerPosition(Vector3 position,Vector3 velocity, NetworkPlayer player){
		if (game.deadReckoningOn)
			playerUpdateCountDR ++;
		else
			playerUpdateCountNoDR++;


		if (player == player1) {
			game.updatePlayerServer(1,position,velocity);
			if(player2 !=null)
				networkView.RPC ("receivePlayerUpdate", RPCMode.All, player2, position,  velocity);

		} else if (player == player2) {
			game.updatePlayerServer(2,position,velocity);
			if(player1!=null)
				networkView.RPC ("receivePlayerUpdate", RPCMode.All, player1, position,  velocity);
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
	void updateDeadReckoningClients(bool dr){}
	[RPC]
	void receivePlayerUpdate(NetworkPlayer toPlayer, Vector3 pos, Vector3 vel){}
	[RPC]
	void receiveEnemyUpdate(int key, Vector3 pos, Vector3 vel){}
	[RPC]
	void connectedToServer(NetworkPlayer player, int connections){}
	[RPC]
	void receiveMap(NetworkPlayer p, Vector3 sizes, Vector3 pos, Vector3 plane, int count, int size){}

}
