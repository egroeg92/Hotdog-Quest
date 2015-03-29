using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;

	//public Hashtable players;
	NetworkPlayer player1 ;
	NetworkPlayer player2 ;
	string net1 = null;
	string net2 = null;




	void Start(){
		Debug.Log ("server started");

	}

	int connections = 0;

	public void destroy(){
		game.server = null;
		Network.Disconnect(250);
		Destroy (this);
	}

	void Update(){

	}
	void LateUpdate(){
		if(connections == 2)
			sendClientUpdates ();
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
		}
		else
			Debug.LogError ("error : both player slots are filled");

		networkView.RPC ("connectedToServer", RPCMode.All, player, connections);

	}
	[RPC]
	void sendClientUpdates(){
		networkView.RPC ("receiveUpdate", RPCMode.All, game.Player1.transform.position, game.Player2.transform.position);
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
	public void connectedToServer(NetworkPlayer player, int connections){
	
	}
	[RPC]
	public void removePlayer(NetworkPlayer player){
		if (player1 == player) {
			net1 = null;
		} else if (player2 == player){
			net2 = null;
		}else
			Debug.LogError ("Couldn't Remove Player from server");

		connections--;
		
	}
	[RPC]
	public void sendUpdate(){
		Debug.Log ("send update");

		
	}
	
	[RPC]
	public void receiveUpdate(Vector3 pos1, Vector3 pos2){

	}



}
