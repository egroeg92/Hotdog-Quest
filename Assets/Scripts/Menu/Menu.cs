using UnityEngine;
using System.Collections;

using System.Net;



public class Menu : MonoBehaviour {

	public GameController game;
	public Server server;
	public Client client;


	public string IP = "127.0.0.1";
	public int Port = 25001;

	void Start(){
		IP = GetIP ();
		Debug.Log (IP);
		game = GetComponent<GameController> ();
	}

	void OnGUI(){

		if (Network.peerType == NetworkPeerType.Disconnected) {

			IP = GUI.TextField(new Rect (100, 75, 100, 25), IP);

			if (GUI.Button (new Rect (100, 100, 100, 25), "Start Client")) {
				Network.Connect (IP, Port);
				game.gameObject.AddComponent<Client>();
				game.client = game.GetComponent<Client>();
				game.client.game = game;
				game.client.networkView = networkView;


			}
			if (GUI.Button (new Rect (100, 125, 100, 25), "Start Server")) {
				Network.InitializeServer (2, Port);

				
				game.gameObject.AddComponent<Server>();
				game.server = game.GetComponent<Server>();
				game.server.game = game;
				game.server.networkView = networkView;


			}
		} else {
			if (Network.peerType == NetworkPeerType.Client) {
				GUI.Label (new Rect (100, 100, 100, 25), "Client");

				if (GUI.Button (new Rect (100, 125, 100, 25), "Logout")) {

					game.client.destroy();


				}



			}
			if (Network.peerType == NetworkPeerType.Server){

				GUI.Label(new Rect(100,100,100,25), "Server");
				GUI.Label(new Rect(100,125,100,25), "Connections : " + Network.connections.Length);
				//GUI.Label(new Rect(100,150,100,25), "List Connections : "+ game.server.players.Count);

				if(GUI.Button(new Rect(100,175,100,25),"Logout")){

					game.server.destroy();


				}


			}
		}
	}
	string GetIP()
	{
		string strHostName = "";

		strHostName = System.Net.Dns.GetHostName();
		
		IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
		
		IPAddress[] addr = ipEntry.AddressList;
		
		return addr[addr.Length-1].ToString();
		
	}

}
