using UnityEngine;
using System.Collections;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
public class Client : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;
	public NetworkViewID viewID;

	bool connected = false;
	Hashtable enemyPos = new Hashtable();

	public void destroy(){

		removePlayer (Network.player);
		game.client = null;
		game.destroyPlayers ();
		game.destroyCity ();
		game.destroyEnemies ();
		game.enoughEnemies = false;
		Network.Disconnect (250);
		Destroy (this);
	
	}

	public void updatePosition(Vector3 position){
		updatePlayerPosition (position, Network.player);
	}
	[RPC]
	void sendMap(NetworkPlayer p){
	}
	[RPC]
	void receiveMap(NetworkPlayer p, Vector3 sizes, Vector3 pos, Vector3 plane, int count, int size){

		game.addPosition (pos);
		game.addSize (sizes);
		game.enemyAmount = 0;

		if(count == size -1)
			game.createCity(plane);


	}
	[RPC]
	void sendEnemies(){}

	[RPC]
	void receiveEnemies (int key, Vector3 enPos, int count){
		enemyPos.Add (key, enPos);

		if (enemyPos.Count == count) {
			game.enoughEnemies = true;
			game.createEnemies(enemyPos);
		}
	}
	[RPC]
	void updatePlayerPosition(Vector3 position, NetworkPlayer player){
		networkView.RPC ("updatePlayerPosition", RPCMode.Server, position, Network.player);
	}
		
	[RPC]
	void removePlayer(NetworkPlayer player){
		networkView.RPC ("removePlayer", RPCMode.Server, Network.player);
	}
		
	[RPC]
	public void connectedToServer(NetworkPlayer player, int connections){
		if (Network.player == player) {
			game.setPlayer(connections);
		}
	}

	[RPC]
	public void receivePlayerUpdate(Vector3 p1Pos, Vector3 p2Pos){
		Debug.Log (p1Pos + " " + p2Pos);
		game.updateOtherPlayer (1, p1Pos);
		game.updateOtherPlayer (2, p2Pos);

	}
	[RPC]
	void receiveEnemyUpdate(int key, Vector3 pos){
		game.updateEnemy (key, pos);
	}


}
