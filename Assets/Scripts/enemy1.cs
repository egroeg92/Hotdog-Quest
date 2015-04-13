using UnityEngine;
using System.Collections;

public class enemy1 : MonoBehaviour {

	public enemyMovement1 move = null;
	public int id;

	public Vector3 velocity=Vector3.zero;
	public Vector3 pastVelocity;
	// Use this for initialization
	void Start () {

		move = gameObject.AddComponent<enemyMovement1> ();
		move.enemy = this;
		move.velocity = new Vector3(1,0,0);
		move.speed = 1;
		move.wanderAngle = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (move != null) {
			pastVelocity = move.pastVelocity;
			velocity = move.velocity;
		} 

	
	}
	void LateUpdate(){

	}
}
