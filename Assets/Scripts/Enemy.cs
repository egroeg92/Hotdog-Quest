using UnityEngine;
using System.Collections;

public class Enemy : NPC {

	public SteeringManager steering;
	public Transform[] track;
	private Transform visibility;
	public Transform target;
	public Transform target2;
	public ArrayList visiblePlayers;

	public Enemy (Vector3 position) : base(position) {
		Debug.Log("Enemy created at " +position);
	}

	// Use this for initialization
	void Start () {
		base.Start ();
		tag = "enemy";
		this.steering = new SteeringManager(this);
		this.visiblePlayers = new ArrayList();

	}

	// Update is called once per frame
	void Update () {
		this.pastVelocity = getVelocity();

		// on sever, do own movement
		if (this.getOnServer()){

			// check if player visible, if so do seek, else wander
			if (canSee()) {
				steering.seek(getClosestPlayer());
			} else {
				if(Time.frameCount % game.wanderRate == 0)
					steering.wander();
			}

			velocity = steering.update();
			velocity = truncate(velocity, MAX_VELOCITY);
			//Update movement
			//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), 4 * Time.deltaTime);
			transform.Translate(velocity * Time.deltaTime);
		}

	}

	bool canSee(){
		visiblePlayers.Clear();
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		foreach (GameObject p in players) {
			RaycastHit hit;
			Debug.DrawRay(transform.position, p.transform.position - transform.position, Color.black);
			Physics.Raycast(transform.position,p.transform.position - transform.position , out hit);
			if(hit.transform != null){
				if(hit.transform.gameObject == p)
					visiblePlayers.Add(p);
			}

		}
		if(visiblePlayers.Count > 0)
			return true;
		return false;
	}

	Vector3 getClosestPlayer(){
		Vector3 target = transform.position + velocity;
		float dist = float.MaxValue;
		foreach (GameObject p in visiblePlayers) {
			float d = Vector3.Distance (p.transform.position, transform.position);
			if (d < dist) {
				dist = d;
				target = p.transform.position;
			}
		}
		return target;

	}

	void OnCollisionEnter(Collision col){

		if (col.gameObject.tag == "bullet") {
			game.enemyShot(id, false);
			Destroy (gameObject);
		}
	}
}
