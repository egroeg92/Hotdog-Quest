using UnityEngine;
using System.Collections;

public class Enemy : NPC {

	public EnemyMovement movement;
	public SteeringManager steering;
	public Transform[] track;
	private Transform currentWaypoint;
	private int currentIndex;
	private bool seesPlayer;
	private Transform visibility;
	public Transform target;

	public Enemy (Vector3 position) : base(position) {
		Debug.Log("Enemy created at " +position);
	}

	// Use this for initialization
	void Start () {
		this.movement = new EnemyMovement(this);
		this.steering = new SteeringManager(this);
		this.currentWaypoint = track[0];
		this.seesPlayer = false;
		this.visibility = GameObject.Find("Visibility").transform;
		this.target = GameObject.FindGameObjectWithTag("Player").transform;
	}

	// Update is called once per frame
	void Update () {
		// check to see if player is in range
		velocity = movement.update();

		velocity = velocity.normalized;
		velocity *= MAX_VELOCITY;

		velocity = truncate(velocity, MAX_VELOCITY);

		// adjust calculated velocity if we see player
		checkVisibility();
		if(seesPlayer) {
			steering.seek(target.position);
			velocity = steering.update();
			velocity = truncate(velocity, MAX_VELOCITY);
		}
		//Update movement
		transform.Translate(velocity * Time.deltaTime);

	}

	private void checkVisibility() {
		seesPlayer = visibility.collider.bounds.Intersects(target.collider.bounds);
		if (seesPlayer) {
			Debug.Log("Player seen.");
		}
	}

	public void setCurrentWaypoint(Transform waypoint) {
		this.currentWaypoint = waypoint;
	}

	public Transform getCurrentWaypoint() {
		return this.currentWaypoint;
	}

	public void setCurrentIndex(int index) {
		this.currentIndex = index;
	}

	public int getCurrentIndex() {
		return this.currentIndex;
	}

	public Transform[] getTrack() {
		return this.track;
	}

	private void setTrack(Transform[] track) {
		//TODO: do this in a good way
		GameObject result = GameObject.FindGameObjectWithTag("Track");
	}
}
