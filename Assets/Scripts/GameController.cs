using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Player thisPlayer;
	public Player otherPlayer;

	public Client client;
	public Server server;

	NetworkPeerType peerType;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
	

	}
	void OnGUI(){

		if (client != null) {
			if (GUI.Button (new Rect (100, 150, 100, 25), "send message")) {
				
				client.sendUpdate ();
				
				
				
			}
		}
	}





}
