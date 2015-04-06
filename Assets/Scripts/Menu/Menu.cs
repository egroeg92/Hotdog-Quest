using UnityEngine;
using System.Collections;

using System.Net;



public class Menu : MonoBehaviour {

	public GameController game;
	public Server server;
	public Client client;


    public string typeName = "HotdogQuest";
    public string gameName = "Room Name";
	public string IP = "127.0.0.1";
	public int Port = 25001;

    private HostData[] hostlist; 

	void Start(){
		IP = GetIP ();
		game = GetComponent<GameController> ();
	}
    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostlist = MasterServer.PollHostList();

    }
	void OnGUI(){

		if (Network.peerType == NetworkPeerType.Disconnected) {

			//IP = GUI.TextField(new Rect (100, 75, 100, 25), IP);
            if (GUI.Button(new Rect(50, 50, 150, 50), "Refresh Game List"))
            {
                MasterServer.RequestHostList(typeName);
            }
            if (hostlist != null)
            {
                for (int i = 0; i < hostlist.Length; i++)
                {
                    if (GUI.Button(new Rect(200, 50 + (50 * i), 150, 50), "join " + hostlist[i].gameName))
                    {
                        Network.Connect(hostlist[i]);
                        game.gameObject.AddComponent<Client>();
                        game.client = game.GetComponent<Client>();
                        game.client.game = game;
                        game.client.networkView = networkView;


                    }
                }
            }
			if (GUI.Button (new Rect (50, 100, 150, 50), "Start Server")) {
				Network.InitializeServer (2, Port,!Network.HavePublicAddress());
                gameName = IP;
                MasterServer.RegisterHost(typeName, gameName);

				
				game.gameObject.AddComponent<Server>();
				game.server = game.GetComponent<Server>();
				game.server.game = game;
				game.server.networkView = networkView;


			}
		} else {
			if (Network.peerType == NetworkPeerType.Client) {
				if (GUI.Button (new Rect (50, 50, 150, 50), "Logout Client")) {
                    game.client.destroy();
				}



			}
			if (Network.peerType == NetworkPeerType.Server){

                //GUI.Label(new Rect(100, 100, 100, 25), "Server");
				GUI.Label(new Rect(200,50,150,50), "Connections : " + Network.connections.Length);
				if(GUI.Button(new Rect(50,50,150,50),"Logout Server")){
					game.server.destroy();
				}
				string dead = "Turn DR on";
				if(game.deadReckoningOn == true)
					dead = "Turn DR off";

				if(GUI.Button(new Rect(200,100,150,50),dead)){
					game.deadReckoningOn = !game.deadReckoningOn;
					game.server.updateDeadReckoning();
				}


                if (Network.connections.Length == 2)
                {
                    if (GUI.Button(new Rect(50, 100, 150, 50), "Camera to Player 1"))
                    {
                        game.setCamera(1);
                    }
                    if (GUI.Button(new Rect(50, 150, 150, 50), "Camera to Player 2"))
                    {
                        game.setCamera(2);
                    }
                    if (GUI.Button(new Rect(50, 200, 150, 50), "Camera to an Enemy"))
                    {
                        game.setCamera(3);
                    }
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
