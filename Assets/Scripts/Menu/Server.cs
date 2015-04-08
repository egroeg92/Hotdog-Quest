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

	
	float timeSent;
	public float latency1, latency2, clientToclientLatency;
	public float latencyInterval = 600;

	NetworkPlayer player1 ;
	NetworkPlayer player2 ;
	string net1 = null;
	string net2 = null;


	void Start(){
		Debug.Log ("server started");
		game.enemyAmount = enemyAmount;
		game.createMap ();

		latency1 = 0;
		latency2 = 0;
		clientToclientLatency = 0;
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
		if(game.deadReckoningOn)
			DRTime += Time.deltaTime;
		else
			noDRTime += Time.deltaTime;

		if (Time.frameCount % latencyInterval == 0) {
			latencyCheckSend ();
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
			game.instantiateEnemyMovement ();
			sendEnemies ();
			latencyCheckSend();
		} else {
			game.destroyEnemies();
			//game.destroyPlayers();
		}

		if(game.deadReckoningOn)
			networkView.RPC ("connectedToServer", RPCMode.All, player, connections, 1);
		else
			networkView.RPC ("connectedToServer", RPCMode.All, player, connections, 0);

	}
	[RPC]
	public void latencyCheckSend(){
		timeSent = Time.realtimeSinceStartup;
		if (connections == 2) {
			networkView.RPC ("latencyRecieve", RPCMode.All);
		}
	}
	[RPC]
	public void latencyCheckReceive(NetworkPlayer p){
		if (p == player1) {
			latency1 = Time.realtimeSinceStartup - timeSent;

		} else if (p == player2){
			latency2 = Time.realtimeSinceStartup - timeSent;
		}
		clientToclientLatency = latency1 + latency2;
	}

	[RPC]
	public void updateDeadReckoning(){
		networkView.RPC ("updateDeadReckoningClients", RPCMode.All, game.deadReckoningOn);
	}
	[RPC]
	public void enemyShotServer(int id){
		if (game.enemies [id] != null)
			game.enemyShot (id, true);

		networkView.RPC ("enemyShotRecieve",RPCMode.All, id);
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
			networkView.RPC ("receiveEnemyUpdate", RPCMode.All, (int)d.Key,((Enemy)d.Value).transform.position, ((Enemy)d.Value).velocity, player1, latency1);
			networkView.RPC ("receiveEnemyUpdate", RPCMode.All, (int)d.Key,((Enemy)d.Value).transform.position, ((Enemy)d.Value).velocity, player2, latency2);
		}
	}
	[RPC]
	public void sendEnemyUpdates(int key){
		enemyUpdateCountDR++;
		networkView.RPC ("receiveEnemyUpdate", RPCMode.All, key, ((Enemy)game.enemies[key]).transform.position, ((Enemy)game.enemies[key]).velocity,player1, latency1);
		networkView.RPC ("receiveEnemyUpdate", RPCMode.All, key, ((Enemy)game.enemies[key]).transform.position, ((Enemy)game.enemies[key]).velocity,player2, latency2);
	}
	[RPC]
	void updatePlayerPosition(Vector3 position,Vector3 velocity, NetworkPlayer player){
		if (game.deadReckoningOn)
			playerUpdateCountDR ++;
		else
			playerUpdateCountNoDR++;


		if (player == player1) {
			game.updatePlayerServer(1,position,velocity,latency1);
			if(player2 !=null)
				networkView.RPC ("receivePlayerUpdate", RPCMode.All, player2, position,  velocity,clientToclientLatency);

		} else if (player == player2) {
			game.updatePlayerServer(2,position,velocity,latency2);
			if(player1!=null)
				networkView.RPC ("receivePlayerUpdate", RPCMode.All, player1, position,  velocity,clientToclientLatency);
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
	public void createBulletServer(Vector3 pos, Vector3 vel, NetworkPlayer player){
		if (player == player1)
			game.createBullet (pos, vel, latency1);
		else if (player == player2)
			game.createBullet (pos, vel, latency2);

		networkView.RPC ("createBulletClient", RPCMode.All, pos, vel, player, clientToclientLatency);

	}
	[RPC]
	void enemyShotRecieve(int id){}
	[RPC]
	void latencyRecieve(){}
	[RPC]
	void createBulletClient (Vector3 pos, Vector3 vel, NetworkPlayer player, float latency){}
	[RPC]
	void updateDeadReckoningClients(bool dr){}
	[RPC]
	void receivePlayerUpdate(NetworkPlayer toPlayer, Vector3 pos, Vector3 vel, float latency){}
	[RPC]
	void receiveEnemyUpdate(int key, Vector3 pos, Vector3 vel,NetworkPlayer player, float latency){}
	[RPC]
	void connectedToServer(NetworkPlayer player, int connections, int dr){}
	[RPC]
	void receiveMap(NetworkPlayer p, Vector3 sizes, Vector3 pos, Vector3 plane, int count, int size){}

}
