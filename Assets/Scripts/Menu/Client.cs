using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;
	public NetworkViewID viewID;
	bool connected = false;

	public void destroy(){

		removePlayer (Network.player);
		game.client = null;
		Network.Disconnect (250);
		Destroy (this);
	
	}

	public void updatePosition(Vector3 position){
		updatePlayerPosition (position, Network.player);
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
		Debug.Log (Network.player == player);
		if (Network.player == player) {

			game.setPlayer(connections);

		}
	}

	[RPC]
	public void sendUpdate(){
	
	}
	
	[RPC]
	public void receiveUpdate(Vector3 p1Pos, Vector3 p2Pos){
		game.updateOtherPlayer (1, p1Pos);
		game.updateOtherPlayer (2, p2Pos);

	}


}
