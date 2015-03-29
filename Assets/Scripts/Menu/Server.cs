using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;

	public void destroy(){
		game.server = null;
		Destroy (this);
	}

	[RPC]
	public void sendUpdate(){
		Debug.Log ("send update");

		
	}
	
	[RPC]
	public void receiveUpdate(){
		Debug.Log ("yay");
	}

}
