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

	public mapGen mapGenerator;
	GameObject map;
	ArrayList buildingPos = new ArrayList();
	ArrayList buildingSize = new ArrayList();


	// Use this for initialization
	void Start () {
		//Player1 = GameObject.Find ("Player1").GetComponent<Player>() ;
		//Player2 = GameObject.Find ("Player2").GetComponent<Player>();

	}
	
	// Update is called once per frame
	void Update () {
		if(client != null && thisPlayer != null)
			client.updatePosition (thisPlayer.transform.position);

	}

	public void setPlayer(int player){
		if (player == 1) {
			Debug.Log ("Set to player 1");
			instantiatePlayers();
			thisPlayer = Player1;
			otherPlayer = Player2;
			playerId = 1;
		
		} else {
			Debug.Log("set to player 2");
			instantiatePlayers();

			thisPlayer = Player2;
			otherPlayer = Player1;
			playerId = 2;
		}
		Player1.transform.position = new Vector3 (1, 2, 1);
		
		Player2.transform.position = new Vector3 (3, 2, 1);


		thisPlayer.enablePlayerControls();
		otherPlayer.disablePlayerControls ();
	}
	public void instantiatePlayers(){
		Player1 = (Player) Instantiate(Player1);
		Player2 = (Player) Instantiate(Player2);

		Player1.renderer.material.color =(Color.blue);
		Player2.renderer.material.color =(Color.red);
		

	}

	public void updatePlayer(int id, Vector3 position){
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

	public void createMap(){
		mapGenerator = Instantiate (mapGenerator) as mapGen;

	}
	public mapGen getMap(){
		return mapGenerator;
	}
	public void createCity(Vector3 plane){
		Debug.Log ("map gen");
		GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
		p.transform.localScale = plane;
		p.tag = "city";
		GameObject b;
		for (int i = 0; i< buildingSize.Count; i++) {
			b = GameObject.CreatePrimitive(PrimitiveType.Cube);
			b.transform.localScale = (Vector3)buildingSize[i];
			b.transform.position = (Vector3)buildingPos[i];
			b.tag = "city";
		}

	}
	public void addPosition(Vector3 p){
		buildingPos.Add (p);
	}
	
	public void addSize(Vector3 s){
		buildingSize.Add (s);
	}

	public void destroyCity(){
		buildingPos.Clear();
		buildingSize.Clear();

		GameObject[] city = GameObject.FindGameObjectsWithTag ("city");
		foreach (GameObject g in city)
			Destroy (g);

		Destroy (thisPlayer);
		Destroy (otherPlayer);
	}



}
