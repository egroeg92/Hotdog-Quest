using UnityEngine;
using System.Collections;

public class enemy1 : MonoBehaviour {

	public enemyMovement1 move = null;
	public int id;

	public Vector3 velocity=Vector3.zero;
	public Vector3 pastVelocity;
	// Use this for initialization
	void Start () {
	
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
