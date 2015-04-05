using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Player Player1;
	public Player Player2;

	Player thisPlayer;
	Player otherPlayer;

	public Client client;
	public Server server;

	public int enemyAmount;
	public Hashtable enemies;
	public enemy1 enemy;
	public float enemySpeed;
	public bool enoughEnemies = false;
	public Camera mainCamera;


	int playerId = -1;

	NetworkPeerType peerType;

	public mapGen mapGenerator;
	GameObject map;
	ArrayList buildingPos = new ArrayList();
	ArrayList buildingSize = new ArrayList();


	/*
	 * Initialization + setters
	 */
	void Start () {
		enemies = new Hashtable ();
		mainCamera.orthographic = true;
	}
	
	public void createEnemies(){
		for (int i = 0; i< enemyAmount; i++) {
			enemy = Instantiate(Resources.Load("Enemy", typeof(enemy1)), getValidPosition(), Quaternion.identity) as enemy1;
			enemy.id = i;
			enemies.Add(i,enemy);
		}
	}
	
	public void createEnemies(Hashtable positions){
		Debug.Log ("Create Enemies");
		enemies.Clear ();
		foreach(DictionaryEntry d in positions){
			enemy = Instantiate(Resources.Load("Enemy", typeof(enemy1)), (Vector3)d.Value, Quaternion.identity) as enemy1;
			enemy.id = (int)d.Key;
			enemies.Add(enemy.id,enemy);
		}
	}
	public void instantiateEnemies(){
		foreach (DictionaryEntry e in enemies) {
			((enemy1)e.Value).gameObject.AddComponent<enemyMovement1>();
			((enemy1)e.Value).GetComponent<enemyMovement1>().velocity = new Vector3 (Random.Range (0.0f, 100.0f), 0, Random.Range (0.0f, 100.0f));
			((enemy1)e.Value).GetComponent<enemyMovement1>().speed = enemySpeed;
		}
	}
	public void createMap(){
		mapGenerator = Instantiate (mapGenerator) as mapGen;
		
	}
	
	public void addPosition(Vector3 p){
		buildingPos.Add (p);
	}
	
	public void addSize(Vector3 s){
		buildingSize.Add (s);
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
		mainCamera.transform.position = thisPlayer.transform.position + new Vector3(0,20,0);
		mainCamera.transform.parent = thisPlayer.transform;

		Player1.transform.position = new Vector3 (1, 2, 1);
		
		Player2.transform.position = new Vector3 (3, 2, 1);


		thisPlayer.enablePlayerControls();
		otherPlayer.disablePlayerControls ();
	}
	public void instantiatePlayers(){
		Player1 = Instantiate(Resources.Load("Player", typeof(Player))) as Player;
		Player2 = (Player) Instantiate(Resources.Load("Player", typeof(Player)));

        Player1.transform.position = Vector3.zero;
        Player2.transform.position = Vector3.zero;

            
		Player1.renderer.material.color =(Color.blue);
		Player2.renderer.material.color =(Color.red);
		

	}
    public void setCamera(int player)
    {
        if (player == 1)
        {
            if (Player1 != null)
            {
                mainCamera.transform.position = Player1.transform.position + new Vector3(0, 20, 0);
                mainCamera.transform.parent = Player1.transform;
            }
        }
        else if (player == 2)
        {
            if (Player2 != null)
            {
                mainCamera.transform.position = Player2.transform.position + new Vector3(0, 20, 0);
                mainCamera.transform.parent = Player2.transform;
            }
        }
        else
        {
            if (enemies.Count != 0)
            {
                mainCamera.transform.position = ((enemy1)enemies[Random.Range(0, enemies.Count)]).transform.position + new Vector3(0, 20, 0);
                mainCamera.transform.parent = ((enemy1)enemies[Random.Range(0, enemies.Count)]).transform;
            }
        }

    }

	/*
	 *  UPDATES
	 */
	void Update () {
		if (client != null && thisPlayer != null) {
			client.updatePosition (thisPlayer.transform.position);
		}
	}

	public void updatePlayer(int id, Vector3 position){

		if (id == 1) {
			if(Player1 != null)
				Player1.transform.position = position;
		} else if (id == 2)  {
			if(Player2 != null)
				Player2.transform.position = position;
		}

	}
	public void updateOtherPlayer(int id, Vector3 position){
		if (id != playerId) {
			otherPlayer.transform.position = position;
		} 
		
	}
	public void updateEnemy(int key, Vector3 position){
		if (enoughEnemies) {
			((enemy1)enemies [key]).transform.position = position;
		}
	}

	/*
	 * GETTERS
	*/
	public Hashtable getEnemies(){
		return enemies;
	}


	public Vector3 getPlayerPosition(){
		return thisPlayer.transform.position;
	}
	public mapGen getMap(){
		return mapGenerator;
	}
	public void createCity(Vector3 plane){
		Debug.Log ("map gen");
		GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
		p.transform.localScale = plane;
		p.tag = "city";
		GameObject w = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;
		GameObject e = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;
		GameObject n = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;
		GameObject s = GameObject.CreatePrimitive (PrimitiveType.Cube) as GameObject;

		n.transform.position = new Vector3 (0, 1, p.renderer.bounds.max.z);
		s.transform.position = new Vector3 (0, 1, p.renderer.bounds.min.z);
		e.transform.position = new Vector3 (p.renderer.bounds.max.x, 1, 0);
		w.transform.position = new Vector3 (p.renderer.bounds.min.x, 1, 0);
		
		n.transform.localScale = new Vector3 (p.renderer.bounds.max.x - p.renderer.bounds.min.x, 3, .1f);
		s.transform.localScale = new Vector3 (p.renderer.bounds.max.x - p.renderer.bounds.min.x, 3, .1f);
		e.transform.localScale = new Vector3 (.1f, 3, p.renderer.bounds.max.z - p.renderer.bounds.min.z);
		w.transform.localScale = new Vector3 (.1f, 3, p.renderer.bounds.max.z - p.renderer.bounds.min.z);
		
		n.tag = "city";
		s.tag = "city";
		e.tag = "city";
		w.tag = "city";

		GameObject b;
		for (int i = 0; i< buildingSize.Count; i++) {
			b = GameObject.CreatePrimitive(PrimitiveType.Cube);
			b.transform.localScale = (Vector3)buildingSize[i];
			b.transform.position = (Vector3)buildingPos[i];
			b.tag = "city";
			b.renderer.material.color = Color.green;
		}

	}



	/*
	 *  Destroyers
	 * 
	 */
    public void destroyCity()
    {
        buildingPos.Clear();
        buildingSize.Clear();

        GameObject[] city = GameObject.FindGameObjectsWithTag("city");
        foreach (GameObject g in city)
            Destroy(g);

    }

	public void destroyPlayers(){
		mainCamera.transform.parent = null;
		if(thisPlayer != null)
			Destroy (thisPlayer.gameObject);
		if(otherPlayer != null)
			Destroy (otherPlayer.gameObject);
	}

	public void destroyPlayer(int player){
		if (player == 1) {
			if (Player1 != null) {
				Destroy (Player1.gameObject);
				Destroy (Player1);
			}
		}else {
			if (Player2 != null) {
				Destroy (Player2.gameObject);
				Destroy (Player2);
			}
		}
	}

	public void destroyEnemies(){
        Debug.Log(enemies.Count);
		foreach (DictionaryEntry e in enemies) {
			Destroy (((enemy1)e.Value).gameObject);
		}
		enemies.Clear ();
	}

	/*
	 * Helpers
	 */
	Vector3 getValidPosition(){
		return Vector3.zero;
	}



}
