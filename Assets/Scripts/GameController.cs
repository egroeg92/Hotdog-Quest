using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Player Player1;
	public Player Player2;

	Player thisPlayer;
	Player otherPlayer;

	public Client client;
	public Server server;

	int playerId = -1;

	NetworkPeerType peerType;

	// Use this for initialization
	void Start () {
		Player1 = GameObject.Find ("Player1").GetComponent<Player>() ;
		Player2 = GameObject.Find ("Player2").GetComponent<Player>();

	}
	
	// Update is called once per frame
	void Update () {
		if(client != null && thisPlayer != null)
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
			playerId = 1;

		} else {
			Debug.Log("set to player 2");
			thisPlayer = Player2;
			otherPlayer = Player1;
			playerId = 2;
		}
		
		thisPlayer.enablePlayerControls();
	}

	public void updatePlayer(int id, Vector3 position){
		Debug.Log (playerId);
		if (id == 1) {
			Player1.transform.position = position;
		} else if (id == 2)  {
			Player2.transform.position = position;
		}

	}
	public void updateOtherPlayer(int id, Vector3 position){

		if (id != playerId) {
			otherPlayer.transform.position = position;
		} 
		
	}





}
