using UnityEngine;
using System.Collections;

public class Enemy : NPC {

	public EnemyMovement movement;
	public Transform[] track;
	private Transform currentWaypoint;
	private int currentIndex;

	public Enemy (Vector3 position) : base(position) {
		Debug.Log("Enemy created at " +position);
	}

	// Use this for initialization
	void Start () {
		this.movement = new EnemyMovement(this);
		this.currentWaypoint = track[0];
	}

	// Update is called once per frame
	void Update () {
		velocity = movement.update();

		velocity = velocity.normalized;
		velocity *= MAX_VELOCITY;
		velocity = truncate(velocity, MAX_VELOCITY);

		//Update movement
		transform.Translate(velocity * Time.deltaTime);

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
