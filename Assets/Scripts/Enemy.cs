using UnityEngine;
using System.Collections;

public class Enemy : NPC {

	public SteeringManager steering;
	public Transform[] track;
	public int id;
	private bool seesPlayer;
	private Transform visibility;
	public Transform target;

	public Enemy (Vector3 position) : base(position) {
		Debug.Log("Enemy created at " +position);
	}

	// Use this for initialization
	void Start () {
		this.steering = new SteeringManager(this);
		this.seesPlayer = false;
		this.target = GameObject.FindGameObjectWithTag("Player").transform;
	}

	// Update is called once per frame
	void Update () {
		this.pastVelocity = getVelocity();

		if (this.getOnServer()){
			// on sever, do own movement
			steering.wander();
			velocity = steering.update();
			velocity = truncate(velocity, MAX_VELOCITY);
			//Update movement
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), 4 * Time.deltaTime);
			transform.Translate(velocity * Time.deltaTime);
		}

	}

	private void checkVisibility() {
		seesPlayer = visibility.collider.bounds.Intersects(target.collider.bounds);
		if (seesPlayer) {
			Debug.Log("Player seen.");
		}
	}
}
