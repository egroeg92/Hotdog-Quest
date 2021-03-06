﻿using UnityEngine;
using System.Collections;

public class Enemy : NPC {

	public SteeringManager steering;
	public Transform[] track;
	private Transform visibility;
	public Transform target;
	public Transform target2;
	public ArrayList visiblePlayers;

	int wanderRate;

	public Enemy (Vector3 position) : base(position) {
		Debug.Log("Enemy created at " +position);
	}

	// Use this for initialization
	void Start () {
		base.Start ();
		tag = "enemy";
		this.steering = new SteeringManager(this);
		this.visiblePlayers = new ArrayList();
		wanderRate = game.wanderRate;
	}

	// Update is called once per frame
	void Update () {
		this.pastVelocity = getVelocity();

		// on sever, do own movement
		if (this.getOnServer()){
			Vector3 avoidVector = Vector3.zero;

			//check if player visible, if so do seek, else wander
			if (canSee()) {
				steering.seek(getClosestPlayer());
			} else {
				steering.doAvoid();
			}
			// if nothing to avoid, wander
			if (Time.frameCount  % wanderRate == 0 ) {
			  	steering.wander();
			}
			velocity = steering.update();
			velocity.y = 0;
			// //Update movement

			velocity = Vector3.ClampMagnitude(velocity, getMaxVelocity());
			transform.position +=(velocity * Time.deltaTime);

			if(game.isOutOfBounds(transform.position)){
				transform.position = game.getValidPosition();
			}

		}
	}

	bool canSee(){
		visiblePlayers.Clear();
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		foreach (GameObject p in players) {
			RaycastHit hit;
			Physics.Raycast(transform.position,p.transform.position - transform.position , out hit);
			if(hit.transform != null){
				if(hit.transform.gameObject == p){
					visiblePlayers.Add(p);
					Debug.DrawRay(transform.position, p.transform.position - transform.position, Color.black);
					}
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
