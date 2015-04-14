using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Player Player1;
	public Player Player2;

	public Player thisPlayer;
	Player otherPlayer;

	public Client client;
	public Server server;

	public int enemyAmount;
	public Hashtable enemies;
	public Hashtable enemyPositions;
	public Enemy enemy;
	public bool enoughEnemies = false;
	public Camera mainCamera;


	public float enemySpeed;
	public float playerSpeed;
	public float bulletSpeed = 5;
	public float enemyRespawnRate = 10;
	public int enemyLimit = 50;

	public int mapDim = 100;
	public int mapDiv = 100;
	public int mapCourtProb = 100;
	public int mapNoise = 1;

	public int wanderRate = 1;


	public bool deadReckoningOn = false;

	public GameObject plane;

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
		mapGenerator.dim = mapDim;
		mapGenerator.divisionDim = mapDiv;
		mapGenerator.courtProb = mapCourtProb;
		mapGenerator.neighbourhoodSizeNoise = mapNoise;

	}

	public void createEnemies(){
		for (int i = 0; i< enemyAmount; i++) {
			enemy = Instantiate(Resources.Load("Enemy", typeof(Enemy)), getValidPosition(), Quaternion.identity) as Enemy;
			enemy.id = i;
			enemies.Add(i,enemy);
			enemy.livesOnServer = false;
		}
	}

	public void createEnemies(Hashtable positions){
		Debug.Log ("Create Enemies");
		enemies.Clear ();
		foreach(DictionaryEntry d in positions){
			enemy = Instantiate(Resources.Load("Enemy", typeof(Enemy)), (Vector3)d.Value, Quaternion.identity) as Enemy;
			enemy.id = (int)d.Key;
			enemies.Add(enemy.id,enemy);
			enemy.livesOnServer = false;
		}
	}

	public void instantiateEnemyMovement(){
		foreach (DictionaryEntry e in enemies) {
			Enemy en = (Enemy) e.Value;
			Vector3 velocity = new Vector3 (Random.Range (0.0f, enemySpeed), 0, Random.Range (0.0f, enemySpeed));
			en.velocity = velocity;
			en.setMaxVelocity(enemySpeed);
			en.livesOnServer = true;
		}

	}
	public Enemy createNewEnemy(){

		int key = enemies.Count;
		while (enemies[key] != null)
			key++;
		Vector3 pos = getValidPosition ();
		Debug.Log (pos);
		Enemy e = Instantiate(Resources.Load("Enemy", typeof(Enemy)), pos, Quaternion.identity) as Enemy;
		e.id = key;
		e.velocity = new Vector3 (Random.Range (0.0f, enemySpeed), 0, Random.Range (0.0f, enemySpeed));
		e.setMaxVelocity (enemySpeed);
		e.livesOnServer = true;
		enemies.Add(key,e);

		return e;
	}
	public void createNewEnemy (int id, Vector3 pos, Vector3 velocity){
		Enemy e = Instantiate(Resources.Load("Enemy", typeof(Enemy)), pos, Quaternion.identity) as Enemy;
		e.velocity = velocity;
		e.id = id;
		if (enemies [id] != null)
			Debug.LogError ("enemy already exists");
		else
			enemies.Add(id,e);
	

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

		Player1.transform.position = getValidPosition();
		Player2.transform.position = getValidPosition();


		Player1.id = 1;
		Player2.id = 2;


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
				Enemy e =  ((Enemy)enemies[Random.Range(0, enemies.Count)]);
                mainCamera.transform.position = e.transform.position + new Vector3(0, 20, 0);
                mainCamera.transform.parent = e.transform;
            }
        }

    }
	public void sendBullet(Vector3 pos, Vector3 vel){
		if (client != null)
			client.sendBullet (pos, vel);

	}
	public void createBullet(Vector3 pos, Vector3 vel, float latency){
		//create bullet
		Bullet bulletClone = Instantiate(Resources.Load("Bullet", typeof(Bullet)), pos + (vel * latency), Quaternion.identity) as Bullet;
		bulletClone.velocity = vel;
	}
	/*
	 *  UPDATES
	 */
	void Update () {
		if (Player1 != null && Player2 != null) {


			if(Player1.getHealth() <= 0){
				Player1.renderer.material.color = Color.black;
				Player1.Dead = true;
			}

			if(Player2.getHealth() <= 0){
				Player2.renderer.material.color = Color.black;
				Player2.Dead = true;
			}

		}

		if(deadReckoningOn){
			updatePlayersDeadReckoning();
			updateEnemiesDeadReckoning();
		}else{
			if(client!=null && thisPlayer != null)
				client.updatePosition (thisPlayer.transform.position,thisPlayer.velocity);
			if(server!=null)
				server.sendEnemyUpdates();
		}

	}
	public void enemyShot(int id, bool received){
		enemies.Remove (id);

		if (client != null && !received)
			client.enemyShotSend (id);
		else if (server != null && !received)
			server.enemyShotServer (id);


	}
	void updatePlayersDeadReckoning(){
		//Clients
		if(client != null && thisPlayer != null && otherPlayer != null){
			// if velocity changes, send update
			if(thisPlayer.pastVelocity != thisPlayer.velocity){
				Debug.Log ("SEND IT "+thisPlayer.velocity);
				client.updatePosition (thisPlayer.transform.position, thisPlayer.velocity);
			}

			// update other player according to deadReckoning
			if(otherPlayer != null){
				otherPlayer.transform.position = deadReckoningPlayer(otherPlayer);
			}
		}
		// server
		else if (server != null && Player1 != null && Player2 != null) {
			Player1.transform.position = deadReckoningPlayer(Player1);
			Player2.transform.position = deadReckoningPlayer(Player2);
		}
	}
	void updateEnemiesDeadReckoning(){

		if (client != null) {
			foreach (DictionaryEntry d in enemies) {
				((Enemy)enemies [(int)d.Key]).transform.position = deadReckoningEnemy ((Enemy)enemies [(int)d.Key]);
			}
		} else if (server != null) {
			foreach (DictionaryEntry d in enemies) {
				Enemy e = (Enemy)d.Value;
				if(e.velocity != e.pastVelocity)
					server.sendEnemyUpdates((int)d.Key);
			}
		}
	}


	public void updatePlayerServer(int id, Vector3 position,Vector3 velocity, float latency){
		if (id == 1) {
			if(Player1 != null){
				Player1.transform.position = position + (velocity * latency);
				Player1.velocity = velocity;
			}
		} else if (id == 2)  {
			if(Player2 != null){
				Player2.transform.position = position + (velocity * latency);
				Player2.velocity = velocity;
			}
		}

	}

	public void updateOtherPlayer(Vector3 position, Vector3 velocity){

		if(otherPlayer!= null){
			otherPlayer.transform.position = position;
			otherPlayer.velocity = velocity;

		}

	}
	public void updateEnemy(int key, Vector3 position, Vector3 velocity, float latency){
		if (enoughEnemies) {
			if (enemies [key] != null){
				((Enemy)enemies [key]).transform.position = position + (velocity * latency);
				((Enemy)enemies [key]).velocity = velocity;
		}
		}
	}
	public void playerHit (int id, Vector3 position, float health, bool received){
		if (client != null && !received) {
			if (id == 1) {
				client.playerHitClient (id, position, Player1.getHealth ());
				playerSetHealth (id, health, position);


			} else if (id == 2) {
				client.playerHitClient (id,position, Player2.getHealth ());
				playerSetHealth (id, health, position);

			}
		} else if (server != null && !received) {
			if (id == 1) {
				server.playerHitServer (id, Player1.getHealth (), position);
				playerSetHealth (id, health, position);

			} else if (id == 2) {
				server.playerHitServer (id, Player2.getHealth (), position);
				playerSetHealth (id, health, position);

			}
		}

	}
	void playerSetHealth(int id , float health, Vector3 position){
		if (id == 1) {
			Player1.setHealth (health);
			Player1.velocity = Vector3.zero;

		} else if (id == 2) {
			Player2.setHealth (health);
			Player2.velocity = Vector3.zero;
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
		this.plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		this.plane.transform.localScale = plane;
		this.plane.tag = "city";

		for (float i = this.plane.renderer.bounds.min.z; i < this.plane.renderer.bounds.max.z; i++) {
			GameObject w = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
			w.transform.position = new Vector3(i+.5f,1,this.plane.renderer.bounds.min.z);
			GameObject e = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
			e.transform.position = new Vector3(i+.5f,1,this.plane.renderer.bounds.max.z);
			w.transform.localScale = new Vector3 (1f, 3, .1f);
			e.transform.localScale = new Vector3 (1f, 3, .1f);
			
			GameObject n = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
			n.transform.position = new Vector3(this.plane.renderer.bounds.min.x, 1 , i+.5f);
			GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
			s.transform.position = new Vector3(this.plane.renderer.bounds.max.x,1,i+.5f);
			s.transform.localScale = new Vector3 (.1f, 3, 1f);
			n.transform.localScale = new Vector3 (.1f, 3, 1f);
			
			e.tag = "city";
			w.tag = "city";
			
			s.tag = "city";
			n.tag = "city";
		}

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
		mainCamera.transform.parent = null;
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
		if (enemies.Count > 0) {
			foreach (DictionaryEntry e in enemies) {
				Destroy (((Enemy)e.Value).gameObject);
			}
		}
		enemies.Clear ();
	}

	/*
	 * Helpers
	 */
	public Vector3 getValidPosition(){
		// check for an empty position
		if (plane == null) {
			plane = mapGenerator.plane;
		}

		while (true) {
			float posx = Random.Range(this.plane.renderer.bounds.min.x, this.plane.renderer.bounds.max.x);
			float posz = Random.Range(this.plane.renderer.bounds.min.z, this.plane.renderer.bounds.max.z);
			Vector3 pos = new Vector3(posx, 1, posz);
			float spawn_radius = 0.3f;
			Collider[] hitColliders = Physics.OverlapSphere(pos, spawn_radius);

			if (hitColliders.Length == 0) {
				return pos;
			}
		}
	}
	public bool isOutOfBounds(Vector3 pos){
		if (plane == null && mapGenerator != null) {
			plane = mapGenerator.plane;
		}
		if (pos.x < this.plane.renderer.bounds.min.x ||
			pos.x > this.plane.renderer.bounds.max.x ||
			pos.z < this.plane.renderer.bounds.min.z ||
			pos.z > this.plane.renderer.bounds.max.z) {
			return true;
		}
		return false;
	}

	Vector3 deadReckoningPlayer(Player player){
		Vector3 deadReckoning = player.transform.position + ((player.velocity * Time.deltaTime));
		return deadReckoning;

	}
	Vector3 deadReckoningEnemy(Enemy enemy){
		Vector3 deadReckoning = enemy.transform.position + ((enemy.velocity * Time.deltaTime));
		return deadReckoning;

	}



}
