using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour{

	public GameController game;
	public NetworkView networkView;


	public void destroy(){
		game.client = null;
		Destroy (this);
	
	}
	[RPC]
	public void sendUpdate(){
		Debug.Log ("send update");
		networkView.RPC("receiveUpdate", RPCMode.Server);
		
	}
	
	[RPC]
	public void receiveUpdate(){
		
	}


}
