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
	void Update(){
		if (Network.peerType == NetworkPeerType.Disconnected) {

			MasterServer.RequestHostList (typeName);
		}
	}
	void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostlist = MasterServer.PollHostList();

    }

	void OnGUI(){

		if (Network.peerType == NetworkPeerType.Disconnected) {

			//IP = GUI.TextField(new Rect (100, 75, 100, 25), IP);

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
			if (GUI.Button (new Rect (50, 50, 150, 50), "Start Server")) {
				Network.InitializeServer (2, Port,!Network.HavePublicAddress());
                gameName = IP;
                MasterServer.RegisterHost(typeName, gameName);

				
				game.gameObject.AddComponent<Server>();
				game.server = game.GetComponent<Server>();
				game.server.game = game;
				game.server.networkView = networkView;


			}
		} else {
			if(game.Player1 != null && game.Player2 != null){
				if(game.Player1.getHealth() <= 0 ){
					GUI.Label(new Rect(100,0,100,50), "Player 1 is DEAD");
				}else{
					GUI.Label(new Rect(100,0,100,50), "Player 1 Health " + game.Player1.getHealth() + "/ 100");
				}
				if(game.Player2.getHealth() <= 0){
					GUI.Label(new Rect(300,0,100,50), "Player 2 is DEAD");

				}else{
					GUI.Label(new Rect(300,0,100,50), "Player 2 Health " + game.Player2.getHealth() + "/ 100");
				}
			}


			if (Network.peerType == NetworkPeerType.Client) {
				GUI.Label(new Rect(200,50,250,50), "Number Of Enemies left " + game.enemies.Count);


				if (GUI.Button (new Rect (50, 50, 150, 50), "Logout Client")) {
                    game.client.destroy();
				}



			}
			if (Network.peerType == NetworkPeerType.Server){

				GUI.color = Color.black;

				string dead = "Turn DR on";
				if(game.deadReckoningOn == true)
					dead = "Turn DR off";
				
				if(GUI.Button(new Rect(200,50,150,50),dead)){
					game.deadReckoningOn = !game.deadReckoningOn;
					game.server.updateDeadReckoning();
				}

				GUI.Label(new Rect(200,100,150,50), "Connections : " + Network.connections.Length);

				int DRtotal = game.server.playerUpdateCountDR;
				int noDRtotal = game.server.playerUpdateCountNoDR;
				int total = DRtotal + noDRtotal;
				
				int eDRtotal = game.server.enemyUpdateCountDR;
				int eNoDRtotal = game.server.enemyUpdateCountNoDR;
				int eTotal = eDRtotal + eNoDRtotal;


				float totalTimeDR= game.server.DRTime;
				float totalTimeNoDR = game.server.noDRTime;
				float totalTime = totalTimeNoDR + totalTimeDR;

				GUI.Label(new Rect(200,150,250,50), "Player Updates Total " + total + " / "+totalTime) ;
				GUI.Label(new Rect(200,200,250,50), "Player Updates with DR " + DRtotal + " / " +totalTimeDR);
				GUI.Label(new Rect(200,250,250,50), "Player Updates without DR " + noDRtotal+ " / " + totalTimeNoDR);

				GUI.Label(new Rect(200,300,250,50), "Number of Enemies " + game.enemies.Count);
				GUI.Label(new Rect(200,350,250,50), "Enemy Updates Total " + eTotal + " / "+totalTime) ;
				GUI.Label(new Rect(200,400,250,50), "Enemy Updates with DR " + eDRtotal + " / " +totalTimeDR);
				GUI.Label(new Rect(200,450,250,50), "Enemy Updates without DR " + eNoDRtotal+ " / " + totalTimeNoDR);

				GUI.Label(new Rect(450,400,250,50), "Player 1 latency " + game.server.latency1);
				GUI.Label(new Rect(450,450,250,50), "Player 2 latency " + game.server.latency2);



				
				if(GUI.Button(new Rect(50,50,150,50),"Logout Server")){
					game.server.destroy();
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
