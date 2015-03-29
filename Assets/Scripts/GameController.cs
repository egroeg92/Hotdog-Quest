using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	Player Player1;
	Player Player2;

	Player thisPlayer;
	Player otherPlayer;

	public Client client;
	public Server server;

	NetworkPeerType peerType;

	// Use this for initialization
	void Start () {
		Player1 = GameObject.Find ("Player1").GetComponent<Player>() ;
		Player2 = GameObject.Find ("Player2").GetComponent<Player>();

	}
	
	// Update is called once per frame
	void Update () {
		if(client != null)
			client.updatePosition (thisPlayer.transform.position);

	}
	void OnGUI(){

		if (client != null) {
			if (GUI.Button (new Rect (100, 150, 100, 25), "send message")) {
				
				client.sendUpdate ();
				
				
				
			}
		}
	}

	public void setPlayer(int player){
		if (player == 1) {
			Debug.Log ("Set to player 1");
			thisPlayer = Player1;
			otherPlayer = Player2;




		} else {
			Debug.Log("set to player 2");
			thisPlayer = Player2;
			otherPlayer = Player1;
		}
		
		otherPlayer.disablePlayerControls();
	}





}
