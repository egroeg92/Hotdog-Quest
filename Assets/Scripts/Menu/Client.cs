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

	public void updatePosition(Vector3 position, Vector3 velocity){
		updatePlayerPosition (position,velocity, Network.player);
	}
	[RPC]
	public void sendBullet(Vector3 pos, Vector3 vel){
		networkView.RPC ("createBullet", RPCMode.Server, pos, vel);
	}
	[RPC]
	void updateDeadReckoningClients(bool dr){
		game.deadReckoningOn = dr;
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
	void updatePlayerPosition(Vector3 position,Vector3 velocity, NetworkPlayer player){
		networkView.RPC ("updatePlayerPosition", RPCMode.Server, position,velocity, Network.player);
	}
		
	[RPC]
	void removePlayer(NetworkPlayer player){
		networkView.RPC ("removePlayer", RPCMode.Server, Network.player);
	}
		
	[RPC]
	public void connectedToServer(NetworkPlayer player, int connections, int dr){
		if (Network.player == player) {
			game.setPlayer(connections);
			if(dr == 1)
				game.deadReckoningOn = true;
			else
				game.deadReckoningOn = false;
		}
	}

	[RPC]
	public void receivePlayerUpdate(NetworkPlayer  player , Vector3 pos,Vector3 vel){
		if (player == Network.player) {
			game.updateOtherPlayer (pos, vel);
		}

	}
	[RPC]
	void receiveEnemyUpdate(int key, Vector3 pos, Vector3 vel){
		game.updateEnemy (key, pos, vel);
	}

	[RPC]
	void createBullet(Vector3 pos, Vector3 vel){}
}
