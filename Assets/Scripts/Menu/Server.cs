using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;

	public ArrayList players;

	void Start(){
		Debug.Log ("server started");
		players = new ArrayList ();
	}

	int connections = 0;

	public void destroy(){
		game.server = null;
		Network.Disconnect(250);
		Destroy (this);
	}

	void Update(){

	}
	void OnPlayerConnected(NetworkPlayer player){
		Debug.Log("Player " + " connected from " + player.ipAddress);
		players.Add (player);
		connections++;
		networkView.RPC ("connectedToServer", RPCMode.All, player, connections);

	}

	[RPC]
	void updatePlayerPosition(Vector3 position, NetworkPlayer player){
		Debug.Log (position);
	}

	[RPC]
	public void connectedToServer(NetworkPlayer player, int connections){
	
	}
	[RPC]
	public void removePlayer(NetworkPlayer player){
		players.Remove (player);
		
	}
	[RPC]
	public void sendUpdate(){
		Debug.Log ("send update");

		
	}
	
	[RPC]
	public void receiveUpdate(NetworkViewID id){
		Debug.Log ("server receive update " + id);
	}



}
